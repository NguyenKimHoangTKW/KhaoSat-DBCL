﻿using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(CTDT.App_Start.Startup))]

namespace CTDT.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login"),
            });

            // Enable external Google authentication
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "183100229430-394rpj38v42o4kfgum7hvnjplnv3ebrl.apps.googleusercontent.com",
                ClientSecret = "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",
            });
        }
    }
}
