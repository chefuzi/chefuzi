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
    //[Authorize(Roles = "100,101,102,103,104,105")]//系统管理员角色
    public class ManageAudioController : Controller
    {
        #region 专辑列表
        public ActionResult AlbumList(string currentpage, long del = 0)
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
                    Child_Audio_Album myDelRecord = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.AlbumImage);
                        myOperating.Child_Audio_Album.Remove(myDelRecord);
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
                IQueryable<Child_Audio_Album> myIQueryable = null;
                myIQueryable = myOperating.Child_Audio_Album;
                //
                if (myIQueryable != null)
                {
                    List<Child_Audio_Album> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
        #region 专辑添加-编辑
        public ActionResult AlbumAdd(AudioAlbumModel model, string ReturnUrl, Nullable<int> myid = 0)
        {
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
                    ViewBag.Headline = "专辑编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Audio_Album editRecord = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == mySelfId);
                    if (editRecord != null)
                    {
                        model.AlbumId = editRecord.AlbumId;
                        model.AlbumTitle = editRecord.AlbumTitle;
                        model.Anchor = editRecord.Anchor;
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
                        Child_Audio_Album addRecord = new Child_Audio_Album();
                        addRecord.ClassId = 0;
                        addRecord.AlbumTitle = model.AlbumTitle;
                        addRecord.Anchor = model.Anchor;
                        addRecord.AlbumImage = model.AlbumImage;
                        addRecord.AlbumDescrib = model.AlbumDescrib;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.AudioCount = 0;
                        addRecord.PlayTimes = 0;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Audio_Album.Add(addRecord);
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
                        Child_Audio_Album editSaveRecord = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == model.AlbumId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.AlbumTitle = model.AlbumTitle;
                            editSaveRecord.Anchor = model.Anchor;
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
                    #region 默认值
                    try
                    {
                        model.OrderBy = myOperating.Child_Audio_Album.Max(p => p.OrderBy) + 1;
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
        //
        #region 音频列表
        public ActionResult AudioList(string albumid, string currentpage, long del = 0)
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
            ViewBag.Headline = "音频管理";//栏目主题
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
                    Child_Audio_List myDelRecord = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.AudioImage);
                        myOperating.Child_Audio_List.Remove(myDelRecord);
                        #region 同步删除收藏中的内容
                        DataOptionClass myDataOptionClass = new DataOptionClass();
                        myDataOptionClass.CollectOption(2, del, true);//删除收藏中的内容 
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
                IQueryable<Child_Audio_List> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(albumid))
                {
                    int myInt = 0;
                    bool isOk = int.TryParse(albumid, out myInt);
                    if (isOk)
                    {
                        myIQueryable = myOperating.Child_Audio_List.Where(p => p.AlbumId == myInt);
                    }
                    else
                    {
                        myIQueryable = null;
                    }
                }
                //
                if (myIQueryable != null)
                {
                    List<Child_Audio_List> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc).ToList();
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
        #region 音频添加-编辑
        public ActionResult AudioAdd(AudioListModel model, string ReturnUrl, Nullable<long> myid = 0, int AlbumId = 0)
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
                    ViewBag.Headline = "音频编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Child_Audio_List editRecord = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == mySelfId);
                    if (editRecord != null)
                    {
                        model.AudioId = editRecord.AudioId;
                        model.AlbumId = editRecord.AlbumId;
                        model.AudioTitle = editRecord.AudioTitle;
                        model.AudioUrl = editRecord.AudioUrl;
                        model.AudioWords = editRecord.AudioWords;
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
                        Child_Audio_List addRecord = new Child_Audio_List();
                        addRecord.AlbumId = model.AlbumId;
                        addRecord.AudioTitle = model.AudioTitle;
                        addRecord.AudioUrl = model.AudioUrl;
                        addRecord.AudioWords = model.AudioWords;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.TimeSeconds = AllTimeSecond;
                        addRecord.PlayTimes = 0;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.Status = 200;
                        //
                        myOperating.Child_Audio_List.Add(addRecord);
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
                        Child_Audio_List editSaveRecord = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == model.AudioId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.AudioTitle = model.AudioTitle;
                            editSaveRecord.AudioUrl = model.AudioUrl;
                            editSaveRecord.AudioWords = model.AudioWords;
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
                        model.OrderBy = myOperating.Child_Audio_List.Where(p => p.AlbumId == AlbumId).Max(p => p.OrderBy) + 1;
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
        //
        #region 用户上传的音频列表
        public ActionResult UserAudioList(string mobilephone, string currentpage, long del = 0)
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
            ViewBag.Headline = "用户上传音频管理";//栏目主题
            //
            ViewBag.mobilephone = mobilephone;
            ViewBag.DataList = null;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Child_Audio_List_User myDelRecord = myOperating.Child_Audio_List_User.FirstOrDefault(p => p.AudioUserId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.AudioUrl);
                        myOperating.Child_Audio_List_User.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion

                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AudioUserId";
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
                IQueryable<Child_Audio_List_User> myIQueryable = null;
                if (!String.IsNullOrWhiteSpace(mobilephone))
                {
                    myIQueryable = myOperating.Child_Audio_List_User.Where(p => p.MobilePhone == mobilephone);
                }
                else
                {
                    myIQueryable = myOperating.Child_Audio_List_User;
                }
                //
                if (myIQueryable != null)
                {
                    IQueryable<Child_Audio_List_User> UserSoundTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, isDesc);
                    //
                    List<UserSoundData> myUserSoundDataList = new List<UserSoundData>();
                    if (UserSoundTable != null)
                    {
                        foreach (Child_Audio_List_User item in UserSoundTable)
                        {
                            UserSoundData myUserSoundDataItem = new UserSoundData();
                            Nullable<long> myAudioId = 0;//听一听主键
                            string myAudioWords = "";
                            string mySoundTitle = "佚名";
                            //
                            myAudioId = item.AudioId;
                            #region 音乐名称和时长
                            if (myAudioId != null)
                            {
                                Child_Audio_List myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == myAudioId);
                                if (myChild_Audio_List != null)
                                {
                                    mySoundTitle = myChild_Audio_List.AudioTitle;
                                    myAudioWords = myChild_Audio_List.AudioWords;
                                }
                            }
                            #endregion
                            //
                            myUserSoundDataItem.AudioUserId = item.AudioUserId;
                            myUserSoundDataItem.MobilePhone = item.MobilePhone;
                            myUserSoundDataItem.MusicTitle = mySoundTitle;
                            if (!String.IsNullOrWhiteSpace(item.BgMusicUrl))
                            myUserSoundDataItem.BgMusicUrl = StaticVarClass.myDomain + item.BgMusicUrl;
                            myUserSoundDataItem.AudioWords = myAudioWords;
                            if (!String.IsNullOrWhiteSpace(item.AudioUrl))
                            myUserSoundDataItem.AudioUrl = StaticVarClass.myDomain+item.AudioUrl;
                            myUserSoundDataItem.AddDate = item.AddDate;
                            myUserSoundDataItem.PlayTimes = item.PlayTimes;
                            //
                            myUserSoundDataList.Add(myUserSoundDataItem);
                        }
                        ViewBag.DataList = myUserSoundDataList;
                    }

                    //
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
    //
    #region 录音秀数据结构
    public class UserSoundData
    {
        public long AudioUserId { get; set; }
        public string MobilePhone { get; set; }
        public string MusicTitle { get; set; }
        public string AudioWords { get; set; }
        public string AudioUrl { get; set; }
        public string BgMusicUrl { get; set; }
        public Nullable<int> PlayTimes { get; set; }
        public Nullable<DateTime> AddDate { get; set; }
    }
    #endregion
}
