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
    public class ManageDiscoverController : Controller
    {
        #region 文章列表
        public ActionResult ItemList(int currentpage = 1, int status = 0, long del = 0)
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
            ViewBag.Headline = "文章列表";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Discover_Article myDelRecord = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.ArticleImages);
                        FunctionClass.delFile(myDelRecord.ArticleVideo);
                        myOperating.Discover_Article.Remove(myDelRecord);
                        #region 同步删除收藏中的内容
                        DataOptionClass myDataOptionClass = new DataOptionClass();
                        myDataOptionClass.CollectOption(9, del, true);//删除收藏中的内容 
                        #endregion
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 审核
                if (status > 0)
                {
                    Discover_Article myStatusRecord = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == status);
                    if (myStatusRecord != null)
                    {
                        if (myStatusRecord.Status == 200)
                        {
                            myStatusRecord.Status = 300;
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
                string orderbyfiled = "ArticlId";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Discover_Article> myIQueryable = null;
                myIQueryable = myOperating.Discover_Article;
                //
                if (myIQueryable != null)
                {
                    IQueryable<Discover_Article> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true);
                    List<DiscoverArticleList> myDiscoverArticleList = new List<DiscoverArticleList>();
                    long temId = 0;
                    foreach (Discover_Article item in BookTable)
                    {
                        temId = item.ArticlId;
                        DiscoverArticleList DiscoverArticleItem = new DiscoverArticleList();
                        DiscoverArticleItem.ArticlId = temId;
                        DiscoverArticleItem.ArticleTitle = item.ArticleTitle;
                        DiscoverArticleItem.AddDate = item.AddDate;
                        DiscoverArticleItem.MobilePhone = item.MobilePhone;
                        DiscoverArticleItem.ReadTimes = item.ReadTimes;
                        DiscoverArticleItem.Status = item.Status;
                        DiscoverArticleItem.CommentCount = myOperating.Discover_Article_Comment.Count(p=>p.AboutId == temId);
                        myDiscoverArticleList.Add(DiscoverArticleItem);
                    }
                    //
                    ViewBag.DataList = myDiscoverArticleList;
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
        #region 文章查看
        public ActionResult ItemView(DiscoverArticleModel model, string ReturnUrl, Nullable<long> myid = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "文章查看";//栏目主题
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    long mySelfId = 0;
                    long.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "文章查看";
                    //
                    model.Operate = "";
                    #region 取出数据
                    Discover_Article editRecord = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == mySelfId);
                    if (editRecord != null)
                    {
                        model.ArticlId = editRecord.ArticlId;
                        model.MobilePhone = editRecord.MobilePhone;
                        model.ArticleTitle = editRecord.ArticleTitle;
                        model.ArticleImages = FunctionClass.GetFileUrl(editRecord.ArticleImages);
                        model.ArticleVideo = FunctionClass.GetFileUrl(editRecord.ArticleVideo);
                        model.ArticleContent = editRecord.ArticleContent;
                        model.ReadTimes = editRecord.ReadTimes;
                        model.OrderBy = editRecord.OrderBy;
                        model.Status = editRecord.Status;
                        model.Operate = "look";
                    }
                    #endregion
                }
               
            }
            //
            return View(model);
        }
        #endregion
        //---------------------CommentList
        #region 评论列表
        public ActionResult CommentList(int currentpage = 1, long aboutid = 0, long del = 0)
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
            ViewBag.Headline = "评论管理";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Discover_Article_Comment myDelRecord = myOperating.Discover_Article_Comment.FirstOrDefault(p => p.CommentId == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Discover_Article_Comment.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "CommentId";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Discover_Article_Comment> myIQueryable = null;
                if (aboutid > 0)
                {
                    myIQueryable = myOperating.Discover_Article_Comment.Where(p => p.AboutId == aboutid);
                }
                else
                {
                    myIQueryable = myOperating.Discover_Article_Comment;
                }

                //
                if (myIQueryable != null)
                {
                    List<Discover_Article_Comment> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
                    //
                    ViewBag.DataList = BookTable;
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
                //
            }
            return View();
        }
        #endregion
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
    //
    #region 发现列表结构
    public class DiscoverArticleList
    {
        public long ArticlId { get; set; }
        public string MobilePhone { get; set; }
        public string ArticleTitle { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentCount { get; set; }
        public Nullable<int> Status { get; set; }
    } 
    #endregion
}
