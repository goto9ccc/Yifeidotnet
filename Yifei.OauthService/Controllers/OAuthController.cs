using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Yifei.Models;

namespace Yifei.OauthService.Controllers
{
    public class OAuthController : Controller
    {

        // GET: OAuth
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');

            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
                {
                    identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                    foreach (var scope in scopes)
                    {
                        identity.AddClaim(new Claim("urn:oauth:scope", scope));
                    }
                    string client_id = Request.QueryString["client_id"];
                    using (DSCSYSEntities context = new DSCSYSEntities())
                    {
                        var client = context.YFPLUS_Client.AsNoTracking().AsQueryable().FirstOrDefault(s => s.ClientIdentify == client_id);
                        client_id = client.ID + "";
                    }
                    identity.AddClaim(new Claim("ClientID", client_id));
                    authentication.SignIn(identity);
                }
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
                {
                    authentication.SignOut("Application");
                    authentication.Challenge("Application");
                    return new HttpUnauthorizedResult();
                }
            }
            return View();
        }
    }
}