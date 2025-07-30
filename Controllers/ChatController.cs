using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace dotnet_store.Controllers
{
    public class ChatController : Controller
    {
        // Chat responses database
        private readonly Dictionary<string, string> _chatResponses = new()
        {
            { "ürün bilgisi", "Ürünlerimiz hakkında detaylı bilgi almak için kategoriler sayfasını ziyaret edebilirsiniz. Hangi ürün hakkında bilgi almak istiyorsunuz?" },
            { "sipariş durumu", "Sipariş durumunuzu öğrenmek için sipariş numaranızı paylaşabilir misiniz?" },
            { "iade işlemi", "İade işlemleri için müşteri hizmetlerimizle iletişime geçebilirsiniz. İade koşullarımızı web sitemizde bulabilirsiniz." },
            { "fiyat bilgisi", "Fiyat bilgileri için ürün sayfalarını ziyaret edebilirsiniz. Özel indirimlerden haberdar olmak için bültenimize abone olabilirsiniz." },
            { "kargo", "Kargo bilgileri için sipariş numaranızı paylaşabilir misiniz? Kargo takibi yapabilirsiniz." },
            { "ödeme", "Ödeme seçeneklerimiz: Kredi kartı, banka kartı, havale/EFT ve kapıda ödeme. Güvenli ödeme altyapımız mevcuttur." },
            { "indirim", "Güncel indirimlerimizi takip etmek için bültenimize abone olabilirsiniz. Ayrıca sosyal medya hesaplarımızı takip edebilirsiniz." },
            { "iletişim", "Bize ulaşmak için: Telefon: 0212 XXX XX XX, E-posta: info@example.com, Adres: İstanbul, Türkiye" }
        };

        private readonly string[] _followUpQuestions = new[]
        {
            "Başka bir sorunuz var mı?",
            "Size başka nasıl yardımcı olabilirim?",
            "Başka bir konuda yardıma ihtiyacınız var mı?"
        };

        [HttpPost]
        public async Task<IActionResult> GetResponse([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { error = "Mesaj boş olamaz" });
                }

                // Simple keyword matching
                var response = "Teşekkür ederiz! Sorunuzla ilgili size yardımcı olmaya çalışacağız. Daha detaylı bilgi için müşteri hizmetlerimizle iletişime geçebilirsiniz.";
                
                var lowerMessage = request.Message.ToLower();
                
                foreach (var kvp in _chatResponses)
                {
                    if (lowerMessage.Contains(kvp.Key))
                    {
                        response = kvp.Value;
                        break;
                    }
                }

                // Add some randomness to make it more natural
                var random = new Random();
                var followUp = _followUpQuestions[random.Next(_followUpQuestions.Length)];

                // Log the chat interaction (optional)
                await LogChatInteraction(request.Message, response);

                return Json(new
                {
                    response = response,
                    followUp = followUp,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                });

            }
            catch (Exception)
            {
                // Log the error
                // _logger.LogError(ex, "Chat API Error");
                
                return StatusCode(500, new { error = "Sunucu hatası oluştu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Analytics([FromBody] AnalyticsRequest request)
        {
            try
            {
                // Log analytics data
                await LogAnalytics(request.Event, request.Data);
                
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Analytics error" });
            }
        }

        [HttpGet]
        public IActionResult Health()
        {
            return Json(new
            {
                status = "OK",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            });
        }

        private async Task LogChatInteraction(string userMessage, string botResponse)
        {
            // Burada chat etkileşimlerini loglayabilirsiniz
            // Örnek: Database'e kaydetme, log dosyasına yazma vb.
            await Task.CompletedTask;
        }

        private async Task LogAnalytics(string eventName, object data)
        {
            // Burada analytics verilerini loglayabilirsiniz
            // Örnek: Google Analytics, custom analytics vb.
            await Task.CompletedTask;
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class AnalyticsRequest
    {
        public string Event { get; set; } = string.Empty;
        public object Data { get; set; } = new();
    }
}