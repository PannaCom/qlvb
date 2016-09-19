using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using qlvb.Models;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using DocumentFormat.OpenXml;
using System.Globalization;
namespace qlvb
{
    public class Config
    {
        public static string sp = "____________";
        private static qlvbEntities db=new qlvbEntities();
        public static string domain = "http://vanbanquocgia.com";//"http://localhost:59574/";
        public static Hashtable allword=null;
        public static int minRank = 0;
        public static int heso1 = 1000;
        public static int heso2 = 500;
        public static int heso3 = 50;
        public static int heso4 = 10;
        public static int heso5 = 5;
        public static int heso6 = 2;
        public static bool isRunning = false;
        public static void changeHeso(int? tps,string k)
        {
            heso1 = 1000;
            heso2 = 500;
            heso3 = 50;
            heso4 = 10;
            heso5 = 5;
            heso6 = 2;
            if (tps == 1)
            {
                //query += "WHEN 7 THEN KEY_TBL.RANK*" + Config.heso1 + " ";
                //query += "WHEN 18 THEN KEY_TBL.RANK*" + Config.heso2 + " ";
                //query += "WHEN 15 THEN KEY_TBL.RANK*" + Config.heso3 + " ";
                //query += "WHEN 5 THEN KEY_TBL.RANK*" + Config.heso4 + " ";
                //query += "WHEN 23 THEN KEY_TBL.RANK*" + Config.heso5 + " ";
                //query += "WHEN 6 THEN KEY_TBL.RANK*" + Config.heso6 + " ";
                heso1 = (int)db.cat2.Find(7).no;
                heso2 = (int)db.cat2.Find(18).no;
                heso3 = (int)db.cat2.Find(15).no;
                heso4 = (int)db.cat2.Find(5).no;
                heso5 = (int)db.cat2.Find(23).no;
                heso6 = (int)db.cat2.Find(6).no;
                //if (k != null & k != "" && k.Split(' ').Length <= 1) minRank = -1; else minRank = 0;
            }

        }
        public static void loadDic(){
            var p = (from q in db.dic_normal select q).ToList();
            if (allword == null || allword.Count <= 0) allword = new Hashtable(); 
            //else return;
            for (int i = 0; i < p.Count; i++) {
                if (!allword.ContainsKey(p[i].word.ToLowerInvariant().Trim())) {
                    allword.Add(p[i].word.ToLowerInvariant().Trim(), "1");
                }
            }
        }
        public static string showQuote(string content,string keyword){
            if (keyword.Trim() == "") return content;
            string[] sen = content.Split('.');
            string rs = "";
            for (int i = 0; i < sen.Length; i++) {
                if (sen[i].Contains(keyword) || sen[i].ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                {
                    sen[i] = keyword != "" ? sen[i].ToLowerInvariant().Replace(keyword.ToLowerInvariant(), "<span style=\"background:yellow;color:black;\">" + keyword + "</span>") : sen[i];
                    rs += "<blockquote>..." + sen[i] + "<p></p>...</blockquote></p>";
                }
            }
            return rs;
        }
        public static string showQuoteText(string content, string keyword)
        {
            if (keyword.Trim() == "") return content;
            string[] sen = content.Split('.');
            string[] basicword = keyword.Split(' ');
            string rs = "";
            bool found=false;
            int fromii = 0;
            for (int i = 0; i < sen.Length; i++)
            {
                found=false;
                if (!sen[i].Contains(keyword))
                {
                    for (int l = 0; l < basicword.Length; l++)
                    {
                        if (sen[i].Contains(basicword[l])) { fromii = l; found = true; break; }
                    }
                }
                if (found || sen[i].Contains(keyword) || sen[i].ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                {
                    sen[i] = keyword != "" ? sen[i].ToLowerInvariant().Replace(keyword.ToLowerInvariant(), "<span style=\"background:yellow;color:black;\">" + keyword + "</span>") : sen[i];
                    int from = sen[i].IndexOf(keyword);
                    if (found && !sen[i].Contains(keyword)) from = sen[i].IndexOf(basicword[fromii]);
                    int ffrom = from - 100 > 0 ? from - 100 : 0;
                    int fto = from + 100 > sen[i].Length ? sen[i].Length - 2 : from + 100-2;
                    rs += "..." + sen[i].Substring(ffrom,fto-ffrom) + "...";
                    
                    break;
                }
            }
            return rs;
        }
        public static string showCHD(string content)
        {
            string[] sen = content.Split('_');

            return "Chương " + sen[0] + ". Điều "+sen[1];
        }
        public static string getPublish(string content)
        {
            try
            {
                content=content.Trim();
                int to = content.IndexOf("Số:");
                if (to < 0) to = int.MaxValue;
                int to2 = content.IndexOf("CỘNG HÒA");
                int to3 = content.IndexOf("CỘNG HOÀ");
                int to4 = content.IndexOf("__");
                if (to2>0 && to2 < to) to = to2;
                if (to3>0 && to3 < to) to = to3;
                if (to4 > 0 && to4 < to) to = to4;
                string val = content.Substring(0, to);
                val = val.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                val = val.Replace("  ", " ").Trim();
                return val;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getPeopleSign(string content)
        {
            try
            {
                content = content.Trim().ToLowerInvariant();
                int to = content.IndexOf("(Đã ký)");
                if (to < 0) to = content.IndexOf("(đã ký)");
                if (to > 0) {
                    var p = (from q in db.cat3 select q.name).ToList();
                    for (int i = 0; i < p.Count;i++ )
                    {
                        string item = p[i].ToLowerInvariant();
                        if (content.Contains(item) && content.IndexOf(item) >= to)
                        {
                            return item;
                        }
                    }
                }
                
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getCode(string content){
            try{
                content = content.Replace(" /", "/");
                Regex titRegex = new Regex(@"[0-9]*[^a-zA-Z0-9][0-9]{4}[^a-zA-Z0-9][a-zA-Z]*\S\S*", RegexOptions.IgnoreCase);//Số: .*/.*/.*\S-*([A-Z])\r    //Số: (.*?)/(.*?)/.*[A-Z]\s
                Match titm = titRegex.Match(content);
                bool notfound = false;
                string code = "";
                if (titm.Success)
                {
                    code = titm.Groups[0].Value;
                }
                else notfound = true;
                if (code.Trim().EndsWith("-")) notfound = true;
                if (notfound) { 
                    int from=content.IndexOf("Số:");
                    int to=content.IndexOf("CỘNG HÒA");
                    if (to < 0) to = content.IndexOf("CỘNG HOÀ");
                    code = content.Substring(from, to - from);
                    code = code.Replace(" ", "");
                    code = code.Replace("Số:", "");
                }
                //string[] code = content.Split(' ');
                //return code[1];
                return code.Trim();
            }catch{
                return "";
            }
        }
        public static string getP5(string content)
        {
            try
            {
                content = content.Replace(" /", "/");
                Regex titRegex = new Regex(@"\s[0-9]*[^a-zA-Z0-9][0-9]{4}[^a-zA-Z0-9][a-zA-Z]*\S\S*", RegexOptions.IgnoreCase);//\s[0-9]*[^a-zA-Z0-9][0-9]{4}[^a-zA-Z0-9][a-zA-Z]*[^a-zA-Z0-9]\S\S*
                
                //Match titm = titRegex.Match(content);
                //if (titm.Success)
                //{
                //    content = titm.Groups[0].Value;
                //}
                //else return "";
                MatchCollection titm = titRegex.Matches(content);
                content = "";
                foreach (Match m in titm)
                {
                    //Console.WriteLine("'{0}' found at index {1}.",
                    //                  m.Value, m.Index);
                    string temp = m.Value.Replace(";", "").Replace(".", "").Replace(")", "").Replace("(", "").Replace(",", "");
                    if (temp.Split('/').Length >= 3)
                    {
                        if (!content.Contains(temp)) content += temp + " , ";
                    }
                }
                //string[] code = content.Split(' ');
                return content;
            }
            catch
            {
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
                
                content = content.Replace("\n", " ").Replace(".", " ").Replace(",", " ").Trim();
                content = content.Replace("\r", "");
                content = content.Replace("\n", "");
                content = content.Replace("  ", " ");
                Regex titRegex = new Regex(@"năm [0-9]{4}(.*?)\s\S.*\s\S.*", RegexOptions.IgnoreCase);//năm [0-9]{4}\s\S\s\S\s\S(.*?).*\s\S.*\s\S.* //năm [0-9]{4}\r\n(.*?)\s\S.*\s\S.*
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Groups[0].Value;
                }
                else return "";
                //string[] code = content.Split('\r');
                //string rs = "";
                //int l = code.Length > 10 ? 10 : code.Length;
                //for (int i = 1; i < l; i++) {
                //    if (code[i].StartsWith("Căn cứ")) break;
                //    if (code[i] != "\a" && code[i] != "") {
                //        rs += code[i] + " ";
                //    }
                    
                //}
                //Bóc tách từ khóa
                
                for (int i = 1999; i <= 2051; i++) {
                    string temp="năm " + i.ToString();
                    if (content.Trim().StartsWith(temp)) {
                        content = content.Substring(temp.Length, content.Length - temp.Length-1);
                        break;
                    }
                }
                string rs="";
                if (content.IndexOf("Căn cứ")>0) rs = content.Substring(0, content.IndexOf("Căn cứ"));
                //if (rs.Equals("")) { 

                //}
                if (rs.Contains("_")) rs = rs.Substring(0, rs.IndexOf("_"));
                if (rs.Contains("----")) rs = rs.Substring(0, rs.IndexOf("----"));
                
                return rs.Trim();//getKeyWordFromContent(rs);
                //return rs;
            }
            catch
            {
                return "";
            }
        }
        
        public static string getKeyWordFromContent(string content){
            try
            {
                content = content.Replace("\n", " ").ToLowerInvariant().Replace(".", " ").Replace(",", " ").Trim();
                content = content.Replace("  ", " ");
                int lengthWord = 8;
                string result = "";
                string[] arrContent = content.Split(' ');
                while (lengthWord >= 2)
                {
                    for (int l = 0; l <= arrContent.Length - lengthWord; l++)
                    {
                        string tempword = "";
                        for (int l1 = l; l1 < l + lengthWord; l1++)
                        {
                            tempword += arrContent[l1] + " ";
                        }
                        tempword = tempword.ToLowerInvariant().Trim();
                        int wordCount = Regex.Matches(content, "\\b" + Regex.Escape(tempword) + "\\b", RegexOptions.IgnoreCase).Count;
                        if ((allword.ContainsKey(tempword) && !result.Contains(tempword) && tempword.Split(' ').Length >= 2) || wordCount>=4)
                        {
                            result += tempword + " , ";
                        }
                        if (result.Split(' ').Length >= 80) break;
                    }
                    lengthWord--;
                    if (result.Split(' ').Length >= 80) break;
                }
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static bool hasNormalWord(string content)
        {
            string[] no = new string[] { "quy định tại khoản","các khoản","trước ngày", "tại các khoản", "quy định tại các", "bộ tài nguyên và môi trường", "tỉnh  thành phố trực thuộc trung ương", "được cơ quan có thẩm quyền" };
            for (int i = 0; i < no.Length; i++)
            {
                if (content.Contains(no[i])) return true;
            }
            return false;
        }
        public static string getTopKeyword(string content)
        {
            try
            {
                content = content.Replace("\n", " ").ToLowerInvariant().Replace(".", " ").Replace(",", " ").Trim();
                int lengthWord = 8;
                string result = "";
                string[] arrContent = content.Split(' ');
                int tempcount = 0;
                int l1 = 0;
                Dictionary<String, int> top = new Dictionary<String, int>();
                while (lengthWord >= 4)
                {
                    int l = 0;
                    while(l <= arrContent.Length - lengthWord)
                    {
                        StringBuilder atempword = new StringBuilder();
                        for (l1 = l; l1 < l + lengthWord; l1++)
                        {
                            atempword.Append(arrContent[l1] + " ");
                        }
                        string tempword = atempword.ToString().ToLowerInvariant().Trim();
                        if (hasNormalWord(tempword) || result.Contains(tempword))
                        { l++; continue; }
                        int wordCount = Regex.Matches(content, "\\b" + Regex.Escape(tempword) + "\\b", RegexOptions.IgnoreCase).Count;
                        if ((allword.ContainsKey(tempword) && tempword.Split(' ').Length >= 2) || wordCount>=4)
                        {
                            result += tempword + " , ";
                            if (top.ContainsKey(tempword))
                            {
                                tempcount = top[tempword] + 1;
                                top.Remove(tempword);
                                top.Add(tempword, tempcount);
                            }
                            else
                            {
                                top.Add(tempword, 1);
                            }
                            l = l1;
                        }
                        if (top.Count >= 24) break;
                        l++;
                    }
                    lengthWord--;
                    if (top.Count >= 24) break;
                }
                var sortedDict = from entry in top orderby entry.Value descending select entry;
                tempcount = 0;
                result = "";
                foreach (var entry in sortedDict)
                {
                    tempcount++;
                    result += entry.Key + " , ";
                    if (tempcount >= 24) break;
                }
                return result;
            }
            catch (Exception ex) {
                return "";
            }
        }
        public static string getHotKeyword(string content)
        {
            try
            {
                content = content.ToLowerInvariant().Replace(".", " ").Trim();
                string result = "";
                string[] arrContent = content.Split(',');
                int tempcount = 0;
                Dictionary<String, int> top = new Dictionary<String, int>();

                for (int l = 0; l < arrContent.Length; l++)
                {
                    string tempword = arrContent[l];
                    tempword = tempword.ToLowerInvariant().Trim();
                    if (tempword!="" && tempword.Split(' ').Length >= 2)
                    {
                        //result += tempword + " , ";
                        if (top.ContainsKey(tempword))
                        {
                            int klength = tempword.Split(' ').Length;
                            if (klength <= 2) klength = 1; else klength = klength * 10;
                            tempcount = top[tempword] + klength;
                            top.Remove(tempword);
                            top.Add(tempword, tempcount);
                        }
                        else
                        {
                            int klength = tempword.Split(' ').Length;
                            if (klength <= 2) klength = 1; else klength=klength*10;
                            top.Add(tempword, klength);
                        }
                    }
                }

                var sortedDict = from entry in top orderby entry.Value descending select entry;
                tempcount = 0;
                foreach (var entry in sortedDict)
                {
                    if (entry.Key.Split(' ').Length >= 10) continue;
                    tempcount++;
                    
                    //result += entry.Key + " , ";
                    if (!entry.Key.Equals("xã hội chủ nghĩa") && !entry.Key.Equals("cộng hòa") && !entry.Key.Equals("việt nam")) result += "<li><a class='filteritem' style=\"cursor:pointer;\" onclick=\"searchkw('" + entry.Key + "');\">" + entry.Key + "</a></li>";
                    if (tempcount >= 20) break;
                }
                return result;
            }
            catch (Exception ex) {
                return "";
            }
        }
        public static string getP1(string content) {
            try
            {
                //Regex titRegex = new Regex(@"(?<=Điều 1. )(.*)(?=Điều 2. )", RegexOptions.IgnoreCase);
                //Match titm = titRegex.Match(content);
                //if (titm.Success)
                //{
                //    content = titm.Groups[0].Value;
                //}
                //else return "";
                
                //return content;
                return getKeyWordFromContent(content);
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
                string content1,content2;
                Regex titRegex = new Regex(@"(\sĐiều 1. )(.*?)(\sĐiều 2. )", RegexOptions.IgnoreCase);//(?<=Điều 1. )(.*)(?=Điều 2. )
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content1 = titm.Groups[0].Value;
                }
                else content1="";
                content1 = content1.Replace("Điều 1. ", "").Replace("Điều 2. ", "");
                titRegex = new Regex(@"(\sĐiều 2. )(.*?)(\sĐiều 3. )", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content2 = titm.Groups[0].Value;
                }
                else content2 = "";
                content2 = content2.Replace("Điều 2. ", "").Replace("Điều 3. ", "");
                return getKeyWordFromContent(content1 + content2);
            }
            catch
            {
                return "";
            }
        }
        public static string getP3(string content)
        {
            try
            {
                string content1, content2, content3;
                Regex titRegex = new Regex(@"(\sChương I )(.*?)(\sChương II )", RegexOptions.IgnoreCase);//(?<=Điều 1. )(.*)(?=Điều 2. )
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content1 = titm.Groups[0].Value;
                }
                else content1 = "";
                //content1 = content1.Replace("Điều 1. ", "").Replace("Điều 2. ", "");
                titRegex = new Regex(@"(\sChương II )(.*?)(\sChương III )", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content2 = titm.Groups[0].Value;
                }
                else content2 = "";
               // content2 = content2.Replace("Điều 2. ", "").Replace("Điều 3. ", "");
                titRegex = new Regex(@"(\sChương III )(.*?)(\sChương IV )", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content3 = titm.Groups[0].Value;
                }
                else content3 = "";
                //content3 = content2.Replace("Điều 3. ", "").Replace("Điều 4. ", "");
                return getKeyWordFromContent(content1 + content2+content3);
            }
            catch
            {
                return "";
            }
        }
        public static string getP4(string content)
        {

            try
            {
                Regex titRegex = new Regex(@"năm [0-9]{4}(.*?).*\s\S.*\s\S.", RegexOptions.IgnoreCase);//năm [0-9]{4}\s\S\s\S\s\S(.*?).*\s\S.*\s\S.*
                Match titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Value;
                }
                else content="";
                //string[] code = content.Split('\r');
                //string rs = "";
                //int l = code.Length > 10 ? 10 : code.Length;
                //for (int i = 1; i < l; i++)
                //{
                //    if (code[i].StartsWith("Căn cứ")) break;
                //    if (code[i] != "\a" && code[i] != "")
                //    {
                //        rs += code[i] + " ";
                //    }

                //}
                //Bóc tách từ khóa
                return getTopKeyword(content);//getKeyWordFromContent(rs);
                //return rs;
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
        public static string  makeQuery(int? ft,string k,string cols,string f1,string f2,string f3,string f4){
            string fts = "freetexttable";
            if (ft == 1) { fts = "CONTAINSTABLE"; }
            else
            { k = k.Replace("%20", " "); }
            string query="select catid,name,total,no from (select catid,name,count(id) as total from ";
            query+="(select catid,name,id from ";
            query += "(select id as catid,name from cat" + cols + ") as A left join ";
            query += "(select FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.id from documents AS FT_TBL INNER JOIN " + fts + "(documents, auto_des,'" + k + "')  AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] and KEY_TBL.RANK>" + Config.minRank + ") as B on A.catid=B.cat" + cols + "_id ";
            
                string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                for (int f = 0; f < filter.Length; f++)
                {
                    if (filter[f] != null && filter[f] != "")
                    {
                        query += " and (cat" + (f+1) + "_id="+filter[f]+") ";
                    }
                }
                query += " ) as C group by catid,name";
                query += ") as total left join (select id,no from cat" + cols + ") as total2 on total.catid=total2.id order by no desc, name";
            return query;
        }
        public static string makeQueryCat(string col,int? cat1,int? cat2,int? cat4)
        {
            string query = " select catid,name,count(*) as total from ";
                  query +="(";
                  query += "select cat" + col + "_id as catid,cat" + col + " as name from ";
                  query +="(select id,code,name,cat1_id,cat2_id,cat4_id,views from documents) as A left join ";
                  query +="(select name as cat1,id as idcat1 from cat1) as B on A.cat1_id=B.idcat1 left join ";
                  query +="(select name as cat2,id as idcat2,no from cat2) as C on A.cat2_id=C.idcat2 left join ";
                  query +="(select name as cat4,id as idcat4 from cat4) as D on A.cat4_id=D.idcat4 where 1=1 ";
                  if (cat1!=null) query +=" and cat1_id="+cat1;
                  if (cat2!=null) query +=" and cat2_id="+cat2;
                  if (cat4!=null) query +=" and cat4_id="+cat4;
                  query += ") as total group by catid,name ";
            return query;
        }
        public static string hashtags(string f)
        {
            string val = "";
            try
            {
                if (f == null) return "";
                if (f != "")
                {
                    string[] word = f.Split(',');
                    for (int i = 0; i < word.Length; i++)
                        if (word[i].Trim() != "" && word[i].Trim().Split(' ').Length >= 4)
                        {
                            val += "<a class='filteritem' style=\"cursor:pointer;\" onclick=\"searchkw('" + word[i].Trim() + "');\">" + word[i].Trim() + "</a>,";
                        }
                }
            }
            catch (Exception ex) {
                return f;
            }
            return val;
        }
        public static string viewallhashtags(string f,string code)
        {
            string val = "";
            try
            {
                
                if (f == null) return "";
                if (f != "")
                {
                    string[] word = f.Split(',');
                    for (int i = 0; i < word.Length; i++)
                        if (word[i].Trim() != "" && !word[i].Trim().Equals(code))
                        {
                            string temp = word[i].Trim();
                            var p = db.documents.Where(o => o.code.Contains(temp)).FirstOrDefault();
                            int? idvb = null;
                            if (p != null) idvb = p.id;
                            if (idvb != null)
                            {
                                string name = p.name;
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + idvb + "\"><span style='font-size:12px;color:#000000;'>" + name + "</span><br>" + word[i].Trim() + "</a>&nbsp;";
                            }
                            else
                            {
                                val += "<a class='filteritem' style=\"cursor:pointer;\" onclick=\"searchkw('" + word[i].Trim() + "');\">" + word[i].Trim() + "</a>&nbsp;";
                            }
                        }
                    //Tìm xem còn văn bản nào điều chỉnh văn bản này không?
                    var rel = db.documents.Where(o => o.link_to.Contains(code)).FirstOrDefault();
                    if (rel != null) {
                        var li = (from q in db.documents where q.link_to.Contains(code) select q).ToList();
                        for (int ii = 0; ii < li.Count; ii++)
                        {
                            if (!f.Contains(li[ii].code)) {
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + li[ii].id + "\"><span style='font-size:10px;color:#000000;'>" + li[ii].name + "</span><br>" + li[ii].code + "</a>&nbsp;";
                            }
                        }
                    }

                }
            }
            catch (Exception ex) {
                return f;
            }
            return val;
        }
        public static string viewhashtags(string f)
        {
            string val = "";
            if (f == null) return "";
            try
            {
                if (f != "")
                {
                    string[] word = f.Split(',');
                    for (int i = 0; i < word.Length; i++)
                        if (word[i].Trim() != "")
                        {
                            string temp = word[i].Trim();
                            var p = db.documents.Where(o => o.code.Contains(temp)).FirstOrDefault();
                            int? idvb = null;
                            if (p != null) idvb = p.id;
                            if (idvb != null)
                            {
                                string name = p.name;
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + idvb + "\"><span style='font-size:12px;color:#000000;'>" + name + "</span><br>" + word[i].Trim() + "</a>&nbsp;";
                            }
                            else
                            {
                                val += "<a class='filteritem' style=\"cursor:pointer;\" onclick=\"searchkw('" + word[i].Trim() + "');\">" + word[i].Trim() + "</a>&nbsp;";
                            }
                        }
                    //Tìm xem còn văn bản nào điều chỉnh văn bản này hoặc căn cứ vào văn bản này không?


                }
            }
            catch (Exception ex) {
                return f;
            }
            return val;
        }
        public static string tags(string f1,string f2, string f3,string f4)
        {
            string val = "";

            if (f1 != "")
            {
                int tf1 = -1;
                bool res = Int32.TryParse(f1, out tf1);
                if (res == true)
                {
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(1,'');\">" + getCatNameById(1, tf1) + "</a>,";
                }
            }
            if (f2 != "")
            {
                int tf2 = -1;
                bool res = Int32.TryParse(f2, out tf2);
                if (res == true)
                {
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(2,'');\">" + getCatNameById(2, tf2) + "</a>,";
                }
            }
            if (f3 != "")
            {
                int tf3 = -1;
                bool res = Int32.TryParse(f3, out tf3);
                if (res == true)
                {
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(3,'');\">" + getCatNameById(3, tf3) + "</a>,";
                }
            }
            if (f4 != "")
            {
                int tf4 = -1;
                bool res = Int32.TryParse(f4, out tf4);
                if (res == true)
                {
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(4,'');\">" + getCatNameById(4, tf4) + "</a>,";
                }
            }
           
            return val;
        }
        public static string tagscat(int? f1, int? f2, int? f4)
        {
            string val = "";

            if (f1 != null)
            {
                int? tf1 = f1;
               
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(1,'');\">" + getCatNameById(1, tf1) + "</a>,";
                
            }
            if (f2 != null)
            {
                int? tf2 = f2;
                
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(2,'');\">" + getCatNameById(2, tf2) + "</a>,";
                
            }

            if (f4 != null)
            {
                int? tf4 = f4;
                
                    val += "<a style=\"cursor:pointer;\" onclick=\"setCat(4,'');\">" + getCatNameById(4, tf4) + "</a>,";
              
            }

            return val;
        }
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = HttpUtility.UrlEncode(value);
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[v].Value.ToString());
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary> 
        ///  Read Plain Text in all XmlElements of word document 
        /// </summary> 
        /// <param name="element">XmlElement in document</param> 
        /// <returns>Plain Text in XmlElement</returns> 
        public static string GetPlainText(OpenXmlElement element)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;


                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        break;


                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;


                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;


                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }


            return PlainTextInWord.ToString();
        }
        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace("%", "");//Replace(".", "").Replace("_", "").
            return input;
        }
        public static DateTime convertToDate(string d)
        {
            DateTime d1;
            try
            {
               
                DateTime.TryParse(d,out d1);
                return d1;
            }
            catch (Exception ex) {
                return DateTime.MinValue;
            }
        }
        public class searchitem2
        {
            //FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id, FT_TBL.views, KEY_TBL.RANK
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
            public int cat1_id { get; set; }
            public int cat2_id { get; set; }
            public int cat3_id { get; set; }
            public int cat4_id { get; set; }
            public int? views { get; set; }
            public int RANK { get; set; }
            public byte? status { get; set; }

        }
        public static string showTree(int id, string k, string f1, string f2, string f3, string f4, int? st, byte? status, byte? tps, int? ft, string order, string to)
        {
            string rs = "";
            string fts = "freetexttable";
            try
            {
                if (st == 2 || k.Contains("/")) k = "";
                if (tps == 2 && (st != 1 & st != 2))
                {
                    string tempf1 = Config.getMaxCat1(k);
                    if (tempf1 != "" && tps == 2) f1 = tempf1;
                }
                if (tps == 1)
                {
                    Config.changeHeso(tps, k);
                }
                if (k != null && k != "")
                {
                    if (ft == 1) { fts = "CONTAINSTABLE"; k = k.Replace(" ", "%"); }
                    else
                    { k = k.Replace("%20", " ").Replace("%", " "); }

                    f1 = f1 != null ? f1 : ""; f2 = f2 != null ? f2 : ""; f3 = f3 != null ? f3 : "";
                    f4 = f4 != null ? f4 : "";
                    if (st == null) st = 0;
                    if (status == null) status = 2;

                    string query = "select top 30 * from (SELECT  ";
                    query += "FT_TBL.id,FT_TBL.name,FT_TBL.code,FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.views, RANK=CASE FT_TBL.cat2_id ";
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
                        query = "select top 30 id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,RANK=CASE cat2_id ";
                        query += "WHEN 7 THEN " + Config.heso1 + " ";
                        query += "WHEN 18 THEN " + Config.heso2 + " ";
                        query += "WHEN 15 THEN " + Config.heso3 + " ";
                        query += "WHEN 5 THEN " + Config.heso4 + " ";
                        query += "WHEN 23 THEN " + Config.heso5 + " ";
                        query += "WHEN 6 THEN " + Config.heso6 + " ";
                        query += "ELSE 0 ";
                        query += "END,status from documents where (code like N'" + k + "%' or code=N'" + k + "' or code=N'%" + k + "' or code like N'%" + k + "%') ";
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
                            query = "select top 30 id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,RANK=CASE cat2_id ";
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
                            query = "select top 30 id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,RANK=CASE cat2_id ";
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
                    rs = "";// "<ul id=\"treemenu2\" class=\"treeview\">";
                    int? preCatId = -1;
                    var p = db.Database.SqlQuery<searchitem2>(query).ToList();
                    for (int j = 0; j < p.Count; j++)
                    {
                        string spacer = getSpacer(p[j].cat2_id);
                        if (p[j].cat2_id != preCatId)
                        {
                            //spacer += "<img src=\"/Images/leaf.gif\">" + getCatNameById(2, p[j].cat2_id);
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  >" + spacer + "<img src=\"/Images/leaf.gif\"><b>" + getCatNameById(2, p[j].cat2_id) + "</b></div>";
                            preCatId = p[j].cat2_id;
                        }
                        if (p[j].id!=id){
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  ><table><tr><td nowrap>" + spacer + "<img src=\"/Images/elbow-end.gif\"><span ><a href=\"/Document/Details?id=" + p[j].id + "&keyword=" + k + "&f1=" + f1 + "&f2=" + f2 + "&f3=" + f3 + "&f4=" + f4 + "&st=" + st + "&status=" + status + "&order=" + order + "&to=" + to + "\" target=\"_blank\">" + p[j].name + "-" + p[j].code + "</a><span></td></tr></table></div>";
                        }
                        else {
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  ><table><tr><td nowrap>" + spacer + "<img src=\"/Images/elbow-end.gif\"><span ><a href=\"/Document/Details?id=" + p[j].id + "&keyword=" + k + "&f1=" + f1 + "&f2=" + f2 + "&f3=" + f3 + "&f4=" + f4 + "&st=" + st + "&status=" + status + "&order=" + order + "&to=" + to + "\" target=\"_blank\"><b>" + p[j].name + "-" + p[j].code + "</b></a><span></td></tr></table></div>";
                        }
                    }
                    //rs += "</ul>";
                }
                else
                {
                    //Neu ma khong co tu khoa thi lay theo linh vuc van ban do, sap xep tu Luat--> Nghi dinh-->giam dan
                    int? cat1_id = db.documents.Find(id).cat1_id;
                    string query2 = "select top 30 id,name,code,cat1_id,cat2_id,cat3_id,cat4_id,views,RANK=CASE cat2_id ";
                            query2 += "WHEN 7 THEN " + Config.heso1 + " ";
                            query2 += "WHEN 18 THEN " + Config.heso2 + " ";
                            query2 += "WHEN 15 THEN " + Config.heso3 + " ";
                            query2 += "WHEN 5 THEN " + Config.heso4 + " ";
                            query2 += "WHEN 23 THEN " + Config.heso5 + " ";
                            query2 += "WHEN 6 THEN " + Config.heso6 + " ";
                            query2 += "ELSE 0 ";
                            query2 += "END,status from documents where 1=1 ";
                    if (cat1_id != null) query2 += " and cat1_id=" + cat1_id;
                    if (order == null || order == "") order = "Rank";
                    query2 += " order by " + order;
                    if (to == null || to == "") to = "desc";
                    query2 += " " + to;
                    rs = "";// "<ul id=\"treemenu2\" class=\"treeview\">";
                    int? preCatId = -1;
                    var p2 = db.Database.SqlQuery<searchitem2>(query2).ToList();
                    for (int j = 0; j < p2.Count; j++)
                    {
                        string spacer = getSpacer(p2[j].cat2_id);
                        if (p2[j].cat2_id != preCatId)
                        {
                            //spacer += "<img src=\"/Images/leaf.gif\">" + getCatNameById(2, p[j].cat2_id);
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  >" + spacer + "<img src=\"/Images/leaf.gif\"><b>" + getCatNameById(2, p2[j].cat2_id) + "</b></div>";
                            preCatId = p2[j].cat2_id;
                        }
                        if (p2[j].id != id)
                        {
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  ><table><tr><td nowrap style=\"width:95%;\">" + spacer + "<img src=\"/Images/elbow-end.gif\"><span ><a href=\"/Document/Details?id=" + p2[j].id + "&keyword=" + k + "&f1=" + f1 + "&f2=" + f2 + "&f3=" + f3 + "&f4=" + f4 + "&st=" + st + "&status=" + status + "&tps="+tps+"&ft="+ft+"&order=" + order + "&to=" + to + "\" target=\"_blank\">" + p2[j].name + "-" + p2[j].code + "</a><span></td></tr></table></div>";
                        }
                        else
                        {
                            rs += "<div style=\"width:95%;cursor:pointer;text-align:left;\"  ><table><tr><td nowrap style=\"width:95%;\">" + spacer + "<img src=\"/Images/elbow-end.gif\"><span ><a href=\"/Document/Details?id=" + p2[j].id + "&keyword=" + k + "&f1=" + f1 + "&f2=" + f2 + "&f3=" + f3 + "&f4=" + f4 + "&st=" + st + "&status=" + status + "&tps=" + tps + "&ft=" + ft + "&order=" + order + "&to=" + to + "\" target=\"_blank\"><b>" + p2[j].name + "-" + p2[j].code + "</b></a><span></td></tr></table></div>";
                        }
                    }
                    //rs += "</ul>";
                }
            }
            catch (Exception ex)
            {

            }
            return rs;
        }
        public static string getSpacer(int depth){
            if (depth==7) depth=1;
            else
                if (depth==18) depth=2;
            else
                if (depth==15) depth=3;
            else
                if (depth==5) depth=4;
            else
                if (depth==23) depth=5;
            else
                if (depth==6) depth=6;
            else
                depth=7;
           
            string spacer="<img src=\"/Images/spacer.gif\" width=16>";
            for (var j = 0; j <depth; j++) {
                            spacer+="<img src=\"/Images/spacer.gif\" width=16>";
            }
            return spacer;
        }
        public class getMaxCat
        {
            public int name { get; set; }
            public int count{get;set;}
        }
        public static string getMaxCat1(string k)
        {
            try
            {
                string query = "select cat1_id as name,count(*) as count from documents where ";
                query += " (code like N'" + k + "%' or code=N'" + k + "' or code like N'%" + k + "' or code like N'%" + k + "%' or auto_des like N'" + k + "%' or auto_des like N'%" + k + "' or auto_des like N'%" + k + "%' or name like N'" + k + "%' or name=N'" + k + "' or name like N'%" + k + "' or name like N'%" + k + "%' or full_content like N'" + k + "%' or  full_content like N'%" + k + "%' or  full_content like N'%" + k.Replace(" ","%") + "%') ";
                query += " group by cat1_id order by count desc";
                var p = db.Database.SqlQuery<getMaxCat>(query).FirstOrDefault();
                if (p.count > 0) return p.name.ToString();
            }
            catch (Exception ex) { 

            }
            return "";
        }
        public static string formatDDMMYYYY(DateTime? d)
        {
            if (d == null) return "";
            return String.Format("{0:dd-MM-yyyy}", d);
        }
        public static string statusStr(byte? status)
        {
            if (status == null || status == 1) return "hết/chưa hiệu lực"; else return "có hiệu lực";
        }
    }
    
}