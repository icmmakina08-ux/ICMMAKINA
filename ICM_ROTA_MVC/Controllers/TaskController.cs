using Microsoft.AspNetCore.Mvc;
using ICM_ROTA_MVC.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ICM_ROTA_MVC.Controllers
{
    public class TaskController : Controller
    {
        private readonly string _connectionString;

        public TaskController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Bağlantı dizesi bulunamadı.");
        }

        // Görevleri Listele
        public IActionResult Index()
        {
            var viewModel = new TaskViewModel();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Tablo yoksa oluşturma veya güncelleme mantığı
                string checkTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PANEL_TASKS]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE PANEL_TASKS (
                            ID INT PRIMARY KEY IDENTITY(1,1),
                            Baslik NVARCHAR(200),
                            Aciklama NVARCHAR(MAX),
                            AtananKisi NVARCHAR(MAX),
                            Kullanici_IDs NVARCHAR(MAX),
                            Oncelik NVARCHAR(50),
                            Durum NVARCHAR(50),
                            OlusturmaTarihi DATETIME DEFAULT GETDATE(),
                            BitisTarihi DATETIME NULL
                        );
                    END
                    ELSE IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PANEL_TASKS]') AND name = 'Kullanici_IDs')
                    BEGIN
                        ALTER TABLE PANEL_TASKS ADD Kullanici_IDs NVARCHAR(MAX);
                        ALTER TABLE PANEL_TASKS ALTER COLUMN AtananKisi NVARCHAR(MAX);
                    END";
                using (SqlCommand checkCmd = new SqlCommand(checkTableSql, conn)) { checkCmd.ExecuteNonQuery(); }

                // Kullanıcı Listesini Çek (KULLANICI_PANEL tablosundan)
                string userSql = "SELECT Kullanıcı_ID, Kullanıcı_Mail, Kullanıcı_Name, Kullanıcı_Surname FROM KULLANICI_PANEL";
                using (SqlCommand userCmd = new SqlCommand(userSql, conn))
                {
                    using (SqlDataReader dr = userCmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            viewModel.Users.Add(new UserLookup {
                                ID = (int)dr["Kullanıcı_ID"],
                                Email = dr["Kullanıcı_Mail"].ToString() ?? "",
                                FullName = (dr["Kullanıcı_Name"].ToString() + " " + dr["Kullanıcı_Surname"].ToString()).Trim()
                            });
                        }
                    }
                }

                // Görevleri Çek
                string sql = "SELECT * FROM PANEL_TASKS ORDER BY OlusturmaTarihi DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            viewModel.Tasks.Add(new TaskItem {
                                ID = (int)dr["ID"],
                                Baslik = dr["Baslik"].ToString() ?? "",
                                Aciklama = dr["Aciklama"].ToString() ?? "",
                                AtananKisi = dr["AtananKisi"].ToString() ?? "",
                                Kullanici_IDs = dr["Kullanici_IDs"]?.ToString() ?? "",
                                Oncelik = dr["Oncelik"].ToString() ?? "Orta",
                                Durum = dr["Durum"].ToString() ?? "Beklemede",
                                OlusturmaTarihi = (DateTime)dr["OlusturmaTarihi"],
                                BitisTarihi = dr["BitisTarihi"] != DBNull.Value ? (DateTime)dr["BitisTarihi"] : null
                            });
                        }
                    }
                }
            }
            return View(viewModel);
        }

        // Yeni Görev Kaydet
        [HttpPost]
        public IActionResult AddTask(TaskItem model, List<int> SelectedUserIds)
        {
            // Seçilen kullanıcıların maillerini ve ID'lerini topla
            List<string> selectedEmails = new List<string>();
            List<string> selectedIds = new List<string>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                
                if (SelectedUserIds != null && SelectedUserIds.Count > 0)
                {
                    string idList = string.Join(",", SelectedUserIds);
                    string userSql = $"SELECT Kullanıcı_ID, Kullanıcı_Mail FROM KULLANICI_PANEL WHERE Kullanıcı_ID IN ({idList})";
                    using (SqlCommand userCmd = new SqlCommand(userSql, conn))
                    {
                        using (SqlDataReader dr = userCmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                selectedEmails.Add(dr["Kullanıcı_Mail"].ToString() ?? "");
                                selectedIds.Add(dr["Kullanıcı_ID"].ToString() ?? "");
                            }
                        }
                    }
                }

                string sql = "INSERT INTO PANEL_TASKS (Baslik, Aciklama, AtananKisi, Kullanici_IDs, Oncelik, Durum, OlusturmaTarihi) " +
                             "VALUES (@baslik, @aciklama, @atanan, @kullaniciIds, @oncelik, 'Beklemede', GETDATE())";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@baslik", model.Baslik);
                    cmd.Parameters.AddWithValue("@aciklama", model.Aciklama);
                    cmd.Parameters.AddWithValue("@atanan", string.Join(", ", selectedEmails));
                    cmd.Parameters.AddWithValue("@kullaniciIds", string.Join(",", selectedIds));
                    cmd.Parameters.AddWithValue("@oncelik", model.Oncelik);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE PANEL_TASKS SET Durum = @status";
                
                // Eğer durum 'Tamamlandi' ise bitiş tarihini de ata
                if (status == "Tamamlandi") {
                    sql += ", BitisTarihi = GETDATE()";
                } else {
                    sql += ", BitisTarihi = NULL";
                }
                
                sql += " WHERE ID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }
    }
}
