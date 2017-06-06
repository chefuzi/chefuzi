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

namespace CheFuZi.Controllers.Service
{
    [ValidateInput(false)]
    [Authorize(Roles = "100,101,102,103,104,105")]
    public class ManageUserController : Controller
    {
        #region 用户列表
        public ActionResult UserList(int currentpage = 1, string status = "", string del = "",string search="")
        {
            #region 翻页定义
            ViewBag.CurrentPage = 0;//当前页
            ViewBag.PPage = 0;//上一页
            ViewBag.NPage = 0;//下一页
            ViewBag.PageCount = 0;//总页数
            ViewBag.RecordCount = 0;//记录总数
            ViewBag.IsFirstPage = "";//第一条记录，禁用首页和上一页
            ViewBag.IsEndPage = "";//最后条记录，禁用首页和下一页 
            #endregion
            //
            ViewBag.Headline = "用户列表";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            ViewBag.Search = search;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (!String.IsNullOrWhiteSpace(del))
                {
                    User_UserName myDelRecord = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == del && p.RoleId<100);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.HeadImage);
                        myOperating.User_UserName.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 锁定
                if (!String.IsNullOrWhiteSpace(status))
                {
                    User_UserName myStatusRecord = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == status && p.RoleId < 100);
                    if (myStatusRecord != null)
                    {
                        if (myStatusRecord.Status == 200)
                        {
                            myStatusRecord.Status = 201;
                        }
                        else
                        {
                            myStatusRecord.Status = 200;
                        }
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AddDate";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<User_UserName> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(search))
                {
                    myIQueryable = myOperating.User_UserName.Where(p => p.MobilePhone.Contains(search));
                }
                else
                {
                    myIQueryable = myOperating.User_UserName;
                }
                //
                if (myIQueryable != null)
                {
                    List<User_UserName> myDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
                    //
                    ViewBag.DataList = myDataTable;
                    #region 页数取值
                    ViewBag.CurrentPage = sqlCurrentpage;
                    ViewBag.PageCount = pageCount;
                    ViewBag.RecordCount = recordCount;
                    if (sqlCurrentpage > 1)
                    {
                        ViewBag.PPage = sqlCurrentpage - 1;
                    }
                    else
                    {
                        ViewBag.IsFirstPage = "disabled";
                        ViewBag.PPage = 1;
                    }
                    if (sqlCurrentpage < pageCount)
                    {
                        ViewBag.NPage = sqlCurrentpage + 1;
                    }
                    else
                    {
                        ViewBag.NPage = sqlCurrentpage;
                        ViewBag.IsEndPage = "disabled";
                    }
                    #endregion
                }
                #endregion
            }
            return View();
        }
        #endregion
        //
        #region 用户信息查看
        public ActionResult UserView(User_UserName model, string mobilestr = "")
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "用户查看";//栏目主题
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if (!String.IsNullOrWhiteSpace(mobilestr))
                {
                    #region 取出数据
                    model = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == mobilestr);
                    #endregion
                }
            }
            //
            return View(model);
        }
        #endregion
        //
        //-----------------------------
        #region 网页跳转程序
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}
