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
    public class ManageGameController : Controller
    {
        #region 内容列表
        public ActionResult ItemList(int currentpage = 1, long del = 0)
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
            ViewBag.Headline = "游戏管理";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (del > 0)
                {
                    Game_List myDelRecord = myOperating.Game_List.FirstOrDefault(p => p.GameId == del);
                    if (myDelRecord != null)
                    {
                        FunctionClass.delFile(myDelRecord.GameImage);
                        myOperating.Game_List.Remove(myDelRecord);
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
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Game_List> myIQueryable = null;

                myIQueryable = myOperating.Game_List;
                //
                if (myIQueryable != null)
                {
                    List<Game_List> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
                //
            }
            return View();
        }
        #endregion
        //
        #region 游戏添加-编辑
        public ActionResult ItemAdd(GameModel model, string ReturnUrl, Nullable<long> myid = 0)
        {
            #region 获取来路路径
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery;
            }
            #endregion
            ViewBag.Headline = "游戏添加";//栏目主题
            ViewBag.ButtonValue = "添加";//按钮名称
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                if ((myid > 0) && (model.Operate == null))
                {
                    long mySelfId = 0;
                    long.TryParse(myid.ToString(), out mySelfId);
                    ViewBag.Headline = "游戏编辑";
                    ViewBag.ButtonValue = "修改";
                    model.Operate = "edit";
                    //
                    #region 取出数据
                    Game_List editRecord = myOperating.Game_List.FirstOrDefault(p => p.GameId == mySelfId);
                    if (editRecord != null)
                    {
                        model.GameId = editRecord.GameId;
                        model.GameName = editRecord.GameName;
                        model.GameImage = editRecord.GameImage;
                        model.GameDescribe = editRecord.GameDescribe;
                        model.GameUrl = editRecord.GameUrl;
                        model.OrderBy = editRecord.OrderBy;
                    }
                    #endregion
                }
                else if (model.Operate == "add")
                {
                    #region 保存添加
                    if (ModelState.IsValid)
                    {
                        Game_List addRecord = new Game_List();
                        addRecord.GameName = model.GameName;
                        addRecord.GameImage = model.GameImage;
                        addRecord.GameDescribe = model.GameDescribe;
                        addRecord.GameUrl = model.GameUrl;
                        addRecord.OrderBy = model.OrderBy;
                        addRecord.AddDate = DateTime.Now;
                        addRecord.PlayTimes = 0;
                        addRecord.GameClass = 1;
                        addRecord.Status = 200;
                        //
                        myOperating.Game_List.Add(addRecord);
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
                    //
                    #region 保存编辑
                    if (ModelState.IsValid)
                    {
                        Game_List editSaveRecord = myOperating.Game_List.FirstOrDefault(p => p.GameId == model.GameId);
                        if (editSaveRecord != null)
                        {
                            editSaveRecord.GameName = model.GameName;
                            editSaveRecord.GameImage = model.GameImage;
                            editSaveRecord.GameDescribe = model.GameDescribe;
                            editSaveRecord.GameUrl = model.GameUrl;
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
                    model.Operate = "add";
                    #region 默认值
                    try
                    {
                        model.OrderBy = myOperating.Game_List.Max(p => p.OrderBy) + 1;
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
}
