using Microsoft.Owin.Security.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace RadioChannels.Controllers
{
    public class LinqToTwitterAuthenticationProvider : TwitterAuthenticationProvider
    {
        public const string AccessToken = "828778881524105219-mikcECgciWYtaQX5AEMpxBDEQDX83Hq";
        public const string AccessTokenSecret = "oMbGojydxGnL97PrTRX61xA0zR2M0i8WrFSCinl7bTInG";

        public override Task Authenticated(TwitterAuthenticatedContext context)
        {            
            context.Identity.AddClaims(
                new List<Claim>
                {
                new Claim(AccessToken, context.AccessToken),
                new Claim(AccessTokenSecret, context.AccessTokenSecret)
                });

            return base.Authenticated(context);
        }
    }
}