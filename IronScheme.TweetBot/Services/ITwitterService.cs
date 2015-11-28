using TweetSharp;

namespace IronScheme.TweetBot.Services
{
    public interface ITwitterService
    {
        void Tweet(string msg);
        void Reply(long id, string user, string msg);
        TwitterStatus[] GetMentions(long sinceId);
    }
}