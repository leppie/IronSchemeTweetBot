using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.ServiceProcess;

namespace IronSchemeTweetBot
{
    static class Program
    {
        static void Main()
        {
            var ServicesToRun = new ServiceBase[]
            {
        new Service()//.Run(); Console.ReadLine();
            }
            ; ServiceBase.Run(ServicesToRun);
        }

    }

    public class Service : ServiceBase
    {
        Thread runner;
        public Service()
        {
            ServiceName = "IronScheme Twitter bot";
            runner = new Thread(Run);
        }

        protected override void OnStart(string[] args)
        {
            runner.Start();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            runner.Abort();
            base.OnStop();
        }

        class Response
        {
#pragma warning disable 649
            public string result;
            public string output;
            public string error;
#pragma warning restore 649
        }

        public void Run()
        {
            var twitter = new TwitterService("", "");

            var requestToken = twitter.GetRequestToken();
            twitter.AuthenticateWith("", "");

            var wc = new WebClient();

            while (true)
            {
                try
                {
                    var tweets = twitter.ListTweetsMentioningMe(new ListTweetsMentioningMeOptions { SinceId = Properties.Settings.Default.SinceId });
                    if (tweets != null)
                    {
                        foreach (var m in tweets.OrderBy(x => x.CreatedDate))
                        {
                            Properties.Settings.Default.SinceId = m.Id;
                            var req = Regex.Match(m.Text, @"@ironscheme\s*:?(?<expr>.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            var expr = req.Groups["expr"].Value;
                            expr = System.Web.HttpUtility.HtmlDecode(expr);

                            var nv = new System.Collections.Specialized.NameValueCollection();
                            nv.Add("expr", expr);
                            var res = wc.UploadValues("http://eval.ironscheme.net/", nv);
                            var r = JsonConvert.DeserializeObject<Response>(Encoding.UTF8.GetString(res));
                            if (r.error != null && (r.error.Contains("&undefined") || r.error.Contains("&lexical") || r.error.Contains("&syntax")))
                            {
                                //ignore
                                continue;
                            }

                            if (r.result != null && r.result.Contains("unspecified") && !string.IsNullOrWhiteSpace(r.output))
                            {
                                twitter.SendTweet(new SendTweetOptions { Status = r.output });
                                if (m.Author.ScreenName == "leppie")
                                {
                                    continue;
                                }
                                else
                                {
                                    twitter.SendTweet(new SendTweetOptions { Status = string.Format("@{0}: Y U SINK MY SUBMARINE?", m.Author.ScreenName), InReplyToStatusId = m.Id });
                                    continue;
                                }
                            }

                            var mentions = m.Entities.Mentions.Select(x => "@" + x.ScreenName).Where(x => !string.Equals(x, "@IronScheme", StringComparison.InvariantCultureIgnoreCase)).Distinct().ToList();

                            var msg = (r.result ?? r.error).Trim();

                            if (string.IsNullOrWhiteSpace(msg))
                            {
                                continue;
                            }

                            if (mentions.Any())
                            {
                                msg += ("\n(cc " + string.Join(" ", mentions) + ")");
                            }

                            twitter.SendTweet(new SendTweetOptions { Status = string.Format("@{0}: {1}", m.Author.ScreenName, msg), InReplyToStatusId = m.Id });
                        }
                        Properties.Settings.Default.Save();
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Launch();
                    Debugger.Break();
                    Console.WriteLine(ex);
                }
                Thread.Sleep(60000);
            }
        }
    }
}

