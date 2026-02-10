using System;

namespace ICM_ROTA_MVC.Models
{
    public class TaskViewModel
    {
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public List<UserLookup> Users { get; set; } = new List<UserLookup>();
    }

    public class UserLookup
    {
        public int ID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class TaskItem
    {
        public int ID { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string AtananKisi { get; set; } = string.Empty; // Mail adreslerini tutacak
        public string Kullanici_IDs { get; set; } = string.Empty; // Virgüllü ID'leri tutacak
        public string Oncelik { get; set; } = "Orta";
        public string Durum { get; set; } = "Beklemede";
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
}
