@echo off
echo ========================================================
echo   Building TukangSayurOnline Android APK (Release)
echo   API Target: http://tukangsayur-api.tryasp.net/
echo ========================================================
echo.

dotnet publish src/TukangSayurOnline.Mobile/TukangSayurOnline.Mobile.csproj -c Release -f net9.0-android

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================================
    echo   BUILD SUCCESSFUL!
    echo   File APK tersimpan di folder:
    echo   src\TukangSayurOnline.Mobile\bin\Release\net9.0-android\publish\
    echo ========================================================
) else (
    echo.
    echo [ERROR] Build Gagal! Periksa log kesalahan di atas.
)

pause
