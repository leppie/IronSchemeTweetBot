using System.Linq;
using IronScheme.TweetBot.Settings;
using TweetSharp;

namespace IronScheme.TweetBot.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly ISettings settings;
        private TweetSharp.ITwitterService twitterService;

        public TwitterService(ISettings settings)
        {
            this.settings = settings;
        }

        public void Tweet(string msg)
        {
            Twitter.SendTweet(new SendTweetOptions {Status = msg});
        }

        public void Reply(long id, string user, string msg)
        {
            Twitter.SendTweet(new SendTweetOptions
            {
                Status = string.Format("@{0}: {1}", user, msg),
                InReplyToStatusId = id
            });
        }

        public TwitterStatus[] GetMentions(long sinceId)
        {
            var options = new ListTweetsMentioningMeOptions {SinceId = sinceId};
            return Twitter.ListTweetsMentioningMe(options).ToArray();
        }

        private TweetSharp.ITwitterService Twitter
        {
            get
            {
                if (twitterService != null)
                    return twitterService;
                twitterService = new TweetSharp.TwitterService(settings.ConsumerKey, settings.ConsumerSecret);
                twitterService.GetRequestToken();
                twitterService.AuthenticateWith(settings.Token, settings.TokenSecret);
                return twitterService;
            }
        }
    }
}