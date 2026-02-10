import React, { useState } from 'react';
import './Login.css';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        console.log('Login attempt:', { username, password });
        // TODO: Connect to backend/Supabase
    };

    return (
        <div className="login-container">
            <div className="login-card">
                <div className="login-header">
                    {/* Logo placeholder - replace with actual image later */}
                    <div className="logo-placeholder">
                        <span className="logo-text-orange">ICM</span>
                        <span className="logo-text-black">ROTA M3</span>
                    </div>
                    <h2>Giriş Yap</h2>
                    <p className="subtitle">Görev takip sistemine hoş geldiniz</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    <div className="form-group">
                        <label htmlFor="username">Kullanıcı Adı</label>
                        <input
                            type="text"
                            id="username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="Kullanıcı adınızı giriniz"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Şifre</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Şifrenizi giriniz"
                            required
                        />
                    </div>

                    <div className="form-actions">
                        <a href="#" className="forgot-password">Şifremi Unuttum</a>
                    </div>

                    <button type="submit" className="login-button">
                        Giriş Yap
                    </button>
                </form>
            </div>
        </div>
    );
};

export default Login;
