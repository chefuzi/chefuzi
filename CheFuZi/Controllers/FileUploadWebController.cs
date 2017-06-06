using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
//
using CheFuZi.Function;
using CheFuZi.DataReturn;
using FolderActions;

namespace CheFuZi.Controllers
{
    [ValidateInput(false)]
    [Authorize(Roles = "100,101,102,103,104,105")]
    public class FileUploadWebController : Controller
    {
        [HttpPost]
        #region 通过网页上传
        public string FileUpLoad()
        {
            string UpLoadFileUrl = "";//
            string mobilePhone = "defalut";
            //
            int _sizeMax = 500 * (1024 * 1024);//500Mb
            string myPath = "";
            string myFolder = "";
            int RandKey = 1000;
            string filePath = "";
            if (User.Identity.IsAuthenticated)
            {
                mobilePhone = User.Identity.Name;
                //文件夹不存在则创建
                HttpFileCollection _fileList = System.Web.HttpContext.Current.Request.Files;
                int fileCount = _fileList.Count;
                if (fileCount > 0)
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        try
                        {
                            System.Web.HttpPostedFile postedFile = _fileList[i];
                            string fileName = postedFile.FileName;
                            if (fileName != string.Empty)
                            {
                                int mySize = postedFile.ContentLength;
                                if (mySize >= _sizeMax) //判断上传的大小
                                {
                                    return "文件太大";
                                }
                                else if (mySize > 10)
                                {
                                    string fileExt = fileName.Substring(fileName.LastIndexOf(".")).ToLower();    //图片类型
                                    folderActions myForderObject = new folderActions();
                                    //
                                    myPath = StaticVarClass.ImageFolderCfg + "/" + DateTime.Now.Year.ToString() + "/" + mobilePhone + "/" + DateTime.Now.Month.ToString();
                                    #region 判断文件类型
                                    switch (fileExt)
                                    {
                                        case ".jpg":
                                        case ".png":
                                        case ".gif":
                                        case ".jpeg":
                                        case ".bmp":
                                            myPath = StaticVarClass.ImageFolderCfg;
                                            break;
                                        case ".mp3":
                                        case ".aac":
                                            myPath = StaticVarClass.AudioFolderCfg;
                                            break;
                                        case ".mp4":
                                            myPath = StaticVarClass.VideoFolderCfg;
                                            break;
                                        default:
                                            return "文件类型错误";
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
                                    filePath = myFolder + "/" + fileName + fileExt;

                                    //
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                    postedFile.SaveAs(filePath);
                                    UpLoadFileUrl = myPath + "/" + fileName + fileExt;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            return "发生未知错误";
                        }
                    }
                }
                else
                {
                    return "请选择要上传的文件";
                }
            }
            else
            {
                return "授权码无效";//授权码无效
            }
            return UpLoadFileUrl;
        }
        #endregion
        //
        #region 通过编辑器上传图片
        public void ImageUpLoadEditor()
        {
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            string callback = System.Web.HttpContext.Current.Request["callback"];
            string editorId = System.Web.HttpContext.Current.Request["editorid"];
            Hashtable infoList = new Hashtable();
            //-----------------------------
            bool uploadSuccess = true;
            string uploadMsg = "";
            //=================================
            string UpLoadFileUrl = "";//
            string mobilePhone = "defalut";
            //
            int _sizeMax = 500 * (1024 * 1024);//500Mb
            string myPath = "";
            string myFolder = "";
            int RandKey = 1000;
            string filePath = "";
            int mySize = 0;//文件大小
            string fileExt = "";//文件类型
            if (User.Identity.IsAuthenticated)
            {
                mobilePhone = User.Identity.Name;
                //文件夹不存在则创建
                HttpFileCollection _fileList = System.Web.HttpContext.Current.Request.Files;
                int fileCount = _fileList.Count;
                if (fileCount > 0)
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        try
                        {
                            System.Web.HttpPostedFile postedFile = _fileList[i];
                            string fileName = postedFile.FileName;
                            if (fileName != string.Empty)
                            {
                                mySize = postedFile.ContentLength;
                                if (mySize >= _sizeMax) //判断上传的大小
                                {
                                    uploadSuccess = false;
                                    uploadMsg = "文件大于" + (_sizeMax / (1024 * 1024)).ToString() + "MB";
                                    // "文件太大";
                                }
                                else if (mySize > 10)
                                {
                                    fileExt = fileName.Substring(fileName.LastIndexOf(".")).ToLower();    //图片类型
                                    folderActions myForderObject = new folderActions();
                                    //
                                    myPath = StaticVarClass.ImageFolderCfg + "/" + DateTime.Now.Year.ToString() + "/" + mobilePhone + "/" + DateTime.Now.Month.ToString();
                                    #region 判断文件类型
                                    switch (fileExt)
                                    {
                                        case ".jpg":
                                        case ".png":
                                        case ".gif":
                                        case ".jpeg":
                                        case ".bmp":
                                            myPath = StaticVarClass.ImageFolderCfg;
                                            break;
                                        case ".mp3":
                                        case ".aac":
                                            myPath = StaticVarClass.AudioFolderCfg;
                                            break;
                                        case ".mp4":
                                            myPath = StaticVarClass.VideoFolderCfg;
                                            break;
                                        default:
                                            uploadSuccess = false;
                                            uploadMsg = "文件类型错误";
                                            break;
                                    }
                                    if (uploadSuccess)
                                    {
                                        myPath = myPath + "/" + DateTime.Now.Year.ToString() + "/" + mobilePhone + "/" + DateTime.Now.Month.ToString();
                                    #endregion
                                        //
                                        myForderObject.setFolderFullName = myPath;
                                        myForderObject.CreateFolder();
                                        myFolder = myForderObject.getFolderRealPath;
                                        Random ran = new Random(unchecked((int)DateTime.Now.Ticks));
                                        RandKey = ran.Next(1, 9999);
                                        fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + RandKey.ToString();
                                        filePath = myFolder + "/" + fileName + fileExt;

                                        //
                                        if (System.IO.File.Exists(filePath))
                                        {
                                            System.IO.File.Delete(filePath);
                                        }
                                        postedFile.SaveAs(filePath);
                                        UpLoadFileUrl = myPath + "/" + fileName + fileExt;
                                        break;
                                    }
                                }
                                else
                                {
                                    uploadSuccess = false;
                                    uploadMsg = "文件为空";
                                }
                            }
                            else
                            {
                                uploadSuccess = false;
                                uploadMsg = "文件为空";
                            }
                        }
                        catch
                        {
                            uploadSuccess = false;
                            uploadMsg = "发生未知错误";
                        }
                    }
                }
                else
                {
                    uploadSuccess = false;
                    uploadMsg = "请选择要上传的文件";
                }
            }
            else
            {
                uploadSuccess = false;
                uploadMsg = "授权码无效";
            }
            //==============================
            if (uploadSuccess)
            {
                infoList.Add("state", "SUCCESS");
                infoList.Add("url", StaticVarClass.myDomain+UpLoadFileUrl);
                infoList.Add("originalName", "children.png");
                infoList.Add("name", "45c3bdfa-46ac-49cd-b299-889940307e0e.png");
                infoList.Add("size", mySize);
                infoList.Add("type", fileExt);
            }
            else
            {
                infoList.Add("state", "FAIL");
                infoList.Add("url", StaticVarClass.myDomain + UpLoadFileUrl);
                infoList.Add("originalName", "children.png");
                infoList.Add("name", "45c3bdfa-46ac-49cd-b299-889940307e0e.png");
                infoList.Add("size", mySize);
                infoList.Add("type", fileExt);
            }
            string json = BuildJson(infoList);
            System.Web.HttpContext.Current.Response.ContentType = "text/html";
            if (callback != null)
            {
                System.Web.HttpContext.Current.Response.Write(String.Format("<script>{0}(JSON.parse(\"{1}\"));</script>", callback, json));
            }
            else
            {
                System.Web.HttpContext.Current.Response.Write(json);
            }
        }
        #endregion
        #region Json生成
        private string BuildJson(Hashtable info)
        {
            List<string> fields = new List<string>();
            string[] keys = new string[] { "originalName", "name", "url", "size", "state", "type" };
            for (int i = 0; i < keys.Length; i++)
            {
                fields.Add(String.Format("\"{0}\": \"{1}\"", keys[i], info[keys[i]]));
            }
            return "{" + String.Join(",", fields) + "}";
        } 
        #endregion
    }

}
