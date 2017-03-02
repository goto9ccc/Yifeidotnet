using Microsoft.Practices.Unity;
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
        [Dependency]
        protected DSCSYSEntities dbContext { get; set; }

    }
}
