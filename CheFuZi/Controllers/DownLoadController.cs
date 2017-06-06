using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
//
using CheFuZi.Models;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
using CheFuZi.DataOption;

namespace CheFuZi.Controllers
{
    public class DownLoadController : Controller
    {
        #region APP下载页
        /// <summary>
        /// 使用非芮卡家园APP扫码后打开该页下载APP
        /// </summary>
        ///<param name="typestr">1</param>
        /// <param name="codestr">二维码字符串</param>
        /// <returns></returns>
        public ActionResult App(string typestr, string codestr)
        {
            string showMsg = "";//提示信息
            int typeId = 0;//类型；1学一学课件注册
            bool typeIsInt = int.TryParse(typestr, out typeId);
            //-----------------------
            if (typeId == 1)
            {
                #region 判断并开通学一学电子书
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    Sys_BookStudy_QRCode mySys_BookStudy_QRCode = new Sys_BookStudy_QRCode();
                    mySys_BookStudy_QRCode = myOperating.Sys_BookStudy_QRCode.FirstOrDefault(p => p.QRCode == codestr);
                    if (mySys_BookStudy_QRCode != null)
                    {
                        long myBookId = mySys_BookStudy_QRCode.BookStudyID;
                        if (mySys_BookStudy_QRCode.Used)
                        {//注册码已经使用过
                            showMsg = "该二维码已经使用过，不能再次使用，请核对准确。";
                        }
                        else
                        {
                            #region 要开通那本图书
                            Child_Book_Study myChild_Book_Study = new Child_Book_Study();
                            myChild_Book_Study = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == myBookId);
                            if (myChild_Book_Study != null)
                            {
                                showMsg = "要开通：《" + myChild_Book_Study.BookName + "》点读";
                                if (!String.IsNullOrWhiteSpace(myChild_Book_Study.BookImage))
                                {
                                    ViewBag.CoverImage = StaticVarClass.BookStudyResourceUrl + myChild_Book_Study.BookImage;
                                }
                            }
                            else
                            {
                                showMsg = "该电子书不存在，请核实您的二维码。";
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        showMsg = "该编码不存在，请核实您的二维码。";
                    }
                }
                #endregion
            }
            else
            {
                showMsg = "二维码错误。";
            }
            ViewBag.Message = showMsg;
            //========================
            return View();
        }
        #endregion
    }
}
