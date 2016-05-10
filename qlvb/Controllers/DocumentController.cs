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
//using Microsoft.Office.Interop.Word;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using PagedList;
namespace qlvb.Controllers
{
    public class DocumentController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /Document/
        public class searchitem
        {
            //FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id, FT_TBL.views, KEY_TBL.RANK
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
            public int cat1_id { get; set; }
            public int cat2_id { get; set; }
            public int cat3_id { get; set; }
            public int cat4_id { get; set; }
            public int views { get; set; }
            public int RANK { get; set; }

        }
        public class catlist
        {
            //FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id, FT_TBL.views, KEY_TBL.RANK
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public int cat1_id { get; set; }
            public string cat1 { get; set; }
            public int cat2_id { get; set; }
            public string cat2 { get; set; }
            public int cat4_id { get; set; }
            public string cat4 { get; set; }
            public int? views { get; set; }
            public byte? no { get; set; }

        }
        public class catitem
        {
            public int catid { get; set; }
            public string name { get; set; }
            public int total { get; set; }
        }
        public ActionResult Index(string k, string f1, string f2, string f3, string f4, string order, string to, int? pg)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            ViewBag.user = Config.getCookie("userid");
            //try
            //{
            if (k != null && k.Trim() != "")
            {
                k = k.Replace("%20", " ");

                f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
                f4 = f4 != null ? f4 : "";

                ViewBag.keyword = k;
                if (pg == null) pg = 1;
                string query = "SELECT top 100 ";
                query += "FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.views, RANK=CASE FT_TBL.cat2_id ";
                query +="WHEN 7 THEN KEY_TBL.RANK*7 ";
                query +="WHEN 18 THEN KEY_TBL.RANK*6 ";
                query +="WHEN 15 THEN KEY_TBL.RANK*5 ";
		        query +="WHEN 5 THEN KEY_TBL.RANK*4 ";
                query +="WHEN 23 THEN KEY_TBL.RANK*3 ";
		        query +="WHEN 6 THEN KEY_TBL.RANK*2 ";
                query +="ELSE KEY_TBL.RANK ";
                query +="END FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + k + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                query += " where (RANK>" + Config.minRank + ") ";

                string[] item = new string[10];
                int i = 0;
                string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                for (int f = 0; f < filter.Length; f++)
                {
                    if (filter[f] != null && filter[f] != "")
                    {
                        query += " and (cat" + (f + 1) + "_id=" + filter[f] + ") ";
                    }
                }
                if (order == null || order == "") order = "RANK";
                query += " order by " + order;
                if (to == null || to == "") to = "Desc";
                query += " " + to;

                ViewBag.f1 = f1;
                ViewBag.f2 = f2;
                ViewBag.f3 = f3;
                ViewBag.f4 = f4;
                try
                {
                    string query1 = Config.makeQuery(k, "1", f1, f2, f3, f4);
                    string query2 = Config.makeQuery(k, "2", f1, f2, f3, f4);
                    string query3 = Config.makeQuery(k, "3", f1, f2, f3, f4);
                    string query4 = Config.makeQuery(k, "4", f1, f2, f3, f4);
                    int jj = 0;
                    string scat1 = "", scat2 = "", scat3 = "", scat4 = "";
                    try
                    {
                        var cat1 = db.Database.SqlQuery<catitem>(query1).ToList();
                        scat1 = "<b>Lĩnh vực:</b>";
                        string color = "";
                        for (jj = 0; jj < cat1.Count; jj++)
                        {
                            if (cat1[jj].total <= 0) continue;
                            color = "";
                            if (cat1[jj].catid.ToString() == f1) color = "color:red;font-weight:bold;";
                            else if (cat1[jj].total > 0) color = "color:blue;";
                            scat1 += "<a class='filteritem' onclick='setCat(1," + cat1[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat1[jj].name + "(" + cat1[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc1)
                    {
                    }
                    try
                    {
                        var cat2 = db.Database.SqlQuery<catitem>(query2).ToList();
                        scat2 = "<b>Loại văn bản:</b>";
                        string color = "";
                        for (jj = 0; jj < cat2.Count; jj++)
                        {
                            if (cat2[jj].total <= 0) continue;
                            color = "";
                            if (cat2[jj].catid.ToString() == f2) color = "color:red;font-weight:bold;";
                            else if (cat2[jj].total > 0) color = "color:blue;";
                            scat2 += "<a class='filteritem' onclick='setCat(2," + cat2[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat2[jj].name + "(" + cat2[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc2)
                    {
                    }
                    try
                    {
                        var cat3 = db.Database.SqlQuery<catitem>(query3).ToList();
                        scat3 = "<b>Người ký:</b>";
                        string color = "";
                        for (jj = 0; jj < cat3.Count; jj++)
                        {
                            if (cat3[jj].total <= 0) continue;
                            color = "";
                            if (cat3[jj].catid.ToString() == f3) color = "color:red;font-weight:bold;";
                            else if (cat3[jj].total > 0) color = "color:blue;";
                            scat3 += "<a class='filteritem' onclick='setCat(3," + cat3[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat3[jj].name + "(" + cat3[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc3)
                    {
                    }
                    try
                    {
                        var cat4 = db.Database.SqlQuery<catitem>(query4).ToList();
                        scat4 = "<b>Cơ quan ban hành:</b>";
                        string color = "";
                        for (jj = 0; jj < cat4.Count; jj++)
                        {
                            if (cat4[jj].total <= 0) continue;
                            color = "";
                            if (cat4[jj].catid.ToString() == f4)
                                color = "color:red;font-weight:bold;";
                            else if (cat4[jj].total > 0) color = "color:blue;";

                            scat4 += "<a class='filteritem' onclick='setCat(4," + cat4[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat4[jj].name + "(" + cat4[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc4)
                    {
                    }

                    ViewBag.cat1 = scat1;
                    ViewBag.cat2 = scat2;
                    ViewBag.cat3 = scat3;
                    ViewBag.cat4 = scat4;
                }
                catch (Exception ex2)
                {
                }
                ViewBag.page = pg;
                ViewBag.order = order;
                ViewBag.to = to;
                var p = db.Database.SqlQuery<searchitem>(query);
                int pageSize = 10;
                int pageNumber = (pg ?? 1);
                return View(p.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                k = "";

                f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
                f4 = f4 != null ? f4 : "";

                ViewBag.keyword = k;
                if (pg == null) pg = 1;
                string query = "SELECT top 100 ";
                query += " id, name, code, cat1_id, cat2_id, cat3_id, cat4_id, views, 0 as Rank FROM documents ";
                query += " order by  views desc";
                //string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                //for (int f = 0; f < filter.Length; f++)
                //{
                //    if (filter[f] != null && filter[f] != "")
                //    {
                //        query += " and (cat" + (f + 1) + "=" + filter[f] + ") ";
                //    }
                //}

                //    select catid,name,count(id) as total from
                //(select catid,name,id from
                //(select id as catid,name from cat1) as A left join
                //(select cat1_id,id from documents where cat1_id=1) as B on A.catid=B.cat1_id
                //) as C group by catid,name

                ViewBag.f1 = f1;
                ViewBag.f2 = f2;
                ViewBag.f3 = f3;
                ViewBag.f4 = f4;

                ViewBag.page = pg;
                ViewBag.order = order;
                ViewBag.to = to;
                var p = db.Database.SqlQuery<searchitem>(query);
                int pageSize = 10;
                int pageNumber = (pg ?? 1);
                return View(p.ToPagedList(pageNumber, pageSize));
            }
            //}
            //catch (Exception exmain) {
            //    return View();
            //}
        }
        public ActionResult Cat(int? cat11,int? cat22,int? cat44,string order,string to,int? pg) { 
           
                if (pg == null) pg = 1;
                string query = "select id,code,name,cat1_id,cat1,cat2_id,cat2,cat4_id,cat4,views,no from ";
                       query +=    " (select id,code,name,cat1_id,cat2_id,cat4_id,views from documents) as A left join ";
                       query +=" (select name as cat1,id as idcat1 from cat1) as B on A.cat1_id=B.idcat1 left join ";
                       query +="(select name as cat2,id as idcat2,no from cat2) as C on A.cat2_id=C.idcat2 left join ";
                       query +="(select name as cat4,id as idcat4 from cat4) as D on A.cat4_id=D.idcat4 where 1=1 ";
                       if (cat11 != null) query += " and cat1_id=" + cat11;
                       if (cat22 != null) query += " and cat2_id=" + cat22;
                       if (cat44 != null) query += " and cat4_id=" + cat44;
                if (order == null || order == "") order = "no";
                query += " order by " + order;
                if (to == null || to == "") to = "desc";
                query += " " + to;
               
                try
                {
                    string query1 = Config.makeQueryCat("1", cat11,cat22,cat44);
                    string query2 = Config.makeQueryCat("2", cat11, cat22, cat44);
                    string query4 = Config.makeQueryCat("4", cat11, cat22, cat44);
                    int jj = 0;
                    string scat1 = "", scat2 = "", scat3 = "", scat4 = "";
                    try
                    {
                        var cat1 = db.Database.SqlQuery<catitem>(query1).ToList();
                        scat1 = "<b>Lĩnh vực:</b>";
                        string color = "";
                        for (jj = 0; jj < cat1.Count; jj++)
                        {
                            if (cat1[jj].total <= 0) continue;
                            color = "";
                            if (cat1[jj].catid == cat11) color = "color:red;font-weight:bold;";
                            else if (cat1[jj].total > 0) color = "color:blue;";
                            scat1 += "<a class='filteritem' onclick='setCat(1," + cat1[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat1[jj].name + "(" + cat1[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc1)
                    {
                    }
                    try
                    {
                        var cat2 = db.Database.SqlQuery<catitem>(query2).ToList();
                        scat2 = "<b>Loại văn bản:</b>";
                        string color = "";
                        for (jj = 0; jj < cat2.Count; jj++)
                        {
                            if (cat2[jj].total <= 0) continue;
                            color = "";
                            if (cat2[jj].catid == cat22) color = "color:red;font-weight:bold;";
                            else if (cat2[jj].total > 0) color = "color:blue;";
                            scat2 += "<a class='filteritem' onclick='setCat(2," + cat2[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat2[jj].name + "(" + cat2[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc2)
                    {
                    }
                    
                    try
                    {
                        var cat4 = db.Database.SqlQuery<catitem>(query4).ToList();
                        scat4 = "<b>Cơ quan ban hành:</b>";
                        string color = "";
                        for (jj = 0; jj < cat4.Count; jj++)
                        {
                            if (cat4[jj].total <= 0) continue;
                            color = "";
                            if (cat4[jj].catid == cat44)
                                color = "color:red;font-weight:bold;";
                            else if (cat4[jj].total > 0) color = "color:blue;";

                            scat4 += "<a class='filteritem' onclick='setCat(4," + cat4[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat4[jj].name + "(" + cat4[jj].total + ")</a>,";
                        }
                    }
                    catch (Exception exc4)
                    {
                    }

                    ViewBag.cat1 = scat1;
                    ViewBag.cat2 = scat2;
                    ViewBag.cat3 = scat3;
                    ViewBag.cat4 = scat4;
                }
                catch (Exception ex2)
                {
                }
                ViewBag.f1 = cat11;
                ViewBag.f2 = cat22;
                ViewBag.f3 = "";      
                ViewBag.f4 = cat44;

                ViewBag.page = pg;
                ViewBag.order = order;
                ViewBag.to = to;
                var p = db.Database.SqlQuery<catlist>(query);
                int pageSize = 10;
                int pageNumber = (pg ?? 1);
                return View(p.ToPagedList(pageNumber, pageSize));
        }
        public class search
        {
            public string value { get; set; }
            public int id { get; set; }
        }
        public string getDoc(string keyword) {
            if (keyword != null && (keyword.Contains("/") || keyword.Contains("-")))
            {
                //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.code).Take(20);
                string query="SELECT top 10 ";
                query += "FT_TBL.code+' '+ FT_TBL.name as value,FT_TBL.id,RANK=CASE FT_TBL.cat2_id ";
                query += "WHEN 7 THEN KEY_TBL.RANK*7 ";
                query += "WHEN 18 THEN KEY_TBL.RANK*6 ";
                query += "WHEN 15 THEN KEY_TBL.RANK*5 ";
                query += "WHEN 5 THEN KEY_TBL.RANK*4 ";
                query += "WHEN 23 THEN KEY_TBL.RANK*3 ";
                query += "WHEN 6 THEN KEY_TBL.RANK*2 ";
                query += "ELSE KEY_TBL.RANK ";
                query += "END FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
			     query+=" where RANK>0 order by Rank Desc";
                 var p = db.Database.SqlQuery<search>(query);
                return JsonConvert.SerializeObject(p.ToList());
            }
            else
            {
                //Sẽ thay bằng search fulltext
                //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.name).Take(20);
                //return JsonConvert.SerializeObject(p.ToList());
                string query = "SELECT top 10 ";
                query += "FT_TBL.name +' ' +FT_TBL.code as value,FT_TBL.id,RANK=CASE FT_TBL.cat2_id ";
                query += "WHEN 7 THEN KEY_TBL.RANK*7 ";
                query += "WHEN 18 THEN KEY_TBL.RANK*6 ";
                query += "WHEN 15 THEN KEY_TBL.RANK*5 ";
                query += "WHEN 5 THEN KEY_TBL.RANK*4 ";
                query += "WHEN 23 THEN KEY_TBL.RANK*3 ";
                query += "WHEN 6 THEN KEY_TBL.RANK*2 ";
                query += "ELSE KEY_TBL.RANK ";
                query += "END  FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                query += " where RANK>0 order by Rank Desc";
                var p = db.Database.SqlQuery<search>(query);
                return JsonConvert.SerializeObject(p.ToList());
            }
        }
        public string getDocByCode(string code) {
            var p = (from q in db.documents where q.code.Contains(code) select q.code).ToList();
            return JsonConvert.SerializeObject(p);
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
            ViewBag.user = Config.getCookie("userid");
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
                ViewBag.keyword3 = document.keyword3;
                ViewBag.keyword4 = document.keyword4;
                ViewBag.keyword5 = document.keyword5;
                ViewBag.be_linked = document.be_linked;
                ViewBag.link_to = document.link_to;
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
            string nameFile = String.Format("{0}", Config.removeSpecialChar(Request.Files[0].FileName.Replace(" ", "_")));//Guid.NewGuid().ToString()
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            StringBuilder sb = new StringBuilder();
            string code = "";
            string year = "";
            string title = "";
            string p1 = "";
            string p2 = "", p3 = "", p4 = "", p5 = "";
            string type_document="";
            string publish = "";
            string people_sign = "";//trả về người ký văn bản
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                        Request.Files[i].SaveAs(fullPath);
                    }
                    catch (Exception ex) { 

                    }
                }
                else Request.Files[i].SaveAs(fullPath);
                
                //return nameFile;
                try
                {
                    //Application application = new Application();
                    //Microsoft.Office.Interop.Word.Document document = application.Documents.Open(fullPath);
                    //string content = document.Content.Text;
                    string content = "";
                    WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(fullPath, true);
                    //content = wordprocessingDocument.MainDocumentPart.Document.InnerText;
                    sb = new StringBuilder();
                    OpenXmlElement element = wordprocessingDocument.MainDocumentPart.Document.Body;
                    if (element == null)
                    {
                        content=string.Empty;
                    }
                    sb.Append(Config.GetPlainText(element));
                    content=sb.ToString();
                    content = content.Replace("\t", "\r\n");
                    content = content.Replace("\r\n\r\n", "\r\n");                    
                    Config.loadDic();
                    title = Config.getTitle(content).Replace("\n"," ").Trim();
                    p1 = Config.getP1(title);
                    //var Regex = new Regex();
                    //Bỏ đi các từ khóa thông tư, nghị định... ở đầu, lấy ra loại tài liệu là thông tư? nghị định
                    Array arrT = Config.getCat2();
                    foreach (string item in arrT)
                    {
                        if (title.StartsWith(item.ToUpperInvariant()))
                        {
                            title = Config.ReplaceFirst(title, item.ToUpperInvariant(), "").Trim();
                            type_document = item;
                            break;
                        }
                    }
                    content = content.Replace("\r\a", " ");
                    content = content.Replace("\r\n", " ");
                    content = content.Replace("\r", " ");
                    content = content.Replace("\n", " ");
                    code = Config.getCode(content).Replace("\r","");
                    year = Config.getYear(content);
                    //p1 = Config.getP1(content);
                    p2 = Config.getP2(content);
                    p3 = Config.getP3(content);
                    p4 = Config.getP4(content);
                    p5 = Config.getP5(content);
                    publish = Config.getPublish(content);
                    people_sign = Config.getPeopleSign(content);
                    wordprocessingDocument.Close();
                    wordprocessingDocument = null;
                    // Close word.
                    //application.Quit();
                    break;
                }
                catch (Exception exdoc)
                {
                    return ""; 
                        //code + Config.sp + title + Config.sp + p1 + Config.sp + p2 + Config.sp + p3 + Config.sp + p4 + Config.sp + p5 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2 + Config.sp + exdoc.ToString();
                }
            }
            return code + Config.sp + title + Config.sp + p1 + Config.sp + p2 + Config.sp + p3 + Config.sp + p4 + Config.sp + p5 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + publish + Config.sp + people_sign;// code + Config.sp + title + Config.sp + p1 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2;// "/Files/" + nameFile;
            //return nameFile;
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
        public string addNewDocument(int id, string name, string code, string link, string keyword1, string keyword2, string keyword3, string keyword4, string keyword5, int cat1, int cat2, int cat3, int cat4, int year, string related_id, string be_linked,string link_to)
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
                    doc.keyword3 = keyword3;
                    doc.keyword4 = keyword4;
                    doc.keyword5 = keyword5;
                    doc.cat1_id = cat1;
                    doc.cat2_id = cat2;
                    doc.cat3_id = cat3;
                    doc.cat4_id = cat4;
                    string f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name;
                    string f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name;
                    string f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name;
                    string f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name;
                    string allKeyWord = keyword1 + " " + " " + keyword2 + " " + " " + keyword3 + " " + " " + keyword4 + " " + " " + keyword5;
                    allKeyWord = allKeyWord.Replace(" , ", " ");
                    doc.auto_des = code + " " + name + " " + code + " " + name + " " + code + " " + name + " " + allKeyWord + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    doc.status = 0;
                    doc.type = 0;
                    doc.year = year;
                    doc.views = 0;
                    doc.be_linked = be_linked;
                    doc.link_to = link_to;
                    db.documents.Add(doc);
                    db.SaveChanges();
                    ////Tự động chèn vào từ khóa có liên quan của văn bản bị điều chỉnh, sửa đổi
                    //if (link_to != "") {
                    //    string[] items = link_to.Split(',');
                    //    for (int i = 0; i < items.Length; i++) {
                    //        if (items[i].Trim() != "") {
                    //            db.Database.ExecuteSqlCommand("update documents set keyword5=keyword5+N'" + code + "' where code=N'" + items[i].Trim() + "'");
                    //        }
                    //    }
                    //}
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
                    doc.keyword3 = keyword3;
                    doc.keyword4 = keyword4;
                    doc.keyword5 = keyword5;
                    doc.cat1_id = cat1;
                    doc.cat2_id = cat2;
                    doc.cat3_id = cat3;
                    doc.cat4_id = cat4;
                    string f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name;
                    string f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name;
                    string f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name;
                    string f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name;
                    string allKeyWord = keyword1 + " " + " " + keyword2 + " " + " " + keyword3 + " " + " " + keyword4 + " " + " " + keyword5;
                    allKeyWord = allKeyWord.Replace(" , ", " ");
                    doc.auto_des = code + " " + name + " " + code + " " + name + " " + code + " " + name + " " + allKeyWord + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    //doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    //doc.status = 0;
                    //doc.type = 0;
                    doc.year = year;
                    doc.be_linked = be_linked;
                    doc.link_to = link_to;
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
            try
            {
                if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
                document document = db.documents.Find(id);
                string physicalPath = HttpContext.Server.MapPath("/Files/");
                string nameFile = document.link;
                string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
                db.documents.Remove(document);
                db.SaveChanges();
                if (System.IO.File.Exists(fullPath))
                {
                    //System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex) { 
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