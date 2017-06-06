using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace CheFuZi.Models
{
    #region 内容
    public class BookStudyModel
    {
        private long _BookID = 0;
        private Nullable<int> _BookClassID = 0;
        private Nullable<int> _OrderBy = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public long BookID
        {
            get { return _BookID; }
            set { _BookID = value; }
        }
        public Nullable<int> BookClassID
        {
            get { return _BookClassID; }
            set { _BookClassID = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "名称：")]
        public string BookName { get; set; }
        [Display(Name = "在线阅读服务器路径：")]
        public string BookOnlineUrl { get; set; }
        [Display(Name = "Zip压缩包服务器路径：")]
        public string BookDownLoadZip { get; set; }
        [Display(Name = "Zip压缩包文件名：")]
        public string BookZipName { get; set; }

        [Display(Name = "排序：")]
        public Nullable<int> OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
    //
    #region 类别
    public class BookStudyClassModel
    {
        private int _BookClassID = 0;
        private Nullable<int> _ImgWidth = 0;
        private Nullable<int> _ImgHeight = 0;
        private Nullable<int> _OrderBy = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public int BookClassID
        {
            get { return _BookClassID; }
            set { _BookClassID = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "名称：")]
        public string BookClassName { get; set; }
        public System.DateTime AddDate
        {
            get { return _AddDate; }
            set { _AddDate = value; }
        }
        [Display(Name = "排序：")]
        public Nullable<int> OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }
        public Nullable<int> ImgWidth { get { return _ImgWidth; } set { _ImgWidth = value; } }
        public Nullable<int> ImgHeight { get { return _ImgHeight; } set { _ImgHeight = value; } }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}