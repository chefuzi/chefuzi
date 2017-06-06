using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;

namespace CheFuZi.Controllers.Service
{
    public class TeacherVideoController : Controller
    {
        //
        #region 获取大类
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Teacher_Video_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { ClassId = s.ClassId, ClassTitle = s.ClassTitle }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取课程类别
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult LessonList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Teacher_Video_Class_Lesson.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { ClassId = s.ClassId, ClassTitle = s.ClassTitle }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取视频列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult VideoList(string classid, string lessonid, string currentpage, string pagesize, string orderby, string isasc)
        {//orderby=1按日期排序，2按播放次数排序
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "Recommended";
                string orderbyfiled2 = "OrderBy";
                bool isDesc = true;//倒序
                //
                #region 指定排序规则
                if (String.IsNullOrWhiteSpace(orderby))
                {
                    orderby = "";
                }
                switch (orderby)
                {
                    case "1":
                        orderbyfiled = "AddDate";
                        break;
                    case "2":
                        orderbyfiled = "PlayTimes";
                        break;
                    case "3":
                        orderbyfiled = "OrderBy";
                        break;
                }
                if (String.IsNullOrWhiteSpace(isasc))
                {
                    isasc = "";
                }
                switch (isasc)
                {
                    case "1":
                        isDesc = false;//正序
                        break;
                    case "2":
                        isDesc = true;//倒序
                        break;
                }
                #endregion
                //当前页
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                if (!String.IsNullOrWhiteSpace(pagesize))
                {
                    bool isOk = int.TryParse(pagesize, out sqlPagesize);
                    if (!isOk) sqlPagesize = 10;
                }
                #endregion
                IQueryable<Teacher_Video_List> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(classid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(classid, out myInt);
                    if ((isOk) && (myInt > 0))
                    {
                        //lessonid
                        if (!String.IsNullOrWhiteSpace(lessonid))
                        {//课程类别
                            int myLessonid = 0;
                            isOk = int.TryParse(lessonid, out myLessonid);
                            if ((isOk) && (myLessonid > 0))
                            {
                                myIQueryable = myOperating.Teacher_Video_List.Where(p => p.ClassId == myInt && p.LessonId == myLessonid && p.Status == 200);
                            }
                            else
                            {
                                myIQueryable = myOperating.Teacher_Video_List.Where(p => p.ClassId == myInt && p.Status == 200);
                            }
                        }
                        else
                        {
                            myIQueryable = myOperating.Teacher_Video_List.Where(p => p.ClassId == myInt && p.Status == 200);
                        }
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Video_List.Where(p => p.Status == 200 && p.Recommended>0);
                }
                //
                if (myIQueryable != null)
                {
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc, orderbyfiled2).Select(s => new { VideoId = s.VideoId, VideoTitle = s.VideoTitle, VideoDes = s.VideoDes, VideoImage = StaticVarClass.myDomain + s.VideoImage, VideoUrl = StaticVarClass.myDomain + s.VideoUrl, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds, ClassId = s.ClassId }).ToList();

                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = BookTable;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取视频内容
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetVideo(string myid)
        {//orderby=1按日期排序，2按播放次数排序
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if (!String.IsNullOrWhiteSpace(myid))
                {
                    long myLong = 0;
                    bool isOk = long.TryParse(myid, out myLong);
                    if (isOk)
                    {
                        var myTeacher_Video_List = myOperating.Teacher_Video_List.Where(p => p.VideoId == myLong && p.Status == 200).Select(s => new { VideoId = s.VideoId, VideoTitle = s.VideoTitle, VideoDes = s.VideoDes, VideoImage = StaticVarClass.myDomain + s.VideoImage, VideoUrl = StaticVarClass.myDomain + s.VideoUrl, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds, ClassId = s.ClassId }).ToList();
                        if (myTeacher_Video_List != null)
                            myStatusData.dataTable = myTeacher_Video_List;
                        myStatusData.operateStatus = 200;
                    }
                    else
                    {
                        myStatusData.operateStatus = 400;
                    }
                }
                else
                {
                    myStatusData.operateStatus = 400;
                }
            }
            return Json(myStatusData);
        }
        #endregion
        //====================================
        #region 获取教师社区图文大类
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ArticleClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Teacher_Article_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { ClassId = s.ClassId, ClassTitle = s.ClassTitle }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取教师社区图文列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ArticleList(string classid, string currentpage, string pagesize)
        {
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AddDate";
                bool isDesc = true;//倒序
                //当前页
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                if (!String.IsNullOrWhiteSpace(pagesize))
                {
                    bool isOk = int.TryParse(pagesize, out sqlPagesize);
                    if (!isOk) sqlPagesize = 10;
                }
                #endregion
                IQueryable<Teacher_Article> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(classid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(classid, out myInt);
                    if ((isOk) && (myInt > 0))
                    {
                        myIQueryable = myOperating.Teacher_Article.Where(p => p.ClassId == myInt && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = myOperating.Teacher_Article.Where(p => p.Status == 200);
                    }
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Article.Where(p => p.Status == 200);
                }
                //
                if (myIQueryable != null)
                {
                    List<ArticlDataList> myArticlDataList = new List<ArticlDataList>();
                    var EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { ArticlId = s.ArticlId, ArticleTitle = s.ArticleTitle, ArticleImages = s.ArticleImages, ReadTimes = s.ReadTimes }).ToList();
                    if (EfDataTable != null)
                    {
                        long ArticlId = 0;
                        for (int i = 0; i < EfDataTable.Count(); i++)
                        {
                            ArticlDataList ArticlDataListItem = new ArticlDataList();
                            ArticlId = EfDataTable[i].ArticlId;
                            ArticlDataListItem.ArticleId = EfDataTable[i].ArticlId;
                            ArticlDataListItem.ArticleTitle = EfDataTable[i].ArticleTitle;
                            if (!String.IsNullOrWhiteSpace(EfDataTable[i].ArticleImages))
                            {
                                ArticlDataListItem.ArticleImages = StaticVarClass.myDomain + EfDataTable[i].ArticleImages;
                            }
                            else
                            {
                                ArticlDataListItem.ArticleImages = StaticVarClass.myDomain + "defaultTeacherArticle.png";
                            }

                            ArticlDataListItem.OpenUrl = StaticVarClass.myDomain + "Community/" + EfDataTable[i].ArticlId + ".html";
                            ArticlDataListItem.ReadTimes = EfDataTable[i].ReadTimes;
                            ArticlDataListItem.CommentCount = myOperating.Teacher_Article_Comment.Count(p => p.AboutId == ArticlId);
                            myArticlDataList.Add(ArticlDataListItem);
                        }
                    }

                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = myArticlDataList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取教师社区图文内容
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ArticleContent(string articleid)
        {
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                ArticlDataContent myArticlDataContent = new ArticlDataContent();
                long myLong = 0;//文章主键
                bool isLong = long.TryParse(articleid, out myLong);
                if (isLong)
                {
                    Teacher_Article myTable = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == myLong && p.Status == 200);
                    DataMethodClass myDataMethodClass = new DataMethodClass();
                    if (myTable != null)
                    {
                        myArticlDataContent.ArticleId = myLong;
                        myArticlDataContent.ArticleTitle = myTable.ArticleTitle;
                        if (!String.IsNullOrWhiteSpace(myTable.ArticleImages))
                        {
                            myArticlDataContent.ArticleImages = StaticVarClass.myDomain + myTable.ArticleImages;
                        }
                        else
                        {
                            myArticlDataContent.ArticleImages = StaticVarClass.myDomain + "defaultTeacherArticle.png";
                        }
                        myArticlDataContent.AddDate = myTable.AddDate;
                        myArticlDataContent.ReadTimes = myTable.ReadTimes;
                        myArticlDataContent.OpenUrl = StaticVarClass.myDomain + "Community/" + myLong + ".html";
                        //
                        #region 阅读次数加一
                        if (myTable.ReadTimes != null)
                        {
                            myTable.ReadTimes = myTable.ReadTimes + 1;
                        }
                        else
                        {
                            myTable.ReadTimes = 1;
                        }
                        myOperating.SaveChanges();
                        #endregion
                    }
                    myStatusData.dataTable = myArticlDataContent;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //====================================
        #region 获取评论列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult CommentList(string articleid, string currentpage, string pagesize)
        {
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
            }
            #endregion
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AddDate";
                bool isDesc = true;//倒序
                //当前页
                int sqlCurrentpage = 1;
                if (!String.IsNullOrWhiteSpace(currentpage))
                {
                    bool isOk = int.TryParse(currentpage, out sqlCurrentpage);
                    if (!isOk) sqlCurrentpage = 1;
                }
                //页大小
                int sqlPagesize = 10;
                if (!String.IsNullOrWhiteSpace(pagesize))
                {
                    bool isOk = int.TryParse(pagesize, out sqlPagesize);
                    if (!isOk) sqlPagesize = 10;
                }
                #endregion
                IQueryable<Teacher_Article_Comment> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(articleid))
                {
                    long myLong = 0;
                    bool isOk = long.TryParse(articleid, out myLong);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Teacher_Article_Comment.Where(p => p.AboutId == myLong && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Teacher_Article_Comment.Where(p => p.Status == 200);
                }
                //
                if (myIQueryable != null)
                {
                    List<CommentData> myCommentDataList = new List<CommentData>();
                    var EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { CommentId = s.CommentId, Detail = s.Detail, AddDate = s.AddDate, MobilePhone = s.MobilePhone }).ToList();
                    //
                    if (EfDataTable != null)
                    {
                        for (int i = 0; i < EfDataTable.Count(); i++)
                        {
                            #region 昵称和头像
                            string dataMobilePhone = EfDataTable[i].MobilePhone;
                            DataMethodClass myDataMethodClass = new DataMethodClass();
                            string myHeadImage = StaticVarClass.DefaultHeadImage;//头像
                            string myNickName = "匿名";//昵称
                            myDataMethodClass.GetNickNameAndPic(dataMobilePhone, out myNickName, out myHeadImage);
                            #endregion
                            CommentData myCommentData = new CommentData();
                            myCommentData.CommentId = EfDataTable[i].CommentId;
                            myCommentData.Detail = EfDataTable[i].Detail;
                            myCommentData.AddDate = EfDataTable[i].AddDate;
                            myCommentData.NickName = myNickName;
                            myCommentData.HeadImage = myHeadImage;
                            myCommentDataList.Add(myCommentData);
                        }
                    }
                    //
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = myCommentDataList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 添加评论
        [HttpPost]
        public JsonResult AddComment(string articleid, string content)
        {
            StatusData myStatusData = new StatusData();//返回的类型
            string mobilePhone = "";//
            //
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    Teacher_Article_Comment myRecord = new Teacher_Article_Comment();

                    long myLong = 0;//要评论的文章
                    bool isOk = long.TryParse(articleid, out myLong);
                    if (isOk)
                    {
                        try
                        {
                            myRecord.AboutId = myLong;
                            myRecord.Detail = content;
                            myRecord.MobilePhone = mobilePhone;
                            myRecord.NickName = "";
                            myRecord.AddDate = DateTime.Now;
                            myRecord.Status = 200;
                            myOperating.Teacher_Article_Comment.Add(myRecord);
                            myOperating.SaveChanges();
                            myStatusData.operateStatus = 200;
                        }
                        catch
                        {
                            myStatusData.operateStatus = -1;
                        }
                    }
                    else
                    {
                        myStatusData.operateStatus = 400;
                        myStatusData.operateMsg = "参数错误";
                    }
                }
            }
            else
            {
                myStatusData.operateStatus = 5;
            }
            return Json(myStatusData);
        }
        #endregion
    }
    //
    #region 视频内容结构
    public class TeacherVideoRecord
    {//
        public long VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoUrl { get; set; }
        public string VideoImage { get; set; }
        public string VideoDes { get; set; }
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> PlayTimes { get; set; }
    }
    #endregion
    //
    #region 教师社区文章内容数据结构
    public class ArticlDataContent
    {
        public long ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImages { get; set; }
        public string OpenUrl { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    //
    #region 教师社区文章内容数据结构
    public class ArticlDataList
    {
        public long ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImages { get; set; }
        public string OpenUrl { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentCount { get; set; }
    }
    #endregion
    //
    #region 评论内容数据结构
    public class CommentData
    {
        public long CommentId { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public string Detail { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
}
