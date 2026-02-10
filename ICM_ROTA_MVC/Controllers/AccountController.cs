using Microsoft.AspNetCore.Mvc;
using ICM_ROTA_MVC.Models;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        string sql = "SELECT Kullanıcı_ID, Kullanıcı_Mail, Kullanıcı_Name, Kullanıcı_Surname FROM KULLANICI_PANEL WHERE Kullanıcı_Mail = @mail AND Password = @sifre";
                        
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@mail", model.Email);
                            cmd.Parameters.AddWithValue("@sifre", model.Password);

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, dr["Kullanıcı_Mail"].ToString() ?? ""),
                                        new Claim(ClaimTypes.NameIdentifier, dr["Kullanıcı_ID"].ToString() ?? ""),
                                        new Claim(ClaimTypes.GivenName, dr["Kullanıcı_Name"].ToString() ?? ""),
                                        new Claim(ClaimTypes.Surname, dr["Kullanıcı_Surname"].ToString() ?? ""),
                                        new Claim("Email", dr["Kullanıcı_Mail"].ToString() ?? "")
                                    };

                                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                        new ClaimsPrincipal(claimsIdentity), authProperties);

                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "E-posta adresi veya şifre hatalı.");
                                }
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
