using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
//
using CheFuZi.DataOption;
namespace CheFuZi.Controllers.Service
{
    [ValidateInput(false)]
    [Authorize(Roles = "100,101,102,103,104,105")]
    public class ManageController : Controller
    {
        //
        [AllowAnonymous]
        #region 默认管理首页-判断角色
        public ActionResult Index()
        {
            UserOptionClass myUserOptionClass = new UserOptionClass();
            ViewBag.Message = "";
            //
            var formId = System.Web.HttpContext.Current.User.Identity as FormsIdentity;
            if (formId != null && formId.IsAuthenticated)
            {
                string[] rolesTemp = formId.Ticket.UserData.Split(',');
                string[] sysRoles = { "100", "101", "102", "103", "104", "105" };
                string roles = rolesTemp[0];
                if (!sysRoles.Contains(roles))
                {
                    myUserOptionClass.LoginOut();
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
            else
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            return View();
        } 
        #endregion
    }
}
