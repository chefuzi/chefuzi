using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Diagnostics;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;

namespace CheFuZi.Controllers.Service
{
    public class ScanCodeController : Controller
    {
        #region 扫码开通课件
        [HttpPost]
        public JsonResult Scan(string codestr)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            string myMobilePhone = "";//手机号码
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            int myTypeInt = 0;//类型;0注册开通课件；1学一学电子书注册
            string myCodeStr = "";//二维码
            string tempCodeStr = "";//临时
            #region 解析二维码字符串
            if (!String.IsNullOrWhiteSpace(codestr))
            {
                tempCodeStr = codestr.ToUpper().Replace(StaticVarClass.QRCodeUrl.ToUpper(), "");
                tempCodeStr = tempCodeStr.Replace("\\", "/");
                string[] temCode = tempCodeStr.Split('/');
                if (temCode.Length > 1)
                {
                    bool isInt = int.TryParse(temCode[0], out myTypeInt);
                    if (isInt)
                    {
                        myCodeStr = temCode[1];//
                    }
                    else
                    {
                        if (myTypeInt < 1)
                        {
                            myStatusData.operateStatus = 400;
                            myStatusData.operateMsg = "类型错误：" + codestr;
                            return Json(myStatusData);
                        }
                    }
                }
                else
                {
                    myTypeInt = 0;
                    myCodeStr = codestr;
                }
            }
            else
            {
                myStatusData.operateStatus = 400;
                return Json(myStatusData);
            }
            #endregion
            //
            if (checkAuthCodeBool)
            {
                myMobilePhone = myAuthCodeInstance.mobilePhone;
                //
                switch (myTypeInt)
                {
                    case 0:
                        break;
                    case 1:
                        #region 判断并开通学一学电子书
                        using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                        {
                            Sys_BookStudy_QRCode mySys_BookStudy_QRCode = new Sys_BookStudy_QRCode();
                            mySys_BookStudy_QRCode = myOperating.Sys_BookStudy_QRCode.FirstOrDefault(p => p.QRCode == myCodeStr);
                            if (mySys_BookStudy_QRCode != null)
                            {
                                if (mySys_BookStudy_QRCode.Used)
                                {//注册码已经使用过
                                    myStatusData.operateStatus = 500;
                                }
                                else
                                {
                                    #region 判断是否已经开通该课程，否则开通
                                    int TheBookIsReg = myOperating.User_RegBook_List.Count(p => p.MobilePhone == myMobilePhone && p.BookStudyID == mySys_BookStudy_QRCode.BookStudyID);
                                    if (TheBookIsReg == 0)
                                    {//判断该用户是否已经开通该课件
                                        #region 注册开通
                                        User_RegBook_List newUser_RegBook_List = new User_RegBook_List();
                                        int isReg = myOperating.User_RegBook_List.Count(p => p.QRCode == myCodeStr);
                                        if (isReg == 0)
                                        {
                                            newUser_RegBook_List.QRCode = myCodeStr;
                                            newUser_RegBook_List.MobilePhone = myMobilePhone;
                                            newUser_RegBook_List.BookStudyID = mySys_BookStudy_QRCode.BookStudyID;
                                            newUser_RegBook_List.RegDate = DateTime.Now;
                                            myOperating.User_RegBook_List.Add(newUser_RegBook_List);
                                            //
                                            mySys_BookStudy_QRCode.MobilePhone = myMobilePhone;
                                            mySys_BookStudy_QRCode.Used = true;
                                            mySys_BookStudy_QRCode.UsedDate = DateTime.Now;
                                            //
                                            myOperating.SaveChanges();
                                            myStatusData.operateStatus = 200;
                                        }
                                        else
                                        {
                                            myStatusData.operateStatus = 500;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        myStatusData.operateStatus = 400;
                                        myStatusData.operateMsg = "已经开通，无需重新开通";
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                myStatusData.operateStatus = 400;
                                myStatusData.operateMsg = "不存在";
                            }
                        }
                        #endregion
                        break;
                }
                //
            }
            else
            {
                myStatusData.operateStatus = 5;
            }
            return Json(myStatusData);
        }
        #endregion
    }
}
