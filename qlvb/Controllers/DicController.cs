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
using System.IO;
using System.Collections;

namespace qlvb.Controllers
{
    public class DicController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Dic/

        public ActionResult Index(string word, int? page)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (word == null) word = "";
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var p = (from q in db.dic_normal where q.word.Contains(word) select q).OrderBy(o => o.word).Take(30000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }

        //
        // GET: /Dic/Details/5

        public ActionResult Details(int id = 0)
        {
            dic_normal dic_normal = db.dic_normal.Find(id);
            if (dic_normal == null)
            {
                return HttpNotFound();
            }
            return View(dic_normal);
        }

        //
        // GET: /Dic/Create

        public ActionResult Create()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View();
        }

        //
        // POST: /Dic/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dic_normal dic_normal)
        {
            if (ModelState.IsValid)
            {
                db.dic_normal.Add(dic_normal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dic_normal);
        }

        //
        // GET: /Dic/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            dic_normal dic_normal = db.dic_normal.Find(id);
            if (dic_normal == null)
            {
                return HttpNotFound();
            }
            return View(dic_normal);
        }

        //
        // POST: /Dic/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(dic_normal dic_normal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dic_normal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dic_normal);
        }

        //
        // GET: /Dic/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            dic_normal dic_normal = db.dic_normal.Find(id);
            if (dic_normal == null)
            {
                return HttpNotFound();
            }
            return View(dic_normal);
        }
        public string import() {
            //StreamReader sr = new StreamReader( HttpContext.Server.MapPath("../vietanh.index"));
            //Hashtable w = new Hashtable();
            //while (!sr.EndOfStream) {
            //    string line = sr.ReadLine();
            //    line = line.Replace("\t", " ");
            //    string[] arr = line.Split(' ');
            //    if (arr.Length >= 6) {
            //        line = arr[0] + " " + arr[1] + " " + arr[2] + " " + arr[3];
            //        line = line.Trim();
            //    }else
            //    if (arr.Length >= 5)
            //    {
            //        line = arr[0] + " " + arr[1] + " " + arr[2];
            //        line = line.Trim();
            //    }
            //    else
            //        if (arr.Length >= 4)
            //        {
            //            line = arr[0] + " " + arr[1];
            //            line = line.Trim();
            //        }
            //        else
            //            if (arr.Length >= 3)
            //            {
            //                line = arr[0];
            //                line = line.Trim();
            //            }

            //    if (line != "" && !w.ContainsKey(line))
            //    {
            //        dic_normal dn = new dic_normal();
            //        dn.word = line;
            //        db.dic_normal.Add(dn);
            //        db.SaveChanges();
            //        w.Add(line, "1");
            //    }
            //}
            //sr.Close();

            return "Đã xong";
        }
        //
        // POST: /Dic/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dic_normal dic_normal = db.dic_normal.Find(id);
            db.dic_normal.Remove(dic_normal);
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