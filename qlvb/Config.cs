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
namespace qlvb
{
    public class Config
    {
        public static string sp = "____________";
        private static qlvbEntities db=new qlvbEntities();
        public static string domain = "http://vanbanquocgia.com";//"http://localhost:59574/";
        public static Hashtable allword=null;
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
        public static string getPublish(string content)
        {
            try
            {
                content=content.Trim();
                int to = content.IndexOf("Số:");
                if (to < 0) to = int.MaxValue;
                int to2 = content.IndexOf("CỘNG HÒA");
                int to3 = content.IndexOf("CỘNG HOÀ");
                if (to2>0 && to2 < to) to = to2;
                if (to3>0 && to3 < to) to = to3;
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
                Regex titRegex = new Regex(@"\s[0-9]*[^a-zA-Z0-9][0-9]{4}[^a-zA-Z0-9][a-zA-Z]*\S\S*", RegexOptions.IgnoreCase);//Số: .*/.*/.*\S-*([A-Z])\r    //Số: (.*?)/(.*?)/.*[A-Z]\s
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
                int lengthWord = 4;
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
                        if (allword.ContainsKey(tempword) && !result.Contains(tempword) && tempword.Split(' ').Length >= 2)
                        {
                            result += tempword + " , ";
                        }
                    }
                    lengthWord--;
                }
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getTopKeyword(string content)
        {
            try
            {
                content = content.Replace("\n", " ").ToLowerInvariant().Replace(".", " ").Replace(",", " ").Trim();
                int lengthWord = 4;
                string result = "";
                string[] arrContent = content.Split(' ');
                int tempcount = 0;
                Dictionary<String, int> top = new Dictionary<String, int>();
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
                        if (allword.ContainsKey(tempword) && tempword.Split(' ').Length >= 2)
                        {
                            //result += tempword + " , ";
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
                        }
                    }
                    lengthWord--;
                }
                var sortedDict = from entry in top orderby entry.Value descending select entry;
                tempcount = 0;
                foreach (var entry in sortedDict)
                {
                    tempcount++;
                    result += entry.Key + " , ";
                    if (tempcount >= 4) break;
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
                            tempcount = top[tempword] + 1;
                            top.Remove(tempword);
                            top.Add(tempword, tempcount);
                        }
                        else
                        {
                            top.Add(tempword, 1);
                        }
                    }
                }

                var sortedDict = from entry in top orderby entry.Value descending select entry;
                tempcount = 0;
                foreach (var entry in sortedDict)
                {
                    tempcount++;
                    //result += entry.Key + " , ";
                    if (!entry.Key.Equals("xã hội chủ nghĩa") && !entry.Key.Equals("cộng hòa") && !entry.Key.Equals("việt nam")) result += "<a class='filteritem' style=\"cursor:pointer;\" onclick=\"searchkw('" + entry.Key + "');\">" + entry.Key + "</a>&nbsp;";
                    //if (tempcount >= 10) break;
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
        public static string  makeQuery(string k,string cols,string f1,string f2,string f3,string f4){
            string query="select catid,name,count(id) as total from ";
            query+="(select catid,name,id from ";
            query += "(select id as catid,name from cat" + cols + ") as A left join ";
            query += "(select FT_TBL.cat1_id,FT_TBL.cat2_id,FT_TBL.cat3_id,FT_TBL.cat4_id,FT_TBL.id from documents AS FT_TBL INNER JOIN FREETEXTTABLE(documents, auto_des,'" + k + "')  AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY]) as B on A.catid=B.cat" + cols + "_id ";
            
                string[] filter = new string[4]; filter[0] = f1; filter[1] = f2; filter[2] = f3; filter[3] = f4;
                for (int f = 0; f < filter.Length; f++)
                {
                    if (filter[f] != null && filter[f] != "")
                    {
                        query += " and (cat" + (f+1) + "_id="+filter[f]+") ";
                    }
                }
                query += " ) as C group by catid,name order by name";
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
                        if (word[i].Trim() != "")
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
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + idvb + "\"><span style='font-size:12px;color:#ffffff;'>" + name + "</span><br>" + word[i].Trim() + "</a>&nbsp;";
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
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + li[ii].id + "\"><span style='font-size:10px;color:#ffffff;'>" + li[ii].name + "</span><br>" + li[ii].code + "</a>&nbsp;";
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
                                val += "<a class='filteritem' style=\"cursor:pointer;\" href=\"/Document/Details/" + idvb + "\"><span style='font-size:12px;color:#ffffff;'>" + name + "</span><br>" + word[i].Trim() + "</a>&nbsp;";
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
    }
}