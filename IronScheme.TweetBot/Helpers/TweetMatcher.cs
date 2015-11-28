using System.Net;
using System.Text.RegularExpressions;

namespace IronScheme.TweetBot.Helpers
{
    public class TweetMatcher : ITweetMatcher
    {
        private static readonly Regex pattern = new Regex(@"@ironscheme\s*:?(?<expr>.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public string Match(string msg)
        {
            var match = pattern.Match(msg);
            var expression = WebUtility.HtmlDecode(match.Groups["expr"].Value);
            return expression;
        }
    }
}