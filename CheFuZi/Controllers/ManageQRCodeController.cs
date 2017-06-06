using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Diagnostics;
//
using EntityFramework.Extensions;
//
using CheFuZi.Models;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
using CheFuZi.DataOption;
//
using NPOI;

namespace CheFuZi.Controllers
{
    [ValidateInput(false)]
    [Authorize(Roles = "100,101,102,103,104,105")]
    public class ManageQRCodeController : Controller
    {
        #region 内容列表
        public ActionResult ItemList(int currentpage = 1, string del = "", long bookid = 0, int searchBatchNum = 0)
        {
            ViewBag.BookID = 0;//要生成那本书的二维码
            //
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
            ViewBag.Headline = "二维码管理";//栏目主题
            //
            ViewBag.DataList = null;
            ViewBag.RecordItem = null;
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 删除
                if (!String.IsNullOrWhiteSpace(del))
                {
                    Sys_BookStudy_QRCode myDelRecord = myOperating.Sys_BookStudy_QRCode.FirstOrDefault(p => p.QRCode == del);
                    if (myDelRecord != null)
                    {
                        myOperating.Sys_BookStudy_QRCode.Remove(myDelRecord);
                        myOperating.SaveChanges();
                    }
                }
                #endregion
                //
                #region 翻页属性
                int recordCount = 0;
                int pageCount = 0;
                string orderbyfiled = "AddDate";
                //
                //当前页
                int sqlCurrentpage = currentpage;
                if (sqlCurrentpage < 1) sqlCurrentpage = 1;
                //页大小
                int sqlPagesize = 10;
                #endregion
                //
                #region 取出内容
                IQueryable<Sys_BookStudy_QRCode> myIQueryable = null;
                if (bookid > 0)
                {
                    if (searchBatchNum > 0)
                    {
                        ViewBag.BatchNum = searchBatchNum;
                        myIQueryable = myOperating.Sys_BookStudy_QRCode.Where(p => p.BookStudyID == bookid && p.BatchNum == searchBatchNum);
                    }
                    else
                    {
                        myIQueryable = myOperating.Sys_BookStudy_QRCode.Where(p => p.BookStudyID == bookid);
                    }
                }
                else
                {
                    if (searchBatchNum > 0)
                    {
                        ViewBag.BatchNum = searchBatchNum;
                        myIQueryable = myOperating.Sys_BookStudy_QRCode.Where(p => p.BatchNum == searchBatchNum);
                    }
                    else
                    {
                        myIQueryable = myOperating.Sys_BookStudy_QRCode;
                    }
                }

                //
                if (myIQueryable != null)
                {
                    List<Sys_BookStudy_QRCode> BookTable = QueryableExtensions.OrderBy(myIQueryable, orderbyfiled, out recordCount, out pageCount, ref sqlCurrentpage, sqlPagesize, true).ToList();
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
                if (bookid > 0)
                {
                    #region 要开通那本图书
                    Child_Book_Study myChild_Book_Study = new Child_Book_Study();
                    myChild_Book_Study = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == bookid);
                    if (myChild_Book_Study != null)
                    {
                        ViewBag.BookName = myChild_Book_Study.BookName;
                        ViewBag.BookID = myChild_Book_Study.BookID;
                    }
                    #endregion
                }
            }
            return View();
        }
        #endregion
        //
        #region 生成二维码
        public string addQRCode(long BookStudyID, int BatchNum, int CountNum)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //
            int returnStatus = 0;//0未知错误；
            //
            //mobilePhone = myAuthCodeInstance.mobilePhone;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                int isExist = myOperating.Child_Book_Study.Count(p => p.BookID == BookStudyID);
                if (isExist > 0)
                {
                    DateTime nowDate = DateTime.Now;
                    List<Sys_BookStudy_QRCode> listRecord = new List<Sys_BookStudy_QRCode>();
                    #region 保存添加
                    for (int itemNum = 0; itemNum < CountNum; itemNum++)
                    {
                        listRecord.Add(new Sys_BookStudy_QRCode
                        {
                            QRCode = BookStudyID + CreateNewGuidClass.CreateNewGuid(),
                            MobilePhone = "",//如果已经使用为使用者的手机号
                            BookStudyID = BookStudyID,
                            BatchNum = BatchNum,
                            PrintStatus = false,
                            Used = false,
                            Status = 200,
                            UsedDate = nowDate,
                            AddDate = nowDate,

                        });
                        if ((itemNum != 0) && (itemNum % 500) == 0)
                        {
                            myOperating.Sys_BookStudy_QRCode.AddRange(listRecord);
                            myOperating.SaveChanges();
                            listRecord.Clear();
                        }
                    }
                    //
                    try
                    {
                        if (listRecord.Count > 0)
                        {
                            myOperating.Sys_BookStudy_QRCode.AddRange(listRecord);
                            myOperating.SaveChanges();
                            returnStatus = 200;//完成
                        }
                    }
                    catch
                    {
                        returnStatus = -1;//系统错误
                    }
                    #endregion
                    //
                }
                else
                {
                    returnStatus = 0;//系统错误
                }

            }
            //
            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Debug.WriteLine(ts2.TotalMilliseconds.ToString());
            return returnStatus.ToString();
        }
        #endregion
        //
        #region 根据批次和书号导出excel
        public void ExportExcel(int batchNum, long bookid)
        {
            //
            int rowNum = 0;
            string saveFileName = batchNum.ToString() + "_" + bookid.ToString();
            string xlsSheetName = "";
            ///
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 要开通那本图书
                Child_Book_Study myChild_Book_Study = new Child_Book_Study();
                myChild_Book_Study = myOperating.Child_Book_Study.FirstOrDefault(p => p.BookID == bookid);
                if (myChild_Book_Study != null)
                {
                    xlsSheetName = myChild_Book_Study.BookName;
                    saveFileName = myChild_Book_Study.BookName;
                }
                #endregion
                //
                if (!String.IsNullOrWhiteSpace(saveFileName))
                {
                    NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    NPOI.SS.UserModel.ISheet sheet = book.CreateSheet(xlsSheetName);
                    // 添加数据
                    IQueryable<Sys_BookStudy_QRCode> myDataTable = myOperating.Sys_BookStudy_QRCode.Where(p => p.BatchNum == batchNum && p.BookStudyID == bookid);
                    NPOI.SS.UserModel.IRow row = null;
                    foreach (Sys_BookStudy_QRCode item in myDataTable)
                    {
                        row = sheet.CreateRow(rowNum);
                        NPOI.SS.UserModel.ICell cell = row.CreateCell(0);
                        cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                        cell.SetCellValue(StaticVarClass.QRCodeUrl +"1/"+ item.QRCode);
                        sheet.SetColumnWidth(0, 20000);
                        rowNum++;
                    }
                    #region 导出文件
                    if (rowNum > 0)
                    {
                        // 写入 
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        book.Write(ms);
                        book = null;
                        //
                        int updateCount = myOperating.Sys_BookStudy_QRCode.Where(p => p.BatchNum == batchNum && p.BookStudyID == bookid && p.PrintStatus == false).Update(q => new Sys_BookStudy_QRCode() { PrintStatus = true });
                        if (updateCount>0)
                        {
                            EFCachClear.ClearTable("Sys_BookStudy_QRCode");
                        }
                        //
                        Response.ClearHeaders();
                        Response.Clear();
                        Response.Expires = 0;
                        Response.Buffer = true;
                        Response.AddHeader("Accept-Language", "zh-tw");
                        //
                        saveFileName = saveFileName + "_" + batchNum.ToString() + "_" + rowNum + ".xls";
                        //
                        Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(saveFileName, System.Text.Encoding.UTF8));
                        Response.ContentType = "application/octet-stream;charset=gbk";
                        Response.BinaryWrite(ms.ToArray());
                        ms.Close();
                        ms.Dispose();
                        Response.End();
                    }
                    else
                    {
                        Response.AddHeader("Accept-Language", "zh-tw");
                        Response.Write("没有任何数据导出，请选择图书和批次。");
                        Response.End();
                    } 
                    #endregion
                }
            }
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

}
