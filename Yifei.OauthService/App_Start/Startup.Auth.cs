using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Yifei.Models;
using Yifei.Services.User;

namespace Yifei.OauthService
{

    public partial class Startup
    {
        /// <summary>
        /// AccessToken 失效时间
        /// </summary>
        static TimeSpan AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(10);
        public void ConfigureAuth(IAppBuilder app)
        {
            //启用程序内登陆 Cookie 的身份票据保存
            // Enable Application Sign In Cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Application",
                AuthenticationMode = AuthenticationMode.Passive,
                LoginPath = new PathString(""),
                LogoutPath = new PathString(""),
                //ExpireTimeSpan = TimeSpan.FromSeconds(30)
            });


            //启用外部登陆的 Cookie 身份票据保存
            // Enable External Sign In Cookie
            app.SetDefaultSignInAsAuthenticationType("External");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "External",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "External",
                ExpireTimeSpan = TimeSpan.FromMinutes(15),
            });

            //设置授权服务
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                ApplicationCanDisplayErrors = true,
                AllowInsecureHttp = true,
                AuthorizeEndpointPath = new PathString("/OAuth/Authorize"),
                TokenEndpointPath = new PathString("/OAuth/Token"),

                //
                Provider = new OAuthAuthorizationServerProvider()
                {
                    OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    OnGrantClientCredentials = GrantClientCredetails,
                    //OnGrantRefreshToken = GrantRefreshToken,
                },

                // 设置授权码提供规则
                AuthorizationCodeProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },

                // 设置刷新令牌提供规则，这里可以做令牌的数据库持久化操作，防止程序重启后造成的令牌丢失。
                RefreshTokenProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                    OnCreateAsync = CreateRefreshTokenAsync,
                    OnReceiveAsync = ReceiveRefreshTokenAsync,
                },
                AccessTokenExpireTimeSpan = AccessTokenExpireTimeSpan,// AccessToken 失效时间。

                //AccessTokenProvider = new AuthenticationTokenProvider
                //{
                //    OnCreate = CreateAccessToken,
                //    OnReceive = ReceiveAccessToken,
                //}
            });
        }
        /// <summary>
        /// 收到刷新令牌
        /// </summary>
        /// <param name="obj"></param>
        private Task ReceiveRefreshTokenAsync(AuthenticationTokenReceiveContext arg)
        {
            YFPLUS_RefreshToken model = null;

            Guid ID = Guid.Parse(arg.Token);


            using (DSCSYSEntities context = new DSCSYSEntities())
            {
                model = context.YFPLUS_RefreshToken.Find(ID);
                if (model == null) return Task.FromResult(0);
                context.YFPLUS_RefreshToken.Remove(model);
                context.SaveChanges();
            }

            //解码保护资源
            arg.DeserializeTicket(model.ProtectedTicket);

            //因为原 Ticket 已经失效，所以才要刷新，这里需要新生成一个 Ticket 。
            DateTime now = DateTime.UtcNow;

            Dictionary<string, string> param = new Dictionary<string, string>();
            foreach (var kv in arg.Ticket.Properties.Dictionary)
            {
                if (kv.Key != ".issued" && kv.Key != ".expires")
                {
                    param.Add(kv.Key, kv.Value);
                }
            }

            param.Add(".issued", now.ToString("R"));
            param.Add(".expires", (now + AccessTokenExpireTimeSpan).ToString("R"));

            AuthenticationTicket ticket = new AuthenticationTicket(arg.Ticket.Identity, new AuthenticationProperties(param));

            arg.SetTicket(ticket);

            return Task.FromResult(0);
        }
        /// <summary>
        /// 创建刷新令牌
        /// </summary>
        /// <param name="obj"></param>
        private Task CreateRefreshTokenAsync(AuthenticationTokenCreateContext arg)
        {
            int client_id = int.Parse(arg.Ticket.Identity.FindFirst("ClientID").Value);
            int? user_id = null;
            if (arg.Ticket.Identity.FindFirst("UserID") != null)
                user_id = int.Parse(arg.Ticket.Identity.FindFirst("UserID").Value);

            string ticket = arg.SerializeTicket();

            string refreshToken = GetRefreshToken(client_id, user_id, ticket);

            arg.SetToken(refreshToken);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 将核心信息保存到数据库中，并返回 RefreshTokenID
        /// </summary>
        /// <param name="client_id"></param>
        /// <param name="user_id"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private static string GetRefreshToken(int client_id, int? user_id, string ticket)
        {
            var now = DateTime.UtcNow;

            YFPLUS_RefreshToken refreshTokenModel = new YFPLUS_RefreshToken()
            {
                ClientID = client_id,
                UserID = user_id,
                IssuedUtc = now,
                ExpiresUtc = now.AddHours(3),
                ProtectedTicket = ticket
            };

            using (DSCSYSEntities context = new DSCSYSEntities())
            {
                context.YFPLUS_RefreshToken.Add(refreshTokenModel);
                context.SaveChanges();
            }
            string refreshToken = refreshTokenModel.ID.ToString("N");
            return refreshToken;
        }

        /// <summary>
        /// 收到刷新令牌 -- 无效
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext obj)
        {
            throw new NotImplementedException();

            //obj.DeserializeTicket(obj.Token);

            //var tick = obj.Ticket;
        }
        /// <summary>
        /// 创建刷新令牌 -- 无效
        /// </summary>
        /// <param name="obj"></param>
        private void CreateRefreshToken(AuthenticationTokenCreateContext obj)
        {
            throw new NotImplementedException();

            //obj.SetToken(obj.SerializeTicket());
        }
        /// <summary>
        /// 收到验证码
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext obj)
        {
            string val = (string)HttpContext.Current.Cache.Get(obj.Token);

            if (!string.IsNullOrEmpty(val))
            {
                HttpContext.Current.Cache.Remove(obj.Token);
                obj.DeserializeTicket(val);
            }
        }
        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="obj"></param>
        private void CreateAuthenticationCode(AuthenticationTokenCreateContext obj)
        {
            obj.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            //10 分钟的缓存，过了 10 分钟，该 code 则失效，需要重新请求
            HttpContext.Current.Cache.Add(obj.Token, obj.SerializeTicket(), null, DateTime.Now.AddMinutes(10), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// 当客户端请求类型为 "grant_type=client_credentials" 时触发
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task GrantClientCredetails(OAuthGrantClientCredentialsContext arg)
        {
            YFPLUS_Client client = null;
            using (DSCSYSEntities context = new DSCSYSEntities())
            {
                client = context.YFPLUS_Client.AsNoTracking().AsQueryable().SingleOrDefault(s => s.ClientIdentify == arg.ClientId);
            }
            var claims = new List<Claim>();
            claims.Add(new Claim("ClientID", client.ID + ""));
            claims.AddRange(arg.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            var identity = new ClaimsIdentity(new GenericIdentity(client.Name, OAuthDefaults.AuthenticationType), claims);

            arg.Validated(identity);

            return Task.FromResult(0);
        }
        /// <summary>
        /// 授予资源所有者凭据
        /// 当客户端请求为 grant_type=password 时触发
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext arg)
        {
            //这里需要验证用户登录信息以及客户端信息
            DSCMA user = null;
            YFPLUS_Client client = null;
            using (DSCSYSEntities context = new DSCSYSEntities())
            {
                user = new UserService().GetUser(arg.UserName, arg.Password);              
                client = context.YFPLUS_Client.AsNoTracking().FirstOrDefault(s => s.ClientIdentify == arg.ClientId);
            }
            if (user != null)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("ClientID", client.ID + ""));
                claims.AddRange(arg.Scope.Select(x => new Claim("urn:oauth:scope", x)));
                claims.Add(new Claim("UserID", user.MA001 + ""));

                var identity = new ClaimsIdentity(
                new GenericIdentity(user.MA001, OAuthDefaults.AuthenticationType), claims);
                arg.Validated(identity);
            }



            return Task.FromResult(0);
        }

        /// <summary>
        /// 验证客户端认证信息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext arg)
        {
            try
            {
                string clientId;
                string clientSecret;
                if (arg.TryGetBasicCredentials(out clientId, out clientSecret))
                {
                    using (DSCSYSEntities context = new DSCSYSEntities())
                    {
                        if (context.YFPLUS_Client.AsNoTracking().AsQueryable().Any(s => s.ClientIdentify == clientId && s.ClientSecret == clientSecret))
                        {
                            arg.Validated();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                arg.SetError("unknow error", ex.Message);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// 验证客户端回调URL
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private System.Threading.Tasks.Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext arg)
        {
            try
            {
                using (DSCSYSEntities context = new DSCSYSEntities())
                {
                    var client = context.YFPLUS_Client.AsNoTracking().AsQueryable().FirstOrDefault(s => s.ClientIdentify == arg.ClientId);
                    if (client == null) arg.SetError("客户端失效", "客户端失效");
                    else
                    {
                        arg.Validated(client.RedirectUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                arg.SetError("unknow error", ex.Message);
            }
            return Task.FromResult(0);
        }
    }
}