using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;

namespace TwitterClean.Controllers
{
    public class TwitterController : Controller
    {
        //
        // GET: /Twitter/

        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Authorize()
		{
			// Step 1 - Retrieve an OAuth Request Token
			TwitterService service = new TwitterService("RP210WVfv1GbkYOd3oaHw", "z6p22FQTTU3ADIpjp6PThKyVWLAmikUfUfiOdycfm4w");

			// This is the registered callback URL
			OAuthRequestToken requestToken = service.GetRequestToken("http://localhost:8081/Twitter/AuthorizeCallback");

			// Step 2 - Redirect to the OAuth Authorization URL
			Uri uri = service.GetAuthorizationUri(requestToken);
			return new RedirectResult(uri.ToString(), false /*permanent*/);
		}

		// This URL is registered as the application's callback at http://dev.twitter.com
		public ActionResult AuthorizeCallback(string oauth_token, string oauth_verifier)
		{
			var requestToken = new OAuthRequestToken { Token = oauth_token };

			// Step 3 - Exchange the Request Token for an Access Token
			TwitterService service = new TwitterService("RP210WVfv1GbkYOd3oaHw", "z6p22FQTTU3ADIpjp6PThKyVWLAmikUfUfiOdycfm4w");
			OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);

			// Step 4 - User authenticates using the Access Token
			service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

			IEnumerable<TwitterStatus> tweets = service.ListTweetsOnUserTimeline(250);

			if (tweets != null)
			{
				@ViewBag.msg += string.Format("<p>{0} tweets</p>", tweets.Count());

				foreach (var tweet in tweets)
				{
					TwitterStatus result = service.DeleteTweet(tweet.Id);
					@ViewBag.msg += string.Format("<p>{0} - {1}</p>", tweet.Id, tweet.Text);
				}
			}
			else
			{
				@ViewBag.msg += string.Format("<p>{0} tweets</p>", 0);
			}
			
			@ViewBag.msg += "<hr>";

			IEnumerable<TwitterStatus> tweets1 = service.ListRetweetsByMe(250);

			if (tweets1 != null)
			{
				@ViewBag.msg += string.Format("<p>{0} retweets</p>", tweets1.Count());

				foreach (var tweet in tweets1)
				{
					TwitterStatus result = service.DeleteTweet(tweet.Id);
					@ViewBag.msg += string.Format("<p>{0} - {1}</p>", tweet.Id, tweet.Text);
				}
			}
			else
			{
				@ViewBag.msg += string.Format("<p>{0} retweets</p>", 0);
			}
			
			//TwitterUser user = service.VerifyCredentials();
			//@ViewBag.msg = string.Format("Your username is {0}", user.ScreenName);

			return View();
		}
    }
}

