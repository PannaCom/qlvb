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
            var p = (from q in db.cat2 where keyword.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat3(string keyword)
        {
            var p = (from q in db.cat3 where keyword.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat4(string keyword)
        {
            var p = (from q in db.cat4 where keyword.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat(int type)
        {
            switch(type){
                case 1:
                    var p = (from q in db.cat1 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p);
                    break;
                case 2:
                    var p2 = (from q in db.cat2 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p2);
                    break;
                case 3:
                    var p3 = (from q in db.cat3 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p3);
                    break;
                case 4:
                    var p4 = (from q in db.cat4 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p4);
                    break;
            }
            return "";
        }
    }
}
