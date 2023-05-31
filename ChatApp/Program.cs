using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatApp
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter your message (type 'exit' to quit): ");
                var userMessage = Console.ReadLine();

                if (userMessage is null)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }

                if (userMessage.Trim().ToLower() == "exit")
                    break;

                var response = await ChatWithGpt3(userMessage);

                Console.WriteLine("GPT-3 Response: " + response);
            }
        }

        static async Task<string> ChatWithGpt3(string userMessage)
        {
            var openAiUrl = "https://api.openai.com/v1/chat/completions";
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                }
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

            var response = await client.PostAsync(openAiUrl, requestContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            return ParseGpt3Response(responseContent);
        }

        static string ParseGpt3Response(string responseContent)
        {
            var response = JsonConvert.DeserializeObject<OpenAiResponse>(responseContent);
            string? gpt3Response = null;

            if (response.choices != null && response.choices.Length > 0)
            {
                var firstChoice = response.choices[0];

                if (firstChoice.message != null)
                {
                    gpt3Response = firstChoice.message.content;
                }
            }

            return gpt3Response ?? "No response";
        }
    }

    public class OpenAiResponse
    {
        public OpenAiChoice[]? choices { get; set; }
    }

    public class OpenAiChoice
    {
        public OpenAiMessage? message { get; set; }
    }

    public class OpenAiMessage
    {
        public string? content { get; set; }
    }
}