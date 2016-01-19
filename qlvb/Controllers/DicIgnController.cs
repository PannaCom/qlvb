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
    public class DicIgnController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /DicIgn/

        public ActionResult Index(string word, int? page)
        {
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.dic_ignore where q.word.Contains(word) select q).OrderBy(o => o.word).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /DicIgn/Details/5

        public ActionResult Details(int id = 0)
        {
            dic_ignore dic_ignore = db.dic_ignore.Find(id);
            if (dic_ignore == null)
            {
                return HttpNotFound();
            }
            return View(dic_ignore);
        }

        //
        // GET: /DicIgn/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DicIgn/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dic_ignore dic_ignore)
        {
            if (ModelState.IsValid)
            {
                db.dic_ignore.Add(dic_ignore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dic_ignore);
        }

        //
        // GET: /DicIgn/Edit/5

        public ActionResult Edit(int id = 0)
        {
            dic_ignore dic_ignore = db.dic_ignore.Find(id);
            if (dic_ignore == null)
            {
                return HttpNotFound();
            }
            return View(dic_ignore);
        }

        //
        // POST: /DicIgn/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(dic_ignore dic_ignore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dic_ignore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dic_ignore);
        }

        //
        // GET: /DicIgn/Delete/5

        public ActionResult Delete(int id = 0)
        {
            dic_ignore dic_ignore = db.dic_ignore.Find(id);
            if (dic_ignore == null)
            {
                return HttpNotFound();
            }
            return View(dic_ignore);
        }

        //
        // POST: /DicIgn/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dic_ignore dic_ignore = db.dic_ignore.Find(id);
            db.dic_ignore.Remove(dic_ignore);
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