using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;
using PagedList;
using Newtonsoft.Json;
namespace qlvb.Controllers
{
    public class HomeController : Controller
    {
        private qlvbEntities db = new qlvbEntities();
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public string getCat2(string keyword)
        {
            var p = (from q in db.cat2 where keyword.Contains(keyword) select q.name).Distinct().Take(20).ToList();
            return JsonConvert.SerializeObject(p);
        }
    }
}
