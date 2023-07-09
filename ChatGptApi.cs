using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace WindowsGPT
{
  public class ChatGptApi
  {
    private readonly HttpClient _httpClient;

    public ChatGptApi()
    {
      _httpClient = new HttpClient();
    }

    public static async Task SendPromptStream(string prompt, System.Action<string> resultHandler)
    {
      OpenAIAPI api = new(new APIAuthentication("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", "org-dYSyL1OmJkg7R4Jf5yRg7UbM"));
      var chat = api.Chat.CreateConversation(new ChatRequest()
      {
        Model = Model.ChatGPTTurbo
      });

      chat.AppendSystemMessage("You are a helpful assistant. Make your answers short.");
      chat.AppendUserInput(prompt);

      await chat.StreamResponseFromChatbotAsync(resultHandler);
    }

    public string GeneratePrompt(string content)
    {
      switch (content)
      {
        case "Translate":
          return "You are a helpful translator. Please translate the following:";
        case "Explain":
          return "You are an AI tutor. Please explain in different levels of understanding.";
        case "Synonyms":
          return "You are a human thesaurus. Please provide synonyms.";
        case "Fix":
          return "You are an AI proofreader. Please correct any errors and make the text better.";
        default:
          return
            "You are a helpful assistant. If context is not provided, be helpful and guess what the user needs. Do not ask for clarification, give your best answer";
      }
    }

    public async Task<string> SendPrompt(string contextToggle, string prompt)
    {
      var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
      var key = KeyProvider.LoadAPIKey();
      if (string.IsNullOrEmpty(key))
      {
        throw new AuthenticationException("The OpenAI API Key is missing. To provide it, please open the sidebar and navigate to 'Settings', then enter your key into the appropriate field. This key is necessary for the application to function properly. If you don't have a key, please visit the OpenAI website to get one.");
      }
      request.Headers.Add("Authorization", $"Bearer {key}");
      request.Content = new StringContent(JsonConvert.SerializeObject(new
      {
        model = "gpt-3.5-turbo",
        messages = new[] {
                    new { role = "system", content = GeneratePrompt(contextToggle) },
                    new { role = "user", content = prompt } }
      }), Encoding.UTF8, "application/json");

      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseContent = await response.Content.ReadAsStringAsync();
      var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

      return responseObject.choices[0].message.content;
    }
  }
}
