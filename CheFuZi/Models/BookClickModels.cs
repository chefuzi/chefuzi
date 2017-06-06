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
    public class BookClickModel
    {
        private long _BookID = 0;
        private Nullable<int> _BookClassID = 0;
        private bool _ScreenH = true;//true横屏,false竖屏
        
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
        [Display(Name = "存放服务器路径：")]
        public string BookOnlineUrl { get; set; }
        public bool ScreenH
        {
            get { return _ScreenH; }
            set { _ScreenH = value; }
        }
        
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
    public class BookClickClassModel
    {
        private int _BookClassID = 0;
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
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}