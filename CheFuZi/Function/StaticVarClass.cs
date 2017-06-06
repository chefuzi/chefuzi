using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CheFuZi.Function
{
    public static class StaticVarClass
    {
        //登录有效期，单位天
        public static double LoginExpiredDate = double.Parse(ConfigurationManager.AppSettings["LoginExpiredDate"]);
        //验证码过期日期
        public static string CheckCodeExpiredDate = ConfigurationManager.AppSettings["CheckCodeExpiredDate"];
        //点一点资源地址
        public static string BookClickResourceUrl = ConfigurationManager.AppSettings["BookClickResourceUrl"];
        //学一学资源地址
        public static string BookStudyResourceUrl = ConfigurationManager.AppSettings["BookStudyResourceUrl"];
        //公开访问URL地址
        public static string myDomain = ConfigurationManager.AppSettings["DomainUrl"];
        //上传文件保存地址
        public static string ImageFolderCfg = ConfigurationManager.AppSettings["ImageFolder"];
        public static string VideoFolderCfg = ConfigurationManager.AppSettings["VideoFolder"];
        public static string AudioFolderCfg = ConfigurationManager.AppSettings["AudioFolder"];
        //默认头像
        public static string DefaultHeadImage = myDomain + "defaultHeadImage.png";
        //扫码前缀网址识别
        public static string QRCodeUrl = ConfigurationManager.AppSettings["QRCodeUrl"];
        //APP下载地址
        public static string AppDownLoadUrlAndroid = ConfigurationManager.AppSettings["AppDownLoadUrlAndroid"];
        public static string AppDownLoadUrlIOS = ConfigurationManager.AppSettings["AppDownLoadUrlIOS"];
    }

}