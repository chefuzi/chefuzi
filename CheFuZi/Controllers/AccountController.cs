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
   [Authorize(Roles = "100,101,102,103,104,105")]
    public class AccountController : Controller
    {
        UserOptionClass myUserOptionClass = new UserOptionClass();
        //
        // GET: /Account/Login
        #region 登录界面
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "Manage");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        #endregion
        // POST: /Account/Login
        #region 登录提交数据
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                StatusData myStatusData = new StatusData();//返回状态
                //
                myStatusData = myUserOptionClass.Login(model.UserName, model.Password, model.RememberMe);
                if (myStatusData.operateStatus == 200)
                {
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("index", "Manage");
                }
                else
                {
                    ModelState.AddModelError("", myStatusData.operateMsg);
                }
            }
            else
            {
                ModelState.AddModelError("", "提供的用户名或密码不正确2。");
            }
            return View(model);
        } 
        #endregion
        //
        #region 修改密码
        public ActionResult EditPwd(EditPasswordModel model, string returnUrl)
        {
            StatusData myStatusData = new StatusData();//返回状态
            //
            if (ModelState.IsValid)
            {
                myStatusData = myUserOptionClass.EditPwd(User.Identity.Name, model.NewPassword);
                ModelState.AddModelError("", myStatusData.operateMsg);
            }
            else
            {
                ModelState.AddModelError("", "填写错误。");
            }
            return View(model);
        } 
        #endregion
        //
        #region 退出
        public ActionResult LogOff()
        {
            bool isOk = myUserOptionClass.LoginOut();
            return RedirectToAction("Index", "Home");
        } 
        #endregion
        //

        #region 跳转程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
        //
    }
}