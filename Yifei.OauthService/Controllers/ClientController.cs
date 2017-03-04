using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yifei.Models;

namespace Yifei.OauthService.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client

        DSCSYSEntities context = new DSCSYSEntities();
        // GET: Client
        public ActionResult Index()
        {
            var list = context.
                YFPLUS_Client.AsNoTracking().AsQueryable();
            return View(list.ToList());
        }

        public ActionResult Edit(int? ID)
        {
            var entity = context.YFPLUS_Client.Find(ID) ?? new YFPLUS_Client();
            if (entity.ID == 0)
            {
                //生成ID与密串
                entity.ClientIdentify = Guid.NewGuid().ToString("N");
                entity.ClientSecret = Guid.NewGuid().ToString("N");
            }
            return View(entity);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(int? ID, string Password)
        {
            int status = 500;
            string msg = null;
            try
            {
                var entity = context.YFPLUS_Client.Find(ID) ?? new YFPLUS_Client();
                TryUpdateModel(entity, null, Request.Form.AllKeys);



                if (context.YFPLUS_Client.AsQueryable().Any(s => s.ID != entity.ID && s.ClientIdentify == entity.ClientIdentify))
                    throw new Exception(string.Format("ID {0} 已被使用！", entity.ClientIdentify));

                if (entity.ID == 0)
                {
                    context.YFPLUS_Client.Add(entity);
                }
                else
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                status = 200;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Status = status, Message = msg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int? ID)
        {
            int status = 500;
            string msg = null;
            try
            {
                var entity = context.YFPLUS_Client.Find(ID);
                if (entity != null)
                {
                    context.YFPLUS_Client.Remove(entity);
                    context.SaveChanges();
                }
                status = 200;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Status = status, Message = msg });
        }
    }
}