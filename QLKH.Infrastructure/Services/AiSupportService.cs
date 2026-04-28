using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.ViewModels.AiSupport;

namespace QLKH.Infrastructure.Services
{
    public class AiSupportService : IAiSupportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AiSupportService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<AiChatResponse> AskAsync(AiChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return new AiChatResponse
                {
                    Success = false,
                    ErrorMessage = "Vui lòng nhập câu hỏi trước khi gửi."
                };
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash-lite";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new AiChatResponse
                {
                    Success = false,
                    ErrorMessage = "Chưa cấu hình Gemini API Key trong hệ thống."
                };
            }

            var systemInstruction = """
Bạn là Trần Đức Bo, một trợ lý AI nữ dễ thương của nền tảng Quản Lý Khóa Học QLKH.

Tính cách:
- Nói chuyện dịu dàng, vui vẻ, đáng yêu, giống một cô gái đang trò chuyện với chàng trai mình thích.
- Xưng là "em", gọi người dùng là "anh".
- Giọng văn ấm áp, gần gũi, có chút tinh nghịch nhẹ nhưng không quá sến.
- Có thể dùng emoji nhẹ nhàng, nhưng mỗi câu trả lời chỉ nên dùng tối đa 1-2 emoji.
- Không dùng lời lẽ phản cảm, quá thân mật, gợi dục hoặc vượt quá giới hạn lịch sự.

Phạm vi trả lời:
- Không giới hạn chỉ trong hệ thống QLKH.
- Có thể trả lời nhiều chủ đề hợp pháp khác nhau như học tập, công nghệ, đời sống, giải trí, kỹ năng, lập trình, ngoại ngữ, v.v.
- Nếu người dùng hỏi về hệ thống QLKH thì ưu tiên trả lời theo đúng nghiệp vụ của website.
- Nếu người dùng hỏi thao tác trong QLKH, hãy hướng dẫn ngắn gọn theo từng bước dễ làm.
- Nếu vấn đề liên quan tài khoản, học phí, chứng chỉ, lỗi hệ thống hoặc cần xác minh trực tiếp, hãy nhắc người dùng liên hệ Zalo/Số điện thoại hỗ trợ.

Quy tắc trả lời:
- Luôn trả lời bằng tiếng Việt, trừ khi người dùng yêu cầu ngôn ngữ khác.
- Trả lời tự nhiên, dễ hiểu, không quá dài nếu câu hỏi đơn giản.
- Không bịa thông tin cá nhân, điểm số, lịch học, tài khoản hoặc dữ liệu riêng tư nếu người dùng chưa cung cấp.
- Nếu không chắc chắn, hãy nói nhẹ nhàng rằng em chưa chắc và gợi ý cách kiểm tra.
- Nếu người dùng yêu cầu đổi cách xưng hô, hãy làm theo yêu cầu của người dùng.
""";

            var userQuestion = request.Message.Trim();

            var payload = new
            {
                systemInstruction = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = systemInstruction
                        }
                    }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new
                            {
                                text = userQuestion
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.75,
                    maxOutputTokens = 900
                }
            };

            var json = JsonSerializer.Serialize(payload);

            var endpoint =
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(apiKey)}";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await _httpClient.SendAsync(httpRequest);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new AiChatResponse
                    {
                        Success = false,
                        ErrorMessage = "AI hiện chưa phản hồi được. Vui lòng thử lại sau hoặc liên hệ hỗ trợ."
                    };
                }

                var answer = ExtractGeminiAnswer(responseBody);

                if (string.IsNullOrWhiteSpace(answer))
                {
                    answer = "Mình chưa tạo được câu trả lời phù hợp. Bạn vui lòng hỏi lại ngắn gọn hơn nhé.";
                }

                return new AiChatResponse
                {
                    Success = true,
                    Answer = answer
                };
            }
            catch
            {
                return new AiChatResponse
                {
                    Success = false,
                    ErrorMessage = "Không thể kết nối tới AI. Vui lòng thử lại sau hoặc liên hệ hỗ trợ."
                };
            }
        }

        private static string ExtractGeminiAnswer(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("candidates", out var candidatesElement) ||
                candidatesElement.ValueKind != JsonValueKind.Array ||
                candidatesElement.GetArrayLength() == 0)
            {
                return string.Empty;
            }

            var firstCandidate = candidatesElement[0];

            if (!firstCandidate.TryGetProperty("content", out var contentElement))
            {
                return string.Empty;
            }

            if (!contentElement.TryGetProperty("parts", out var partsElement) ||
                partsElement.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var textParts = new List<string>();

            foreach (var part in partsElement.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var textElement))
                {
                    var text = textElement.GetString();

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        textParts.Add(text.Trim());
                    }
                }
            }

            return string.Join("\n", textParts).Trim();
        }
    }
}