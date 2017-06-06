using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
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
    public class SuggestionController : Controller
    {
        //
        #region 添加意见反馈
        [HttpPost]
        public JsonResult AddSuggestion(string contact, string content)
        {
            StatusData myStatusData = new StatusData();//返回的类型
            string mobilePhone = "";//
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                Sys_Suggestion myRecord = new Sys_Suggestion();

                bool isOk = true;
                if (String.IsNullOrWhiteSpace(contact.Trim()))
                {
                    isOk = false;
                    myStatusData.operateStatus = 400;
                    myStatusData.operateMsg = "联系方式不能为空";
                }
                else if (contact.Trim().Length < 5)
                {
                    isOk = false;
                    myStatusData.operateStatus = 400;
                    myStatusData.operateMsg = "联系方式无效";
                }
                //
                if (String.IsNullOrWhiteSpace(content.Trim()))
                {
                    isOk = false;
                    myStatusData.operateStatus = 400;
                    myStatusData.operateMsg = "内容不能为空";
                }
                else if (content.Trim().Length<5)
                {
                    isOk = false;
                    myStatusData.operateStatus = 400;
                    myStatusData.operateMsg = "内容长度不能少于5个字符";
                }
                if (isOk)
                {
                    try
                    {
                        myRecord.Contact = contact;
                        myRecord.Describ = content;
                        myRecord.MobilePhone = mobilePhone;
                        myRecord.SuggestionTitle = "";
                        myRecord.AddDate = DateTime.Now;
                        myOperating.Sys_Suggestion.Add(myRecord);
                        myOperating.SaveChanges();
                        myStatusData.operateStatus = 200;
                    }
                    catch
                    {
                        myStatusData.operateStatus = -1;
                    }
                }
            }
            return Json(myStatusData);
        }
        #endregion
    }
}
