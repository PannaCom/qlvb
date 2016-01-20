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
using Microsoft.Office.Interop.Word;
using System.Text;
namespace qlvb.Controllers
{
    public class DocumentController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Document/

        public ActionResult Index()
        {
            return View(db.documents.ToList());
        }

        //
        // GET: /Document/Details/5

        public ActionResult Details(int id = 0)
        {
            document document = db.documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        //
        // GET: /Document/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Document/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(document document)
        {
            if (ModelState.IsValid)
            {
                db.documents.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(document);
        }

        //
        // GET: /Document/Edit/5

        public ActionResult Edit(int id = 0)
        {
            document document = db.documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        //
        // POST: /Document/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadDocProcess(HttpPostedFileBase file)
        {
            string physicalPath = HttpContext.Server.MapPath("../Files/");
            string nameFile = String.Format("{0}.doc", Guid.NewGuid().ToString());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                Application application = new Application();
                Document document = application.Documents.Open(fullPath);
                //Số: .*/.*/.*\S-*([A-Z])\w+ Lấy ra ký hiệu văn bản
                // Loop through all words in the document.
                //int count = document.Words.Count;
                //for (int j = 1; j <= count; j++)
                //{
                //    // Write the word.
                //    try
                //    {
                //        string text = document.Words[j].Text;
                //        sb.Append(text);
                //    }
                //    catch (Exception ex) { 

                //    }
                   
                //}
                
                // Close word.
                application.Quit();
                break;
            }
            return sb.ToString();// "/Files/" + nameFile;
        }
        //
        // GET: /Document/Delete/5

        public ActionResult Delete(int id = 0)
        {
            document document = db.documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        //
        // POST: /Document/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            document document = db.documents.Find(id);
            db.documents.Remove(document);
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