namespace IronScheme.TweetBot.Settings
{
    public interface ISettings
    {
        string ConsumerKey { get; }

        string ConsumerSecret { get; }

        string Token { get; }

        string TokenSecret { get; }

        int Timeout { get; }

        int DueTime { get; }

        long SinceId { get; }

        string Uri { get; }

        string Grandpa { get; }

        void UpdateSinceId(long sinceId);
    }
}