using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;

namespace CheFuZi
{

    public class AuthCodeClass
    {
        HeaderClass myHeaderClass = new HeaderClass();//
        //
        #region 生成授权码
        /// <summary>
        /// 生成授权码
        /// </summary>
        /// <returns></returns>
        public string CreateAuthCode(string mobilePhone, int userRole = 0, bool remember = true)
        {
            string returnAuthCode = "";
            if (!String.IsNullOrWhiteSpace(mobilePhone))
            {
                try
                {
                    FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1, mobilePhone, DateTime.Now, DateTime.Now.AddDays(StaticVarClass.LoginExpiredDate), remember, userRole.ToString(), "/");
                    returnAuthCode = FormsAuthentication.Encrypt(Ticket);
                    HttpCookie UserCookie = new HttpCookie(FormsAuthentication.FormsCookieName, returnAuthCode);
                    if (remember)
                    {
                        UserCookie.Expires = DateTime.Now.AddDays(StaticVarClass.LoginExpiredDate);
                    }
                    System.Web.HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    System.Web.HttpContext.Current.Response.Cookies.Add(UserCookie);
                    //
                }
                catch
                { }
            }
            return returnAuthCode;
        }
        #endregion

        #region 检查授权码，过期则更新
        public bool checkAuthCode(out AuthCodeInstance myAuthCodeData)
        {
            bool authCodeValid = false;//无效
            int userRole = 0;//角色
            string mobilePhone = "";//手机号
            myAuthCodeData = new AuthCodeInstance();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated) //验证过的用户才进行role的处理
            {
                try
                {
                    System.Web.HttpContext myContent = System.Web.HttpContext.Current;
                    mobilePhone = myContent.User.Identity.Name;
                    myAuthCodeData.mobilePhone = mobilePhone;
                    //
                    FormsIdentity userId = (FormsIdentity)myContent.User.Identity;
                    FormsAuthenticationTicket Ticket = userId.Ticket;  //取得身份验证票 
                    int.TryParse(Ticket.UserData, out userRole);
                    myAuthCodeData.userRoleId = userRole;
                    authCodeValid = true;
                }
                catch
                {
                    authCodeValid = false;//无效
                }
            }
            else
            {
                string myHeadAuthCode = "";
                myHeadAuthCode = myHeaderClass.getHeadAuthCode();//从Header中取
                if (!String.IsNullOrWhiteSpace(myHeadAuthCode))
                {
                    try
                    {
                        FormsAuthenticationTicket myTicket = FormsAuthentication.Decrypt(myHeadAuthCode);
                        int.TryParse(myTicket.UserData, out userRole);
                        myAuthCodeData.mobilePhone = myTicket.Name;
                        myAuthCodeData.userRoleId = userRole;
                        authCodeValid = true;
                    }
                    catch
                    {
                        authCodeValid = false;//无效
                    }
                }
            }
            //
            return authCodeValid;
        }
        #endregion
    }
    //
    #region 授权码实体
    public class AuthCodeInstance
    {
        int _userRoleId = 0;//角色
        string _mobilePhone = "";//手机号
        public int userRoleId
        {
            get
            {
                return _userRoleId;
            }
            set
            {
                _userRoleId = value;
            }
        }
        public string mobilePhone
        {
            get
            {
                return _mobilePhone;
            }
            set
            {
                _mobilePhone = value;
            }
        }
    }
    #endregion

}
