using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RestSharp;

namespace OpenAIChat
{
    class Program
    {
        class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        class ChatCompletion
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
        }

        static void Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");

            if (apiKey == null)
            {
                Console.WriteLine("OPENAI_KEY must be set");
                return;
            }

            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var request = new RestRequest(Method.Post);

            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");

            while (true)
            {
                Console.Write("Enter your message (type 'exit' to quit): ");
                var userMessage = Console.ReadLine();

                if (userMessage == "exit")
                {
                    break;
                }

                var chat = new ChatCompletion
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<Message>
                    {
                        new Message
                        {
                            role = "user",
                            content = userMessage
                        }
                    }
                };

                var jsonChat = JsonConvert.SerializeObject(chat);
                request.AddParameter("application/json", jsonChat, ParameterType.RequestBody);

                var response = client.Execute(request);
                var responseText = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Accessing the generated text
                string text = responseText.choices[0].message.content;
                Console.WriteLine($"GPT-3 Response: {text}");
            }
        }
    }
}
