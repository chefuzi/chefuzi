using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace CheFuZi.Function
{
    public class FunctionClass
    {
        #region 判断手机号规则，11位数字
        /// <summary>
        /// 判断手机号
        /// </summary>
        /// <returns></returns>
        public static bool isMobilePhone(string myMoblie)
        {
            bool isTrue = false;
            myMoblie = myMoblie.Trim();
            if (String.IsNullOrWhiteSpace(myMoblie))
            {
                isTrue = false;
            }
            else if (myMoblie.Length != 11)
            {
                isTrue = false;
            }
            else
            {
                long myMobileLong = 0;
                bool isLong = long.TryParse(myMoblie, out myMobileLong);
                if ((myMobileLong > 13000000000) && (myMobileLong < 19900000000))
                {
                    isTrue = true;
                }
            }
            return isTrue;
        }
        #endregion
        //
        #region 生成五位数随机字符串
        /// <summary>
        /// 生成五位数随机字符串
        /// </summary>
        /// <returns></returns>
        public static string RandomCode()
        {
            System.Random aa = new Random();
            int j = aa.Next(10000, 99999);
            return j.ToString();
        }
        #endregion
        //
        #region 删除本地文件
        /// <summary>
        /// 删除本地文件
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public static bool DeleteFile(string filePath)
        {
            bool isOk = false;
            string localPath = "";
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                string[] fileList = filePath.Split(';');
                for (int i = 0; i < fileList.Length; i++)
                {
                    try
                    {
                        localPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/"), fileList[i].Replace(StaticVarClass.myDomain, ""));
                        if (System.IO.File.Exists(localPath))
                        {
                            System.IO.File.Delete(localPath);
                            isOk = true;
                        }
                    }
                    catch
                    { }
                }
            }
            return isOk;
        }
        #endregion
        //
        #region 处理URL参数，去除指定参数，返回正常参数
        /// <summary>
        /// 参数必须小写
        /// </summary>
        /// <param name="Removekeys"></param>
        /// <returns></returns>
        public static string UrlString(string[] Removekeys)
        {
            StringBuilder myBuilder = new StringBuilder();
            Dictionary<String, String> pList = new Dictionary<String, String>();
            System.Web.HttpRequest myRequest = System.Web.HttpContext.Current.Request;
            List<string> temRemove = new System.Collections.Generic.List<string>();
            if (Removekeys != null)
            {
                foreach (string item in Removekeys)
                {
                    temRemove.Add(item.ToLower());
                }
            }
            //
            foreach (string key in myRequest.QueryString.AllKeys)
            {
                if (temRemove != null)
                {
                    if (!temRemove.Contains(key.ToLower()))
                    {
                        pList.Add(key, myRequest.QueryString[key]);
                    }
                }
                else
                {
                    pList.Add(key, myRequest.QueryString[key]);
                }
            }
            if ((pList != null) && (pList.Count() > 0))
            {
                foreach (var item in pList)
                {
                    if (myBuilder.Length > 0)
                    {
                        myBuilder.Append("&");
                    }
                    else
                    {
                        myBuilder.Append("?");
                    }
                    myBuilder.Append(item.Key);
                    myBuilder.Append("=");
                    myBuilder.Append(item.Value);
                }
            }
            else
            {
                myBuilder.Append("?");
                myBuilder.Append("1");
                myBuilder.Append("=");
                myBuilder.Append("1");
            }

            return myBuilder.ToString();
        }
        #endregion
        //
        #region 判断IOS设备
        ///<summary>
        /// 根据 Agent 判断是否是IOS
        ///</summary>
        ///<returns></returns>
        public static bool CheckPhoneSys()
        {
            bool flag = false;

            string agent = System.Web.HttpContext.Current.Request.UserAgent;
            string[] IOSkeywords = { "iPhone", "iPod", "iPad", "Macintosh" };
            //string[] WINkeywords = { "Android", "Windows Phone", "MQQBrowser", "Windows NT" };

            foreach (string item in IOSkeywords)
            {
                if (agent.Contains(item))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        #endregion
        //
        #region 添加域名前缀返回完整Url
        public static string GetFileUrl(string originalUrl)
        {
            StringBuilder returnUrl = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(originalUrl))
            {
                string[] temUrl = originalUrl.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                int fileCount = 0;
                foreach (string item in temUrl)
                {
                    if (item.IndexOf("http://") < 0)
                    {
                        if (fileCount == 0)
                        {
                            returnUrl.Append(StaticVarClass.myDomain);
                            returnUrl.Append(item);
                        }
                        else
                        {
                            returnUrl.Append(";");
                            returnUrl.Append(StaticVarClass.myDomain);
                            returnUrl.Append(item);
                        }
                        fileCount++;
                    }
                }
            }
            return returnUrl.ToString();
        }
        #endregion
        //
        #region 删除文件
        public static bool delFile(string filepath, string[] filepathlist = null)
        {
            bool isOk = false;
            string[] temUrl = null;
            //
            if (!String.IsNullOrWhiteSpace(filepath))
            {
                temUrl = filepath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if ((filepathlist != null) && (filepathlist.Length > 0))
            {
                temUrl = filepathlist;

            }
            if ((temUrl != null) && (temUrl.Length > 0))
            {
                try
                {
                    foreach (string item in temUrl)
                    {
                        string realPath = item.Replace("\\", "/");
                        if (item.IndexOf("http://") == 0)
                        {
                            int domainPosition = 0;
                            realPath = item.Replace("http://", "");
                            domainPosition = realPath.IndexOf('/');
                            realPath = realPath.Substring(domainPosition);
                        }
                        //
                        if (realPath.IndexOf('/') != 0)
                        {
                            realPath = "/" + realPath;
                        }
                        //
                        string FullPath = System.Web.HttpContext.Current.Server.MapPath(realPath);
                        if (File.Exists(FullPath))
                        {
                            File.Delete(FullPath);
                            isOk = true;
                        }
                    }
                }
                catch
                {
                    isOk = false;
                }
            }
            return isOk;
        }
        #endregion
        //
        #region 取得HTML中所有图片的 URL
        /// <summary> 
        /// 取得HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }
        #endregion
        //
        #region 判断是否微信浏览器
        public static bool CheckWeiXinBrowser()
        {
            bool isWeiXin = false;
            try
            {
                System.Collections.Specialized.NameValueCollection myHeader = HttpContext.Current.Request.Headers;
                string myHeaderAgent = myHeader.ToString();
                if (myHeaderAgent.ToLower().IndexOf("micromessenger") > -1)
                {
                    isWeiXin = true;
                }
            }
            catch
            { }
            return isWeiXin;
        } 
        #endregion

    }
}