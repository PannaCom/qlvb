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
using System.Text.RegularExpressions;
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
            document document=new document();
            ViewBag.id = "-1";
            ViewBag.cat1 = "-1";
            ViewBag.cat2 = "-1";
            ViewBag.cat3 = "-1";
            ViewBag.cat4 = "-1";
            return View(document);
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
            //Array test = Config.getCat2();
            //return "";
            string physicalPath = HttpContext.Server.MapPath("../Files/");
            string nameFile = String.Format("{0}.doc", Guid.NewGuid().ToString());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            StringBuilder sb = new StringBuilder();
            string code = "";
            string year = "";
            string title = "";
            string p1 = "";
            string p2 = "";
            string type_document="";
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
                //Ngày tháng năm ngày.* tháng .* năm .*\w lấy ra ngày đầu tiên
                //string content=document.Content.Words.ToString();
                
                // Loop through all words in the document.
                //int count = document.Words.Count;
                //string text = "";
                //for (int j = 1; j <= count; j++)
                //{
                //    // Write the word.
                //    try
                //    {
                //        text = document.Words[j].Text;
                //        sb.Append(text);
                //    }
                //    catch (Exception ex)
                //    {

                //    }

                //}
                string content = document.Content.Text;
                title = Config.getTitle(content);
                //var Regex = new Regex();
                Array arrT=Config.getCat2();
                foreach (string item in arrT) {
                    if (title.StartsWith(item.ToUpperInvariant())) {
                        title = Config.ReplaceFirst(title, item.ToUpperInvariant(), "").Trim();
                        type_document = item;
                        break;
                    }
                }
                content = content.Replace("\r\a", " ");
                code = Config.getCode(content);
                year = Config.getYear(content);
                p1 = Config.getP1(content);
                p2 = Config.getP2(content);
                // Close word.
                application.Quit();
                break;
            }
            return code + Config.sp + title + Config.sp + p1 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2;// "/Files/" + nameFile;
        }
        public string addNewCat(int type, string value) {
            switch (type) { 
                case 1:
                    cat1 c1 = new cat1();
                    c1.name = value;
                    c1.no = 0;
                    db.cat1.Add(c1);
                    db.SaveChanges();
                    return c1.id + Config.sp + c1.name;
                    break;
                case 2:
                    cat2 c2 = new cat2();
                    c2.name = value;
                    c2.no = 0;
                    db.cat2.Add(c2);
                    db.SaveChanges();
                    return c2.id + Config.sp + c2.name;
                    break;
                case 3:
                    cat3 c3 = new cat3();
                    c3.name = value;
                    c3.no = 0;
                    db.cat3.Add(c3);
                    db.SaveChanges();
                    return c3.id + Config.sp + c3.name;
                    break;
                case 4:
                    cat4 c4 = new cat4();
                    c4.name = value;
                    c4.no = 0;
                    db.cat4.Add(c4);
                    db.SaveChanges();
                    return c4.id + Config.sp + c4.name;
                    break;
            }
            return "0";
        }
        public string addNewDocument(int id, string name, string code, string link, string keyword1, string keyword2, int cat1, int cat2, int cat3, int cat4, int year, string related_id)
        {
            try
            {
                if (id != -1)
                {
                    document doc = new document();
                    doc.name = name;
                    doc.code = code;
                    doc.link = link;
                    doc.keyword1 = keyword1;
                    doc.keyword2 = keyword2;
                    doc.keyword3 = "";
                    doc.cat1_id = cat1;
                    doc.cat2_id = cat2;
                    doc.cat3_id = cat3;
                    doc.cat4_id = cat4;
                    string f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name;
                    string f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name;
                    string f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name;
                    string f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name;
                    doc.auto_des = code + " " + name + " " + keyword1 + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    doc.status = 0;
                    doc.type = 0;
                    doc.year = year;
                    db.documents.Add(doc);
                    db.SaveChanges();
                    return "1";
                }
                else
                {
                    document doc = db.documents.Find(id);
                    doc.name = name;
                    doc.code = code;
                    doc.link = link;
                    doc.keyword1 = keyword1;
                    doc.keyword2 = keyword2;
                    doc.keyword3 = "";
                    doc.cat1_id = cat1;
                    doc.cat2_id = cat2;
                    doc.cat3_id = cat3;
                    doc.cat4_id = cat4;
                    string f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name;
                    string f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name;
                    string f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name;
                    string f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name;
                    doc.auto_des = code + " " + name + " " + keyword1 + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    //doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    //doc.status = 0;
                    //doc.type = 0;
                    doc.year = year;
                    db.Entry(doc).State = EntityState.Modified;
                    db.SaveChanges();
                    return "1";
                }
            }
            catch (Exception ex) {
                    return "0";
            }
            
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