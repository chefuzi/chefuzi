using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//
using CheFuZi.Models;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;

namespace CheFuZi.Controllers
{
    public class HomeController : Controller
    {
        #region 首页
        public ActionResult Index()
        {
            return View();
        }
        #endregion
        //
        #region 关于
        public ActionResult About()
        {

            return View();
        }
        #endregion
        //
        #region 联系方式
        public ActionResult Contact()
        {

            return View();
        }
        #endregion
        //
        #region 分享网页
        /// <summary>
        /// domain/share/{classtype}/{aboutid}
        /// </summary>
        /// <param name="classtype">表类型</param>
        /// <param name="aboutid">内容主键</param>
        /// <returns></returns>
        [OutputCache(CacheProfile = "commoncache")]
        public ActionResult Share(int classtype = 0, long aboutid = 0)
        {
            ShareModel myShareModel = new ShareModel();
            myShareModel.ClassType = classtype;//属于哪一个表的内容
            myShareModel.AboutTitle = "芮卡家园";
            //
            #region 根据类别取出分享的内容
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                switch (classtype)
                {///1点一点；2听一听；3看一看；
                    ///4课程讲解；5幼儿公开课；6教育技能；
                    ///7操作说明；8教师社区；9精彩瞬间;
                    case 1://点一点
                        #region 点一点
                        Child_Book_Click myChild_Book_Click = new Child_Book_Click();
                        myChild_Book_Click = myOperating.Child_Book_Click.FirstOrDefault(p => p.BookID == aboutid);
                        if (myChild_Book_Click != null)
                        {
                            myShareModel.AboutTitle = myChild_Book_Click.BookName;
                            if (!String.IsNullOrWhiteSpace(myChild_Book_Click.BookImage))
                            {
                                myShareModel.AboutImages = StaticVarClass.BookClickResourceUrl + myChild_Book_Click.BookImage;
                            }
                        }
                        #endregion
                        return View("ShareClick", myShareModel);
                        //break;
                    case 2://2听一听
                        #region 听一听
                        Child_Audio_List myChild_Audio_List = new Child_Audio_List();
                        myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == aboutid);
                        if (myChild_Audio_List != null)
                        {
                            myShareModel.AboutTitle = myChild_Audio_List.AudioTitle;
                            #region 取专辑图片
                            int AlibumId = myChild_Audio_List.AlbumId;
                            try
                            {
                                Child_Audio_Album myChild_Audio_Album = new Child_Audio_Album();
                                myChild_Audio_Album  = myOperating.Child_Audio_Album.FirstOrDefault(p => p.AlbumId == AlibumId);
                                if (myChild_Audio_Album != null)
                                {
                                    if (!String.IsNullOrWhiteSpace(myChild_Audio_Album.Anchor))
                                    {
                                        myShareModel.AboutAuthor = myChild_Audio_Album.Anchor;
                                    }
                                    if (!String.IsNullOrWhiteSpace(myChild_Audio_Album.AlbumImage))
                                    {
                                        myShareModel.AboutImages = StaticVarClass.myDomain +  myChild_Audio_Album.AlbumImage;
                                    }
                                }
                            }
                            catch { } 
                            #endregion
                            if (!String.IsNullOrWhiteSpace(myChild_Audio_List.AudioUrl))
                            {
                                myShareModel.AboutAudio = StaticVarClass.myDomain + myChild_Audio_List.AudioUrl;
                            }
                            myShareModel.AboutContent = myChild_Audio_List.AudioWords;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(myChild_Audio_List.AddDate.ToString(), out myDateTime);
                            myShareModel.AddDate = myDateTime;
                        }
                        #endregion
                        break;
                    case 3://3看一看
                        #region 看一看
                        Child_Video_List myChild_Video_List = new Child_Video_List();
                        myChild_Video_List = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                        if (myChild_Video_List != null)
                        {
                            myShareModel.AboutTitle = myChild_Video_List.VideoTitle;
                            if (!String.IsNullOrWhiteSpace(myChild_Video_List.VideoImage))
                            {
                                myShareModel.AboutImages = StaticVarClass.myDomain + myChild_Video_List.VideoImage;
                            }
                            if (!String.IsNullOrWhiteSpace(myChild_Video_List.VideoUrl))
                            {
                                myShareModel.AboutVideo = StaticVarClass.myDomain + myChild_Video_List.VideoUrl;
                            }

                            myShareModel.AboutContent = myChild_Video_List.VideoDes;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(myChild_Video_List.AddDate.ToString(), out myDateTime);
                            myShareModel.AddDate = myDateTime;
                        }
                        #endregion
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7://4课程讲解；5幼儿公开课；6教育技能；7操作说明
                        #region 教师视频
                        Teacher_Video_List myTeacher_Video_List = new Teacher_Video_List();
                        myTeacher_Video_List = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                        if (myTeacher_Video_List != null)
                        {
                            myShareModel.AboutTitle = myTeacher_Video_List.VideoTitle;
                            if (!String.IsNullOrWhiteSpace(myTeacher_Video_List.VideoImage))
                            {
                                myShareModel.AboutImages = StaticVarClass.myDomain + myTeacher_Video_List.VideoImage;
                            }
                            if (!String.IsNullOrWhiteSpace(myTeacher_Video_List.VideoUrl))
                            {
                                myShareModel.AboutVideo = StaticVarClass.myDomain + myTeacher_Video_List.VideoUrl;
                            }
                            myShareModel.AboutContent = myTeacher_Video_List.VideoDes;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(myTeacher_Video_List.AddDate.ToString(), out myDateTime);
                            myShareModel.AddDate = myDateTime;
                        }
                        #endregion
                        break;
                    case 8://8教师社区
                        #region 教师社区
                        Teacher_Article myTeacher_Article = new Teacher_Article();
                        myTeacher_Article = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == aboutid);
                        if (myTeacher_Article != null)
                        {
                            myShareModel.AboutTitle = myTeacher_Article.ArticleTitle;
                            if (!String.IsNullOrWhiteSpace(myTeacher_Article.ArticleImages))
                            {
                                myShareModel.AboutImages = StaticVarClass.myDomain + myTeacher_Article.ArticleImages;
                            }
                            if (!String.IsNullOrWhiteSpace(myTeacher_Article.ArticleVideo))
                            {
                                myShareModel.AboutVideo = StaticVarClass.myDomain + myTeacher_Article.ArticleVideo;
                            }
                            myShareModel.AboutContent = myTeacher_Article.ArticleContent;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(myTeacher_Article.AddDate.ToString(), out myDateTime);
                            myShareModel.AddDate = myDateTime;
                        }
                        #endregion
                        break;
                    case 9://9精彩瞬间
                        #region 精彩瞬间
                        Discover_Article myDiscover_Article = new Discover_Article();
                        myDiscover_Article = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == aboutid);
                        if (myDiscover_Article != null)
                        {
                            myShareModel.AboutTitle = myDiscover_Article.ArticleTitle;
                            myShareModel.AboutImages = myDiscover_Article.ArticleImages;
                            myShareModel.AboutVideo = myDiscover_Article.ArticleVideo;
                            myShareModel.AboutContent = myDiscover_Article.ArticleContent;
                            DateTime myDateTime = DateTime.Now;
                            DateTime.TryParse(myDiscover_Article.AddDate.ToString(), out myDateTime);
                            myShareModel.AddDate = myDateTime;
                        }
                        #endregion
                        break;
                }
            }
            #endregion

            return View(myShareModel);
        } 
        #endregion
        //
        #region 游戏默认页
        public ActionResult Game(string gameid="")
        {
            return View();
        }
        #endregion
        //
        #region 帮助中心
        public ActionResult Help()
        {
            return View();
        }
        #endregion
        //
        #region 下载App
        //[OutputCache(CacheProfile = "commoncache")]
        public ActionResult DownLoad()
        {
            return View();
        }
        #endregion
    }
}
