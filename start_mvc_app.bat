@echo off
TITLE ICM ROTA MVC Baslatici
echo ==========================================
echo    ICM ROTA (.NET 9 MVC) Baslatiliyor...
echo ==========================================

:: Veritabani guncellemesi (opsiyonel ama guvenli)
echo [1/2] Veritabani guncellemeleri kontrol ediliyor...
cd /d %~dp0ICM_ROTA_MVC
dotnet ef database update

:: Uygulamayi baslat
echo [2/2] Uygulama baslatiliyor (http://localhost:5000)...
start http://localhost:5000
dotnet run

pause
