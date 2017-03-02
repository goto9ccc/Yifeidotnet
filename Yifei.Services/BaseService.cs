using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yifei.Models;

namespace Yifei.Services
{
    public class BaseService
    {
        public static object Test()
        {
            DSCSYSEntities db = new DSCSYSEntities();
            return db.DSCMA.ToList();
        }
    }
}
