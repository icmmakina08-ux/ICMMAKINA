using Microsoft.AspNetCore.Mvc;
using ICM_ROTA_MVC.Models;
using Microsoft.Data.SqlClient;

namespace ICM_ROTA_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Bağlantı dizesi bulunamadı.");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        // KULLANICI_PANEL tablosunda Kullanıcı_Mail ve Password kontrolü
                        string sql = "SELECT COUNT(1) FROM KULLANICI_PANEL WHERE Kullanıcı_Mail = @mail AND Password = @sifre";
                        
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@mail", model.Email);
                            cmd.Parameters.AddWithValue("@sifre", model.Password);

                            int userCount = (int)cmd.ExecuteScalar();

                            if (userCount > 0)
                            {
                                // Şimdilik giriş başarılı sayıyoruz. 
                                // Gelecekte Cookie authentication eklenebilir.
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "E-posta adresi veya şifre hatalı.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Veritabanı bağlantı hatası: " + ex.Message);
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
