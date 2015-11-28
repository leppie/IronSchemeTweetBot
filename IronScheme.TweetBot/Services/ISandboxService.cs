using IronScheme.TweetBot.Models;

namespace IronScheme.TweetBot.Services
{
    public interface ISandboxService
    {
        Response Send(string expression);
    }
}