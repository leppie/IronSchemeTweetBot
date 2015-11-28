namespace IronScheme.TweetBot.Models
{
    public class Response
    {
        public string Result { get; set; }

        public string Output { get; set; }

        public string Error { get; set; }

        public bool HasErrors()
        {
            return Error != null && (Error.Contains("&undefined") || Error.Contains("&lexical") || Error.Contains("&syntax"));
        }

        public bool HasUnspecifiedBehavior()
        {
            return Result != null && Result.Contains("unspecified") && !string.IsNullOrWhiteSpace(Output);
        }
    }
}