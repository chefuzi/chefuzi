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
    public class ManageTeacherController : Controller
    {
        #region 视频列表
        public ActionResult TeacherVideoList(int currentpage = 1, int ClassId = 0, int LessonId = 0, long del = 0)
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
            ViewBag.Headline = "教师视频管理";//栏目主题
            #region 栏目名称
            switch (ClassId)
            {
                case 1:
                    ViewBag.Headline = "芮卡课程讲解";
                    break;
                case 2:
                    ViewBag.Headline = "幼儿公开课";
                    break;
                case 3:
                    ViewBag.Headline = "教育技能";
                    break;
                case 4:
                    ViewBag.Headline = "芮卡多媒体操作说明";
                    break;
            }
            #endregion

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
                    Teacher_Video_List myDelRecord = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.VideoImage);
                        FunctionClass.delFile(myDelRecord.VideoUrl);
                        myOperating.Teacher_Video_List.Remove(myDelRecord);
                        #region 同步删除收藏中的内容
                        DataOptionClass myDataOptionClass = new DataOptionClass();
                        myDataOptionClass.CollectOption(4, del, true);//删除收藏中的内容 
                        myDataOptionClass.CollectOption(5, del, true);//删除收藏中的内容 
                        myDataOptionClass.CollectOption(6, del, true);//删除收藏中的内容 
                        myDataOptionClass.CollectOption(7, del, true);//删除收藏中的内容 
                        #endregion
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "Recommended";
                string orderbyfiled2 = "OrderBy";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Teacher_Video_List> myIQueryable = null;
                if (ClassId > 0)
                {
                    if ((ClassId == 1) && (LessonId > 0))
                    {
                        myIQueryable = myOperating.Teacher_Video_List.Where(p => p.ClassId == ClassId && p.LessonId == LessonId);
                    }
                    else
                    {
                        myIQueryable = myOperating.Teacher_Video_List.Where(p => p.ClassId == ClassId);
                    }
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Video_List;
                }
                //
                if (myIQueryable != null)
                {
                    List<Teacher_Video_List> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true, orderbyfiled2).ToList();
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
                ViewBag.LessonId = LessonId;//当前课程门类
                //
                #region 芮卡课程讲解需要取出课程门类
                if (ClassId == 1)
                {//芮卡课程讲解
                    List<Teacher_Video_Class_Lesson> LessonTable = myOperating.Teacher_Video_Class_Lesson.Where(p => p.Status == 200).OrderByDescending(p => p.OrderBy).ToList();
                    ViewBag.DataListLesson = LessonTable;
                }
                #endregion
            }
            return View();
        }
        #endregion
        //
        #region 视频添加-编辑
        public ActionResult TeacherVideoAdd(TeacherVideoListModel model, string ReturnUrl, Nullable<long> myid = 0, int ClassId = 0, int LessonId = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "音频添加";//栏目主题
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
                    ViewBag.Headline = "视频编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Teacher_Video_List editRecord = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == mySelfId);
                    if (editRecord != null)
                    {
                        ClassId = editRecord.ClassId;
                        model.VideoId = editRecord.VideoId;
                        model.ClassId = editRecord.ClassId;
                        model.LessonId = editRecord.LessonId;
                        model.VideoTitle = editRecord.VideoTitle;
                        model.VideoImage = editRecord.VideoImage;
                        model.VideoUrl = editRecord.VideoUrl;
                        model.VideoDes = editRecord.VideoDes;
                        model.Recommended = editRecord.Recommended;
                        //
                        if (model.Recommended == null)
                        {
                            model.Recommended = 0;
                        }
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
                        Teacher_Video_List addRecord = new Teacher_Video_List();
                        addRecord.ClassId = model.ClassId;
                        addRecord.LessonId = model.LessonId;
                        addRecord.VideoTitle = model.VideoTitle;
                        addRecord.VideoImage = model.VideoImage;
                        addRecord.VideoUrl = model.VideoUrl;
                        addRecord.VideoDes = model.VideoDes;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.Recommended = model.Recommended;
                        addRecord.TimeSeconds = AllTimeSecond;
                        addRecord.PlayTimes = 0;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Teacher_Video_List.Add(addRecord);
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
                        Teacher_Video_List editSaveRecord = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == model.VideoId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.ClassId = model.ClassId;
                            editSaveRecord.LessonId = model.LessonId;
                            editSaveRecord.VideoTitle = model.VideoTitle;
                            editSaveRecord.VideoImage = model.VideoImage;
                            editSaveRecord.VideoUrl = model.VideoUrl;
                            editSaveRecord.VideoDes = model.VideoDes;
                            editSaveRecord.Recommended = model.Recommended;
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
                    model.ClassId = ClassId;//类别编号
                    model.LessonId = LessonId;
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.OrderBy = myOperating.Teacher_Video_List.Where(p => p.ClassId == ClassId).Max(p => p.OrderBy) + 1;
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
                #region 芮卡课程讲解需要取出课程门类
                if (ClassId == 1)
                {//芮卡课程讲解
                    List<Teacher_Video_Class_Lesson> LessonTable = myOperating.Teacher_Video_Class_Lesson.Where(p => p.Status == 200).OrderByDescending(p => p.OrderBy).ToList();
                    ViewBag.DataListLesson = LessonTable;
                }
                #endregion
            }
            //
            return View(model);
        }
        #endregion
        //==============================
        #region 教师社区文章列表
        public ActionResult TeacherCommunityList(int currentpage = 1, int ClassId = 0, long del = 0)
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
            ViewBag.Headline = "教师社区管理";//栏目主题
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
                    Teacher_Article myDelRecord = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.ArticleImages);
                        FunctionClass.delFile(myDelRecord.ArticleVideo);
                        string[] contentImages = FunctionClass.GetHtmlImageUrlList(myDelRecord.ArticleContent);
                        FunctionClass.delFile("", contentImages);
                        myOperating.Teacher_Article.Remove(myDelRecord);
                        #region 同步删除收藏中的内容
                        DataOptionClass myDataOptionClass = new DataOptionClass();
                        myDataOptionClass.CollectOption(8, del, true);//删除收藏中的内容 
                        #endregion
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
                IQueryable<Teacher_Article> myIQueryable = null;
                if (ClassId > 0)
                {
                    myIQueryable = myOperating.Teacher_Article.Where(p => p.ClassId == ClassId);
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Article;
                }
                //
                if (myIQueryable != null)
                {
                    IQueryable<Teacher_Article> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true);
                    //
                    List<teacherCommunityList> myCommunityList = new List<teacherCommunityList>();
                    long temId = 0;
                    foreach (Teacher_Article item in BookTable)
                    {
                        temId = item.ArticlId;
                        teacherCommunityList CommunityItem = new teacherCommunityList();
                        CommunityItem.ArticlId = item.ArticlId;
                        CommunityItem.ArticleTitle = item.ArticleTitle;
                        CommunityItem.OrderBy = item.OrderBy;
                        CommunityItem.CommentCount = myOperating.Teacher_Article_Comment.Count(p => p.AboutId == temId);
                        myCommunityList.Add(CommunityItem);
                    }
                    ViewBag.DataList = myCommunityList;
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
                List<Teacher_Article_Class> ClassTable = myOperating.Teacher_Article_Class.Where(p => p.Status == 200).OrderByDescending(p => p.OrderBy).ToList();
                ViewBag.DataListClass = ClassTable;
            }
            return View();
        }
        #endregion
        //
        #region 文章添加-编辑
        public ActionResult TeacherCommunityAdd(TeacherArticleModel model, string ReturnUrl, Nullable<long> myid = 0, int ClassId = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "文章添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    long mySelfId = 0;
                    long.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "文章编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Teacher_Article editRecord = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == mySelfId);
                    if (editRecord != null)
                    {
                        ClassId = editRecord.ClassId;
                        model.ArticlId = editRecord.ArticlId;
                        model.ClassId = editRecord.ClassId;
                        model.ArticleTitle = editRecord.ArticleTitle;
                        model.ArticleAuthor = editRecord.ArticleAuthor;
                        model.ArticleSummary = editRecord.ArticleSummary;
                        model.ArticleImages = editRecord.ArticleImages;
                        model.ArticleVideo = editRecord.ArticleVideo;
                        model.ArticleContent = editRecord.ArticleContent;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Teacher_Article addRecord = new Teacher_Article();
                        addRecord.ClassId = model.ClassId;
                        addRecord.ArticleTitle = model.ArticleTitle;
                        addRecord.ArticleAuthor = model.ArticleAuthor;
                        addRecord.ArticleSummary = model.ArticleSummary;
                        addRecord.ArticleImages = model.ArticleImages;
                        addRecord.ArticleVideo = model.ArticleVideo;
                        addRecord.ArticleContent = model.ArticleContent;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.ArticleDate = DateTime.Now;
                        addRecord.ReadTimes = 0;
                        addRecord.PraiseCount = 0;
                        addRecord.Status = 200;
                        //
                        myOperating.Teacher_Article.Add(addRecord);
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
                        Teacher_Article editSaveRecord = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == model.ArticlId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.ClassId = model.ClassId;
                            editSaveRecord.ArticleTitle = model.ArticleTitle;
                            editSaveRecord.ArticleAuthor = model.ArticleAuthor;
                            editSaveRecord.ArticleSummary = model.ArticleSummary;
                            editSaveRecord.ArticleImages = model.ArticleImages;
                            editSaveRecord.ArticleVideo = model.ArticleVideo;
                            editSaveRecord.ArticleContent = model.ArticleContent;
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
                    model.ClassId = ClassId;//类别编号
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        if (ClassId != 0)
                        {
                            model.OrderBy = myOperating.Teacher_Article.Where(p => p.ClassId == ClassId).Max(p => p.OrderBy) + 1;
                        }
                        else
                        {
                            model.OrderBy = myOperating.Teacher_Article.Max(p => p.OrderBy) + 1;
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
                #region 教师社区类别

                List<Teacher_Article_Class> ClassTable = myOperating.Teacher_Article_Class.Where(p => p.Status == 200).ToList();
                ViewBag.DataListClass = ClassTable;
                #endregion
            }
            //
            return View(model);
        }
        #endregion
        //======================
        #region 文章类别列表
        public ActionResult TeacherCommunityClassList(int currentpage = 1, int ClassId = 0, long del = 0)
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
            ViewBag.Headline = "教师文章类别管理";//栏目主题
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
                    Teacher_Article_Class myDelRecord = myOperating.Teacher_Article_Class.FirstOrDefault(p => p.ClassId == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Teacher_Article_Class.Remove(myDelRecord);
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
                IQueryable<Teacher_Article_Class> myIQueryable = null;
                if (ClassId > 0)
                {
                    myIQueryable = myOperating.Teacher_Article_Class.Where(p => p.ClassId == ClassId);
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Article_Class;
                }
                //
                if (myIQueryable != null)
                {
                    List<Teacher_Article_Class> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
        #region 文章类别添加-编辑
        public ActionResult TeacherCommunityClassAdd(TeacherArticleClassModel model, string ReturnUrl, Nullable<int> myid = 0)
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
                    Teacher_Article_Class editRecord = myOperating.Teacher_Article_Class.FirstOrDefault(p => p.ClassId == mySelfId);
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
                        Teacher_Article_Class addRecord = new Teacher_Article_Class();
                        addRecord.ClassId = model.ClassId;
                        addRecord.ClassTitle = model.ClassTitle;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Teacher_Article_Class.Add(addRecord);
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
                        Teacher_Article_Class editSaveRecord = myOperating.Teacher_Article_Class.FirstOrDefault(p => p.ClassId == model.ClassId);
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
                {
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.ClassId = myOperating.Teacher_Article_Class.Max(p => p.ClassId) + 1;
                        model.OrderBy = myOperating.Teacher_Article_Class.Max(p => p.OrderBy) + 1;
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
        //---------------------CommentList
        #region 评论列表
        public ActionResult CommentList(int currentpage = 1,long aboutid=0, long del = 0)
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
                    Teacher_Article_Comment myDelRecord = myOperating.Teacher_Article_Comment.FirstOrDefault(p => p.CommentId == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Teacher_Article_Comment.Remove(myDelRecord);
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
                IQueryable<Teacher_Article_Comment> myIQueryable = null;
                if (aboutid > 0)
                {
                    myIQueryable = myOperating.Teacher_Article_Comment.Where(p => p.AboutId == aboutid);
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Article_Comment;
                }
                
                //
                if (myIQueryable != null)
                {
                    List<Teacher_Article_Comment> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
    #region 社区文章列表结构
    public class teacherCommunityList
    {
        public long ArticlId { get; set; }
        public string ArticleTitle { get; set; }
        public Nullable<int> CommentCount { get; set; }
        public Nullable<int> OrderBy { get; set; }

    } 
    #endregion
}
