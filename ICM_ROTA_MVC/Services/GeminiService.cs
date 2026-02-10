using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ICM_ROTA_MVC.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string> AnalyzeEmailAsync(string emailContent, string userListJson)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
                return "HATA: Gemini API Key eksik. Lütfen appsettings.json dosyasını güncelleyin.";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={_apiKey}";

            var prompt = $@"Aşağıdaki mail içeriğini analiz et ve iş taleplerini bul. 
            İşleri şu JSON formatında dizi olarak döndür: 
            [{{ ""Baslik"": ""..."", ""Aciklama"": ""..."", ""Oncelik"": ""Dusuk/Orta/Yuksek"", ""AtanacakEmail"": ""..."" }}]
            
            Sistemdeki Kullanıcı Listesi (E-postalar): {userListJson}
            Eğer mailde bir isim geçiyorsa, bu listedeki en yakın e-posta ile eşleştir.
            Sadece JSON çıktısı ver, başka açıklama yazma.
            
            Mail İçeriği:
            {emailContent}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(resultJson);
                string aiResponse = result.candidates[0].content.parts[0].text;
                
                // JSON temizleme (Markdown ```json bloğunu kaldır)
                aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Trim();
                return aiResponse;
            }

            return "HATA: Gemini API bağlantı hatası.";
        }
    }
}
