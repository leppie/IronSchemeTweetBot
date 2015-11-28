namespace IronScheme.TweetBot.Helpers
{
    public interface ITweetMatcher
    {
        string Match(string msg);
    }
}