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
    public class Cat3Controller : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Cat3/

        public ActionResult Index(string word, int? page)
        {
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.cat3 where q.name.Contains(word) select q).OrderBy(o => o.name).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /Cat3/Details/5

        public ActionResult Details(int id = 0)
        {
            cat3 cat3 = db.cat3.Find(id);
            if (cat3 == null)
            {
                return HttpNotFound();
            }
            return View(cat3);
        }

        //
        // GET: /Cat3/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Cat3/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(cat3 cat3)
        {
            if (ModelState.IsValid)
            {
                cat3.no = 0;
                db.cat3.Add(cat3);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat3);
        }

        //
        // GET: /Cat3/Edit/5

        public ActionResult Edit(int id = 0)
        {
            cat3 cat3 = db.cat3.Find(id);
            if (cat3 == null)
            {
                return HttpNotFound();
            }
            return View(cat3);
        }

        //
        // POST: /Cat3/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(cat3 cat3)
        {
            if (ModelState.IsValid)
            {
                cat3.no = 0;
                db.Entry(cat3).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat3);
        }

        //
        // GET: /Cat3/Delete/5

        public ActionResult Delete(int id = 0)
        {
            cat3 cat3 = db.cat3.Find(id);
            if (cat3 == null)
            {
                return HttpNotFound();
            }
            return View(cat3);
        }

        //
        // POST: /Cat3/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cat3 cat3 = db.cat3.Find(id);
            db.cat3.Remove(cat3);
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