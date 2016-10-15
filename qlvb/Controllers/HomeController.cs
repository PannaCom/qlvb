using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;
using PagedList;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Text;
using System.IO;
using System.Data;

namespace qlvb.Controllers
{
    public class HomeController : Controller
    {
        private qlvbEntities db = new qlvbEntities();
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
            public DateTime? date_publish { get; set; }
            public DateTime? date_start { get; set; }
            public int RANK { get; set; }
            public byte? status { get; set; }

        }
        public class catitem
        {
            public int catid { get; set; }
            public string name { get; set; }
            public int total { get; set; }
        }
        public ActionResult Index(string k, string f1, string f2, string f3, string f4, int? st, byte? status,byte? tps,int? ft,string order, string to, int? pg)
        {
            string fts = "freetexttable";
            try
            {
                if (tps == 2 && (st!=1 & st!=2)) { 
                    string tempf1 = Config.getMaxCat1(k);
                    if (tempf1 != "" && tps == 2) f1 = tempf1;
                }
                if (tps == 1)
                {
                    Config.changeHeso(tps, k);
                }
                
                if (k != null && k.Trim() != "")
                {
                    if (ft == 1) { fts = "CONTAINSTABLE"; k = k.Replace(" ", "%");}
                    else
                    { k = k.Replace("%20", " ").Replace("%", " "); }

                    f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
                    f4 = f4 != null ? f4 : "";
                    if (st == null) st = 0;
                    if (status == null) status = 2;
                    if (tps == null) tps = 1;
                    if (ft == null) ft = 1;
                    ViewBag.keyword = k.Replace("%", " ");
                    if (pg == null) pg = 1;
                    string query = "select top 300 * from (SELECT ";
                    query += "FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.views,FT_TBL.date_publish, FT_TBL.date_start,RANK=CASE FT_TBL.cat2_id ";
                    query += "WHEN 7 THEN KEY_TBL.RANK*"+Config.heso1+" ";
                    query += "WHEN 18 THEN KEY_TBL.RANK*" + Config.heso2 + " ";
                    query += "WHEN 15 THEN KEY_TBL.RANK*" + Config.heso3 + " ";
                    query += "WHEN 5 THEN KEY_TBL.RANK*" + Config.heso4 + " ";
                    query += "WHEN 23 THEN KEY_TBL.RANK*" + Config.heso5 + " ";
                    query += "WHEN 6 THEN KEY_TBL.RANK*" + Config.heso6 + " ";
                    query += "ELSE KEY_TBL.RANK ";
                    query += "END, FT_TBL.status FROM documents AS FT_TBL INNER JOIN " + fts + "(documents, auto_des,'" + k + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                    query += " where (RANK>"+Config.minRank+") ";

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
                    if (status == 2) {
                        query += " and (status=0 or status=1) ";
                    }else
                        if (status == 1)
                        {
                            query += " and (status=1) ";
                        }
                        else
                            if (status == 0)
                            {
                                query += " and (status=0) ";
                            }
                    query += ") as A ";
                    if (k != null && st==2)
                    {
                        query = "select top 300  id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,date_publish,date_start,RANK=CASE cat2_id ";
                        query += "WHEN 7 THEN " + Config.heso1 + " ";
                        query += "WHEN 18 THEN " + Config.heso2 + " ";
                        query += "WHEN 15 THEN " + Config.heso3 + " ";
                        query += "WHEN 5 THEN " + Config.heso4 + " ";
                        query += "WHEN 23 THEN " + Config.heso5 + " ";
                        query += "WHEN 6 THEN " + Config.heso6 + " ";
                        query += "ELSE 0 ";
                        query += "END,status from documents where (code like N'" + k + "%' or code=N'" + k + "' or code like N'%" + k + "' or code like N'%" + k + "%') ";
                        if (status == 2)
                        {
                            query += " and (status=0 or status=1) ";
                        }
                        else
                            if (status == 1)
                            {
                                query += " and (status=1) ";
                            }
                            else
                                if (status == 0)
                                {
                                    query += " and (status=0) ";
                                }
                        for (int f = 0; f < filter.Length; f++)
                        {
                            if (filter[f] != null && filter[f] != "")
                            {
                                query += " and (cat" + (f + 1) + "_id=" + filter[f] + ") ";
                            }
                        }
                    } else
                    {
                        if (k != null && (st==1))
                        {
                            query = "select top 300  id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,date_publish,date_start,RANK=CASE cat2_id ";
                            query += "WHEN 7 THEN " + Config.heso1 + " ";
                            query += "WHEN 18 THEN " + Config.heso2 + " ";
                            query += "WHEN 15 THEN " + Config.heso3 + " ";
                            query += "WHEN 5 THEN " + Config.heso4 + " ";
                            query += "WHEN 23 THEN " + Config.heso5 + " ";
                            query += "WHEN 6 THEN " + Config.heso6 + " ";
                            query += "ELSE 0 ";
                            query += "END,status from documents where (name like N'" + k + "%' or name=N'" + k + "' or name like N'%" + k + "' or name like N'%" + k + "%') ";
                            if (status == 2)
                            {
                                query += " and (status=0 or status=1) ";
                            }
                            else
                                if (status == 1)
                                {
                                    query += " and (status=1) ";
                                }
                                else
                                    if (status == 0)
                                    {
                                        query += " and (status=0) ";
                                    }
                            for (int f = 0; f < filter.Length; f++)
                            {
                                if (filter[f] != null && filter[f] != "")
                                {
                                    query += " and (cat" + (f + 1) + "_id=" + filter[f] + ") ";
                                }
                            }
                        }
                        if (k != null && (st == 4))
                        {
                            query = "select top 300 id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,date_publish,date_start,RANK=CASE cat2_id ";
                            query += "WHEN 7 THEN " + Config.heso1 + " ";
                            query += "WHEN 18 THEN " + Config.heso2 + " ";
                            query += "WHEN 15 THEN " + Config.heso3 + " ";
                            query += "WHEN 5 THEN " + Config.heso4 + " ";
                            query += "WHEN 23 THEN " + Config.heso5 + " ";
                            query += "WHEN 6 THEN " + Config.heso6 + " ";
                            query += "ELSE 0 ";
                            query += "END,status from documents where (full_content like N'" + k + "%' or  full_content like N'%" + k.Replace(" ","%") + "%') ";
                            if (status == 2)
                            {
                                query += " and (status=0 or status=1) ";
                            }
                            else
                                if (status == 1)
                                {
                                    query += " and (status=1) ";
                                }
                                else
                                    if (status == 0)
                                    {
                                        query += " and (status=0) ";
                                    }
                            for (int f = 0; f < filter.Length; f++)
                            {
                                if (filter[f] != null && filter[f] != "")
                                {
                                    query += " and (cat" + (f + 1) + "_id=" + filter[f] + ") ";
                                }
                            }
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
                    ViewBag.st = st;
                    ViewBag.status = status;
                    ViewBag.tps = tps;
                    ViewBag.ft = ft;
                    try
                    {
                        string query1 = Config.makeQuery(ft,k, "1", f1, f2, f3, f4);
                        string query2 = Config.makeQuery(ft, k, "2", f1, f2, f3, f4);
                        string query3 = Config.makeQuery(ft, k, "3", f1, f2, f3, f4);
                        string query4 = Config.makeQuery(ft, k, "4", f1, f2, f3, f4);
                        int jj = 0;
                        string scat1 = "", scat2 = "", scat3 = "", scat4 = "";
                        try
                        {
                            var cat1 = db.Database.SqlQuery<catitem>(query1).ToList();
                            scat1 = "<b style=\"color:#555661;\">Lĩnh vực:</b> ";
                            string color = "";
                            for (jj = 0; jj < cat1.Count; jj++)
                            {
                                if (cat1[jj].total <= 0) continue;
                                color = "";
                                if (cat1[jj].catid.ToString() == f1) color = "color:#545432;font-weight:bold;";
                                else if (cat1[jj].total > 0) color = "color:#545432;";
                                scat1 += "<a class='filteritem' onclick='setCat(1," + cat1[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat1[jj].name + " (" + cat1[jj].total + ")</a>, ";// + "(" + cat1[jj].total + ")
                            }
                        }
                        catch (Exception exc1) { 
                        }
                        try
                        {
                            var cat2 = db.Database.SqlQuery<catitem>(query2).ToList();
                            scat2 = "<b style=\"color:#555661;\">Loại văn bản:</b> ";
                            string color = "";
                            for (jj = 0; jj < cat2.Count; jj++)
                            {
                                if (cat2[jj].total <= 0) continue;
                                color = "";
                                if (cat2[jj].catid.ToString() == f2) color = "color:#545432;font-weight:bold;";
                                else if (cat2[jj].total > 0) color = "color:#545432;";
                                scat2 += "<a class='filteritem' onclick='setCat(2," + cat2[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat2[jj].name + " (" + cat2[jj].total +")</a>,";//" (" + cat2[jj].total + 
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
                                if (cat3[jj].catid.ToString() == f3) color = "color:#545432;font-weight:bold;";
                                else if (cat3[jj].total > 0) color = "color:#545432;";
                                scat3 += "<a class='filteritem' onclick='setCat(3," + cat3[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat3[jj].name + "(" + cat3[jj].total + ")</a>,";
                            }
                        }
                        catch (Exception exc3)
                        {
                        }
                        try
                        {
                            var cat4 = db.Database.SqlQuery<catitem>(query4).ToList();
                            scat4 = "<b style=\"color:#555661;\">Cơ quan ban hành:</b> ";
                            string color = "";
                            for (jj = 0; jj < cat4.Count; jj++)
                            {
                                if (cat4[jj].total <= 0) continue;
                                color = "";
                                if (cat4[jj].catid.ToString() == f4)
                                    color = "color:#545432;font-weight:bold;";
                                else if (cat4[jj].total > 0) color = "color:#545432;";

                                scat4 += "<a class='filteritem' onclick='setCat(4," + cat4[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat4[jj].name + " (" + cat4[jj].total + ")</a>,";//"(" + cat4[jj].total + 
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
                    if (st == null) st = 0;                   
                    if (status == null) status = 2;
                    if (tps == null) tps = 1;
                    if (ft == null) ft = 1;
                    ViewBag.keyword = k.Replace("%", " ");
                    if (pg == null) pg = 1;
                    string query = "SELECT top 300 ";
                    query += " id, name, code, cat1_id, cat2_id, cat3_id, cat4_id, views,date_publish,date_start, 0 as RANK FROM documents ";
                    if (order == null || order == "") order = "RANK";
                    //query += " order by " + order;
                    //if (to == null || to == "") to = "Desc";
                    //query += " " + to;
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
                    ViewBag.st = st;
                    ViewBag.status = status;
                    ViewBag.tps = tps;
                    ViewBag.ft = ft;
                    ViewBag.page = pg;
                    ViewBag.order = order;
                    ViewBag.to = to;
                    var p = db.Database.SqlQuery<searchitem>(query);
                    int pageSize = 10;
                    int pageNumber = (pg ?? 1);
                    return View(p.ToPagedList(pageNumber, pageSize));
                }
            }
            catch (Exception exmain)
            {
                return View();
            }
        }
        public class document_itemscs
        {
            public int id { get; set; }
            public string item_id { get; set; }
            public int? ch { get; set; }
            public int? d { get; set; }
        }
        
        public string readAll() {
            if (Config.isRunning) return "Running";
            Config.isRunning = true;
            string content = "";
            StringBuilder sb = new StringBuilder();
            WordprocessingDocument wordprocessingDocument = null;
            int minId=int.MinValue;
            if (System.IO.File.Exists(HttpContext.Server.MapPath("../") + "/minId.txt"))
            {
                StreamReader sr = new StreamReader(HttpContext.Server.MapPath("../") + "/minId.txt");
                minId=int.Parse(sr.ReadLine());
                sr.Close();
            }
            var p = db.documents.Where(o => o.id > minId).ToList();
            for (int i = 0; i < p.Count;i++){
                content = "";
                string fullPath = HttpContext.Server.MapPath("../Files/" + p[i].link);
                try { 
                    wordprocessingDocument = WordprocessingDocument.Open(fullPath, true);
                    sb = new StringBuilder();
                    OpenXmlElement element = wordprocessingDocument.MainDocumentPart.Document.Body;
                    if (element == null)
                    {
                        content = string.Empty;
                    }
                    sb.Append(Config.GetPlainText(element));
                    content = sb.ToString();
                    content = content.Replace("\t", "\r\n");
                    content = content.Replace("\r\n\r\n", "\r\n");
                }
                catch (Exception ex2222)
                {

                }
                wordprocessingDocument = null;
                if (content == "") continue;
                try { 
                    //db.Database.ExecuteSqlCommand("update documents set full_content=N'" + content + "' where id=" + p[i].id);
                    document dt = db.documents.Find(p[i].id);
                    dt.full_content = content;
                    db.Entry(dt).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception exqss)
                {
                    StreamWriter sw2 = new StreamWriter(HttpContext.Server.MapPath("../") + "/error.txt");
                    sw2.WriteLine(content);
                    sw2.Close();
                }
                StreamWriter sw = new StreamWriter(HttpContext.Server.MapPath("../") + "/minId.txt");
                sw.WriteLine(p[i].id.ToString());
                sw.Close();
            }
            Config.isRunning = false;
            return "ok";
        }
        public string updateDatabase()
        {
            List<document_itemscs> ldi=new List<document_itemscs>();            
            string[] temp=null;
            var di = (from q in db.document_items select q).Where(o => o.ch == null && o.d == null).ToList();
            for (int i = 0; i < di.Count; i++) {
                document_itemscs dics = new document_itemscs();
                dics.id=di[i].id;
                dics.item_id=di[i].item_id;
                temp = di[i].item_id.Split('_');
                dics.ch=int.Parse(temp[0]);
                dics.d = int.Parse(temp[1]); 
                ldi.Add(dics);
            }
            foreach(var item in ldi){
                db.Database.ExecuteSqlCommand("update document_items set ch="+item.ch+",d="+item.d+" where id="+item.id);
            }
            return "ok";
        }

        public string getAllHotKeyWord() {
            var p = (from q in db.documents select q).OrderByDescending(o => o.views).Take(10).ToList();
            string all = "";
            for (int i = 0; i < p.Count; i++) {
                all += p[i].keyword1 + "," + p[i].keyword2;
            }
            return Config.getHotKeyword(all);
        }
        public class catitemtotal {
            public int id { get; set; }
            public string name { get;set;}
            public byte? no { get; set; }
            public int total { get; set; }
        }
        public string Log(string keyword)
        {
            if (keyword == "" || keyword==null) return "1";
            try
            {
                bool any = db.logs.Any(o => o.word==keyword);
                if (!any)
                {
                    log lg = new log();
                    lg.count = 1;
                    lg.word = keyword;
                    db.logs.Add(lg);
                    db.SaveChanges();
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update log set count=count+1 where word=N'" + keyword + "'");
                }
                return "1";
            }
            catch (Exception ex) {
                return "0";
            }

        }
        public string getAllCat(int id) {
            try
            {
                string query="select id,name,no,count(*) as total from (select id,name,no from cat"+id+") as A inner join";
                query += "(select cat" + id + "_id from documents) as B on A.id=B.cat" + id + "_id ";
                            query += " group by id,name,no order by no desc,name";
                var cat1 = db.Database.SqlQuery<catitemtotal>(query).ToList();
                string scat1 = "";
                string color = "";
                for (int jj = 0; jj < cat1.Count; jj++)
                {
                    if (cat1[jj].total <= 0) continue;
                    color = "";
                    //if (cat1[jj].catid.ToString() == f1) color = "color:red;font-weight:bold;";
                    //else if (cat1[jj].total > 0) 
                        color = "color:#000000;";
                        scat1 += "<li><a class='filteritem' onclick='gotoCat(" + id + "," + cat1[jj].id + ")' style='cursor:pointer;" + color + "'>" + cat1[jj].name + " (" + cat1[jj].total + ")</a></li>";
                }
                return scat1;
            }
            catch (Exception ex) {
                return "";
            }
        }
        public ActionResult Tree(string f1, string f2, string f3, string f4, string order, string to, int? pg)
        {

            f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
            f4 = f4 != null ? f4 : "";

            //ViewBag.keyword = k;
            if (pg == null) pg = 1;
            string query = "SELECT top 100 ";
            query += " id, name, code, cat1_id, cat2_id, cat3_id, cat4_id, views, 0 as Rank FROM documents ";
            //query += " order by  views desc";
            string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
            for (int f = 0; f < filter.Length; f++)
            {
                if (filter[f] != null && filter[f] != "")
                {
                    query += " and (cat" + (f + 1) + "=" + filter[f] + ") ";
                }
            }

            //    select catid,name,count(id) as total from
            string q1 = "(select catid,name,id from ";
            q1 = "(select id as catid,name from cat1) as A left join ";
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
            int pageSize = 21;
            int pageNumber = (pg ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public string getCat1(string keyword)
        {
            var p = (from q in db.cat1 where q.name.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat2(string keyword)
        {
            var p = (from q in db.cat2 where q.name.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat3(string keyword)
        {
            var p = (from q in db.cat3 where q.name.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat4(string keyword)
        {
            var p = (from q in db.cat4 where q.name.Contains(keyword) select q.name).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat1New(string keyword)
        {
            var p = (from q in db.cat1 where q.name.Contains(keyword) select new { value = q.name, id = q.id }).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat2New(string keyword)
        {
            var p = (from q in db.cat2 where q.name.Contains(keyword) select new { value = q.name, id = q.id }).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat3New(string keyword)
        {
            var p = (from q in db.cat3 where q.name.Contains(keyword) select new { value = q.name, id = q.id }).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public string getCat4New(string keyword)
        {
            var p = (from q in db.cat4 where q.name.Contains(keyword) select new { value = q.name, id = q.id }).Distinct().ToList();
            return JsonConvert.SerializeObject(p);
        }
        public int? getCatIdByName(int type,string keyword)
        {
            switch (type)
            {
                case 1:
                    var p = db.cat1.Where(o => o.name.Equals(keyword)).FirstOrDefault();
                    return p.id;
                    break;
                case 2:
                    var p2 = db.cat2.Where(o => o.name.Equals(keyword)).FirstOrDefault();
                    return p2.id;
                    break;
                case 3:
                    var p3 = db.cat3.Where(o => o.name.Equals(keyword)).FirstOrDefault();
                    return p3.id;
                    break;
                case 4:
                    var p4 = db.cat4.Where(o => o.name.Equals(keyword)).FirstOrDefault();
                    return p4.id;
                    break;
            }
            return null;
        }
        public string getCat(int type)
        {
            switch(type){
                case 1:
                    var p = (from q in db.cat1 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p);
                    break;
                case 2:
                    var p2 = (from q in db.cat2 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p2);
                    break;
                case 3:
                    var p3 = (from q in db.cat3 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p3);
                    break;
                case 4:
                    var p4 = (from q in db.cat4 orderby q.name select q).Distinct().OrderBy(o => o.name).ToList();
                    return JsonConvert.SerializeObject(p4);
                    break;
            }
            return "";
        }
        
    }
}
