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
            public DateTime? date_publish { get; set; }
            public DateTime? date_start { get; set; }
            public int RANK { get; set; }
            public byte? status { get; set; }

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
            public DateTime? date_publish { get; set; }
            public DateTime? date_start { get; set; }
            public byte? no { get; set; }

        }
        public class catitem
        {
            public int catid { get; set; }
            public string name { get; set; }
            public int total { get; set; }
        }
        public class documentitem
        {
            public int id { get; set; }
            public int document_id { get; set; }
            public string item_id { get; set; }
            public string item_content { get; set; }
            public int ch { get; set; }
            public int d { get; set; }
        }
        public ActionResult Index(string k, string f1, string f2, string f3, string f4, int? st, byte? status, byte? tps, int? ft, string order, string to, int? pg)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            string fts = "freetexttable";
            try
            {
                if (tps == 2 && (st != 1 & st != 2))
                {
                    string tempf1 = Config.getMaxCat1(k);
                    if (tempf1 != "" && tps == 2) f1 = tempf1;
                }
                if (tps == 1)
                {
                    Config.changeHeso(tps, k);
                }

                if (k != null && k.Trim() != "")
                {
                    if (ft == 1) { fts = "CONTAINSTABLE"; k = k.Replace(" ", "%"); }
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
                    query += "WHEN 7 THEN KEY_TBL.RANK*" + Config.heso1 + " ";
                    query += "WHEN 18 THEN KEY_TBL.RANK*" + Config.heso2 + " ";
                    query += "WHEN 15 THEN KEY_TBL.RANK*" + Config.heso3 + " ";
                    query += "WHEN 5 THEN KEY_TBL.RANK*" + Config.heso4 + " ";
                    query += "WHEN 23 THEN KEY_TBL.RANK*" + Config.heso5 + " ";
                    query += "WHEN 6 THEN KEY_TBL.RANK*" + Config.heso6 + " ";
                    query += "ELSE KEY_TBL.RANK ";
                    query += "END, FT_TBL.status FROM documents AS FT_TBL INNER JOIN " + fts + "(documents, auto_des,'" + k + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
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
                    query += ") as A ";
                    if (k != null && st == 2)
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
                    }
                    else
                    {
                        if (k != null && (st == 1))
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
                            query += "END,status from documents where (full_content like N'" + k + "%' or  full_content like N'%" + k.Replace(" ", "%") + "%') ";
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
                        string query1 = Config.makeQuery(ft, k, "1", f1, f2, f3, f4);
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
                        catch (Exception exc1)
                        {
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
                                scat2 += "<a class='filteritem' onclick='setCat(2," + cat2[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat2[jj].name + " (" + cat2[jj].total + ")</a>,";//" (" + cat2[jj].total + 
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
        public ActionResult Admin()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View();
        }
        public ActionResult Cat(int? cat11,int? cat22,int? cat44,string order,string to,int? pg) { 
           
                if (pg == null) pg = 1;
                string query = "select id,code,name,cat1_id,cat1,cat2_id,cat2,cat4_id,cat4,views,date_publish,date_start,no from ";
                query += " (select id,code,name,cat1_id,cat2_id,cat4_id,views,date_publish,date_start from documents) as A left join ";
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
                            if (cat1[jj].catid == cat11) color = "color:black;font-weight:bold;";
                            else if (cat1[jj].total > 0) color = "color:black;";
                            scat1 += "<a class='filteritem' onclick='setCat(1," + cat1[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat1[jj].name + " (" + cat1[jj].total + ")</a>,";
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
                            if (cat2[jj].catid == cat22) color = "color:black;font-weight:bold;";
                            else if (cat2[jj].total > 0) color = "color:black;";
                            scat2 += "<a class='filteritem' onclick='setCat(2," + cat2[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat2[jj].name + " (" + cat2[jj].total + ")</a>,";
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
                                color = "color:black;font-weight:bold;";
                            else if (cat4[jj].total > 0) color = "color:black;";

                            scat4 += "<a class='filteritem' onclick='setCat(4," + cat4[jj].catid + ")' style='cursor:pointer;" + color + "'>" + cat4[jj].name + " (" + cat4[jj].total + ")</a>,";
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
        public ActionResult ListCat()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View();
        }
        public class search
        {
            public string value { get; set; }
            public int id { get; set; }
        }
        public string getDoc(string keyword) {
            //if (keyword != null && (keyword.Contains("/") || keyword.Contains("-")))
            //{
                //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.code).Take(20);
                string query = "select * from (SELECT top 10 ";
                //query += "FT_TBL.code+' '+ FT_TBL.name as value,FT_TBL.id,RANK=CASE FT_TBL.cat2_id ";
                //query += "WHEN 7 THEN KEY_TBL.RANK*" + Config.heso1 + " ";
                //query += "WHEN 18 THEN KEY_TBL.RANK*" + Config.heso2 + " ";
                //query += "WHEN 15 THEN KEY_TBL.RANK*" + Config.heso3 + " ";
                //query += "WHEN 5 THEN KEY_TBL.RANK*" + Config.heso4 + " ";
                //query += "WHEN 23 THEN KEY_TBL.RANK*" + Config.heso5 + " ";
                //query += "WHEN 6 THEN KEY_TBL.RANK*" + Config.heso6 + " ";
                //query += "ELSE KEY_TBL.RANK ";
                //query += "END FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                //query += " where RANK>0) as A order by Rank Desc";
                //query = "select code+name as value,id from documents where code like N'" + keyword + "%'";
                // var p = db.Database.SqlQuery<search>(query);
                query = "select top 10 FT_TBL.word as value,KEY_TBL.RANK as id from log AS FT_TBL INNER JOIN FREETEXTTABLE(log, word,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,value";
                var p = db.Database.SqlQuery<search>(query);
                return JsonConvert.SerializeObject(p.ToList());
            //}
            //else
            //{
            //    //Sẽ thay bằng search fulltext
            //    //var p = (from q in db.documents where q.auto_des.Contains(keyword) select q.name).Take(20);
            //    //return JsonConvert.SerializeObject(p.ToList());
            //    string query = "select * from (SELECT top 10 ";
            //    query += "FT_TBL.name +' ' +FT_TBL.code as value,FT_TBL.id,RANK=CASE FT_TBL.cat2_id ";
            //    query += "WHEN 7 THEN KEY_TBL.RANK*" + Config.heso1 + " ";
            //    query += "WHEN 18 THEN KEY_TBL.RANK*" + Config.heso2 + " ";
            //    query += "WHEN 15 THEN KEY_TBL.RANK*" + Config.heso3 + " ";
            //    query += "WHEN 5 THEN KEY_TBL.RANK*" + Config.heso4 + " ";
            //    query += "WHEN 23 THEN KEY_TBL.RANK*" + Config.heso5 + " ";
            //    query += "WHEN 6 THEN KEY_TBL.RANK*" + Config.heso6 + " ";
            //    query += "ELSE KEY_TBL.RANK ";
            //    query += "END  FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
            //    query += " where RANK>0) as A order by Rank Desc";
            //    var p = db.Database.SqlQuery<search>(query);
            //    return JsonConvert.SerializeObject(p.ToList());
            //}
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
        public ActionResult test()
        {
            return View();
        }
        public ActionResult Details(int id, string keyword, string f1, string f2, string f3, string f4, int? st, byte? status, byte? tps, int? ft, string order, string to)
        {
            string fts = "freetexttable";
            document document = db.documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            db.Database.ExecuteSqlCommand("update documents set views=views+1 where id=" + id);
            if (keyword == null || (st == 2 || keyword.Contains("/"))) keyword = "";
            ViewBag.keyword = keyword;
            ViewBag.f1 = f1;
            ViewBag.f2 = f2;
            ViewBag.f3 = f3;
            ViewBag.f4 = f4;
            ViewBag.st = st;
            ViewBag.status = status;
            ViewBag.order = order;
            ViewBag.to = to;
            ViewBag.ft = ft;
            //if (keyword!="" && keyword!=null)
            try
            {
                //string keyword2 = keyword;
                //if (st == 2 || keyword.Contains("/")) keyword2 = "";
                string k = keyword;
                if (ft == 1) { fts = "CONTAINSTABLE"; k = k.Replace(" ", "%"); }
                else
                { k = k.Replace("%20", " ").Replace("%", " "); }
                //if (ft == 1) { 
                    if (k == "" || k == null) {
                        var p = (from q in db.document_items where q.document_id == id && q.item_content.Contains(keyword) select q).OrderBy(o => o.ch).ThenBy(o => o.d).ToList();
                        ViewBag.chd = p;
                    }
                    else {
                        string query = "SELECT FT_TBL.id,FT_TBL.document_id,FT_TBL.item_id,FT_TBL.item_content,FT_TBL.ch,FT_TBL.d,RANK FROM document_items AS FT_TBL INNER JOIN " + fts + "(document_items, item_content,'" + k + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] and FT_TBL.document_id = "+id+" order by ch,d ";
                        var p = db.Database.SqlQuery<documentitem>(query).ToList();
                        ViewBag.chd = p;
                    }
                    
                //}
                //else
                //{
                    //var p = (from q in db.document_items where q.document_id == id && q.item_content.Contains(keyword) select q).OrderBy(o => o.ch).ThenBy(o => o.d).ToList();
                    //ViewBag.chd = p;
                //}
                ViewBag.tree = Config.showTree(id, keyword, f1, f2, f3, f4, st, status,tps,ft, order, to);
            }
            catch (Exception ex) { }
            return View(document);
        }
        public ActionResult Items(int id)
        {
            document_items chd = db.document_items.Find(id);
            return View(chd);
        }
        //
        // GET: /Document/Create
        public string getCHD(int id)
        {
            document_items chd = db.document_items.Find(id);
            return chd.item_content.Replace("\t", " ").Replace("\r\n", "<br>");
        }
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
                ViewBag.date_publish = "";
                ViewBag.date_start = "";
                ViewBag.status = 0;
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
                try { 
                    ViewBag.cat1new = db.cat1.Find(document.cat1_id).name;
                }catch(Exception ex){
                    ViewBag.cat1new = "";
                }
                ViewBag.cat2 = document.cat2_id;
                try { 
                ViewBag.cat2new = db.cat2.Find(document.cat2_id).name;
                }
                catch (Exception ex)
                {
                    ViewBag.cat2new = "";
                }
                ViewBag.cat3 = document.cat3_id;
                try { 
                    ViewBag.cat3new = db.cat3.Find(document.cat3_id).name;
                }
                catch (Exception ex)
                {
                    ViewBag.cat3new = "";
                }
                ViewBag.cat4 = document.cat4_id;
                try { 
                    ViewBag.cat4new = db.cat4.Find(document.cat4_id).name;
                }
                catch (Exception ex)
                {
                    ViewBag.cat4new = "";
                }
                ViewBag.keyword1 = document.keyword1;
                ViewBag.keyword2 = document.keyword2;
                ViewBag.keyword3 = document.keyword3;
                ViewBag.keyword4 = document.keyword4;
                ViewBag.keyword5 = document.keyword5;
                ViewBag.be_linked = document.be_linked;
                ViewBag.link_to = document.link_to;
                ViewBag.link = document.link;
                ViewBag.link_to_over_date = document.link_to_over_date;
                ViewBag.year = document.year;
                ViewBag.related_id = document.related_id;
                DateTime? date_publish = document.date_publish;
                ViewBag.date_publish = date_publish == null ? "" : String.Format("{0:yyyy-MM-dd}", date_publish);//.ToString("MM/dd/yyyy");//.ToShortDateString()
                DateTime? date_start = document.date_start;
                ViewBag.date_start = date_start == null ? "" : String.Format("{0:yyyy-MM-dd}", date_start);//.ToString("MM/dd/yyyy")//.ToShortDateString();
                ViewBag.status = document.status;
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
            string date_publish = "";
            string people_sign = "";//trả về người ký văn bản
            string full_content = "";
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
                    full_content = content;
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
                    date_publish = Config.getDatePublish(content);
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
            return code + Config.sp + title + Config.sp + p1 + Config.sp + p2 + Config.sp + p3 + Config.sp + p4 + Config.sp + p5 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + publish + Config.sp + people_sign + Config.sp + date_publish;// code + Config.sp + title + Config.sp + p1 + Config.sp + nameFile + Config.sp + type_document + Config.sp + year + Config.sp + p2;// "/Files/" + nameFile;
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
        public void addNewCatIfNew(int id,string f1,string f2,string f3,string f4)
        {
            try{
                var p1 = db.cat1.Where(o => o.name == f1).FirstOrDefault();
                if (f1 != null && f1 != "" && p1 == null)
                {
                    cat1 c1 = new cat1();
                    c1.name = f1;
                    c1.no = 0;
                    db.cat1.Add(c1);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("update documents set cat1_id=" + c1.id + " where id=" + id);
                }
            }catch(Exception c1ex){

            }
            try
            {
                var p2 = db.cat2.Where(o => o.name == f2).FirstOrDefault();
                if (f2 != null && f2 != "" && p2 == null)
                {
                    cat2 c2 = new cat2();
                    c2.name = f2;
                    c2.no = 0;
                    db.cat2.Add(c2);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("update documents set cat2_id=" + c2.id + " where id=" + id);
                }
            }
            catch (Exception c2ex)
            {

            }
            try
            {
                var p3 = db.cat3.Where(o => o.name == f3).FirstOrDefault();
                if (f3 != null && f3 != "" && p3 == null)
                {
                    cat3 c3 = new cat3();
                    c3.name = f3;
                    c3.no = 0;
                    db.cat3.Add(c3);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("update documents set cat3_id=" + c3.id + " where id=" + id);
                }
            }
            catch (Exception c3ex)
            {

            }
            try
            {
                var p4 = db.cat4.Where(o => o.name == f4).FirstOrDefault();
                if (f4 != null && f4 != "" && p4 == null)
                {
                    cat4 c4 = new cat4();
                    c4.name = f4;
                    c4.no = 0;
                    db.cat4.Add(c4);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("update documents set cat4_id=" + c4.id + " where id=" + id);
                }
            }
            catch (Exception c4ex)
            {

            }
            
        }
        public void addNewWordToDic(string val)
        {
            string[] a = val.Split(',');
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Trim() != "")
                {
                     try
                     {
                        string temp = a[i].Trim();
                        var p1 = db.dic_normal.Where(o => o.word == temp).FirstOrDefault();
                        if (p1 == null)
                        {
                            dic_normal dic = new dic_normal();
                            dic.word = temp;
                            db.dic_normal.Add(dic);
                            db.SaveChanges();
                        }
                     }
                     catch (Exception c1ex2)
                     {

                     }
                }
            }
        }
        public string addNewCatReturnId(int type, string val)
        {
            if (type == 1) { 
                try
                {
                    var p1 = db.cat1.Where(o => o.name == val).FirstOrDefault();
                    if (val != null && val != "" && p1 == null)
                    {
                        cat1 c1 = new cat1();
                        c1.name = val;
                        c1.no = 0;
                        db.cat1.Add(c1);
                        db.SaveChanges();
                        return c1.ToString();
                    }
                    else if (p1 != null) return p1.id.ToString();
                }
                catch (Exception c1ex)
                {

                }
            }
            if (type == 2)
            {
                try
                {
                    var p2 = db.cat2.Where(o => o.name == val).FirstOrDefault();
                    if (val != null && val != "" && p2 == null)
                    {
                        cat2 c2 = new cat2();
                        c2.name = val;
                        c2.no = 0;
                        db.cat2.Add(c2);
                        db.SaveChanges();
                        return c2.ToString();
                    }
                    else if (p2 != null) return p2.id.ToString();
                }
                catch (Exception c2ex)
                {

                }
            }
            if (type == 3)
            {
                try
                {
                    var p3 = db.cat3.Where(o => o.name == val).FirstOrDefault();
                    if (val != null && val != "" && p3 == null)
                    {
                        cat3 c3 = new cat3();
                        c3.name = val;
                        c3.no = 0;
                        db.cat3.Add(c3);
                        db.SaveChanges();
                        return c3.id.ToString();
                    }
                    else if (p3 != null) return p3.id.ToString();
                }
                catch (Exception c3ex)
                {

                }
            }
            if (type == 4) { 
                try
                {
                    var p4 = db.cat4.Where(o => o.name == val).FirstOrDefault();
                    if (val != null && val != "" && p4 == null)
                    {
                        cat4 c4 = new cat4();
                        c4.name = val;
                        c4.no = 0;
                        db.cat4.Add(c4);
                        db.SaveChanges();
                        return c4.id.ToString();
                    }
                    else if (p4 != null) return p4.id.ToString();
                }
                catch (Exception c4ex)
                {

                }
            }
            return "";
        }
        [HttpPost]
        public string addNewDocument(int id, string name, string code, string link, string keyword1, string keyword2, string keyword3, string keyword4, string keyword5, int? cat1, int? cat2, int? cat3, int? cat4, int? year, string related_id, string be_linked, string link_to,string link_to_over_date, DateTime? date_publish, DateTime? date_start, string full_content, byte? status)
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
                    string f1 = "";
                    if (cat1 != null && cat1 > 0) { try { f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name; } catch (Exception cf1) { } }
                    string f2 = "";
                    if (cat2 != null && cat2 > 0) { try { f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name; } catch (Exception cf2) { } }
                    string f3 = "";
                    if (cat3 != null && cat3 > 0) { try { f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name; } catch (Exception cf3) { } }
                    string f4 = "";
                    if (cat4 != null && cat4 > 0) { try { f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name; } catch (Exception cf4) { } }
                    string allKeyWord = keyword1 + " " + " " + keyword2 + " " + " " + keyword3 + " " + " " + keyword4 + " " + " " + keyword5;
                    allKeyWord = allKeyWord.Replace(" , ", " ");
                    doc.auto_des = code + " " + name + " " + code + " " + name + " " + code + " " + name + " " + allKeyWord + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    doc.status = status;
                    doc.type = 0;
                    doc.year = year;
                    doc.views = 0;
                    doc.be_linked = be_linked;
                    doc.link_to = link_to;
                    doc.link_to_over_date = link_to_over_date;
                    doc.date_publish = date_publish;// Config.convertToDate(date_publish);
                    doc.date_start = date_start;// Config.convertToDate(date_start);
                    //if (doc.date_publish == DateTime.MinValue) doc.date_publish = null;
                    //if (doc.date_start == DateTime.MinValue) doc.date_start = null;
                    doc.full_content = null;
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
                    readFull(link, doc.id);
                    //addNewCatIfNew(doc.id,f1, f2, f3, f4);
                    addNewWordToDic(keyword1 + ","+ keyword2 + ","+keyword3 + ","+ keyword4);
                    return doc.id.ToString();
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
                    string f1 = "";
                    if (cat1 != null && cat1 > 0) { 
                        try { 
                            f1 = db.cat1.Where(o => o.id == cat1).FirstOrDefault().name; 
                        } catch (Exception cf1) { 
                        } 
                    }
                    string f2 = "";
                    if (cat2 != null && cat2 > 0) { try { f2 = db.cat2.Where(o => o.id == cat2).FirstOrDefault().name; } catch (Exception cf2) { } }
                    string f3 = "";
                    if (cat3 != null && cat3 > 0) { try { f3 = db.cat3.Where(o => o.id == cat3).FirstOrDefault().name; } catch (Exception cf3) { } }
                    string f4 = "";
                    if (cat4 != null && cat4 > 0) { try { f4 = db.cat4.Where(o => o.id == cat4).FirstOrDefault().name; } catch (Exception cf4) { } }
                    string allKeyWord = keyword1 + " " + " " + keyword2 + " " + " " + keyword3 + " " + " " + keyword4 + " " + " " + keyword5;
                    allKeyWord = allKeyWord.Replace(" , ", " ");
                    doc.auto_des = code + " " + name + " " + code + " " + name + " " + code + " " + name + " " + allKeyWord + " " + f1 + " " + f2 + " " + f3 + " " + f4;
                    //doc.date_time = DateTime.Now;
                    doc.related_id = related_id;
                    doc.status = status;
                    //doc.type = 0;
                    doc.year = year;
                    doc.be_linked = be_linked;
                    doc.link_to = link_to;
                    doc.link_to_over_date = link_to_over_date;
                    doc.date_publish = date_publish;// Config.convertToDate(date_publish);
                    doc.date_start = date_start;// Config.convertToDate(date_start);
                    //if (doc.date_publish == DateTime.MinValue) doc.date_publish = null;
                    //if (doc.date_start == DateTime.MinValue) doc.date_start = null;
                    doc.full_content = null;
                    db.Entry(doc).State = EntityState.Modified;
                    db.SaveChanges();
                    readFull(link, id);
                    addNewWordToDic(keyword1 + "," + keyword2 + "," + keyword3 + "," + keyword4);
                    //addNewCatIfNew(id,f1, f2, f3, f4);
                    return id.ToString();
                }
            }
            catch (Exception ex) {
                    return "0";
            }
            return "0";
        }
        public void readFull(string link,int id)
        {

            string content = "";
            StringBuilder sb = new StringBuilder();
            WordprocessingDocument wordprocessingDocument = null;
                content = "";
                string fullPath = HttpContext.Server.MapPath("../Files/" + link);
                try
                {
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
                if (content == "") return;
                try
                {
                    //db.Database.ExecuteSqlCommand("update documents set full_content=N'" + content + "' where id=" + p[i].id);
                    document dt = db.documents.Find(id);
                    dt.full_content = content;
                    db.Entry(dt).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception exqss)
                {
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
        public string updateDI(int count, int id, string codevb)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from document_items where document_id=" + id);
                string all_des = " ";
                if (id != 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        try { 
                            string name = Request.Form["cbIndex" + i].ToString();
                            all_des += name+" ";
                            string code = Request.Form["chIndex" + i].ToString() + "_"+Request.Form["dIndex" + i].ToString();
                            if (name != "" && name != null)
                            {
                                document_items gi = new document_items();
                                gi.document_id = id;
                                gi.item_content = name;
                                gi.item_id = code;
                                gi.ch = int.Parse(Request.Form["chIndex" + i].ToString());
                                gi.d = int.Parse(Request.Form["dIndex" + i].ToString());
                                db.document_items.Add(gi);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex2) { 
                        }                        
                    }
                    db.Database.ExecuteSqlCommand("update documents set auto_des=name+N'" + " " + codevb + " " + all_des + "' where id=" + id);
                }
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string getGI(int id)
        {
            var p = (from q in db.document_items where q.document_id == id select q).OrderBy(o=>o.ch).ThenBy(o=>o.d).ToList();
            
            try
            {
               
                return JsonConvert.SerializeObject(p);
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public string setstatus(int id, int? type)
        {
            if (Config.getCookie("userid") == "") return "0";
            try
            {
                db.Database.ExecuteSqlCommand("update documents set status="+type+" where id=" + id);
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}