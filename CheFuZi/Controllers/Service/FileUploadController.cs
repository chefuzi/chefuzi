using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

//
using CheFuZi.Function;
using CheFuZi.DataReturn;
using FolderActions;

namespace CheFuZi.Controllers.Service
{
    public class FileUploadController : Controller
    {

        #region 手机上传文件
        [HttpPost]
        public JsonResult UpdateFile(string filetype, string upfilename)
        {
            StatusData myStatusData = new StatusData();//返回的类型
            #region 检查授权
            string mobilePhone = "";
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            //
            string myPath = "";
            string fileExt = "";
            string myFolder = "";
            int RandKey = 1000;
            string fileName = "";
            string filePath = "";

            //
            #region 判断是否续传
            string upfilenamefullpath = "";
            bool xuchuan = false;
            //
            if (!String.IsNullOrWhiteSpace(upfilename))
            {
                upfilenamefullpath = Path.Combine(HttpRuntime.AppDomainAppPath, upfilename);
                if (System.IO.File.Exists(upfilenamefullpath))
                {
                    xuchuan = true;
                }
                else
                {
                    xuchuan = false;
                }
            }
            #endregion
            //
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
                try
                {
                    if (!xuchuan)
                    {
                        #region 不是续传，获得文件相关属性
                        folderActions myForderObject = new folderActions();
                        //
                        myPath = StaticVarClass.ImageFolderCfg + "/" + DateTime.Now.Year.ToString() + "/" + mobilePhone + "/" + DateTime.Now.Month.ToString();
                        fileExt = filetype;
                        #region 判断文件类型
                        if (!String.IsNullOrWhiteSpace(fileExt))
                        {
                            fileExt = fileExt.ToLower();
                        }
                        else
                        {
                            fileExt = "";
                        }
                        switch (fileExt)
                        {
                            case "jpg":
                            case "png":
                            case "gif":
                            case "jpeg":
                                myPath = StaticVarClass.ImageFolderCfg;
                                break;
                            case "mp3":
                            case "aac":
                                myPath = StaticVarClass.AudioFolderCfg;
                                break;
                            case "mov":
                            case "mp4":
                                myPath = StaticVarClass.VideoFolderCfg;
                                break;
                            default:
                                myStatusData.operateStatus = 400;
                                myStatusData.operateMsg = "文件类型错误";
                                return Json(myStatusData);
                        }
                        myPath = myPath + "/" + DateTime.Now.Year.ToString() + "/" + mobilePhone + "/" + DateTime.Now.Month.ToString();
                        #endregion
                        //
                        myForderObject.setFolderFullName = myPath;
                        myForderObject.CreateFolder();
                        myFolder = myForderObject.getFolderRealPath;
                        Random ran = new Random(unchecked((int)DateTime.Now.Ticks));
                        RandKey = ran.Next(1, 9999);
                        fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + RandKey.ToString();
                        filePath = myFolder + "/" + fileName + "." + fileExt;
                        #endregion
                    }
                    else
                    {//续传，直接用文件路径
                        filePath = upfilenamefullpath;
                    }
                    //
                    try
                    {
                        if ((!xuchuan) && (!System.IO.File.Exists(filePath)))
                        {
                            #region 新文件
                            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                //将字符信息写入文件系统
                                System.IO.Stream s = HttpContext.Request.InputStream;
                                if (s.Length > 10)
                                {
                                    int count = 0;
                                    byte[] buffer = new byte[1024];
                                    while ((count = s.Read(buffer, 0, 1024)) > 0)
                                    {
                                        stream.Write(buffer, 0, count);
                                    }
                                    s.Flush();
                                    s.Close();
                                    s.Dispose();

                                    UploadFileClass myUploadFileClass = new UploadFileClass();
                                    myUploadFileClass.filename = myPath + "/" + fileName + "." + fileExt;
                                    myUploadFileClass.fileurl = myPath + "/" + fileName + "." + fileExt;
                                    //myUploadFileClass.fileurl = StaticVarClass.myDomain + myPath + "/" + fileName + "." + fileExt;
                                    myStatusData.dataTable = myUploadFileClass;
                                    myStatusData.operateStatus = 200;
                                }
                                else
                                {
                                    myStatusData.operateMsg = "文件为空";
                                    myStatusData.operateStatus = 0;
                                }
                            }
                            //
                            #endregion
                        }
                        else if (xuchuan)
                        {
                            #region 续传文件
                            using (Stream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                            {
                                //将字符信息写入文件系统
                                System.IO.Stream s = HttpContext.Request.InputStream;
                                if (s.Length > 10)
                                {
                                    int count = 0;
                                    byte[] buffer = new byte[1024];
                                    while ((count = s.Read(buffer, 0, 1024)) > 0)
                                    {
                                        stream.Write(buffer, 0, count);
                                    }
                                    s.Flush();
                                    s.Close();
                                    s.Dispose();
                                    //
                                    UploadFileClass myUploadFileClass = new UploadFileClass();
                                    myUploadFileClass.filename = upfilename;
                                    myUploadFileClass.fileurl = upfilename;
                                    //myUploadFileClass.fileurl = StaticVarClass.myDomain + upfilename;
                                    myStatusData.dataTable = myUploadFileClass;
                                    myStatusData.operateStatus = 200;
                                }
                                else
                                {
                                    myStatusData.operateMsg = "文件为空";
                                    myStatusData.operateStatus = 0;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            myStatusData.operateStatus = 400;
                            myStatusData.operateMsg = "文件已经存在";
                        }
                    }
                    catch
                    {//发生错误删除文件
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        myStatusData.operateStatus = -1;
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
    }
    //
    #region 返回文件结构
    public class UploadFileClass
    {
        public string filename { get; set; }
        public string fileurl { get; set; }
    }
    #endregion
}
