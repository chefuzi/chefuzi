using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Web.Security;
using System.Data.SqlClient;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;

namespace CheFuZi.DataOption
{
    public class UserOptionClass
    {
        #region 用户登录
        public StatusData Login(string username, string pwd, bool remember = true)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();//授权码类
            //
            User_UserName myUserInfo = new User_UserName();
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                //
                myUserInfo = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == username);
                if (myUserInfo != null)
                {
                    if (!String.IsNullOrWhiteSpace(pwd))
                    {
                        pwd = Md5Class.CreateMd5(pwd);//
                    }
                    if (myUserInfo.PassWord != pwd)
                    {
                        myStatusData.operateStatus = 2;//密码错误
                    }
                    else if (myUserInfo.Status !=200)
                    {
                        myStatusData.operateStatus = myUserInfo.Status;//201锁定，联系客服
                    }
                    else 
                    {
                        string myAuthCodeStr = myAuthCodeClass.CreateAuthCode(myUserInfo.MobilePhone, myUserInfo.RoleId, remember);
                        myStatusData.operateStatus = 200;//登录成功
                        myStatusData.userAuthCode = myAuthCodeStr;
                        myUserInfo.RecentlyLoginDate = DateTime.Now;
                        myOperating.SaveChanges();
                    }
                }
            }
            return myStatusData;
        }
        #endregion
        //
        #region 用户退出
        public bool LoginOut()
        {
            bool isOk = false;
            //
            try
            {
                HttpCookie UserCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                UserCookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(UserCookie);
                FormsAuthentication.SignOut();
                isOk = true;
            }
            catch
            { }
            return isOk;
        }
        #endregion
        //
        #region 修改密码
        public StatusData EditPwd(string username, string newpwd)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            User_UserName myUserInfo = new User_UserName();
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                myUserInfo = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == username);
                if (myUserInfo == null)
                {
                    myStatusData.operateStatus = 1;//用户名错误
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(newpwd))
                    {
                        newpwd = Md5Class.CreateMd5(newpwd);//
                        myUserInfo.PassWord = newpwd;
                        myOperating.SaveChanges();

                        myStatusData.operateStatus = 200;
                    }
                    else
                    {
                        myStatusData.operateMsg = "新密码不能为空";
                        myStatusData.operateStatus = 400;//参数错误
                    }
                }
            }
            return myStatusData;
        }
        #endregion
        //
        #region 获得阅读名次
        public static long ReadDayRank(string username)
        {
            long myRank = 0;
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                //
                try
                {
                    RankNum RankNumList = new RankNum();
                    //
                    StringBuilder mySqlStr = new StringBuilder();
                    mySqlStr.Append("SELECT [Extent1].[MobilePhone],[Extent1].[GradeNum] From ( SELECT [Extent1].[MobilePhone] AS [MobilePhone], row_number() OVER (ORDER BY [Extent1].[ReadDayCount] DESC) AS [GradeNum] FROM [RuiKa_Public].[dbo].[User_Book_Click_ReadDay] AS [Extent1])  AS [Extent1] Where [Extent1].[MobilePhone]=@MobilePhone");
                    RankNumList = myOperating.ObjectContext.ExecuteStoreQuery<RankNum>(mySqlStr.ToString(), new SqlParameter("@MobilePhone", username)).FirstOrDefault();

                    if (RankNumList != null)
                    {
                        myRank = RankNumList.GradeNum;
                    }
                }
                catch
                { }
            }
            return myRank;
        }
        #endregion
    }
    #region 名次数量结构
    public class RankNum
    {
        public string MobilePhone { get; set; }
        public long GradeNum { get; set; }
    }
    #endregion
}