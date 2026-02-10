using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ICM_ROTA_MVC.Controllers
{
    public class DbSchemaController : Controller
    {
        private readonly string _connectionString;
        public DbSchemaController(IConfiguration configuration) { _connectionString = configuration.GetConnectionString("DefaultConnection"); }

        public IActionResult Index()
        {
            List<string> columns = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var schema = conn.GetSchema("Columns", new[] { null, null, "KULLANICI_PANEL" });
                foreach (System.Data.DataRow row in schema.Rows)
                {
                    columns.Add(row["COLUMN_NAME"].ToString());
                }
            }
            return Json(columns);
        }
    }
}
