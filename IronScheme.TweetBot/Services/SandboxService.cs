using System.Collections.Specialized;
using System.Net;
using System.Text;
using IronScheme.TweetBot.Models;
using IronScheme.TweetBot.Settings;
using Newtonsoft.Json;

namespace IronScheme.TweetBot.Services
{
    public class SandboxService : ISandboxService
    {
        private readonly ISettings settings;
        private readonly WebClient webClient = new WebClient();

        public SandboxService(ISettings settings)
        {
            this.settings = settings;
        }

        public Response Send(string expression)
        {
            var nameValueCollection = new NameValueCollection { { "expr", expression } };
            var response = webClient.UploadValues(settings.Uri, nameValueCollection);
            return JsonConvert.DeserializeObject<Response>(Encoding.UTF8.GetString(response));
        }
    }
}