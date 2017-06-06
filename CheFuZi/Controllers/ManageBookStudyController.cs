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
    [ValidateInput(false)]
   [Authorize(Roles = "100,101,102,103,104,105")]
    public class ManageBookStudyController : Controller
    {
        #region 内容列表
        public ActionResult ItemList(int currentpage = 1, int ClassId = 0, long del = 0)
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
            ViewBag.Headline = "图书管理";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            ViewBag.Operate = "add";//add添加；edit编辑取出数据；editsave编辑保存
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Child_Book_Study myDelRecord = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Child_Book_Study.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "OrderBy";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Child_Book_Study> myIQueryable = null;
                if (ClassId > 0)
                {
                    myIQueryable = myOperating.Child_Book_Study.Where(p => p.BookClassID == ClassId);
                }
                else
                {
                    myIQueryable = myOperating.Child_Book_Study;
                }
                //
                if (myIQueryable != null)
                {
                    List<Child_Book_Study> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
                ViewBag.ClassId = ClassId;//当前类别
                //
                List<Child_Book_Study_Class> ClassTable = myOperating.Child_Book_Study_Class.Where(p => p.Status == 200).OrderByDescending(p=>p.OrderBy).ToList();
                ViewBag.DataListClass = ClassTable;
            }
            return View();
        }
        #endregion
        //
        #region 内容添加-编辑
        public ActionResult ItemAdd(BookStudyModel model, string ReturnUrl, Nullable<long> myid = 0, Nullable<int> ClassId = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "图书添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    long mySelfId = 0;
                    long.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "图书编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Book_Study editRecord = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == mySelfId);
                    if (editRecord != null)
                    {
                        ClassId = editRecord.BookClassID;
                        model.BookID = editRecord.BookID;
                        model.BookClassID = editRecord.BookClassID;
                        model.BookName = editRecord.BookName;
                        model.BookOnlineUrl = editRecord.BookOnlineUrl;
                        model.BookDownLoadZip = editRecord.BookDownLoadZip;
                        model.BookZipName = editRecord.BookZipName;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Child_Book_Study addRecord = new Child_Book_Study();
                        addRecord.BookClassID = model.BookClassID;
                        addRecord.BookName = model.BookName;
                        addRecord.BookOnlineUrl = model.BookOnlineUrl;
                        addRecord.BookDownLoadZip = model.BookDownLoadZip;
                        addRecord.BookZipName = model.BookZipName;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.PraiceCount = 0;
                        addRecord.ReadCount = 0;
                        addRecord.BookStatus = 200;
                        //
                        myOperating.Child_Book_Study.Add(addRecord);
                        myOperating.SaveChanges();
                        //
                        return RedirectToLocal(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "输入错误");
                    }
                    #endregion
                }
                else if (model.Operate == "edit")
                {
                    //
                    #region 保存编辑
                    if (ModelState.IsValid)
                    {
                        Child_Book_Study editSaveRecord = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == model.BookID);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.BookClassID = model.BookClassID;
                            editSaveRecord.BookName = model.BookName;
                            editSaveRecord.BookOnlineUrl = model.BookOnlineUrl;
                            editSaveRecord.BookDownLoadZip = model.BookDownLoadZip;
                            editSaveRecord.BookZipName = model.BookZipName;
                            editSaveRecord.OrderBy = model.OrderBy;
                            //
                            myOperating.SaveChanges();
                            //
                            return RedirectToLocal(ReturnUrl);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "输入错误");
                    }
                    #endregion
                }
                else
                { //初次加载-添加
                    model.BookClassID = ClassId;//类别编号
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        if (ClassId != 0)
                        {
                            model.OrderBy = myOperating.Child_Book_Study.Where(p => p.BookClassID == ClassId).Max(p => p.OrderBy) + 1;
                        }
                        else
                        {
                            model.OrderBy = myOperating.Child_Book_Study.Max(p => p.OrderBy) + 1;
                        }

                    }
                    catch
                    {
                        model.OrderBy = 0;
                    }
                    if (model.OrderBy == null)
                    {
                        model.OrderBy = 1;
                    }
                    #endregion
                }
                #region 类别
                List<Child_Book_Study_Class> ClassTable = myOperating.Child_Book_Study_Class.Where(p => p.Status == 200).OrderByDescending(p=>p.OrderBy).ToList();
                ViewBag.DataListClass = ClassTable;
                #endregion
            }
            //
            return View(model);
        }
        #endregion
        //========================================
        #region 类别列表
        public ActionResult ClassList(int currentpage = 1, int ClassId = 0, long del = 0)
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
            ViewBag.Headline = "图书类别管理";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            ViewBag.Operate = "add";//add添加；edit编辑取出数据；editsave编辑保存
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Child_Book_Study_Class myDelRecord = myOperating.Child_Book_Study_Class.FirstOrDefault(p => p.BookClassID == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Child_Book_Study_Class.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "OrderBy";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Child_Book_Study_Class> myIQueryable = null;
                if (ClassId > 0)
                {
                    myIQueryable = myOperating.Child_Book_Study_Class.Where(p => p.BookClassID == ClassId);
                }
                else
                {
                    myIQueryable = myOperating.Child_Book_Study_Class;
                }
                //
                if (myIQueryable != null)
                {
                    List<Child_Book_Study_Class> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
            }
            return View();
        }
        #endregion
        //
        #region 类别添加-编辑
        public ActionResult ClassAdd(BookStudyClassModel model, string ReturnUrl, Nullable<int> myid = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "类别添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    int mySelfId = 0;
                    int.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "类别编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Book_Study_Class editRecord = myOperating.Child_Book_Study_Class.FirstOrDefault(p => p.BookClassID == mySelfId);
                    if (editRecord != null)
                    {
                        model.BookClassID = editRecord.BookClassID;
                        model.BookClassName = editRecord.BookClassName;
                        model.ImgWidth = editRecord.ImgWidth;
                        model.ImgHeight = editRecord.ImgHeight;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Child_Book_Study_Class addRecord = new Child_Book_Study_Class();
                        addRecord.BookClassID = model.BookClassID;
                        addRecord.BookClassName = model.BookClassName;
                        addRecord.ImgWidth = model.ImgWidth;
                        addRecord.ImgHeight = model.ImgHeight;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Book_Study_Class.Add(addRecord);
                        myOperating.SaveChanges();
                        model = null;
                        //
                        return RedirectToLocal(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "输入错误");
                    }
                    #endregion

                }
                else if (model.Operate == "edit")
                {
                    #region 保存编辑
                    if (ModelState.IsValid)
                    {
                        Child_Book_Study_Class editSaveRecord = myOperating.Child_Book_Study_Class.FirstOrDefault(p => p.BookClassID == model.BookClassID);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.BookClassName = model.BookClassName;
                            editSaveRecord.ImgWidth = model.ImgWidth;
                            editSaveRecord.ImgHeight = model.ImgHeight;
                            editSaveRecord.OrderBy = model.OrderBy;
                            //
                            myOperating.SaveChanges();
                            //
                            return RedirectToLocal(ReturnUrl);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "输入错误");
                    }
                    #endregion
                }
                else
                {
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.BookClassID = myOperating.Child_Book_Study_Class.Max(p => p.BookClassID) + 1;
                        model.OrderBy = myOperating.Child_Book_Study_Class.Max(p => p.OrderBy) + 1;
                    }
                    catch
                    {
                        model.OrderBy = 0;
                    }
                    if (model.OrderBy == null)
                    {
                        model.OrderBy = 1;
                    }
                    #endregion
                }
            }
            //
            return View(model);
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
}
