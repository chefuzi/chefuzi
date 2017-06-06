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
using CheFuZi.Models;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;

namespace CheFuZi.Controllers
{
    public class CommunityController : Controller
    {

        #region 文章内容
        [OutputCache(CacheProfile = "commoncache")]
        public ActionResult Details(long myid)
        {
            TeacherArticleModel myArticlDataContent = new TeacherArticleModel();
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                Teacher_Article myTable = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == myid && p.Status == 200);
                DataMethodClass myDataMethodClass = new DataMethodClass();
                if (myTable != null)
                {
                    myArticlDataContent.ArticlId = myid;
                    myArticlDataContent.ArticleTitle = myTable.ArticleTitle;
                    if (!String.IsNullOrWhiteSpace(myTable.ArticleImages))
                    {
                        myArticlDataContent.ArticleImages = StaticVarClass.myDomain + myTable.ArticleImages;
                    }
                    if (!String.IsNullOrWhiteSpace(myTable.ArticleVideo))
                    {
                        myArticlDataContent.ArticleVideo = StaticVarClass.myDomain + myTable.ArticleVideo;
                    }
                    DateTime myDateTime = DateTime.Now;
                    DateTime.TryParse(myTable.AddDate.ToString(), out myDateTime);
                    myArticlDataContent.AddDate = myDateTime;
                    myArticlDataContent.ArticleAuthor = myTable.ArticleAuthor;
                    myArticlDataContent.ArticleContent = myTable.ArticleContent;
                    myArticlDataContent.CommentCount = myOperating.Teacher_Article_Comment.Count(p => p.AboutId == myid);
                    myArticlDataContent.ReadTimes = myTable.ReadTimes;
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
            }
            return View(myArticlDataContent);
        }
        #endregion
        //
        #region 加载评论
        public JsonResult CommentList(string articleid, string currentpage)
        {
            List<CommentData> myCommentDataList = new List<CommentData>();
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AddDate";
                bool isDesc = false;//倒序
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

                    var EfDataTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { CommentId = s.CommentId, Detail = s.Detail, AddDate = s.AddDate, MobilePhone = s.MobilePhone }).ToList();
                    //
                    int PageFloor = (sqlCurrentpage - 1) * sqlPagesize;//楼层计基数
                    int myFloor = 0;//楼层
                    if (EfDataTable != null)
                    {
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
                            CommentData myCommentData = new CommentData();
                            myCommentData.CommentId = EfDataTable[i].CommentId;
                            myCommentData.Detail = EfDataTable[i].Detail;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(EfDataTable[i].AddDate.ToString(), out myDateTime);
                            myCommentData.DateDate = myDateTime.ToString("yyyy-MM-dd");
                            myCommentData.TimeDate = myDateTime.ToString("HH-mm");
                            myCommentData.NickName = myNickName;
                            myCommentData.HeadImage = myHeadImage;
                            myCommentData.FloorNum = myFloor;
                            myCommentData.PageCount = pageCount;
                            myCommentDataList.Add(myCommentData);
                        }
                    }
                }
            }
            return Json(myCommentDataList);
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
            return Json(myStatusData.operateMsg);
        }
        #endregion
    }
    #region 评论内容数据结构
    public class CommentData
    {
        public long CommentId { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public string Detail { get; set; }
        public string DateDate { get; set; }
        public string TimeDate { get; set; }
        public int FloorNum { get; set; }
        public int PageCount { get; set; }
    }
    #endregion
}
