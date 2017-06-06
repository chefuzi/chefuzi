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
    public class VideoController : Controller
    {
        //
        #region 获取看一看大类
        [HttpPost]
        public JsonResult ClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Child_Video_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { ClassId = s.ClassId, ClassTitle = s.ClassTitle }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取看一看专辑
        [HttpPost]
        public JsonResult AlbumList(string classid, string currentpage, string pagesize)
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
                IQueryable<Child_Video_Album> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(classid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(classid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Video_Album.Where(p => p.ClassId == myInt && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Child_Video_Album.Where(p => p.Status == 200);
                }
                //
                if (myIQueryable != null)
                {
                    List<VideoAlbumData> myVideoAlbumDataList = new List<VideoAlbumData>();
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, false).Select(s => new { AlbumId = s.AlbumId, AlbumTitle = s.AlbumTitle, AlbumImage = StaticVarClass.myDomain + s.AlbumImage, AlbumDescrib = s.AlbumDescrib, VideoCount = s.VideoCount, PlayTimes = s.PlayTimes }).ToList();
                    if (BookTable != null)
                    {
                        for (int i = 0; i < BookTable.Count(); i++)
                        {
                            VideoAlbumData myVideoAlbumData = new VideoAlbumData();
                            int myInt = BookTable[i].AlbumId;
                            int ListCount = 0;//总共数量
                            Child_Video_List myChild_Video_List = new Child_Video_List();
                            ListCount = myOperating.Child_Video_List.Count(p => p.AlbumId == myInt);
                            //
                            myVideoAlbumData.AlbumId = BookTable[i].AlbumId;
                            myVideoAlbumData.AlbumTitle = BookTable[i].AlbumTitle;
                            myVideoAlbumData.AlbumImage = BookTable[i].AlbumImage;
                            myVideoAlbumData.AlbumDescrib = BookTable[i].AlbumDescrib;
                            myVideoAlbumData.VideoCount = ListCount;
                            myVideoAlbumData.PlayTimes = BookTable[i].PlayTimes;
                            myVideoAlbumDataList.Add(myVideoAlbumData);
                        }

                    }
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = myVideoAlbumDataList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取看一看专辑视频内容列表
        [HttpPost]
        public JsonResult VideoList(string albumid, string currentpage, string pagesize, string orderby, string isasc)
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
                string orderbyfiled = "OrderBy";
                bool isDesc = false;//正序
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
                IQueryable<Child_Video_List> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(albumid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(albumid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Video_List.Where(p => p.AlbumId == myInt && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                //
                if (myIQueryable != null)
                {
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { VideoId = s.VideoId, VideoTitle = s.VideoTitle, VideoDes = s.VideoDes, VideoImage = StaticVarClass.myDomain + s.VideoImage, VideoUrl = StaticVarClass.myDomain + s.VideoUrl, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds }).ToList();
                    
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
                        var myChild_Video_List = myOperating.Child_Video_List.Where(p => p.VideoId == myLong && p.Status == 200).Select(s => new { VideoId = s.VideoId, VideoTitle = s.VideoTitle, VideoDes = s.VideoDes, VideoImage = StaticVarClass.myDomain + s.VideoImage, VideoUrl = StaticVarClass.myDomain + s.VideoUrl, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds }).ToList();
                        if (myChild_Video_List!=null)
                        myStatusData.dataTable = myChild_Video_List;
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
    }
    //
    #region 专辑数据结构
    public class VideoAlbumData
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumImage { get; set; }
        public string AlbumDescrib { get; set; }
        public int VideoCount { get; set; }
        public Nullable<int> PlayTimes { get; set; }
    }
    #endregion
    //
    #region 看一看内容结构
    public class VideoRecord
    {
        public long VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoUrl { get; set; }
        public string VideoImage { get; set; }
        public string VideoDes { get; set; }
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    } 
    #endregion
}
