﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;
using PagedList;
using Newtonsoft.Json;

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
            public int RANK { get; set; }

        }
        public ActionResult Index(string k, string f1, string f2, string f3, string f4, string order, string to, int? pg)
        {
            if (k != null && k.Trim()!="")
            {
                k = k.Replace("%20", " ");
              
                f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
                f4 = f4 != null ? f4 : ""; 
                
                ViewBag.keyword = k;
                if (pg == null) pg = 1;
                string query = "SELECT top 100 ";
                query += "FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.views, KEY_TBL.RANK FROM documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + k + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] ";
                query += " where (status=0) ";

                string[] item = new string[10];
                int i = 0;
                string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                for (int f = 0; f < filter.Length; f++)
                {
                    if (filter[f] != null && filter[f] != "")
                    {
                        query += " and (cat" + (f+1) + "="+filter[f]+") ";
                    }
                }
                if (order == null || order=="") order = "RANK";
                query += " order by " + order;
                if (to == null || to == "") to = "Desc";
                query += " " + to;
               
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
            else {
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
                int pageSize = 21;
                int pageNumber = (pg ?? 1);
                return View(p.ToPagedList(pageNumber, pageSize));
            }
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
