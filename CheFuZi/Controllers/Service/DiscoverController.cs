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
    public class DiscoverController : Controller
    {
        //
        #region 获取发现栏目图文列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ArticleList(string currentpage, string pagesize)
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
                string orderbyfiled = "ArticlId";
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
                IQueryable<Discover_Article> myIQueryable = null;
                myIQueryable = myOperating.Discover_Article.Where(p => p.Status == 200);
                //
                if (myIQueryable != null)
                {
                    List<DiscoverDataItem> DiscoverDataItemList = new List<DiscoverDataItem>();
                    var EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { ArticlId = s.ArticlId, ArticleTitle = s.ArticleTitle, ArticleImages = s.ArticleImages, AddDate = s.AddDate, ReadTimes = s.ReadTimes, MobilePhone = s.MobilePhone }).ToList();
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
                            int myCommentTimes = 0;//评论次数
                            long myArticlId = 0;//文章主键
                            myArticlId = EfDataTable[i].ArticlId;
                            DiscoverDataItem myDiscoverDataItem = new DiscoverDataItem();
                            myDiscoverDataItem.ArticleId = myArticlId;
                            myDiscoverDataItem.ArticleTitle = EfDataTable[i].ArticleTitle;
                            myDiscoverDataItem.ReadTimes = EfDataTable[i].ReadTimes;
                            myDiscoverDataItem.AddDate = EfDataTable[i].AddDate;
                            //
                            myDiscoverDataItem.NickName = myNickName;
                            myDiscoverDataItem.HeadImage = myHeadImage;
                            //
                            myCommentTimes = myOperating.Discover_Article_Comment.Count(p => p.AboutId == myArticlId);
                            //
                            myDiscoverDataItem.CommentTimes = myCommentTimes;
                            //
                            DiscoverDataItemList.Add(myDiscoverDataItem);
                        }
                    }
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = DiscoverDataItemList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取发现栏目私人图文列表
        [HttpPost]
        public JsonResult PersonalList(string currentpage, string pagesize)
        {
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
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
                    #region 翻页属性
                    int recordCount = 0;
                    int pageCount = 0;
                    string orderbyfiled = "ArticlId";
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
                    IQueryable<Discover_Article> myIQueryable = null;
                    myIQueryable = myOperating.Discover_Article.Where(p => p.MobilePhone == mobilePhone);
                    //
                    if (myIQueryable != null)
                    {
                        List<DiscoverPersonDataItem> DiscoverDataItemList = new List<DiscoverPersonDataItem>();
                        IQueryable<Discover_Article> EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc);
                        foreach (Discover_Article item in EfDataTable)
                        {
                            DiscoverPersonDataItem dataItem = new DiscoverPersonDataItem();
                            dataItem.ArticleId = item.ArticlId;
                            dataItem.ArticleTitle = item.ArticleTitle;
                            dataItem.ArticleImages = FunctionClass.GetFileUrl(item.ArticleImages);
                            dataItem.AddDate = item.AddDate;
                            dataItem.ReadTimes = item.ReadTimes;
                            DiscoverDataItemList.Add(dataItem);
                        }
                        myStatusData.dataPageCount = pageCount;
                        myStatusData.dataRecordCount = recordCount;
                        myStatusData.dataTable = DiscoverDataItemList;
                    }
                    myStatusData.operateStatus = 200;
                }
            }
            else
            {
                myStatusData.operateStatus = 5;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 删除私人文章
        [HttpPost]
        public JsonResult DeleteItem(string myid, string del)
        {
            #region 检查授权
            string myMobilePhone = "";//手机号
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            if (checkAuthCodeBool)
            {
                myMobilePhone = myAuthCodeInstance.mobilePhone;

                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    int operatType = 0;//1删除全部，2删除指定
                    long idLong = 0;
                    #region 判断参数
                    if (!String.IsNullOrWhiteSpace(del))
                    {
                        switch (del.ToLower())
                        {
                            case "all":
                                operatType = 1;
                                break;
                            case "single":
                                if (!String.IsNullOrWhiteSpace(myid))
                                {
                                    bool isLong = long.TryParse(myid, out idLong);
                                    if (isLong) operatType = 2;
                                }
                                break;
                        }
                    }
                    #endregion
                    //
                    if (operatType == 1)
                    {
                        IQueryable<Discover_Article> myList = myOperating.Discover_Article.Where(p => p.MobilePhone == myMobilePhone);
                        if (myList != null)
                        {
                            foreach (Discover_Article recordItem in myList)
                            {
                                FunctionClass.DeleteFile(recordItem.ArticleImages);
                                FunctionClass.DeleteFile(recordItem.ArticleVideo);
                                myOperating.Discover_Article.Remove(recordItem);
                            }
                            myOperating.SaveChanges();
                        }
                        myStatusData.operateStatus = 200;
                    }
                    else if (operatType == 2)
                    {
                        Discover_Article myDataRecord = new Discover_Article();
                        myDataRecord = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == idLong && p.MobilePhone == myMobilePhone);
                        if (myDataRecord != null)
                        {
                            FunctionClass.DeleteFile(myDataRecord.ArticleImages);
                            FunctionClass.DeleteFile(myDataRecord.ArticleVideo);
                            myOperating.Discover_Article.Remove(myDataRecord);
                            myOperating.SaveChanges();
                            myStatusData.operateStatus = 200;
                        }

                    }
                    else
                    {
                        myStatusData.operateStatus = 0;
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
        //
        #region 获取发现图文内容
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
                DiscoverDataContent myDiscoverDataContent = new DiscoverDataContent();
                long myLong = 0;//文章主键
                bool isLong = long.TryParse(articleid, out myLong);
                if (isLong)
                {
                    Discover_Article myTable = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == myLong && p.Status == 200);
                    if (myTable != null)
                    {
                        #region 昵称和头像
                        string dataMobilePhone = myTable.MobilePhone;
                        DataMethodClass myDataMethodClass = new DataMethodClass();
                        string myHeadImage = StaticVarClass.DefaultHeadImage;//头像
                        string myNickName = "匿名";//昵称
                        myDataMethodClass.GetNickNameAndPic(dataMobilePhone, out myNickName, out myHeadImage);
                        #endregion
                        int myCommentTimes = 0;//评论次数
                        myDiscoverDataContent.ArticleId = myTable.ArticlId;
                        myDiscoverDataContent.ArticleTitle = myTable.ArticleTitle;
                        myDiscoverDataContent.ArticleImages = FunctionClass.GetFileUrl(myTable.ArticleImages);
                        myDiscoverDataContent.ArticleVideo = FunctionClass.GetFileUrl(myTable.ArticleVideo);
                        myDiscoverDataContent.ArticleContent = myTable.ArticleContent;
                        myDiscoverDataContent.AddDate = myTable.AddDate;
                        myDiscoverDataContent.ReadTimes = myTable.ReadTimes;
                        myCommentTimes = myOperating.Discover_Article_Comment.Count(p => p.AboutId == myLong);
                        //
                        myDiscoverDataContent.CommentTimes = myCommentTimes;
                        myDiscoverDataContent.HeadImage = myHeadImage;
                        myDiscoverDataContent.NickName = myNickName;
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
                    myStatusData.dataTable = myDiscoverDataContent;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 添加发现图文内容
        [HttpPost]
        public JsonResult AddArticleContent(string articletitle, string content, string images, string videos)
        {
            //
            string mobilePhone = "";//
            //
            StatusData myStatusData = new StatusData();//返回状态
            #region 判断条件
            if (String.IsNullOrWhiteSpace(articletitle.Trim()))
            {
                myStatusData.operateStatus = 400;
                myStatusData.operateMsg = "标题不能为空";
                return Json(myStatusData);
            }
            else if (articletitle.Trim().Length > 50)
            {
                myStatusData.operateStatus = 400;
                myStatusData.operateMsg = "标题不能多于50个字符";
                return Json(myStatusData);
            }
            if (String.IsNullOrWhiteSpace(content.Trim()))
            {
                myStatusData.operateStatus = 400;
                myStatusData.operateMsg = "内容不能为空";
                return Json(myStatusData);
            }
            else if (content.Trim().Length < 2)
            {
                myStatusData.operateStatus = 400;
                myStatusData.operateMsg = "内容太少";
                return Json(myStatusData);
            }
            #endregion
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
                    try
                    {
                        Discover_Article myDiscover_Article = new Discover_Article();
                        myDiscover_Article.MobilePhone = mobilePhone;
                        myDiscover_Article.ArticleTitle = articletitle;
                        myDiscover_Article.ArticleContent = content;
                        myDiscover_Article.ArticleImages = images;
                        myDiscover_Article.ArticleVideo = videos;
                        myDiscover_Article.AddDate = DateTime.Now;
                        myDiscover_Article.OrderBy = 10000;
                        myDiscover_Article.PraiseCount = 0;
                        myDiscover_Article.ReadTimes = 0;
                        myDiscover_Article.Status = 200;
                        myOperating.Discover_Article.Add(myDiscover_Article);
                        myOperating.SaveChanges();
                        myStatusData.operateStatus = 200;
                    }
                    catch
                    {
                        myStatusData.operateStatus = -1;
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
                IQueryable<Discover_Article_Comment> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(articleid))
                {
                    long myLong = 0;
                    bool isOk = long.TryParse(articleid, out myLong);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Discover_Article_Comment.Where(p => p.AboutId == myLong && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Discover_Article_Comment.Where(p => p.Status == 200);
                }
                //
                if (myIQueryable != null)
                {
                    List<DisCommentData> myCommentDataList = new List<DisCommentData>();
                    var EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { CommentId = s.CommentId, Detail = s.Detail, AddDate = s.AddDate, MobilePhone = s.MobilePhone }).ToList();
                    //
                    if (EfDataTable != null)
                    {
                        int PageFloor = (sqlCurrentpage - 1) * sqlPagesize;//楼层计基数
                        int myFloor = 0;//楼层
                        for (int i = 0; i < EfDataTable.Count(); i++)
                        {
                            myFloor = PageFloor + i + 1;//楼层
                            #region 昵称和头像
                            string dataMobilePhone = EfDataTable[i].MobilePhone;
                            DataMethodClass myDataMethodClass = new DataMethodClass();
                            string myHeadImage = StaticVarClass.DefaultHeadImage;//头像
                            string myNickName = "匿名";//昵称
                            myDataMethodClass.GetNickNameAndPic(dataMobilePhone, out myNickName, out myHeadImage);
                            #endregion
                            DisCommentData myCommentData = new DisCommentData();
                            myCommentData.FloorNum = i;
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
                //
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    Discover_Article_Comment myRecord = new Discover_Article_Comment();

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
                            myOperating.Discover_Article_Comment.Add(myRecord);
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
    #region 发现栏目内容列表数据结构
    public class DiscoverDataItem
    {
        public long ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    #region 私人-发现栏目内容列表数据结构
    public class DiscoverPersonDataItem
    {
        public long ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImages { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    #region 发现栏目文章内容数据结构
    public class DiscoverDataContent
    {
        public long ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleImages { get; set; }
        public string ArticleVideo { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public string ArticleContent { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    #region 评论内容数据结构
    public class DisCommentData
    {
        public int FloorNum { get; set; }
        public long CommentId { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public string Detail { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    #region 微讯内容数据结构
    public class WeiXunData
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public string HttpUrl { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
}
