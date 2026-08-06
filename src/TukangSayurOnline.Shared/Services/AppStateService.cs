using System;

namespace TukangSayurOnline.Shared.Services;

public class AppStateService
{
    public event Action? OnChange;

    public string? Token { get; private set; }
    public int UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty; // Admin, TukangSayur, Pelanggan
    public int? TukangSayurId { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);
    public bool IsAdmin => Role == "Admin";
    public bool IsTukangSayur => Role == "TukangSayur";
    public bool IsPelanggan => Role == "Pelanggan";

    public void SetUser(string token, int userId, string fullName, string email, string role, int? tukangSayurId)
    {
        Token = token;
        UserId = userId;
        FullName = fullName;
        Email = email;
        Role = role;
        TukangSayurId = tukangSayurId;
        NotifyStateChanged();
    }

    public void Logout()
    {
        Token = null;
        UserId = 0;
        FullName = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        TukangSayurId = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
