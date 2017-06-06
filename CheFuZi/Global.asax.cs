using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;

namespace CheFuZi
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        #region 身份验证
        protected void Application_AuthorizeRequest(object sender, System.EventArgs e)
        {
            HttpApplication myApp = (HttpApplication)sender;
            HttpContext userContext = myApp.Context;  //获取本次Http请求相关的HttpContext对象 
            if (userContext.Request.IsAuthenticated == true) //验证过的用户才进行role的处理
            {
                FormsIdentity userId = (FormsIdentity)userContext.User.Identity;
                FormsAuthenticationTicket Ticket = userId.Ticket;  //取得身份验证票      
                string[] myRoles = Ticket.UserData.Split(',');  //将身份验证票中的role数据转成字符串数组   
                userContext.User = new GenericPrincipal(userId, myRoles); //将原有的Identity加上角色信息新建一个GenericPrincipal表示当前用户,这样当前用户就拥有了role信息 
            }
        } 
        #endregion
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            BundleTable.EnableOptimizations = true;
        }
    }
}