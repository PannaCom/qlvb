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
        public static string getP5(string content)
        {
            try
            {
                Regex titRegex = new Regex(@"\s[0-9]*[^a-zA-Z0-9][0-9]{4}[^a-zA-Z0-9][a-zA-Z]*[^a-zA-Z0-9]\S\S*", RegexOptions.IgnoreCase);//Số: .*/.*/.*\S-*([A-Z])\r
                
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
                    if (!content.Contains(temp)) content += temp + " , ";
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
                Regex titRegex = new Regex(@"năm [0-9]{4}\r\n(.*?)\s\S.*\s\S.*", RegexOptions.IgnoreCase);//năm [0-9]{4}\s\S\s\S\s\S(.*?).*\s\S.*\s\S.*
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
                //Bóc tách từ khóa
                return rs;//getKeyWordFromContent(rs);
                //return rs;
            }
            catch
            {
                return "";
            }
        }
        
        public static string getKeyWordFromContent(string content){
            content = content.Replace("\n", " ").ToLowerInvariant().Replace(".", " ").Replace(",", " ").Trim();
            int lengthWord = 4;
            string result = "";
            string[] arrContent = content.Split(' ');
            while (lengthWord >= 2) {
                for (int l = 0; l <= arrContent.Length - lengthWord; l++) {
                    string tempword = "";
                    for (int l1 = l; l1 < l + lengthWord; l1++) {
                        tempword += arrContent[l1] + " ";
                    }
                    tempword = tempword.ToLowerInvariant().Trim();
                    if (allword.ContainsKey(tempword) && !result.Contains(tempword) && tempword.Split(' ').Length>=2)
                    {
                        result += tempword + " , ";
                    }
                }
                    lengthWord--;
            }
            return result;
        }
        public static string getTopKeyword(string content)
        {
            content = content.Replace("\n", " ").ToLowerInvariant().Replace(".", " ").Replace(",", " ").Trim();
            int lengthWord = 4;
            string result = "";
            string[] arrContent = content.Split(' ');
            int tempcount=0;
            Dictionary<String,int> top=new Dictionary<String,int>();
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
                        if (top.ContainsKey(tempword)){
                            tempcount=top[tempword]+1;
                            top.Remove(tempword);
                            top.Add(tempword,tempcount);
                        }else{
                            top.Add(tempword,1);
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
    }
}