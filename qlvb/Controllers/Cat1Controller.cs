using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;

namespace qlvb.Controllers
{
    public class Cat1Controller : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Cat1/

        public ActionResult Index()
        {
            return View(db.cat1.ToList());
        }

        //
        // GET: /Cat1/Details/5

        public ActionResult Details(int id = 0)
        {
            cat1 cat1 = db.cat1.Find(id);
            if (cat1 == null)
            {
                return HttpNotFound();
            }
            return View(cat1);
        }

        //
        // GET: /Cat1/Create

        public ActionResult Create()
        {
            ViewBag.cat1 = "1";
            return View();
        }

        //
        // POST: /Cat1/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(cat1 cat1)
        {
            if (ModelState.IsValid)
            {
                cat1.no = 0;
                db.cat1.Add(cat1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat1);
        }

        //
        // GET: /Cat1/Edit/5

        public ActionResult Edit(int id = 0)
        {
            cat1 cat1 = db.cat1.Find(id);
            if (cat1 == null)
            {
                return HttpNotFound();
            }
            return View(cat1);
        }

        //
        // POST: /Cat1/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(cat1 cat1)
        {
            if (ModelState.IsValid)
            {
                cat1.no = 0;
                db.Entry(cat1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat1);
        }

        //
        // GET: /Cat1/Delete/5

        public ActionResult Delete(int id = 0)
        {
            cat1 cat1 = db.cat1.Find(id);
            if (cat1 == null)
            {
                return HttpNotFound();
            }
            return View(cat1);
        }

        //
        // POST: /Cat1/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cat1 cat1 = db.cat1.Find(id);
            db.cat1.Remove(cat1);
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