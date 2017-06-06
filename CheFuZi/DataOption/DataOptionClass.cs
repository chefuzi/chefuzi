using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
namespace CheFuZi.DataOption
{
    public class DataOptionClass
    {
        #region 记录修改后修改收藏图片或者标题
        public bool CollectOption(int classid, long aboutid, bool editOrdel)
        {   ///editOrdel=true编辑；false删除
            ///1点一点；2听一听；3看一看；
            ///4课程讲解；5幼儿公开课；6教育技能；
            ///7操作说明；8教师社区；9精彩瞬间;
            //
            bool OptionComplet = false;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {

                bool isExict = false;//是否存在
                string myCoverImage = "";//图片
                string myTitle = "";//标题
                string mySummart = "";//摘要
                if (!editOrdel)
                {
                    #region 根据收藏类别取出图片和标题
                    switch (classid)
                    {///1点一点；2听一听；3看一看；
                        ///4课程讲解；5幼儿公开课；6教育技能；
                        ///7操作说明；8教师社区；9精彩瞬间;
                        case 1://点一点
                            Child_Book_Click myChild_Book_Click = new Child_Book_Click();
                            myChild_Book_Click = myOperating.Child_Book_Click.FirstOrDefault(p => p.BookID == aboutid);
                            if (myChild_Book_Click != null)
                            {
                                myTitle = myChild_Book_Click.BookName;
                                if (!String.IsNullOrWhiteSpace(myChild_Book_Click.BookImage))
                                {
                                    myCoverImage = StaticVarClass.BookClickResourceUrl + myChild_Book_Click.BookImage;
                                }
                                isExict = true;
                            }
                            break;
                        case 2://2听一听
                            Child_Audio_List myChild_Audio_List = new Child_Audio_List();
                            myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == aboutid);
                            if (myChild_Audio_List != null)
                            {
                                myTitle = myChild_Audio_List.AudioTitle;
                                if (!String.IsNullOrWhiteSpace(myChild_Audio_List.AudioImage))
                                {
                                    myCoverImage = StaticVarClass.myDomain + myChild_Audio_List.AudioImage;
                                }
                                isExict = true;
                            }
                            break;
                        case 3://3看一看
                            Child_Video_List myChild_Video_List = new Child_Video_List();
                            myChild_Video_List = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                            if (myChild_Video_List != null)
                            {
                                myTitle = myChild_Video_List.VideoTitle;
                                if (!String.IsNullOrWhiteSpace(myChild_Video_List.VideoImage))
                                {
                                    myCoverImage = StaticVarClass.myDomain + myChild_Video_List.VideoImage;
                                }
                                isExict = true;
                            }
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7://4课程讲解；5幼儿公开课；6教育技能；7操作说明
                            Teacher_Video_List myTeacher_Video_List = new Teacher_Video_List();
                            myTeacher_Video_List = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                            if (myTeacher_Video_List != null)
                            {
                                myTitle = myTeacher_Video_List.VideoTitle;
                                if (!String.IsNullOrWhiteSpace(myTeacher_Video_List.VideoImage))
                                {
                                    myCoverImage = StaticVarClass.myDomain + myTeacher_Video_List.VideoImage;
                                }
                                isExict = true;
                            }
                            break;
                        case 8://8教师社区
                            Teacher_Article myTeacher_Article = new Teacher_Article();
                            myTeacher_Article = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == aboutid);
                            if (myTeacher_Article != null)
                            {
                                myTitle = myTeacher_Article.ArticleTitle;
                                if (!String.IsNullOrWhiteSpace(myTeacher_Article.ArticleImages))
                                {
                                    myCoverImage = StaticVarClass.myDomain + myTeacher_Article.ArticleImages;
                                }
                                if (myTeacher_Article.ArticleSummary.Length > 50)
                                {
                                    mySummart = myTeacher_Article.ArticleSummary.Substring(0, 50);
                                }
                                else
                                {
                                    mySummart = myTeacher_Article.ArticleSummary;
                                }
                                isExict = true;
                            }
                            break;
                        case 9://9精彩瞬间
                            Discover_Article myDiscover_Article = new Discover_Article();
                            myDiscover_Article = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == aboutid);
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
                                if (myDiscover_Article.ArticleContent.Length > 50)
                                {
                                    mySummart = myDiscover_Article.ArticleContent.Substring(0, 50);
                                }
                                else
                                {
                                    mySummart = myDiscover_Article.ArticleContent;
                                }
                                isExict = true;
                            }
                            break;
                    }
                    #endregion
                }
                else
                {//要执行删除操作，直接允许执行
                    isExict = true;
                }
                //
                if (isExict)
                {
                    User_Collect myUser_Collect = myOperating.User_Collect.FirstOrDefault(p => p.ClassId == classid && p.AboutId == aboutid);
                    if (myUser_Collect != null)
                    {
                        try
                        {
                            if (editOrdel)
                            {
                                myUser_Collect.AboutTitle = myTitle;
                                myUser_Collect.AboutImage = myCoverImage;
                                myUser_Collect.AboutSummary = mySummart;
                            }
                            else
                            {
                                myOperating.User_Collect.Remove(myUser_Collect);
                            }
                            myOperating.SaveChanges();
                            OptionComplet = true;
                        }
                        catch
                        {
                            OptionComplet = false;
                        }
                    }
                }
            }
            return OptionComplet;
        }
        #endregion
        //
        #region 阅读播放次数加一
        public bool TimesAdd(int classid, long aboutid)
        {
            bool isOk = false;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                try
                {
                    switch (classid)
                    {///1点一点；2听一听；3看一看；
                        ///4课程讲解；5幼儿公开课；6教育技能；
                        ///7操作说明；8教师社区；9精彩瞬间;12用户录音
                        case 1://点一点
                            Child_Book_Click myChild_Book_Click = new Child_Book_Click();
                            myChild_Book_Click = myOperating.Child_Book_Click.FirstOrDefault(p => p.BookID == aboutid);
                            if (myChild_Book_Click != null)
                            {
                                myChild_Book_Click.ReadCount = myChild_Book_Click.ReadCount + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 2://2听一听
                            Child_Audio_List myChild_Audio_List = new Child_Audio_List();
                            myChild_Audio_List = myOperating.Child_Audio_List.FirstOrDefault(p => p.AudioId == aboutid);
                            if (myChild_Audio_List != null)
                            {
                                myChild_Audio_List.PlayTimes = myChild_Audio_List.PlayTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 3://3看一看
                            Child_Video_List myChild_Video_List = new Child_Video_List();
                            myChild_Video_List = myOperating.Child_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                            if (myChild_Video_List != null)
                            {
                                myChild_Video_List.PlayTimes = myChild_Video_List.PlayTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7://4课程讲解；5幼儿公开课；6教育技能；7操作说明
                            Teacher_Video_List myTeacher_Video_List = new Teacher_Video_List();
                            myTeacher_Video_List = myOperating.Teacher_Video_List.FirstOrDefault(p => p.VideoId == aboutid);
                            if (myTeacher_Video_List != null)
                            {
                                myTeacher_Video_List.PlayTimes = myTeacher_Video_List.PlayTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 8://8教师社区
                            Teacher_Article myTeacher_Article = new Teacher_Article();
                            myTeacher_Article = myOperating.Teacher_Article.FirstOrDefault(p => p.ArticlId == aboutid);
                            if (myTeacher_Article != null)
                            {
                                myTeacher_Article.ReadTimes = myTeacher_Article.ReadTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 9://9精彩瞬间
                            Discover_Article myDiscover_Article = new Discover_Article();
                            myDiscover_Article = myOperating.Discover_Article.FirstOrDefault(p => p.ArticlId == aboutid);
                            if (myDiscover_Article != null)
                            {
                                myDiscover_Article.ReadTimes = myDiscover_Article.ReadTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                        case 12://用户录音
                            Child_Audio_List_User myChild_Audio_List_User = new Child_Audio_List_User();
                            myChild_Audio_List_User = myOperating.Child_Audio_List_User.FirstOrDefault(p => p.AudioUserId == aboutid);
                            if (myChild_Audio_List_User != null)
                            {
                                myChild_Audio_List_User.PlayTimes = myChild_Audio_List_User.PlayTimes + 1;
                                myOperating.SaveChanges();
                                isOk = true;
                            }
                            break;
                    }
                }
                catch
                { }
            }
            return isOk;
        } 
        #endregion
        //
        #region 获取阅读播放次数
        public int GetReadPlayTimes(int classid, int albumid)
        {
            int returnCounts = 0;
            Nullable<int> counts = 0;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                try
                {
                    switch (classid)
                    {///1点一点；2听一听；3看一看；
                        ///4课程讲解；5幼儿公开课；6教育技能；
                        ///7操作说明；8教师社区；9精彩瞬间;12用户录音
                        case 1://点一点
                            counts = myOperating.Child_Book_Click.Where(p => p.BookClassID == albumid).Sum(p => p.ReadCount);
                             break;
                        case 2://2听一听
                             counts = myOperating.Child_Audio_List.Where(p => p.AlbumId == albumid).Sum(p => p.PlayTimes);
                            break;
                        case 3://3看一看
                            counts = myOperating.Child_Video_List.Where(p => p.AlbumId == albumid).Sum(p=>p.PlayTimes);
                            break;
                    }
                }
                catch
                { }
            }
            if (counts != null) int.TryParse(counts.ToString(),out returnCounts);
            return returnCounts;
        }
        #endregion
    }
}