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
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
namespace qlvb.Controllers
{
    public class DocumentController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Document/

        public ActionResult Index(string word, int? page)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (word == null) word = "";
            ViewBag.word = word;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var p = (from q in db.documents where q.auto_des.Contains(word) select q).OrderBy(o => o.code).Take(1000);
            return View(p.ToPagedList(pageNumber, pageSize));
            //return View(db.cat2.ToList());
        }
        
        public string getDoc(string keyword) {
            if (keyword != null && (keyword.Contains("/") || keyword.Contains("-")))
            {
                //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.code).Take(20);
                string query="SELECT top 10 ";
                query += "FT_TBL.code+' '+ FT_TBL.name as name FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
			     query+="order by Rank Desc";
                 var p = db.Database.SqlQuery<string>(query);
                return JsonConvert.SerializeObject(p.ToList());
            }
            else
            {
                //Sẽ thay bằng search fulltext
                //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.name).Take(20);
                //return JsonConvert.SerializeObject(p.ToList());
                string query = "SELECT top 10 ";
                query += "FT_TBL.name +' ' +FT_TBL.code as name FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                query += "order by Rank Desc";
                var p = db.Database.SqlQuery<string>(query);
                return JsonConvert.SerializeObject(p.ToList());
            }
        }
        public string checkDuplicate(string code){
            return db.documents.Any(o => o.code == code).ToString();
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
            db.Database.ExecuteSqlCommand("update documents set views=views+1 where id=" + id);
            return View(document);
        }

        //
        // GET: /Document/Create

        public ActionResult Create(int? id)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (id == null)
            {
                document document = new document();
                ViewBag.id = "-1";
                ViewBag.cat1 = "-2";
                ViewBag.cat2 = "-2";
                ViewBag.cat3 = "-2";
                ViewBag.cat4 = "-2";
                ViewBag.year = "-2";
                return View(document);
            }
            else {
                document document = db.documents.Find(id);
                if (document == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id = id;
                ViewBag.name = document.name;
                ViewBag.code = document.code;
                ViewBag.cat1 = document.cat1_id;
                ViewBag.cat2 = document.cat2_id;
                ViewBag.cat3 = document.cat3_id;
                ViewBag.cat4 = document.cat4_id;
                ViewBag.keyword1 = document.keyword1;
                ViewBag.keyword2 = document.keyword2;
                ViewBag.link = document.link;
                ViewBag.year = document.year;
                ViewBag.related_id = document.related_id;

                return View(document);
            }
        }

        //
        // POST: /Document/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(document document)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
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
                return nameFile;
                //try
                //{
                //    //Application application = new Application();
                //    //Document document = application.Documents.Open(fullPath);
                //    //string content = document.Content.Text;
                //    //string content = "";
                //    //WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(fullPath, true);
                //    //content = wordprocessingDocument.MainDocumentPart.Document.InnerText;
                    
                //    //title = Config.getTitle(content);
                //    ////var Regex = new Regex();
                //    //Array arrT = Config.getCat2();
                //    //foreach (string item in arrT)
                //    //{
                //    //    if (title.StartsWith(item.ToUpperInvariant()))
                //    //    {
                //    //        title = Config.ReplaceFirst(title, item.ToUpperInvariant(), "").Trim();
                //    //        type_document = item;
                //    //        break;
                //    //    }
                //    //}
                //    //content = content.Replace("\r\a", " ");
                //    //code = Config.getCode(content);
                //    //year = Config.getYear(content);
                //    //p1 = Config.getP1(content);
                //    //p2 = Config.getP2(content);
                //    //// Close word.
                //    ////application.Quit();
                //    //break;
                //}
                //catch (Exception exdoc) {
                //    return code + Config.sp + title + Config.sp + p1 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2 + Config.sp + exdoc.ToString();
                //}
            }
            //return code + Config.sp + title + Config.sp + p1 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2;// "/Files/" + nameFile;
            return nameFile;
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
                if (id == -1)
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
                    doc.views = 0;
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
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
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
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            document document = db.documents.Find(id);
            string physicalPath = HttpContext.Server.MapPath("/Files/");
            string nameFile = document.link;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            db.documents.Remove(document);
            db.SaveChanges();
            if (System.IO.File.Exists(fullPath)) {
                System.IO.File.Delete(fullPath);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}