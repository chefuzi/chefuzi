using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Diagnostics;
using System.Web.Security;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
using CheFuZi.DataOption;

namespace CheFuZi.Controllers.Service
{
    public class UserController : Controller
    {
        UserOptionClass myUserOptionClass = new UserOptionClass();
        //
        #region 用户登录
        [HttpPost]
        public JsonResult Login(string username, string pwd)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            myStatusData = myUserOptionClass.Login(username, pwd);
            return Json(myStatusData);
        }
        #endregion
        //
        #region 用户退出
        [HttpPost]
        public JsonResult LoginOut()
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            bool isOk = myUserOptionClass.LoginOut();
            if (isOk)
            {
                myStatusData.operateStatus = 200;
            }
            else
            {
                myStatusData.operateStatus = 0;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 发送验证码
        [HttpPost]
        public JsonResult SendCheckCode(string username, string operationtype)
        {//operationtype0,注册；1修改密码
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 手机号错误
            if (!FunctionClass.isMobilePhone(username))
            {//手机号规则判断
                myStatusData.operateStatus = 400;
                myStatusData.operateMsg = "手机号错误";
                return Json(myStatusData);
            }
            #endregion
            //
            DateTime myNowDate = DateTime.Now;
            DateTime myExpiredDate = DateTime.Now;
            //验证码有效期---调试屏蔽
            myExpiredDate = DateTime.Now.AddSeconds(int.Parse(StaticVarClass.CheckCodeExpiredDate));
            //
            string myMsg = FunctionClass.RandomCode();
            //
            try
            {
                bool allowSend = false;
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    if (String.IsNullOrWhiteSpace(operationtype))
                    {
                        operationtype = "";
                    }
                    #region 发送验证码的类型判断
                    int isExist = 0;//
                    switch (operationtype)
                    {
                        case "0"://注册
                            isExist = myOperating.User_UserName.Count(p => p.MobilePhone == username);
                            if (isExist > 0)
                            {
                                allowSend = false;
                                myStatusData.operateStatus = 7;
                            }
                            else
                            {
                                allowSend = true;
                            }
                            //allowSend = true;
                            break;
                        case "1"://修改密码
                            isExist = myOperating.User_UserName.Count(p => p.MobilePhone == username);
                            if (isExist > 0)
                            {
                                allowSend = true;
                            }
                            else
                            {
                                myStatusData.operateStatus = 400;
                                myStatusData.operateMsg = "该号码不存在";
                                allowSend = false;
                            }
                            break;
                        default:
                            myStatusData.operateStatus = 400;
                            myStatusData.operateMsg = "参数错误";
                            allowSend = false;
                            break;
                    }
                    #endregion
                    //
                    if (allowSend)
                    {
                        #region 发送验证码
                        bool IsSendOk = true;
                        //
                        int deleteNum = myOperating.User_CheckCode.Where(p => p.ExpiredDate < myNowDate).Delete();
                        if (deleteNum > 0)
                        {
                            EFCachClear.ClearTable("User_CheckCode");
                        }
                        //
                        User_CheckCode myCheckCode = new User_CheckCode();
                        myCheckCode = myOperating.User_CheckCode.FirstOrDefault(p => p.MobilePhone == username);
                        if (myCheckCode != null)
                        {
                            if (myCheckCode.AddDate > myNowDate.AddMinutes(-1))
                            {
                                myStatusData.operateStatus = 0;
                                myStatusData.operateMsg = "不能频繁操作，稍后重试";
                            }
                            else
                            {
                                SmsClass mySmsClass = new SmsClass();
                                IsSendOk = mySmsClass.sendSmsMsg(username, 0, myMsg);
                                //
                                if (IsSendOk)
                                {
                                    myCheckCode.CheckCode = myMsg;
                                    myCheckCode.ExpiredDate = myExpiredDate;
                                    myCheckCode.AddDate = myNowDate;
                                    myCheckCode.AlreadCheck = false;
                                    myOperating.SaveChanges();
                                    //
                                    myStatusData.operateStatus = 200;
                                    myStatusData.operateMsg = "完成";
                                }
                                else
                                {
                                    myStatusData.operateStatus = 0;
                                    myStatusData.operateMsg = "发送失败！请重试";
                                }
                            }
                        }
                        else
                        {
                            //调试暂时屏蔽
                            SmsClass mySmsClass = new SmsClass();
                            IsSendOk = mySmsClass.sendSmsMsg(username, 0, myMsg);
                            //
                            if (IsSendOk)
                            {
                                User_CheckCode newCheckCode = new User_CheckCode();
                                newCheckCode.MobilePhone = username;
                                newCheckCode.CheckCode = myMsg;
                                newCheckCode.ExpiredDate = myExpiredDate;
                                newCheckCode.AddDate = myNowDate;
                                newCheckCode.AlreadCheck = false;
                                myOperating.User_CheckCode.Add(newCheckCode);
                                myOperating.SaveChanges();
                                //
                                myStatusData.operateStatus = 200;
                                myStatusData.operateMsg = "完成";
                            }
                            else
                            {
                                myStatusData.operateStatus = 0;
                                myStatusData.operateMsg = "发送失败！请重试";
                            }
                        } 
                        #endregion
                    }
                }
            }
            catch
            {
                myStatusData.operateStatus = -1;
                throw;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 检验验证码
        [HttpPost]
        public JsonResult CheckCode(string username, string checkcode)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            DateTime myNowDate = DateTime.Now;
            //
            User_UserName myUserInfo = new User_UserName();
            try
            {
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    User_CheckCode myUserCheckCode = myOperating.User_CheckCode.FirstOrDefault(p => p.MobilePhone == username && p.CheckCode == checkcode && p.ExpiredDate > myNowDate);

                    if (myUserCheckCode != null)
                    {
                        try
                        {
                            myUserCheckCode.AlreadCheck = true;
                            myOperating.SaveChanges();
                            myStatusData.operateMsg = "验证通过";
                            myStatusData.operateStatus = 200;
                        }
                        catch
                        {
                            myStatusData.operateStatus = -1;
                        }
                    }
                    else
                    {
                        myStatusData.operateStatus = 6;//验证码错误
                    }
                }
            }
            catch
            {
                myStatusData.operateStatus = -1;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 用户注册
        [HttpPost]
        public JsonResult Register(string username, string pwd)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            DateTime myNowDate = DateTime.Now;
            //
            User_UserName myUserInfo = new User_UserName();
            if (FunctionClass.isMobilePhone(username))
            {//手机号规则判断
                try
                {
                    using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                    {
                        myUserInfo = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == username);
                        if (myUserInfo == null)
                        {
                            int checkCodeCount = myOperating.User_CheckCode.Where(p => p.MobilePhone == username && p.AlreadCheck == true).Count();

                            if (checkCodeCount > 0)
                            {
                                User_UserName newUserInfo = new User_UserName();
                                newUserInfo.MobilePhone = username;
                                newUserInfo.PassWord = Md5Class.CreateMd5(pwd);
                                newUserInfo.AddDate = DateTime.Now;
                                newUserInfo.Status = 200;
                                newUserInfo.CheckDate = DateTime.Now;
                                newUserInfo.RecentlyLoginDate = DateTime.Now;
                                newUserInfo.RoleId = 0;
                                newUserInfo.NickName = "";
                                myOperating.User_UserName.Add(newUserInfo);
                                myOperating.SaveChanges();
                                myStatusData.operateStatus = 200;
                            }
                            else
                            {
                                myStatusData.operateStatus = 6;//验证码错误
                            }
                        }
                        else
                        {
                            myStatusData.operateStatus = 7;//已经存在
                        }
                    }
                }
                catch
                {
                    myStatusData.operateStatus = -1;
                }
            }
            else
            {//手机号错误
                myStatusData.operateStatus = 400;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 修改密码
        [HttpPost]
        public JsonResult EditPwd(string username, string pwd)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            DataMethodClass myDataMethodClass = new DataMethodClass();//检查验证码
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                int checkCodeCount = myOperating.User_CheckCode.Where(p => p.MobilePhone == username && p.AlreadCheck == true).Count();
                if (checkCodeCount > 0)
                {
                    myStatusData = myUserOptionClass.EditPwd(username, pwd);
                }
                else
                {
                    myStatusData.operateStatus = 6;//验证码错误
                }
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取用户信息
        [HttpPost]
        public JsonResult GetUserInfo()
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            if (checkAuthCodeBool)
            {
                DataMethodClass myDataMethodClass = new DataMethodClass();//检查验证码
                //
                //User_UserName myUserInfo = new User_UserName();
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    var myUserInfo = myOperating.User_UserName.Select(s => new { MobilePhone = s.MobilePhone, HeadImage = StaticVarClass.myDomain + s.HeadImage, NickName = s.NickName, Sex = s.Sex, BirthDate = s.BirthDate, ProvinceName = s.ProvinceName, CityName = s.CityName, DistrictName = s.DistrictName, DistrictNum = s.DistrictNum, Address = s.Address, Kindergarten = s.Kindergarten, RoleId = s.RoleId }).FirstOrDefault(p => p.MobilePhone == myAuthCodeInstance.mobilePhone);
                    if (myUserInfo == null)
                    {
                        myStatusData.operateStatus = 1;//用户名错误
                    }
                    else
                    {
                        try
                        {
                            myStatusData.dataTable = myUserInfo;
                            myStatusData.operateStatus = 200;
                        }
                        catch
                        {
                            myStatusData.operateStatus = -1;
                        }
                    }
                }
            }
            else
            {
                myStatusData.operateStatus = 5;//登陆失效
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 完善用户信息
        [HttpPost]
        public JsonResult EditUserInfo(string headimage, string nickname, string birthdate, string districtnum, string address, string kindergarten, string roleid)
        {
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            if (checkAuthCodeBool)
            {
                DataMethodClass myDataMethodClass = new DataMethodClass();//检查验证码
                //
                User_UserName myUserInfo = new User_UserName();
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    myUserInfo = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == myAuthCodeInstance.mobilePhone);
                    if (myUserInfo == null)
                    {
                        myStatusData.operateStatus = 1;//用户名错误
                    }
                    else
                    {
                        int nickNameExist = 0;
                        #region 用户昵称是否可用
                        if (!String.IsNullOrEmpty(nickname))
                        {
                            nickNameExist = myOperating.User_UserName.Count(p => p.MobilePhone != myAuthCodeInstance.mobilePhone && p.NickName == nickname);
                        }
                        #endregion
                        //
                        if (nickNameExist == 0)
                        {
                            try
                            {
                                #region 修改信息
                                if (!String.IsNullOrEmpty(headimage))
                                {
                                    string temPath = headimage.Replace("\\", "/");
                                    if (temPath.IndexOf("http://") == 0)
                                    {
                                        int domainPosition = 0;
                                        temPath = temPath.Replace("http://", "");
                                        domainPosition = temPath.IndexOf('/')+1;
                                        if (domainPosition < temPath.Length)
                                        temPath = temPath.Substring(domainPosition);
                                    }
                                    myUserInfo.HeadImage = temPath;
                                }
                                if (!String.IsNullOrEmpty(nickname))
                                {
                                    myUserInfo.NickName = nickname;

                                }
                                if (!String.IsNullOrEmpty(birthdate))
                                {
                                    DateTime myDateTime = DateTime.Now;
                                    bool isTrue = DateTime.TryParse(birthdate, out myDateTime);
                                    if (isTrue) myUserInfo.BirthDate = myDateTime;
                                }
                                #region 选择地区
                                if (!String.IsNullOrEmpty(districtnum))
                                {
                                    int myInt = 0;
                                    bool isTrue = int.TryParse(districtnum, out myInt);
                                    if (isTrue)
                                    {
                                        Sys_Area_District myDistrictTable = myOperating.Sys_Area_District.FirstOrDefault(p => p.DistrictNum == myInt);
                                        if (myDistrictTable != null)
                                        {
                                            myUserInfo.DistrictNum = myDistrictTable.DistrictNum;
                                            myUserInfo.DistrictName = myDistrictTable.DistrictName;
                                            Sys_Area_City myCity = myOperating.Sys_Area_City.FirstOrDefault(p => p.CityNum == myDistrictTable.CityID);
                                            if (myCity != null)
                                            {
                                                myUserInfo.CityNum = myCity.CityNum;
                                                myUserInfo.CityName = myCity.CityName;
                                                Sys_Area_Province myProvince = myOperating.Sys_Area_Province.FirstOrDefault(p => p.ProvinceNum == myCity.ProvinceID);
                                                if (myProvince != null)
                                                {
                                                    myUserInfo.ProvinceNum = myProvince.ProvinceNum;
                                                    myUserInfo.ProvinceName = myProvince.ProvinceName;
                                                }

                                            }
                                        }
                                    }

                                }
                                #endregion
                                if (!String.IsNullOrEmpty(address))
                                {
                                    myUserInfo.Address = address;
                                }
                                if (!String.IsNullOrEmpty(kindergarten))
                                {
                                    myUserInfo.Kindergarten = kindergarten;
                                }
                                if (!String.IsNullOrEmpty(roleid))
                                {
                                    if (myUserInfo.RoleId < 100)
                                    {
                                        int myInt = 0;
                                        bool isTrue = int.TryParse(roleid, out myInt);
                                        if (isTrue) myUserInfo.RoleId = myInt;
                                    }
                                }
                                #endregion
                                myOperating.SaveChanges();
                                myStatusData.operateStatus = 200;
                            }
                            catch
                            {
                                myStatusData.operateStatus = -1;
                            }
                        }
                        else
                        {
                            myStatusData.operateStatus = 400;
                            myStatusData.operateMsg = "该用户昵称已经存在";
                        }
                    }
                }
            }
            else
            {
                myStatusData.operateStatus = 5;//登陆失效
            }
            return Json(myStatusData);
        }
        #endregion
    }
}
