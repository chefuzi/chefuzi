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
using CheFuZi.DataOption;

namespace CheFuZi.Controllers.Service
{
    public class ClickController : Controller
    {
        //
        #region 获取点一点图书类别
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult BookClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Child_Book_Click_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { BookClassID = s.BookClassID, BookClassName = s.BookClassName, ClassImage = StaticVarClass.BookClickResourceUrl + s.ClassImage, ImgWidth = s.ImgWidth, ImgHeight = s.ImgHeight }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获得点一点电子书列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult BookList(string bookclassid, string currentpage, string pagesize)
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
                if (!String.IsNullOrWhiteSpace(pagesize))
                {
                    bool isOk = int.TryParse(pagesize, out sqlPagesize);
                    if (!isOk) sqlPagesize = 10;
                }
                #endregion
                IQueryable<Child_Book_Click> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(bookclassid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(bookclassid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Book_Click.Where(p => p.BookClassID == myInt && p.BookStatus == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Child_Book_Click.Where(p => p.BookStatus == 200);
                }
                //
                if (myIQueryable != null)
                {
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, false).Select(s => new { BookID = s.BookID, BookName = s.BookName, BookOnlineUrl = StaticVarClass.BookClickResourceUrl + s.BookOnlineUrl, ScreenH = s.ScreenH }).ToList();
                    //
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
        #region 获得点一点内容
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetBook(string myid)
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
                if (!String.IsNullOrWhiteSpace(myid))
                {
                    long myLong = 0;
                    bool isOk = long.TryParse(myid, out myLong);
                    if (isOk)
                    {
                        var myChild_Book_Click = myOperating.Child_Book_Click.Where(p => p.BookID == myLong && p.BookStatus == 200).Select(s => new { BookID = s.BookID, BookName = s.BookName, BookOnlineUrl = StaticVarClass.BookClickResourceUrl + s.BookOnlineUrl, ScreenH = s.ScreenH }).ToList();
                        if (myChild_Book_Click != null)
                        {
                            myStatusData.dataTable = myChild_Book_Click;
                        }
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
        //
        #region 点一点阅读天数加一
        [HttpPost]
        public JsonResult ReadBookDays(string bookid)
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
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                long myLong = 0;
                bool isLong = long.TryParse(bookid, out myLong);
                if (isLong)
                {
                    #region 图书阅读数加一
                    Child_Book_Click myChild_Book_Click = new Child_Book_Click();
                    myChild_Book_Click = myOperating.Child_Book_Click.FirstOrDefault(p => p.BookID == myLong);
                    if (myChild_Book_Click != null)
                    {
                        myChild_Book_Click.ReadCount = myChild_Book_Click.ReadCount + 1;
                    }
                    #endregion
                    //
                    #region 用户阅读天数
                    User_Book_Click_ReadDay myUser_Book_Click_ReadDay = new User_Book_Click_ReadDay();
                    if (checkAuthCodeBool)
                    {
                        DateTime myNowDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                        mobilePhone = myAuthCodeInstance.mobilePhone;
                        //
                        myUser_Book_Click_ReadDay = myOperating.User_Book_Click_ReadDay.FirstOrDefault(p => p.MobilePhone == mobilePhone);
                        if (myUser_Book_Click_ReadDay != null)
                        {
                            if (myUser_Book_Click_ReadDay.UpdateDate == myNowDate.AddDays(-1))
                            {//昨天阅读过，连续
                                myUser_Book_Click_ReadDay.ReadDayCount = myUser_Book_Click_ReadDay.ReadDayCount + 1;
                                myUser_Book_Click_ReadDay.RecentlyReadBookId = myLong;
                                myUser_Book_Click_ReadDay.Continuity = myUser_Book_Click_ReadDay.Continuity + 1;
                                myUser_Book_Click_ReadDay.UpdateDate = myNowDate;
                            }
                            else if (myUser_Book_Click_ReadDay.UpdateDate < myNowDate)
                            {//不连续
                                myUser_Book_Click_ReadDay.ReadDayCount = myUser_Book_Click_ReadDay.ReadDayCount + 1;
                                myUser_Book_Click_ReadDay.RecentlyReadBookId = myLong;
                                myUser_Book_Click_ReadDay.Continuity = 1;
                                myUser_Book_Click_ReadDay.UpdateDate = myNowDate;
                            }
                        }
                        else
                        { //不存在则增加
                            User_Book_Click_ReadDay newUser_Book_Click_ReadDay = new User_Book_Click_ReadDay();
                            newUser_Book_Click_ReadDay.MobilePhone = mobilePhone;
                            newUser_Book_Click_ReadDay.ReadDayCount = 1;
                            newUser_Book_Click_ReadDay.RecentlyReadBookId = myLong;
                            newUser_Book_Click_ReadDay.Continuity = 1;
                            newUser_Book_Click_ReadDay.UpdateDate = myNowDate;
                            myOperating.User_Book_Click_ReadDay.Add(newUser_Book_Click_ReadDay);
                        }
                    }
                    #endregion
                    myOperating.SaveChanges();
                    myStatusData.operateStatus = 200;
                }
            }

            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取单独用户点一点阅读天数
        [HttpPost]
        public JsonResult GetReadDays()
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

            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if (checkAuthCodeBool)
                {
                    mobilePhone = myAuthCodeInstance.mobilePhone;
                    //
                    ReadCount myReadCount = new ReadCount();
                    //
                    var myUser_Book_Click_ReadDay = myOperating.User_Book_Click_ReadDay.Where(p => p.MobilePhone == mobilePhone).Select(s => new { ReadDayCount = s.ReadDayCount, RecentlyReadBookId = s.RecentlyReadBookId }).FirstOrDefault();
                    if (myUser_Book_Click_ReadDay != null)
                    {
                        myReadCount.RecentlyReadBookId = myUser_Book_Click_ReadDay.RecentlyReadBookId;
                        myReadCount.ReadDayCount = myUser_Book_Click_ReadDay.ReadDayCount;
                        myReadCount.RankNum = UserOptionClass.ReadDayRank(mobilePhone);
                    }
                    //
                    myStatusData.dataTable = myReadCount;
                    myStatusData.operateStatus = 200;
                }
            }

            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取用户阅读排行榜列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetReadDaysList(string currentpage, string pagesize)
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
            //
            #region 翻页属性
            int recordCount = 0;
            int pageCount = 0;
            string orderbyfiled = "ReadDayCount";
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
            if (!String.IsNullOrWhiteSpace(pagesize))
            {
                bool isOk = int.TryParse(pagesize, out sqlPagesize);
                if (!isOk) sqlPagesize = 10;
            }
            #endregion
            IQueryable<User_Book_Click_ReadDay> myIQueryable = null;

            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
                //
                myIQueryable = myOperating.User_Book_Click_ReadDay;
                if (myIQueryable != null)
                {
                    #region 阅读排行榜
                    List<ClickBookReadDaysItem> myClickBookReadDaysList = new List<ClickBookReadDaysItem>();
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).Select(s => new { MobilePhone = s.MobilePhone, ReadDayCount = s.ReadDayCount }).ToList();
                    if (BookTable != null)
                    {
                        for (int i = 0; i < BookTable.Count(); i++)
                        {
                            ClickBookReadDaysItem myClickBookReadDaysItem = new ClickBookReadDaysItem();
                            string myMobilePhone = "";
                            if (!String.IsNullOrWhiteSpace(BookTable[i].MobilePhone))
                            {
                                myMobilePhone = BookTable[i].MobilePhone;
                            }
                            myClickBookReadDaysItem.MobilePhone = BookTable[i].MobilePhone;
                            string myNickName = "匿名";
                            string myHeadImage = StaticVarClass.DefaultHeadImage;
                            User_UserName myUser_UserName = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == myMobilePhone);
                            if (myUser_UserName != null)
                            {
                                if (!String.IsNullOrWhiteSpace(myUser_UserName.NickName))
                                {
                                    myNickName = myUser_UserName.NickName;
                                }
                                 //   
                                 if (!String.IsNullOrWhiteSpace(myUser_UserName.HeadImage))
                                 {
                                     myHeadImage = StaticVarClass.myDomain + myUser_UserName.HeadImage;
                                 }
                            }
                            myClickBookReadDaysItem.NickName = myNickName;
                            myClickBookReadDaysItem.HeadImage = myHeadImage;
                            myClickBookReadDaysItem.ReadDayCount = BookTable[i].ReadDayCount;
                            myClickBookReadDaysList.Add(myClickBookReadDaysItem);
                        }
                    }
                    #endregion
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = myClickBookReadDaysList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
    }
    #region 点一点内容结构
    public class ClickRecord
    {
        public long BookID { get; set; }
        public string BookName { get; set; }
        public string BookOnlineUrl { get; set; }
    }
    #endregion
    //
    #region 用户阅读天数和排名信息
    public class ReadCount
    {
        private int _ReadDayCount = 0;
        private long _RankNum = 0;
        public Nullable<long> RecentlyReadBookId { get; set; }
        public int ReadDayCount { get { return _ReadDayCount; } set { _ReadDayCount = value; } }
        public long RankNum { get { return _RankNum; } set { _RankNum = value; } }
    }
    #endregion
}