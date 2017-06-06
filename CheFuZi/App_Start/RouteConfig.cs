using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CheFuZi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //
            #region 数据接口
            routes.MapRoute(
                "Service",
                "Service/{controller}/{action}/{id}", // 
                new { id = UrlParameter.Optional },
                new { controller = @"Audio|Click|Collect|Discover|FileUpload|Game|Public|ScanCode|Study|Suggestion|TeacherVideo|User|Video" }
            );
            #endregion
            //
            #region 扫码打开网页
            routes.MapRoute(
                    "Scan",
                    "Scan/{typestr}/{codestr}", //
                    new { controller = "DownLoad", action = "App", },
                    new { controller = "DownLoad", action = "App", typestr = @"\d+" }
                ); 
            #endregion
            //打开教师社区内容
            #region 打开教师社区内容
            routes.MapRoute(
                "Community",
                "Community/{myid}.html", //
                new { controller = "Community", action = "Details"},
                new { controller = "Community", action = "Details", myid = @"\d+" }
            ); 
            #endregion
            //
            #region 打开分享网页
            routes.MapRoute(
                    "Share",
                    "Share/{classtype}/{aboutid}", //
                    new { controller = "Home", action = "Share" },
                    new { controller = "Home", action = "Share", classtype = @"\d+", aboutid = @"\d+" }
                );
            #endregion
            //默认路由
            routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}