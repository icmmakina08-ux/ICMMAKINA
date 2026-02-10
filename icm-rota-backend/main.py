from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI(title="ICM ROTA M3 API", version="1.0.0")

# CORS ayarları (Frontend ile iletişim için)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Güvenlik için production'da spesifik domain olmalı
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/")
def read_root():
    return {"status": "ok", "message": "ICM ROTA Backend Calisiyor"}

@app.get("/health")
def health_check():
    return {"status": "healthy"}
