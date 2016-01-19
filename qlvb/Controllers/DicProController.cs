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
    public class DicProController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /DicPro/

        public ActionResult Index(string word, int? page)
        {
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.dic_pro where q.word.Contains(word) select q).OrderBy(o => o.word).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /DicPro/Details/5

        public ActionResult Details(int id = 0)
        {
            dic_pro dic_pro = db.dic_pro.Find(id);
            if (dic_pro == null)
            {
                return HttpNotFound();
            }
            return View(dic_pro);
        }

        //
        // GET: /DicPro/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DicPro/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dic_pro dic_pro)
        {
            if (ModelState.IsValid)
            {
                db.dic_pro.Add(dic_pro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dic_pro);
        }

        //
        // GET: /DicPro/Edit/5

        public ActionResult Edit(int id = 0)
        {
            dic_pro dic_pro = db.dic_pro.Find(id);
            if (dic_pro == null)
            {
                return HttpNotFound();
            }
            return View(dic_pro);
        }

        //
        // POST: /DicPro/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(dic_pro dic_pro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dic_pro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dic_pro);
        }

        //
        // GET: /DicPro/Delete/5

        public ActionResult Delete(int id = 0)
        {
            dic_pro dic_pro = db.dic_pro.Find(id);
            if (dic_pro == null)
            {
                return HttpNotFound();
            }
            return View(dic_pro);
        }

        //
        // POST: /DicPro/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dic_pro dic_pro = db.dic_pro.Find(id);
            db.dic_pro.Remove(dic_pro);
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