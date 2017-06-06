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
    public class ManageVideoController : Controller
    {
        //
        #region 类别列表
        public ActionResult ClassList(string currentpage, long del = 0)
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
            ViewBag.Headline = "类别管理";//栏目主题
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
                    Child_Video_Class myDelRecord = myOperating.Child_Video_Class.FirstOrDefault(p => p.ClassId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.ClassImage);
                        myOperating.Child_Video_Class.Remove(myDelRecord);
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
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Child_Video_Class> myIQueryable = null;
                myIQueryable = myOperating.Child_Video_Class;
                //
                if (myIQueryable != null)
                {
                    List<Child_Video_Class> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
        public ActionResult ClassAdd(VideoClassModel model, string ReturnUrl, Nullable<int> myid = 0)
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
                if ((myid > 0) && (model.ClassId == null))
                {
                    int mySelfId = 0;
                    int.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "类别编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Video_Class editRecord = myOperating.Child_Video_Class.FirstOrDefault(p => p.ClassId == mySelfId);
                    if (editRecord != null)
                    {
                        model.ClassId = editRecord.ClassId;
                        model.ClassTitle = editRecord.ClassTitle;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Child_Video_Class addRecord = new Child_Video_Class();
                        addRecord.ClassId = int.Parse(model.ClassId.ToString());
                        addRecord.ClassTitle = model.ClassTitle;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Video_Class.Add(addRecord);
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
                        Child_Video_Class editSaveRecord = myOperating.Child_Video_Class.FirstOrDefault(p => p.ClassId == model.ClassId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.ClassTitle = model.ClassTitle;
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
                {//初次加载，默认值
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.ClassId = myOperating.Child_Video_Class.Max(p => p.ClassId) + 1;
                        model.OrderBy = myOperating.Child_Video_Class.Max(p => p.OrderBy) + 1;
                    }
                    catch
                    {
                        model.ClassId = 1;
                        model.OrderBy = 0;
                    }
                    if (model.ClassId == null)
                    {
                        model.ClassId = 1;
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
        //-------------------------
        #region 专辑列表
        public ActionResult AlbumList(string currentpage, int ClassId = 0, long del = 0)
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
            ViewBag.Headline = "专辑管理";//栏目主题
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
                    Child_Video_Album myDelRecord = myOperating.Child_Video_Album.FirstOrDefault(p => p.AlbumId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.AlbumImage);
                        myOperating.Child_Video_Album.Remove(myDelRecord);
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
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Child_Video_Album> myIQueryable = null;
                if (ClassId > 0)
                {
                    myIQueryable = myOperating.Child_Video_Album.Where(p => p.ClassId == ClassId);
                }
                else
                {
                    myIQueryable = myOperating.Child_Video_Album.Where(p => p.Status == 200);
                }

                //
                if (myIQueryable != null)
                {
                    List<Child_Video_Album> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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

                ViewBag.ClassId = ClassId;//当前类别
                List<Child_Video_Class> ClassTable = myOperating.Child_Video_Class.ToList();
                //
                ViewBag.DataListClass = ClassTable;
            }
            return View();
        }
        #endregion
        //
        #region 专辑添加-编辑
        public ActionResult AlbumAdd(VideoAlbumModel model, string ReturnUrl, int ClassId=0, Nullable<int> myid = 0)
        {
            model.ClassId = ClassId;
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "专辑添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    int mySelfId = 0;
                    int.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "看一看专辑编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Video_Album editRecord = myOperating.Child_Video_Album.FirstOrDefault(p => p.AlbumId == mySelfId);
                    if (editRecord != null)
                    {
                        model.ClassId = editRecord.ClassId;
                        model.AlbumId = editRecord.AlbumId;
                        model.AlbumTitle = editRecord.AlbumTitle;
                        model.AlbumImage = editRecord.AlbumImage;
                        model.AlbumDescrib = editRecord.AlbumDescrib;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Child_Video_Album addRecord = new Child_Video_Album();
                        addRecord.ClassId = model.ClassId;
                        addRecord.AlbumTitle = model.AlbumTitle;
                        addRecord.AlbumImage = model.AlbumImage;
                        addRecord.AlbumDescrib = model.AlbumDescrib;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.VideoCount = 0;
                        addRecord.PlayTimes = 0;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Video_Album.Add(addRecord);
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
                        Child_Video_Album editSaveRecord = myOperating.Child_Video_Album.FirstOrDefault(p => p.AlbumId == model.AlbumId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.ClassId = model.ClassId;
                            editSaveRecord.AlbumTitle = model.AlbumTitle;
                            editSaveRecord.AlbumImage = model.AlbumImage;
                            editSaveRecord.AlbumDescrib = model.AlbumDescrib;
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
                    model.OrderBy = 1;
                }
                //
                List<Child_Video_Class> BookTable = myOperating.Child_Video_Class.ToList();
                //
                ViewBag.DataList = BookTable;
            }
            //
            return View(model);
        }
        #endregion
        //--------------------------
        #region 视频列表
        public ActionResult VideoList(string albumid, string currentpage, long del = 0)
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
            ViewBag.Headline = "视频管理";//栏目主题
            //
            ViewBag.AlbumId = albumid;
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
                    Child_Video_List myDelRecord = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.VideoImage);
                        FunctionClass.delFile(myDelRecord.VideoUrl);
                        myOperating.Child_Video_List.Remove(myDelRecord);
                        #region 同步删除收藏中的内容
                        DataOptionClass myDataOptionClass = new DataOptionClass();
                        myDataOptionClass.CollectOption(3, del, true);//删除收藏中的内容 
                        #endregion
                        myOperating.SaveChanges();
                    }
                }
                #endregion

                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "OrderBy";
                bool isDesc = true;//倒序
                //
                //当前页
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                #endregion
                IQueryable<Child_Video_List> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(albumid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(albumid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Video_List.Where(p => p.AlbumId == myInt);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                //
                if (myIQueryable != null)
                {
                    List<Child_Video_List> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).ToList();
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

            }
            return View();
        }
        #endregion
        //
        #region 视频添加-编辑
        public ActionResult VideoAdd(VideoListModel model, string ReturnUrl, Nullable<long> myid = 0, int AlbumId = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "视频添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            //
            int AllTimeSecond = 0;
            int myMinute = 0;
            int mySecond = 0;
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    long mySelfId = 0;
                    long.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "看一看视频编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Video_List editRecord = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == mySelfId);
                    if (editRecord != null)
                    {
                        model.VideoId = editRecord.VideoId;
                        model.AlbumId = editRecord.AlbumId;
                        model.VideoTitle = editRecord.VideoTitle;
                        model.VideoImage = editRecord.VideoImage;
                        model.VideoUrl = editRecord.VideoUrl;
                        model.VideoDes = editRecord.VideoDes;
                        //
                        #region 时间拆分成分秒
                        if (editRecord.TimeSeconds != null)
                        {
                            int.TryParse(editRecord.TimeSeconds.ToString(), out AllTimeSecond);
                        }
                        myMinute = AllTimeSecond / 60;
                        mySecond = AllTimeSecond % 60;
                        model.TimeMinute = myMinute;
                        model.TimeSeconds = mySecond;
                        #endregion
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 时间组合分秒
                    if (model.TimeMinute != null)
                    {
                        int.TryParse(model.TimeMinute.ToString(), out myMinute);
                    }
                    if (model.TimeSeconds != null)
                    {
                        int.TryParse(model.TimeSeconds.ToString(), out mySecond);
                    }
                    AllTimeSecond = myMinute * 60 + mySecond;
                    #endregion
                    //
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Child_Video_List addRecord = new Child_Video_List();
                        addRecord.AlbumId = model.AlbumId;
                        addRecord.VideoTitle = model.VideoTitle;
                        addRecord.VideoImage = model.VideoImage;
                        addRecord.VideoUrl = model.VideoUrl;
                        addRecord.VideoDes = model.VideoDes;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.TimeSeconds = AllTimeSecond;
                        addRecord.PlayTimes = 0;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Video_List.Add(addRecord);
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
                    #region 时间组合分秒
                    if (model.TimeMinute != null)
                    {
                        int.TryParse(model.TimeMinute.ToString(), out myMinute);
                    }
                    if (model.TimeSeconds != null)
                    {
                        int.TryParse(model.TimeSeconds.ToString(), out mySecond);
                    }
                    AllTimeSecond = myMinute * 60 + mySecond;
                    #endregion
                    //
                    #region 保存编辑
                    if (ModelState.IsValid)
                    {
                        Child_Video_List editSaveRecord = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == model.VideoId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.AlbumId = model.AlbumId;
                            editSaveRecord.VideoTitle = model.VideoTitle;
                            editSaveRecord.VideoImage = model.VideoImage;
                            editSaveRecord.VideoUrl = model.VideoUrl;
                            editSaveRecord.VideoDes = model.VideoDes;
                            editSaveRecord.TimeSeconds = AllTimeSecond;
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
                    model.AlbumId = AlbumId;//类别编号
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.OrderBy = myOperating.Child_Video_List.Where(p => p.AlbumId == AlbumId).Max(p => p.OrderBy) + 1;
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
