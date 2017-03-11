using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Yifei.SelfHost.Controllers
{
    public class HomeController: ApiController
    {

        public object Index()
        {
            return "ok";
        }
        
    }
}
