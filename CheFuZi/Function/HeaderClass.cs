using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;

namespace CheFuZi.Function
{
    /// <summary>
    /// 获得报头键值对
    /// </summary>
    public class HeaderClass
    {
        /// <summary>
        /// 获得客户端授权码
        /// </summary>
        /// <returns></returns>
        #region 获取客户端授权码
        public string getHeadAuthCode()
        {
            string myAuthCode = "";
            try
            {
                //string[] myHeaders = HttpContext.Current.Request.Cookies.AllKeys;
                for (int i = 0; i < HttpContext.Current.Request.Headers.Count; i++)
                {
                    if (HttpContext.Current.Request.Headers.Keys[i].ToLower() == "userauthcode")
                    {
                        myAuthCode = HttpContext.Current.Request.Headers[i].ToString();
                        break;
                    }
                }

                //if (!String.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["userauthcode"]))
                //{
                //    myAuthCode = HttpContext.Current.Request.Headers["userauthcode"];
                //}
                //else
                //{
                //    myAuthCode = "";
                //}
            }
            catch
            {
                myAuthCode = "";
            }
            return myAuthCode;
        }
        #endregion
    }
}
