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
    public class Cat4Controller : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Cat4/

        public ActionResult Index(string word, int? page)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.cat4 where q.name.Contains(word) select q).OrderByDescending(o => o.no).ThenBy(o=>o.name).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /Cat4/Details/5

        public ActionResult Details(int id = 0)
        {
            cat4 cat4 = db.cat4.Find(id);
            if (cat4 == null)
            {
                return HttpNotFound();
            }
            return View(cat4);
        }

        //
        // GET: /Cat4/Create

        public ActionResult Create()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View();
        }

        //
        // POST: /Cat4/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(cat4 cat4)
        {
            if (ModelState.IsValid)
            {
                //cat4.no = 0;
                db.cat4.Add(cat4);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat4);
        }

        //
        // GET: /Cat4/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            cat4 cat4 = db.cat4.Find(id);
            if (cat4 == null)
            {
                return HttpNotFound();
            }
            return View(cat4);
        }

        //
        // POST: /Cat4/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(cat4 cat4)
        {
            if (ModelState.IsValid)
            {
                //cat4.no = 0;
                db.Entry(cat4).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat4);
        }

        //
        // GET: /Cat4/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            cat4 cat4 = db.cat4.Find(id);
            if (cat4 == null)
            {
                return HttpNotFound();
            }
            return View(cat4);
        }

        //
        // POST: /Cat4/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cat4 cat4 = db.cat4.Find(id);
            db.cat4.Remove(cat4);
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