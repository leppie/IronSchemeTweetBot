using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using IronScheme.TweetBot.Helpers;
using IronScheme.TweetBot.Models;
using IronScheme.TweetBot.Properties;
using IronScheme.TweetBot.Settings;
using TweetSharp;

namespace IronScheme.TweetBot.Services
{
    public class ServiceHandler : IServiceHandler
    {
        private readonly ISettings settings;
        private readonly ITwitterService twitterService;
        private readonly ITweetMatcher tweetMatcher;
        private readonly ISandboxService sandboxService;

        private Timer timer;

        public ServiceHandler()
        {
        }

        public ServiceHandler(ISettings settings, ITwitterService twitterService, ITweetMatcher tweetMatcher, ISandboxService sandboxService)
        {
            this.settings = settings;
            this.twitterService = twitterService;
            this.tweetMatcher = tweetMatcher;
            this.sandboxService = sandboxService;
        }

        public void Start()
        {
            timer = new Timer(state =>
            {
                try
                {
                    var tweets = twitterService.GetMentions(settings.SinceId);
                    if (tweets != null)
                    {
                        foreach (var tweet in tweets.OrderBy(x => x.CreatedDate))
                        {
                            settings.UpdateSinceId(tweet.Id);

                            var expression = tweetMatcher.Match(tweet.Text);
                            var response = sandboxService.Send(expression);

                            // Ignore
                            if (response.HasErrors()) continue;

                            if (response.HasUnspecifiedBehavior())
                            {
                                HandleUnspecifiedBehavior(response, tweet);
                                continue;
                            }

                            var message = GetMessage(response);
                            if (string.IsNullOrWhiteSpace(message))
                                continue;

                            Send(tweet, message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Launch();
                    Debugger.Break();
                    Console.WriteLine(ex);
                }
            }, null, settings.DueTime, settings.Timeout);
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }

        private void HandleUnspecifiedBehavior(Response response, TwitterStatus tweet)
        {
            twitterService.Tweet(response.Output);
            if (tweet.Author.ScreenName == settings.Grandpa) return;
            twitterService.Reply(tweet.Id, tweet.Author.ScreenName, Resources.Message);
        }

        private void Send(TwitterStatus tweet, string message)
        {
            var twitterMentions = tweet.Entities.Mentions;
            var mentions = twitterMentions.Select(x => "@" + x.ScreenName)
                                          .Where(x => !string.Equals(x, "@IronScheme", StringComparison.InvariantCultureIgnoreCase))
                                          .Distinct()
                                          .ToArray();
            if (mentions.Length > 0)
            {
                message += "\n(cc " + string.Join(" ", mentions) + ")";
            }
            twitterService.Reply(tweet.Id, tweet.Author.ScreenName, message);
        }

        private static string GetMessage(Response response)
        {
            return (response.Result ?? response.Error).Trim();
        }
    }
}