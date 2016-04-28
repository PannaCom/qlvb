using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;
using PagedList;
using Newtonsoft.Json;
namespace qlvb.Controllers
{
    public class Cat2Controller : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Cat2/

        public ActionResult Index(string word,int? page)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.cat2 where q.name.Contains(word) select q).OrderByDescending(o => o.no).ThenBy(o => o.name).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /Cat2/Details/5

        public ActionResult Details(int id = 0)
        {
            cat2 cat2 = db.cat2.Find(id);
            if (cat2 == null)
            {
                return HttpNotFound();
            }
            return View(cat2);
        }

        //
        // GET: /Cat2/Create

        public ActionResult Create()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View();
        }

        //
        // POST: /Cat2/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(cat2 cat2)
        {

            if (ModelState.IsValid)
            {
                //cat2.no = 0;
                db.cat2.Add(cat2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat2);
        }

        //
        // GET: /Cat2/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            cat2 cat2 = db.cat2.Find(id);
            if (cat2 == null)
            {
                return HttpNotFound();
            }
            return View(cat2);
        }

        //
        // POST: /Cat2/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(cat2 cat2)
        {
            if (ModelState.IsValid)
            {
                //cat2.no = 0;
                db.Entry(cat2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat2);
        }

        //
        // GET: /Cat2/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            cat2 cat2 = db.cat2.Find(id);
            if (cat2 == null)
            {
                return HttpNotFound();
            }
            return View(cat2);
        }

        //
        // POST: /Cat2/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cat2 cat2 = db.cat2.Find(id);
            db.cat2.Remove(cat2);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}