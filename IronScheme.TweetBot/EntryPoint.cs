using IronScheme.TweetBot.Helpers;
using IronScheme.TweetBot.Services;
using Topshelf;

namespace IronScheme.TweetBot
{
    internal static class EntryPoint
    {
        private static string NAME = "TweetBot";
        private static string DESCRIPTION = "IronScheme TweetBot";

        private static void Main()
        {
            HostFactory.Run(c =>
            {
                c.Service<ServiceHandler>(s =>
                {
                    s.ConstructUsing(_ =>
                    {
                        var settings = new Settings.Settings();
                        return new ServiceHandler(settings, new TwitterService(settings), new TweetMatcher(), new SandboxService(settings));
                    });
                    s.WhenStarted(x => x.Start());
                    s.WhenStopped(x => x.Stop());
                });
                c.RunAsLocalSystem();
                c.StartAutomatically();
                c.SetDescription(DESCRIPTION);
                c.SetDisplayName(NAME);
                c.SetServiceName(NAME);
            });
        }
    }
}