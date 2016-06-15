using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebSoketServicesManager.Tool
{
    public class Path
    {
        public static NameValueCollection QueryString(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            path = System.Web.HttpUtility.UrlDecode(path);
            var m = Regex.Matches(path, @"(?<=\?|&)[\w\={}\\\\,-:'\s'""]*(?=[^#\s]|)", RegexOptions.None);        
            if (m.Count <= 0)
            {
                return null;
            }
            NameValueCollection nvcs = new NameValueCollection();
            string[] itemvalues = null;
            for (int i = 0; i < m.Count; i++)
            {
                itemvalues = m[i].Value.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (itemvalues == null || itemvalues.Length == 0) continue;
                nvcs.Add(itemvalues[0], itemvalues.Length <= 1 ? string.Empty : itemvalues[1]);
            }
            return nvcs;
        }
    }
}