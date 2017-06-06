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
    public class CollectController : Controller
    {
        //
        #region 添加收藏
        [HttpPost]
        public JsonResult AddCollect(string classid, string aboutid)
        {//
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
                    int myClassid = 0;//类别
                    long myAboutid = 0;//主键
                    #region 判断输入参数
                    bool classIsNumber = false;//判断输入参数类型
                    bool AboutIsNumber = false;//判断输入参数类型
                    if (!String.IsNullOrEmpty(aboutid))
                    {
                        classIsNumber = int.TryParse(classid, out myClassid);
                    }
                    //
                    if (!String.IsNullOrEmpty(aboutid))
                    {
                        AboutIsNumber = long.TryParse(aboutid, out myAboutid);
                    }
                    //
                    if ((myClassid < 1) || (myClassid > 9) || (myAboutid < 1))
                    {
                        myStatusData.operateMsg = "输入参数错误";
                        myStatusData.operateStatus = 400;
                        return Json(myStatusData);
                    }
                    #endregion
                    //
                    #region 检查是否已经收藏
                    int checkUser_Collect = myOperating.User_Collect.Count(p => p.ClassId == myClassid && p.AboutId == myAboutid && p.MobilePhone == myMobilePhone);
                    if (checkUser_Collect > 0)
                    {
                        myStatusData.operateMsg = "已收藏，不能重复收藏";
                        myStatusData.operateStatus = 400;
                        return Json(myStatusData);
                    }
                    #endregion
                    //
                    bool isTeacherVideo = false;//类别为45679的收藏为true
                    bool isExict = false;//是否存在
                    string myCoverImage = "";//图片
                    string myTitle = "";//标题
                    string mySummart = "";//摘要
                    #region 根据收藏类别取出图片和标题
                    switch (myClassid)
                    {///1点一点；2听一听；3看一看；
                        ///4课程讲解；5幼儿公开课；6教育技能；
                        ///7操作说明；8教师社区；9精彩瞬间;
                        case 1://点一点
                            #region 点一点
                            Child_Book_Click myChild_Book_Click = new Child_Book_Click();
                            myChild_Book_Click = myOperating.Child_Book_Click.FirstOrDefault(p => p.BookID == myAboutid);
                            if (myChild_Book_Click != null)
                            {
                                myTitle = myChild_Book_Click.BookName;
                                if (!String.IsNullOrWhiteSpace(myChild_Book_Click.BookOnlineUrl))
                                {
                                    myCoverImage = StaticVarClass.BookClickResourceUrl + myChild_Book_Click.BookOnlineUrl + "main.png";
                                }
                                isExict = true;
                            }
                            #endregion
                            break;
                        case 2://2听一听
                            #region 听一听
                            Child_Audio_List myChild_Audio_List = new Child_Audio_List();
                            myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == myAboutid);
                            if (myChild_Audio_List != null)
                            {
                                myTitle = myChild_Audio_List.AudioTitle;
                                int AlibumId = myChild_Audio_List.AlbumId;
                                string AudioBgImg = "";
                                try
                                {
                                    AudioBgImg = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == AlibumId).AlbumImage;
                                    if (!String.IsNullOrWhiteSpace(AudioBgImg))
                                    {
                                        myCoverImage = StaticVarClass.myDomain + AudioBgImg;
                                    }
                                }
                                catch { }
                                
                                isExict = true;
                            }
                            #endregion
                            break;
                        case 3://3看一看
                            #region 看一看
                            Child_Video_List myChild_Video_List = new Child_Video_List();
                            myChild_Video_List = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == myAboutid);
                            if (myChild_Video_List != null)
                            {
                                myTitle = myChild_Video_List.VideoTitle;
                                myCoverImage = StaticVarClass.myDomain + myChild_Video_List.VideoImage;
                                isExict = true;
                            }
                            #endregion
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7://4课程讲解；5幼儿公开课；6教育技能；7操作说明
                            #region 教师社区
                            Teacher_Video_List myTeacher_Video_List = new Teacher_Video_List();
                            myTeacher_Video_List = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == myAboutid);
                            if (myTeacher_Video_List != null)
                            {
                                myTitle = myTeacher_Video_List.VideoTitle;
                                myCoverImage = StaticVarClass.myDomain + myTeacher_Video_List.VideoImage;
                                isExict = true;
                                isTeacherVideo = true;
                            }
                            #endregion
                            break;
                        case 8://8教师社区
                            #region 教师社区
                            Teacher_Article myTeacher_Article = new Teacher_Article();
                            myTeacher_Article = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == myAboutid);
                            if (myTeacher_Article != null)
                            {
                                myTitle = myTeacher_Article.ArticleTitle;
                                if(!String.IsNullOrWhiteSpace(myTeacher_Article.ArticleImages))
                                {
                                    myCoverImage = StaticVarClass.myDomain + myTeacher_Article.ArticleImages;
                                }
                                
                                if ((myTeacher_Article.ArticleSummary != null) && (myTeacher_Article.ArticleSummary.Length > 50))
                                {
                                    mySummart = myTeacher_Article.ArticleSummary.Substring(0, 50);
                                }
                                else
                                {
                                    mySummart = myTeacher_Article.ArticleSummary;
                                }
                                isExict = true;
                            }
                            #endregion
                            break;
                        case 9://9精彩瞬间
                            #region 精彩瞬间
                            Discover_Article myDiscover_Article = new Discover_Article();
                            myDiscover_Article = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == myAboutid);
                            if (myDiscover_Article != null)
                            {
                                myTitle = myDiscover_Article.ArticleTitle;
                                if (!String.IsNullOrWhiteSpace(myDiscover_Article.ArticleImages))
                                {//取出第一幅图
                                    string[] imageOne = myDiscover_Article.ArticleImages.Split(';');
                                    if (imageOne.Count() > 0)
                                    {
                                        myCoverImage = StaticVarClass.myDomain + imageOne[0];
                                    }
                                }
                                if ((myDiscover_Article.ArticleContent != null) && (myDiscover_Article.ArticleContent.Length > 50))
                                {
                                    mySummart = myDiscover_Article.ArticleContent.Substring(0, 50);
                                }
                                else
                                {
                                    mySummart = myDiscover_Article.ArticleContent;
                                }
                                isExict = true;
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    //
                    #region 添加到数据库
                    if (isExict)
                    {
                        try
                        {
                            User_Collect myUser_Collect = new User_Collect();
                            //
                            myUser_Collect.ClassId = myClassid;
                            myUser_Collect.AboutId = myAboutid;
                            myUser_Collect.AboutTitle = myTitle;
                            myUser_Collect.AboutImage = myCoverImage;
                            myUser_Collect.AboutSummary = mySummart;
                            myUser_Collect.MobilePhone = myMobilePhone;
                            myUser_Collect.ClassId = myClassid;//收藏类别
                            myUser_Collect.CollectDate = DateTime.Now;
                            myUser_Collect.IsTeacherVideo = isTeacherVideo;
                            myUser_Collect.OrderBy = 10000;
                            //
                            myOperating.User_Collect.Add(myUser_Collect);
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
                        myStatusData.operateMsg = "不存在";
                        myStatusData.operateStatus = 400;
                    }
                    #endregion
                }
            }
            else
            {
                myStatusData.operateStatus = 5;//登陆失效
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 删除用户收藏
        [HttpPost]
        public JsonResult DelCollectItem(string collectid)
        {
            #region 检查授权
            string myMobilePhone = "";
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
                try
                {
                    using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                    {
                        long myLong = 0;//
                        bool isLong = long.TryParse(collectid, out myLong);
                        User_Collect myUser_Collect = new User_Collect();
                        myUser_Collect = myOperating.User_Collect.FirstOrDefault(p =>p.MobilePhone==myMobilePhone && p.CollectId == myLong); ;
                        //
                        if (myUser_Collect != null)
                        {
                            myOperating.User_Collect.Remove(myUser_Collect);
                            //
                            myOperating.SaveChanges();
                            //
                            myStatusData.operateStatus = 200;
                        }
                    }
                }
                catch
                {
                    myStatusData.operateStatus = -1;
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
        #region 获取收藏列表
        [HttpPost]
        public JsonResult GetCollectList(string classid, string currentpage, string pagesize)
        {
            ///1点一点；2听一听；3看一看；
            ///4课程讲解；5幼儿公开课；6教育技能；
            ///7操作说明；8教师社区；9精彩瞬间;
            #region 检查授权
            string myMobilePhone = "";
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
                    #region 翻页属性
                    int recordCount = 0;
                    int pageCount = 0;
                    string orderbyfiled = "CollectDate";
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
                    int myInt = 0;
                    #endregion
                    if (!String.IsNullOrWhiteSpace(classid))
                    {
                        bool isInt = int.TryParse(classid, out myInt);
                    }
                    //
                    IQueryable<User_Collect> myIQueryable = null;
                    if (myInt > 0)
                    {
                        myIQueryable = myOperating.User_Collect.Where(p => p.MobilePhone == myMobilePhone && p.ClassId == myInt);
                    }
                    else
                    {
                        myIQueryable = myOperating.User_Collect.Where(p => p.MobilePhone == myMobilePhone);
                    }
                    //
                    if (myIQueryable != null)
                    {
                        List<collectData> collectDataList = new List<collectData>();
                        var CollcetTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { CollectId = s.CollectId, ClassId = s.ClassId, AboutId = s.AboutId, AboutTitle = s.AboutTitle, AboutImage = s.AboutImage, AboutSummary = s.AboutSummary, CollectDate = s.CollectDate }).ToList();
                        for (int i = 0; i < CollcetTable.Count;i++ )
                        {
                            collectData collectDataItem = new collectData();
                            collectDataItem.CollectId = CollcetTable[i].CollectId;
                            collectDataItem.ClassId = CollcetTable[i].ClassId;
                            long aboutId = CollcetTable[i].AboutId;
                            #region 听一听返回时长和播放次数
                            if (collectDataItem.ClassId == 2)
                            { //听一听返回时长和播放次数

                                Child_Audio_List myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == aboutId);
                                if (myChild_Audio_List != null)
                                {
                                    collectDataItem.TimeSeconds = myChild_Audio_List.TimeSeconds;
                                    collectDataItem.PlayTimes = myChild_Audio_List.PlayTimes;
                                }
                            } 
                            #endregion
                            collectDataItem.AboutId = aboutId;
                            collectDataItem.AboutTitle = CollcetTable[i].AboutTitle;
                            collectDataItem.AboutImage = CollcetTable[i].AboutImage;
                            collectDataItem.AboutSummary = CollcetTable[i].AboutSummary;
                            collectDataItem.CollectDate = CollcetTable[i].CollectDate;
                            collectDataList.Add(collectDataItem);
                        }
                        //
                        myStatusData.dataPageCount = pageCount;
                        myStatusData.dataRecordCount = recordCount;
                        myStatusData.dataTable = collectDataList;
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
    }
    #region 收藏数据结构
    public class collectData
    {//
        private Nullable<int> _TimeSeconds = 0;
        private Nullable<int> _PlayTimes = 0;
        public Nullable<long> CollectId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<long> AboutId { get; set; }
        public string AboutTitle { get; set; }
        public string AboutImage { get; set; }
        public string AboutSummary { get; set; }
        public Nullable<int> TimeSeconds { get { return _TimeSeconds; } set { _TimeSeconds = value; } }
        public Nullable<int> PlayTimes { get { return _PlayTimes; } set { _PlayTimes = value; } }
        public Nullable<DateTime> CollectDate { get; set; }
    } 
    #endregion
}
