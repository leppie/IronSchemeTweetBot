namespace IronScheme.TweetBot.Settings
{
    public class Settings : ISettings
    {
        public Settings()
        {
            ConsumerSecret = "";
            ConsumerKey = "";
            Token = "37156582-";
            TokenSecret = "";
            Timeout = 60*1000;
            DueTime = 30*1000;
            SinceId = 646808514627772416;
            Uri = "http://eval.ironscheme.net/";
            Grandpa = "leppie";
        }

        public string ConsumerSecret { get; private set; }

        public string ConsumerKey { get; private set; }

        public string Token { get; private set; }

        public string TokenSecret { get; private set; }
        
        public int Timeout { get; private set; }
        
        public int DueTime { get; private set; }

        public long SinceId { get; private set; }
        
        public string Uri { get; private set; }
        
        public string Grandpa { get; private set; }

        public void UpdateSinceId(long sinceId)
        {
            SinceId = sinceId;
        }
    }
}