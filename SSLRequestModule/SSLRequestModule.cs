using System;
using System.Web;
using System.Collections.Specialized;

using CMS;
using CMS.DataEngine;
using CMS.Base;
using CMS.Helpers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;


// Registers the custom module into the system
[assembly: RegisterModule(typeof(SSLRequestModule))]

public class SSLRequestModule : Module
{
    // Module class constructor, the system registers the module under the name "SSLRequests"
    public SSLRequestModule()
        : base("SSLRequests")
    {
    }

    // Contains initialization code that is executed when the application starts
    protected override void OnInit()
    {
        base.OnInit();

        // Assigns a handler called before each request is processed
        RequestEvents.Prepare.Execute += HandleSSLRequests;

        // Sets the URL port used for HTTPS requests
        URLHelper.SSLUrlPort = 443;
    }

    // Checks if requests are forwarded as SSL
    private static void HandleSSLRequests(object sender, EventArgs e)
    {
#if NETFRAMEWORK
        if ((HttpContext.Current != null) && (HttpContext.Current.Request != null))
        {
            // Loads the request headers as a collection
            NameValueCollection headers = HttpContext.Current.Request.Headers;

            // Gets the value from the X-Forwarded-Ssl header
            string forwardedSSL = headers.Get("X-Forwarded-Ssl");
            string apacheForwardedSSL = headers.Get("X-Forwarded-Proto");
            RequestContext.IsSSL = false;

            // Checks if the original request used HTTPS
            if (String.Equals(forwardedSSL, "on", StringComparison.OrdinalIgnoreCase) || String.Equals(apacheForwardedSSL, "https", StringComparison.OrdinalIgnoreCase))
            {
                RequestContext.IsSSL = true;
            }
        }
#endif
    }
}