using System;
using System.Linq;
using Yifei.Models;

namespace Yifei.Services.User
{
    public class UserService : BaseService
    {
        private string EnUser(char[] s)
        {

            string fnil = "'(&.&!'%&$\"'&)\" \",&)$(%#$-$#$$\" ";
            string FF1 = " !\"#$%&'()*+,-./";
            int n, i, d;
            string Result;
            i = s.Length;
            Result = fnil.Substring((i - 1) * 2, 30 - (i - 1) * 2);
            for (n = i - 1; n >= 0; n--)
            {
                i = (Convert.ToInt32(s[n]) - 32) % 16;
                d = ((Convert.ToInt32(s[n]) - 32) / 16) + 1;
                Result = Result + Convert.ToChar(32 + d + 1) + FF1[i];
            }
            return Result;
        }


        private string Encode(string EnStr, char[] s)
        {
            int i, n1, n2;
            char FStr1, FStr2, FStr3, Fchar1;
            string Result = EnStr;
            for (i = 0; i < s.Length; i++)
            {
                if (i <= 3)
                {
                    FStr1 = s[i];
                    FStr2 = Result[i];
                    FStr3 = Result[28 + i];
                    n1 = ((Convert.ToInt32(s[i]) - 32) % 8);
                    n2 = ((Convert.ToInt32(s[i]) - 32) / 16);
                    Fchar1 = Convert.ToChar(n2 * 16 + 32);
                    n2 = Convert.ToInt32(FStr2) ^ Convert.ToInt32(FStr1);
                    n2 = (n2 & 0x0F) + 0x20;
                    FStr2 = Convert.ToChar(n2);
                    FStr3 = Convert.ToChar(Convert.ToInt32(Fchar1) + ((Convert.ToInt32(FStr3) + Convert.ToInt32(Fchar1)) % 16));
                    Result = Result.Substring(0, i) + FStr2 + Result.Substring(i + 1, 31 - i);
                    Result = Result.Substring(0, 28 + i) + FStr3 + Result.Substring(29 + i, 3 - i);
                }
                else
                {
                    FStr1 = s[i];
                    FStr2 = Result[i];
                    FStr3 = Result[i - 4];
                    n1 = ((Convert.ToInt32(s[i]) - 32) % 16);
                    n2 = ((Convert.ToInt32(s[i]) - 32) / 16);
                    Fchar1 = Convert.ToChar(n2 * 16 + 32);
                    n2 = Convert.ToInt32(FStr2) ^ Convert.ToInt32(FStr1);
                    n2 = (n2 & 0x0F) + 0x20;
                    FStr2 = Convert.ToChar(n2);
                    FStr3 = Convert.ToChar(Convert.ToInt32(Fchar1) + ((Convert.ToInt32(FStr3) + Convert.ToInt32(Fchar1)) % 16));
                    Result = Result.Substring(0, i) + FStr2 + Result.Substring(i + 1, 31 - i);
                    Result = Result.Substring(0, i - 4) + FStr3 + Result.Substring(i - 3, 35 - i);
                }
            }
            return Result;
        }


        /// <summary>
        /// 专给OAUTH用的，不想引用注入框架了
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DSCMA GetUser(string username, string password)
        {
            using (dbContext = new DSCSYSEntities())
            {
                string pwd = Encode(EnUser(username.ToCharArray()),
                    password.ToCharArray());
                DSCMA dscma = dbContext.DSCMA.Where(o => o.MA001.Equals(username)
                 && o.MA003.Equals(pwd))
                    .FirstOrDefault();
                return dscma;
            }
        }


        public bool Login(string username, string password)
        {
            string pwd = Encode(EnUser(username.ToCharArray()),
                password.ToCharArray());
            DSCMA dscma = dbContext.DSCMA.Where(o => o.MA001.Equals(username))
                .FirstOrDefault();
            if (dscma == null)
            {
                return false;
            }
            if (!dscma.MA003.Equals(pwd))
            {
                return false;
            }
            return true;
        }

    }
}
