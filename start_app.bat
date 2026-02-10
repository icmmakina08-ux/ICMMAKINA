@echo off
TITLE ICM ROTA Uygulama Baslatici
echo ==========================================
echo    ICM ROTA Uygulamasi Baslatiliyor...
echo ==========================================

:: Backend'i baslat
echo [1/2] Backend (FastAPI) sunucusu yeni pencerede baslatiliyor...
start "ICM ROTA - Backend" cmd /k "cd /d %~dp0icm-rota-backend && .\venv\Scripts\activate && uvicorn main:app --reload"

:: Biraz bekle (Backend'in ayaga kalkmasi icin opsiyonel)
timeout /t 2 /nobreak > nul

:: Frontend'i baslat
echo [2/2] Frontend (React) arayuzu yeni pencerede baslatiliyor...
start "ICM ROTA - Frontend" cmd /k "cd /d %~dp0icm-rota-frontend && npm run dev"

:: Tarayiciyi otomatik ac
echo.
echo Uygulama tarayicida aciliyor...
timeout /t 3 /nobreak > nul
start http://localhost:5173

echo.
echo ==========================================
echo    Her iki servis de baslatildi!
echo    Terminal pencerelerini takip edebilirsiniz.
echo ==========================================
pause
