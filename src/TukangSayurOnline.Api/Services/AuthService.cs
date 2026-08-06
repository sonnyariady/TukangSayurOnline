using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TukangSayurOnline.Api.Data;
using TukangSayurOnline.Api.Data.Entities;
using TukangSayurOnline.Api.Models;

namespace TukangSayurOnline.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return new AuthResponse(false, "Email sudah terdaftar.", "", 0, "", "", "", null);
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = DbInitializer.HashPassword(request.Password),
            Role = request.Role,
            Address = request.Address,
            Latitude = request.Latitude != 0 ? request.Latitude : -6.2000,
            Longitude = request.Longitude != 0 ? request.Longitude : 106.8166
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        int? tukangSayurId = null;
        if (request.Role == UserRole.TukangSayur)
        {
            var profile = new TukangSayurProfile
            {
                UserId = user.Id,
                ShopName = string.IsNullOrWhiteSpace(request.ShopName) ? $"Sayur Segar {user.FullName}" : request.ShopName,
                Balance = 0m,
                IsOnline = true,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                CurrentLocationName = user.Address
            };
            _context.TukangSayurProfiles.Add(profile);
            await _context.SaveChangesAsync();
            tukangSayurId = profile.Id;
        }

        var token = GenerateJwtToken(user, tukangSayurId);
        return new AuthResponse(
            true,
            "Registrasi berhasil.",
            token,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            tukangSayurId
        );
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var passwordHash = DbInitializer.HashPassword(request.Password);
        var user = await _context.Users
            .Include(u => u.TukangSayurProfile)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == passwordHash);

        if (user == null)
        {
            return new AuthResponse(false, "Email atau password salah.", "", 0, "", "", "", null);
        }

        int? tukangSayurId = user.TukangSayurProfile?.Id;
        var token = GenerateJwtToken(user, tukangSayurId);

        return new AuthResponse(
            true,
            "Login berhasil.",
            token,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            tukangSayurId
        );
    }

    private string GenerateJwtToken(User user, int? tukangSayurId)
    {
        var secretKey = _config["Jwt:SecretKey"] ?? "SuperSecretKeyTukangSayurOnline2026_WithHighSecurityLevel999!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (tukangSayurId.HasValue)
        {
            claims.Add(new Claim("TukangSayurId", tukangSayurId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "TukangSayurOnline",
            audience: _config["Jwt:Audience"] ?? "TukangSayurOnlineApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
