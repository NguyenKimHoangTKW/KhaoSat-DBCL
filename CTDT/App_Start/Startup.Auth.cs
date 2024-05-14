using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(CTDT.App_Start.Startup))]

namespace CTDT.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "183100229430-394rpj38v42o4kfgum7hvnjplnv3ebrl.apps.googleusercontent.com",
                ClientSecret = "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",
            });
        }     
    }
}
