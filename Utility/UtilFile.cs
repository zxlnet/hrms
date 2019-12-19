using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace GotWell.Utility
{
    public class UtilFile
    {
        /// <summary>
        /// Gets the json from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <Remarks>
        public static List<Dictionary<string, string>> GetJsonFromFile(string path, string fileName)
        {
            StreamReader reader = null;
            string ret = "";
            try
            {
                string filePath = path;

                if (filePath == null || filePath.Equals(string.Empty))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
                }

                reader = new StreamReader(filePath + @"\" + fileName, System.Text.Encoding.Default);
                
                string line="";
                int cnt = 0;
                while((line=reader.ReadLine())!=null)
                {
                    cnt++;
                    if (cnt == 2)
                        break;
                }

                if (line == "DATA")
                {
                    line = reader.ReadToEnd();
                    int start = line.IndexOf("[{");
                    if (start == -1)
                    {
                        start = 0;
                    }
                    ret = line.Substring(start);
                }
                else
                {
                    cnt = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        cnt++;
                        if (cnt > 2)
                        {
                            string t = line.Substring(0, 23);
                            string m = line.Substring(24, line.Length - 24); ;
                            ret += ((ret == "" ? "" : ",") + "{Time:'" + t + "',Text:'" + m + "'}");
                        }
                    }

                    ret = "[" + ret + "]";
                }

                return JavaScriptConvert.DeserializeObject<List<Dictionary<string, string>>>(ret);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <Remarks>
        public static List<Dictionary<string, string>> GetFiles(string path)
        {
            StreamReader reader = null;
            try
            {
                string filePath = path;

                if (filePath == null || filePath.Equals(string.Empty))
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
                }
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                string[] fileNames = Directory.GetFiles(filePath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    if (fileNames[i].EndsWith(".trc"))
                    {
                        reader = new StreamReader(fileNames[i]);
                        string line = reader.ReadLine();
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        int lastIndex = fileNames[i].LastIndexOf('\\');
                        string fileName = fileNames[i];
                        if (lastIndex != -1)
                        {
                            fileName = fileName.Substring(lastIndex + 1, fileName.Length - lastIndex - 1);
                        }
                        dic.Add("fileName", fileName);
                        dic.Add("description", line);
                        list.Add(dic);
                    }
                }
                return list;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
