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
    public class StudyController : Controller
    {
        //
        #region 获取课件类别
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult BookClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Child_Book_Study_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { BookClassID = s.BookClassID, BookClassName = s.BookClassName, ClassImage = StaticVarClass.BookStudyResourceUrl + s.ClassImage, ImgWidth = s.ImgWidth, ImgHeight = s.ImgHeight }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获得学一学电子书列表
        [HttpPost]
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
                IQueryable<Child_Book_Study> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(bookclassid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(bookclassid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Book_Study.Where(p => p.BookClassID == myInt && p.BookStatus == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Child_Book_Study.Where(p => p.BookStatus == 200);
                }
                //
                if (myIQueryable != null)
                {
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, false).Select(s => new { BookID = s.BookID, BookName = s.BookName, BookOnlineUrl = StaticVarClass.BookStudyResourceUrl + s.BookOnlineUrl, BookDownLoadZip = StaticVarClass.BookStudyResourceUrl + s.BookDownLoadZip, BookZipName = s.BookZipName }).ToList();
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    //
                    #region 如果用户登录则返回注册过得课件列表
                    List<User_RegBook_List> myUser_RegBook_List = null; ;
                    if (!String.IsNullOrWhiteSpace(mobilePhone))
                    {
                        myUser_RegBook_List = myOperating.User_RegBook_List.Where(p => p.MobilePhone == mobilePhone).ToList();
                    } 
                    #endregion
                    
                    //
                    List<StudyBookItem> StudyBookList = new List<StudyBookItem>();
                    #region 获取电子书列表
                    if (BookTable != null)
                    {
                        for (int i = 0; i < BookTable.Count(); i++)
                        {
                            StudyBookItem myStudyBookItem = new StudyBookItem();
                            long myBookId = BookTable[i].BookID;
                            myStudyBookItem.BookID = myBookId;
                            myStudyBookItem.BookName = BookTable[i].BookName;
                            myStudyBookItem.BookOnlineUrl = BookTable[i].BookOnlineUrl;
                            myStudyBookItem.BookDownLoadZip = BookTable[i].BookDownLoadZip;
                            myStudyBookItem.BookZipName = BookTable[i].BookZipName;
                            int IsReg = 0;
                            #region 该书是否已经注册
                            if (myUser_RegBook_List != null)
                            {
                                IsReg = myUser_RegBook_List.Count(p => p.BookStudyID == myBookId);
                            } 
                            #endregion
                            if (IsReg > 0)
                            {
                                myStudyBookItem.IsReg = true;
                            }
                            else
                            {
                                myStudyBookItem.IsReg = false;
                            }
                            StudyBookList.Add(myStudyBookItem);
                        }
                        myStatusData.dataTable = StudyBookList;
                    }
                    #endregion
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获得学一学电子书列表--已注册
        [HttpPost]
        public JsonResult BookRegList()
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
            if (!String.IsNullOrWhiteSpace(mobilePhone))
            {
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    List<StudyBookItem> StudyBookList = new List<StudyBookItem>();
                    //
                    List<User_RegBook_List> myRegBookList = myOperating.User_RegBook_List.Where(p => p.MobilePhone == mobilePhone).ToList();
                    if (myRegBookList != null)
                    {
                        //
                        #region 取出所有类别
                        List<Child_Book_Study_Class> myChild_Book_Study_Class = new List<Child_Book_Study_Class>();
                        myChild_Book_Study_Class = myOperating.Child_Book_Study_Class.ToList();
                        #endregion
                        //
                        int bookCount = myRegBookList.Count();
                        myStatusData.dataRecordCount = bookCount;
                        for (int i = 0; i < bookCount; i++)
                        {
                            long myLong = myRegBookList[i].BookStudyID;
                            Child_Book_Study myChildBookStudy = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == myLong && p.BookStatus == 200);
                            if (myChildBookStudy != null)
                            {
                                StudyBookItem myStudyBookItem = new StudyBookItem();
                                myStudyBookItem.BookID = myLong;
                                myStudyBookItem.BookName = myChildBookStudy.BookName;
                                myStudyBookItem.BookOnlineUrl = StaticVarClass.BookStudyResourceUrl + myChildBookStudy.BookOnlineUrl;
                                myStudyBookItem.BookDownLoadZip = StaticVarClass.BookStudyResourceUrl + myChildBookStudy.BookDownLoadZip;
                                myStudyBookItem.BookZipName = myChildBookStudy.BookZipName;
                                myStudyBookItem.IsReg = true;
                                //
                                #region 取出该类别图片的大小
                                Nullable<int> myClassId = myChildBookStudy.BookClassID;
                                Child_Book_Study_Class Child_Book_Study_ClassItem = new Child_Book_Study_Class();
                                Child_Book_Study_ClassItem = myChild_Book_Study_Class.FirstOrDefault(p => p.BookClassID == myClassId);
                                if (Child_Book_Study_ClassItem != null)
                                {
                                    myStudyBookItem.ImgWidth = Child_Book_Study_ClassItem.ImgWidth;
                                    myStudyBookItem.ImgHeight = Child_Book_Study_ClassItem.ImgHeight;
                                }
                                #endregion
                                //
                                StudyBookList.Add(myStudyBookItem);
                            }
                        }
                        myStatusData.dataTable = StudyBookList;
                    }
                }
            }
            myStatusData.operateStatus = 200;
            return Json(myStatusData);
        }
        #endregion
    }
}
