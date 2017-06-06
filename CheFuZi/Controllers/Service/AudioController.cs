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
    public class AudioController : Controller
    {
        //
        #region 获取听一听大类
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult ClassList()
        {
            StatusData myStatusData = new StatusData();//返回的类型
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                var myTable = myOperating.Child_Audio_Class.Where(p => p.Status == 200).OrderBy(p => p.OrderBy).Select(s => new { ClassId = s.ClassId, ClassTitle = s.ClassTitle }).ToList();
                myStatusData.dataTable = myTable;
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData,JsonRequestBehavior.AllowGet);
        }
        #endregion
        //
        #region 获取听一听音频专辑
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult AlbumList(string classid, string currentpage, string pagesize)
        {
            string mobilePhone = "";//
            //
            DataOptionClass myDataOptionClass = new DataOptionClass();
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
                IQueryable<Child_Audio_Album> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(classid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(classid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Audio_Album.Where(p => p.ClassId == myInt && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                else
                {
                    myIQueryable = myOperating.Child_Audio_Album.Where(p => p.Status == 200);
                }
                //
                if (myIQueryable != null)
                {
                    DataMethodClass myDataMethodClass = new DataMethodClass();
                    List<AudioAlbumData> myAudioAlbumDataList = new List<AudioAlbumData>();
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).Select(s => new { AlbumId = s.AlbumId, AlbumTitle = s.AlbumTitle, Anchor = s.Anchor, AlbumImage = StaticVarClass.myDomain + s.AlbumImage, AlbumDescrib = s.AlbumDescrib, AudioCount = s.AudioCount, PlayTimes = s.PlayTimes, ClassId = s.ClassId }).ToList();
                    if (BookTable != null)
                    {
                        for (int i = 0; i < BookTable.Count(); i++)
                        {
                            AudioAlbumData myAudioAlbumData = new AudioAlbumData();
                            int myInt = BookTable[i].AlbumId;
                            int ListCount = 0;//总共数量
                            Child_Audio_List myChild_Audio_List = new Child_Audio_List();
                            ListCount = myOperating.Child_Audio_List.Count(p => p.AlbumId == myInt);
                            //
                            myAudioAlbumData.AlbumId = BookTable[i].AlbumId;
                            myAudioAlbumData.AlbumTitle = BookTable[i].AlbumTitle;
                            myAudioAlbumData.Anchor = BookTable[i].Anchor;
                            myAudioAlbumData.AlbumImage = BookTable[i].AlbumImage;
                            myAudioAlbumData.AlbumDescrib = BookTable[i].AlbumDescrib;
                            myAudioAlbumData.AudioCount = ListCount;
                            myAudioAlbumData.PlayTimes = myDataOptionClass.GetReadPlayTimes(2, myInt);
                            myAudioAlbumData.ClassName = myDataMethodClass.GetAudioClassName(BookTable[i].ClassId);
                            myAudioAlbumDataList.Add(myAudioAlbumData);
                        }

                    }
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = myAudioAlbumDataList;
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取听一听专辑内音频列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult AudioList(string albumid, string currentpage, string pagesize, string orderby, string isasc)
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
                }
                if (String.IsNullOrWhiteSpace(isasc))
                {
                    isasc = "";
                }
                switch (isasc)
                {
                    case "1":
                        isDesc = true;//倒序
                        break;
                    case "2":
                        isDesc = false;//倒序
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
                string soundBgImage = "";//获取专辑图片
                IQueryable<Child_Audio_List> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(albumid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(albumid, out myInt);
                    if (isOk)
                    {
                        #region 背景图片
                        Child_Audio_Album myChild_Audio_Album = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == myInt);
                        if (myChild_Audio_Album != null)
                        {
                            soundBgImage = StaticVarClass.myDomain + myChild_Audio_Album.AlbumImage;
                        }
                        #endregion
                        myIQueryable = myOperating.Child_Audio_List.Where(p => p.AlbumId == myInt && p.Status == 200);
                    }
                    else
                    {
                        myIQueryable = null;
                        myStatusData.operateStatus = 400;
                    }
                }
                //
                if (myIQueryable != null)
                {
                    var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { AudioId = s.AudioId, AudioTitle = s.AudioTitle, AudioImage = soundBgImage, AudioUrl = StaticVarClass.myDomain + s.AudioUrl, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds, AudioWords = s.AudioWords, AddDate = s.AddDate }).ToList();
                    //
                    myStatusData.dataPageCount = pageCount;
                    myStatusData.dataRecordCount = recordCount;
                    myStatusData.dataTable = BookTable;
                    myStatusData.operateStatus = 200;
                }

            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取视音频内容
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetAudio(string myid)
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
                        //
                        var myChild_Audio_List = myOperating.Child_Audio_List.Join(myOperating.Child_Audio_Album, tL => tL.AlbumId, album => album.AlbumId, (tL, album) => new { tL.AudioId, tL.AudioTitle, album.AlbumImage, tL.AudioUrl, tL.AudioWords, tL.PlayTimes, tL.TimeSeconds, tL.Status }).Where(p => p.AudioId == myLong && p.Status == 200).Select(s => new { AudioId = s.AudioId, AudioTitle = s.AudioTitle, AudioImage = StaticVarClass.myDomain + s.AlbumImage, AudioUrl = StaticVarClass.myDomain + s.AudioUrl, AudioWords = s.AudioWords, PlayTimes = s.PlayTimes, TimeSeconds = s.TimeSeconds }).ToList();
                        if (myChild_Audio_List != null)
                            myStatusData.dataTable = myChild_Audio_List;
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
        #region 获取背景音乐
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetBgSound(string currentpage, string pagesize)
        {
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            if (checkAuthCodeBool)
            {
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    #region 翻页属性
                    int recordCount = 0;
                    int pageCount = 0;
                    string orderbyfiled = "OrderBy";
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
                    IQueryable<Audio_Background> myIQueryable = null;

                    myIQueryable = myOperating.Audio_Background;
                    //
                    if (myIQueryable != null)
                    {
                        var BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).Select(s => new { BgMusicId = s.BgMusicId, MusicTitle = s.MusicTitle, MusicUrl = StaticVarClass.myDomain + s.MusicUrl, TimeSeconds = s.TimeSeconds }).ToList();
                        myStatusData.dataPageCount = pageCount;
                        myStatusData.dataRecordCount = recordCount;
                        myStatusData.dataTable = BookTable;
                    }
                    myStatusData.operateStatus = 200;
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
        #region 添加用户录音
        [HttpPost]
        public JsonResult AddSoundRecord(string audioid, string audiourl, string bgmusic, string seconds, string operate)
        {//operate=0保存；1保存并发布
            #region 检查授权
            string myMobilePhone = "";//新授权码
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            #region 替换域名信息
            if (!String.IsNullOrWhiteSpace(bgmusic))
            {
                bgmusic = bgmusic.ToLower().Replace("http://", "");
                int strIndex = bgmusic.IndexOf('/') + 1;
                if (strIndex < bgmusic.Length)
                    bgmusic = bgmusic.Substring(strIndex);
            }
            #endregion
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            if (checkAuthCodeBool)
            {
                myMobilePhone = myAuthCodeInstance.mobilePhone;
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    long myLong = 0;
                    bool isLong = false;
                    if (!String.IsNullOrEmpty(audioid))
                    {
                        isLong = long.TryParse(audioid, out myLong);
                    }
                    if (isLong)
                    {
                        try
                        {
                            Child_Audio_List_User oldChild_Audio_List_User = new Child_Audio_List_User();
                            oldChild_Audio_List_User = myOperating.Child_Audio_List_User.FirstOrDefault(p => p.AudioId == myLong && p.MobilePhone == myMobilePhone);
                            if (oldChild_Audio_List_User == null)
                            {
                                #region 添加新的
                                Child_Audio_List_User myChild_Audio_List_User = new Child_Audio_List_User();
                                //
                                myChild_Audio_List_User.AudioId = myLong;
                                myChild_Audio_List_User.MobilePhone = myMobilePhone;
                                if (!String.IsNullOrEmpty(audiourl))
                                {
                                    myChild_Audio_List_User.AudioUrl = audiourl;
                                }
                                if (!String.IsNullOrEmpty(bgmusic))
                                {
                                    myChild_Audio_List_User.BgMusicUrl = bgmusic;
                                }
                                int myseconds = 0;//时长
                                if (!String.IsNullOrWhiteSpace(seconds))
                                {
                                    int.TryParse(seconds, out myseconds);
                                }
                                myChild_Audio_List_User.TimeSeconds = myseconds;

                                myChild_Audio_List_User.AddDate = DateTime.Now;
                                myChild_Audio_List_User.PlayTimes = 0;
                                myChild_Audio_List_User.PraiceCount = 0;
                                myChild_Audio_List_User.OrderBy = 10000;
                                if (!String.IsNullOrWhiteSpace(operate))
                                {
                                    if (operate == "1")
                                    {
                                        myChild_Audio_List_User.Status = 200;//
                                    }
                                    else
                                    {
                                        myChild_Audio_List_User.Status = 300;//
                                    }
                                }
                                else
                                {
                                    myChild_Audio_List_User.Status = 300;//
                                }
                                myOperating.Child_Audio_List_User.Add(myChild_Audio_List_User);
                                #endregion
                            }
                            else
                            {//修改
                                #region 修改状态
                                if (!String.IsNullOrWhiteSpace(operate))
                                {
                                    if (operate == "1")
                                    {
                                        oldChild_Audio_List_User.Status = 200;//
                                    }
                                    else
                                    {
                                        oldChild_Audio_List_User.Status = 300;//
                                    }
                                }
                                else
                                {
                                    oldChild_Audio_List_User.Status = 300;//
                                }
                                #endregion
                            }
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
                myStatusData.operateStatus = 5;//登陆失效
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取用户录制的音频
        [HttpPost]
        public JsonResult GetUserSoundList(string currentpage, string pagesize)
        {
            #region 检查授权
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AudioUserId";
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
                IQueryable<Child_Audio_List_User> myIQueryable = null;
                myIQueryable = myOperating.Child_Audio_List_User;
                //
                if (myIQueryable != null)
                {
                    var UserSoundTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).ToList();
                    List<UserSoundData> myUserSoundDataList = new List<UserSoundData>();
                    if (UserSoundTable != null)
                    {
                        for (int i = 0; i < UserSoundTable.Count(); i++)
                        {
                            UserSoundData myUserSoundDataItem = new UserSoundData();
                            string myMobilePhone = "";
                            string myNickName = "匿名";
                            Nullable<long> myAudioId = 0;//听一听主键
                            Nullable<int> myAlbumId = 0;//专辑主键
                            string myAudioWords = "";
                            string mySoundTitle = "佚名";
                            string mySoundImage = "";//封面图片
                            #region 昵称
                            if (!String.IsNullOrWhiteSpace(UserSoundTable[i].MobilePhone))
                            {
                                myMobilePhone = UserSoundTable[i].MobilePhone;
                                myUserSoundDataItem.MobilePhone = myMobilePhone;
                                User_UserName myUser_UserName = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == myMobilePhone);
                                if (myUser_UserName != null)
                                {
                                    if (!String.IsNullOrWhiteSpace(myUser_UserName.NickName))
                                    {
                                        myNickName = myUser_UserName.NickName;
                                    }
                                    else
                                    {
                                        if (myUser_UserName.MobilePhone.Length > 7)
                                            myNickName = myUser_UserName.MobilePhone.Substring(7) + "****";
                                    }

                                }
                                myUserSoundDataItem.NickName = myNickName;
                            }
                            #endregion
                            //
                            myAudioId = UserSoundTable[i].AudioId;
                            #region 音乐名称和时长
                            if (myAudioId != null)
                            {
                                Child_Audio_List myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == myAudioId);
                                if (myChild_Audio_List != null)
                                {
                                    myAlbumId = myChild_Audio_List.AlbumId;
                                    mySoundTitle = myChild_Audio_List.AudioTitle;
                                    myAudioWords = myChild_Audio_List.AudioWords;
                                }
                            }
                            #endregion
                            //
                            #region 背景图片
                            if (myAlbumId != null)
                            {
                                Child_Audio_Album myChild_Audio_Album = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == myAlbumId);
                                if (myChild_Audio_Album != null)
                                {
                                    mySoundImage = StaticVarClass.myDomain + myChild_Audio_Album.AlbumImage;
                                }
                            }
                            #endregion
                            //
                            myUserSoundDataItem.AudioUserId = UserSoundTable[i].AudioUserId;
                            myUserSoundDataItem.MusicTitle = mySoundTitle;
                            myUserSoundDataItem.AudioImage = mySoundImage;
                            myUserSoundDataItem.BgMusicUrl = StaticVarClass.myDomain + UserSoundTable[i].BgMusicUrl;
                            myUserSoundDataItem.AudioWords = myAudioWords;
                            myUserSoundDataItem.AudioUrl = StaticVarClass.myDomain + UserSoundTable[i].AudioUrl;
                            myUserSoundDataItem.AddDate = UserSoundTable[i].AddDate;
                            myUserSoundDataItem.TimeSeconds = UserSoundTable[i].TimeSeconds;
                            myUserSoundDataItem.PlayTimes = UserSoundTable[i].PlayTimes;
                            //
                            myUserSoundDataList.Add(myUserSoundDataItem);
                        }
                        myStatusData.dataPageCount = pageCount;
                        myStatusData.dataRecordCount = recordCount;
                        myStatusData.dataTable = myUserSoundDataList;
                    }
                }
                myStatusData.operateStatus = 200;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 获取私人录制的音频
        [HttpPost]
        public JsonResult PersonalAudioList(string currentpage, string pagesize)
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
                    #region 翻页属性
                    int recordCount = 0;
                    int pageCount = 0;
                    string orderbyfiled = "AudioUserId";
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
                    IQueryable<Child_Audio_List_User> myIQueryable = null;
                    myIQueryable = myOperating.Child_Audio_List_User.Where(p => p.MobilePhone == myMobilePhone);
                    //
                    if (myIQueryable != null)
                    {
                        var UserSoundTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).ToList();
                        List<PersonalSoundData> myUserSoundDataList = new List<PersonalSoundData>();
                        if (UserSoundTable != null)
                        {
                            for (int i = 0; i < UserSoundTable.Count(); i++)
                            {
                                PersonalSoundData myUserSoundDataItem = new PersonalSoundData();
                                Nullable<long> myAudioId = 0;//听一听主键
                                Nullable<int> myAlbumId = 0;//专辑主键
                                string mySoundTitle = "佚名";
                                string myAudioWords = "";
                                string mySoundImage = "";//封面图片
                                //
                                myAudioId = UserSoundTable[i].AudioId;
                                #region 音乐名称和歌词
                                if (myAudioId != null)
                                {
                                    Child_Audio_List myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == myAudioId);
                                    if (myChild_Audio_List != null)
                                    {
                                        myAlbumId = myChild_Audio_List.AlbumId;
                                        mySoundTitle = myChild_Audio_List.AudioTitle;
                                        myAudioWords = myChild_Audio_List.AudioWords;
                                    }
                                }
                                #endregion
                                //
                                #region 背景图片
                                if (myAlbumId != null)
                                {
                                    Child_Audio_Album myChild_Audio_Album = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == myAlbumId);
                                    if (myChild_Audio_Album != null)
                                    {
                                        mySoundImage = StaticVarClass.myDomain + myChild_Audio_Album.AlbumImage;
                                    }
                                }
                                #endregion
                                //
                                myUserSoundDataItem.AudioUserId = UserSoundTable[i].AudioUserId;
                                myUserSoundDataItem.MusicTitle = mySoundTitle;
                                myUserSoundDataItem.AudioImage = mySoundImage;
                                myUserSoundDataItem.AudioWords = myAudioWords;
                                myUserSoundDataItem.BgMusicUrl = StaticVarClass.myDomain + UserSoundTable[i].BgMusicUrl;
                                myUserSoundDataItem.AudioUrl = StaticVarClass.myDomain + UserSoundTable[i].AudioUrl;
                                myUserSoundDataItem.AddDate = UserSoundTable[i].AddDate;
                                myUserSoundDataItem.TimeSeconds = UserSoundTable[i].TimeSeconds;
                                myUserSoundDataItem.PlayTimes = UserSoundTable[i].PlayTimes;
                                myUserSoundDataItem.Status = UserSoundTable[i].Status;
                                //
                                myUserSoundDataList.Add(myUserSoundDataItem);
                            }
                            myStatusData.dataPageCount = pageCount;
                            myStatusData.dataRecordCount = recordCount;
                            myStatusData.dataTable = myUserSoundDataList;
                        }
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
        #region 用户私人录音发布
        [HttpPost]
        public JsonResult PublishAudio(string audioid, string operate)
        {//operate=0保存；1保存并发布
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
                    long myLong = 0;
                    bool isLong = false;
                    if (!String.IsNullOrEmpty(audioid))
                    {
                        isLong = long.TryParse(audioid, out myLong);
                    }
                    if (isLong)
                    {
                        #region 修改状态
                        try
                        {
                            Child_Audio_List_User myChild_Audio_List_User = myOperating.Child_Audio_List_User.FirstOrDefault(p => p.AudioUserId == myLong);
                            //
                            if (myChild_Audio_List_User != null)
                            {
                                if (!String.IsNullOrWhiteSpace(operate))
                                {
                                    int myStatus = 0;
                                    bool isInt = int.TryParse(operate, out myStatus);
                                    if (isInt)
                                    {
                                        if (myStatus == 1)
                                        {
                                            myChild_Audio_List_User.Status = 200;//发布
                                        }
                                        else
                                        {
                                            myChild_Audio_List_User.Status = 300;//撤下
                                        }
                                    }
                                    else
                                    {
                                        myChild_Audio_List_User.Status = 300;//撤下
                                    }
                                }
                                else
                                {
                                    myChild_Audio_List_User.Status = 300;//撤下
                                }
                                myOperating.SaveChanges();
                                myStatusData.operateStatus = 200;
                            }
                            else
                            {
                                myStatusData.operateStatus = 400;
                            }
                            //
                        }
                        catch
                        {
                            myStatusData.operateStatus = -1;
                        }
                        #endregion
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
                myStatusData.operateStatus = 5;//登陆失效
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 删除私人录制的音频
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
                        IQueryable<Child_Audio_List_User> myList = myOperating.Child_Audio_List_User.Where(p => p.MobilePhone == myMobilePhone);
                        if (myList != null)
                        {
                            foreach (Child_Audio_List_User recordItem in myList)
                            {
                                FunctionClass.DeleteFile(recordItem.AudioUrl);
                                myOperating.Child_Audio_List_User.Remove(recordItem);
                            }
                            myOperating.SaveChanges();
                        }
                        myStatusData.operateStatus = 200;
                    }
                    else if (operatType == 2)
                    {
                        Child_Audio_List_User myDataRecord = new Child_Audio_List_User();
                        myDataRecord = myOperating.Child_Audio_List_User.FirstOrDefault(p => p.AudioUserId == idLong && p.MobilePhone == myMobilePhone);
                        if (myDataRecord != null)
                        {
                            FunctionClass.DeleteFile(myDataRecord.AudioUrl);
                            myOperating.Child_Audio_List_User.Remove(myDataRecord);
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
        //========================
        #region 阅读或者播放次数
        [HttpPost]
        public JsonResult TimesAdd(int classid, long aboutid)
        {///1点一点；2听一听；3看一看；
            ///4课程讲解；5幼儿公开课；6教育技能；
            ///7操作说明；8教师社区；9精彩瞬间;12用户录音
            bool isOk = false;
            StatusData myStatusData = new StatusData();//返回状态
            DataOptionClass myDataOptionClass = new DataOptionClass();
            isOk = myDataOptionClass.TimesAdd(classid, aboutid);
            if (isOk)
            {
                myStatusData.operateStatus = 200;
            }
            else
            {
                myStatusData.operateStatus = 400;
            }
            return Json(myStatusData);
        }
        #endregion
    }
    //
    #region 专辑数据结构
    public class AudioAlbumData
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string Anchor { get; set; }
        public string AlbumImage { get; set; }
        public string AlbumDescrib { get; set; }
        public int AudioCount { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public string ClassName { get; set; }
    }
    #endregion
    //
    #region 录音秀数据结构
    public class UserSoundData
    {
        public long AudioUserId { get; set; }
        public string MobilePhone { get; set; }
        public string NickName { get; set; }
        public string MusicTitle { get; set; }
        public string AudioWords { get; set; }
        public string AudioUrl { get; set; }
        public string BgMusicUrl { get; set; }
        public string AudioImage { get; set; }
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
    //
    #region 私人录音秀数据结构
    public class PersonalSoundData
    {
        public long AudioUserId { get; set; }
        public string MusicTitle { get; set; }
        public string AudioUrl { get; set; }
        public string BgMusicUrl { get; set; }
        public string AudioImage { get; set; }
        public string AudioWords { get; set; }
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
        public Nullable<int> Status { get; set; }
    }
    #endregion
    //
    #region 返回内容数据结构
    public class AudioRecord
    {
        public long AudioId { get; set; }
        public string AudioTitle { get; set; }
        public string AudioUrl { get; set; }
        public string AudioImage { get; set; }
        public string AudioWords { get; set; }
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    }
    #endregion
}
