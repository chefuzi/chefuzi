using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheFuZi.DataReturn
{
    #region 电子书数据结构
    public class StudyBookItem
    {
        private Nullable<int> _ImgWidth = 0;
        private Nullable<int> _ImgHeight = 0;
        public long BookID { get; set; }
        public string BookName { get; set; }
        public string BookOnlineUrl { get; set; }
        public string BookDownLoadZip { get; set; }
        public string BookZipName { get; set; }
        public bool IsReg { get; set; }
        //
        public Nullable<int> ImgWidth { get { return _ImgWidth; } set { _ImgWidth = value; } }
        public Nullable<int> ImgHeight { get { return _ImgHeight; } set { _ImgHeight = value; } }
    }
    #endregion
    //
    #region 电子书阅读天数数据结构
    public class ClickBookReadDaysItem
    {
        public string MobilePhone { get; set; }
        public string HeadImage { get; set; }
        public string NickName { get; set; }
        public int ReadDayCount { get; set; }
    }
    #endregion
}