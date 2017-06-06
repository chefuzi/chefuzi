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
    #region 教师社区内容
    public class DiscoverArticleModel
    {
        private long _ArticlId = 0;
        private Nullable<int> _OrderBy = 0;
        private Nullable<int> _CommentCount = 0;
        private Nullable<int> _Status = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public long ArticlId
        {
            get { return _ArticlId; }
            set { _ArticlId = value; }
        }
        [Display(Name = "用户ID：")]
        public string MobilePhone { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "标题：")]
        public string ArticleTitle { get; set; }
        [Display(Name = "内容：")]
        public string ArticleContent { get; set; }
        [Display(Name = "图片：")]
        public string ArticleImages { get; set; }
        [Display(Name = "视频：")]
        public string ArticleVideo { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentCount
        {
            get { return _CommentCount; }
            set { _CommentCount = value; }
        }
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
        [Display(Name = "状态：")]
        public Nullable<int> Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    } 
    #endregion
}