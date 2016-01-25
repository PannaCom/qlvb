using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using qlvb.Models;
namespace qlvb
{
    public class Config
    {
        public static string sp = "____________";
        private static qlvbEntities db=new qlvbEntities();
        public static string domain = "http://localhost:59574/";
        public static string getCode(string content){
            try{
                Regex titRegex = new Regex(@"Số: (.*?)/(.*?)/.*[A-Z]\s", RegexOptions.IgnoreCase);//Số: .*/.*/.*\S-*([A-Z])\r
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";
                string[] code = content.Split(' ');
                return code[1];
            }catch{
                return "";
            }
        }
        public static string getYear(string content)
        {
            try
            {
                Regex titRegex = new Regex(@"năm [0-9]{4}", RegexOptions.IgnoreCase);
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";
                string[] code = content.Split(' ');
                return code[1];
            }
            catch
            {
                return "";
            }
        }
        //năm [0-9]{4}\s\S\s\S\s\S(.*?).*\s\S.*\s\S.*
        public static string getTitle(string content)
        { 
            
            try
            {
                Regex titRegex = new Regex(@"năm [0-9]{4}\s\S\s\S\s\S(.*?).*\s\S.*\s\S.*", RegexOptions.IgnoreCase);
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";
                string[] code = content.Split('\r');
                string rs = "";
                int l = code.Length > 10 ? 10 : code.Length;
                for (int i = 1; i < l; i++) {
                    if (code[i].StartsWith("Căn cứ")) break;
                    if (code[i] != "\a" && code[i] != "") {
                        rs += code[i] + " ";
                    }
                    
                }
                return rs;
            }
            catch
            {
                return "";
            }
        }
        public static string getP1(string content) {
            try
            {
                Regex titRegex = new Regex(@"(?<=Điều 1. )(.*)(?=Điều 2. )", RegexOptions.IgnoreCase);
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";
                
                return content;
            }
            catch
            {
                return "";
            }
        }
        public static string getP2(string content)
        {
            try
            {
                Regex titRegex = new Regex(@"(?<=Điều 2. )(.*)(?=Điều 3. )", RegexOptions.IgnoreCase);
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";

                return content;
            }
            catch
            {
                return "";
            }
        }
        //public static string[] arrCat1=new string[100];
        public static Array getCat2()
        {
            var p=(from q in db.cat2 select q.name).ToArray();
            return p;
        }
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string getCatNameById(int type,int? id) { 
            switch(type){
                case 1:
                    string p = db.cat1.Where(o => o.id == id).FirstOrDefault().name;
                    return p;
                    break;
                case 2:
                    string p2 = db.cat2.Where(o => o.id == id).FirstOrDefault().name;
                    return p2;
                    break;
                case 3:
                    string p3 = db.cat3.Where(o => o.id == id).FirstOrDefault().name;
                    return p3;
                    break;
                case 4:
                    string p4 = db.cat4.Where(o => o.id == id).FirstOrDefault().name;
                    return p4;
                    break;
            }
            return "";
        }
        public static string  makeQuery(string k,string cols,string f1,string f2,string f3,string f4){
            string query="select catid,name,count(id) as total from ";
            query+="(select catid,name,id from ";
            query += "(select id as catid,name from cat" + cols + ") as A left join ";
            query += "(select FT_TBL.cat" + cols + "_id,FT_TBL.id from documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'"+k+"')  AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY]) as B on A.catid=B.cat"+cols+"_id ";
            
                string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                for (int f = 0; f < filter.Length; f++)
                {
                    if (filter[f] != null && filter[f] != "")
                    {
                        query += " and (cat" + (f+1) + "_id="+filter[f]+") ";
                    }
                }
            query += " ) as C group by catid,name ";
            return query;
        }
    }
}