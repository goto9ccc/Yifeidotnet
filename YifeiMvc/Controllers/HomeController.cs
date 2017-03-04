﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yifei.Framework.Common;
using Yifei.Framework.Filters;
using Yifei.Services;

namespace YifeiMvc.Controllers
{
    
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
    }
}