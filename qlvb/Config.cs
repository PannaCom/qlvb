﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace qlvb
{
    public class Config
    {
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
                    if (code[i] != "\a" && code[i] != "") {
                        rs += code[i] + " ";
                    }
                    if (code[i].StartsWith("Căn cứ")) break;
                }
                return rs;
            }
            catch
            {
                return "";
            }
        }
    }
}