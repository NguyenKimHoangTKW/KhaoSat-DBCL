using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(CTDT.App_Start.Startup))]

namespace CTDT.App_Start
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "183100229430-394rpj38v42o4kfgum7hvnjplnv3ebrl.apps.googleusercontent.com",
                ClientSecret = "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",
            });
        }     
    }
}
