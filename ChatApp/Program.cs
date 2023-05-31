using System;
using System.Threading.Tasks;
using OpenAI;

namespace ChatApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var openAi = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_KEY"));

            while (true)
            {
                Console.Write("Enter your message (type 'exit' to quit): ");
                var userMessage = Console.ReadLine();

                if (userMessage.Trim().ToLower() == "exit")
                    break;

                var response = await ChatGpt3(openAi, userMessage);

                Console.WriteLine("GPT-3 Response: " + response);
            }
        }

        static async Task<string> ChatGpt3(OpenAIClient openAi, string userMessage)
        {
            var completion = await openAi.CompleteConversationAsync(new[] { new ChatMessage { role = "user", content = userMessage } });
            return completion.choices[0].message.content;
        }
    }
}
