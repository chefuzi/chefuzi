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
    public class TeacherArticleModel
    {
        private long _ArticlId = 0;
        private int _ClassId = 0;
        private Nullable<int> _OrderBy = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public long ArticlId
        {
            get { return _ArticlId; }
            set { _ArticlId = value; }
        }
        public int ClassId
        {
            get { return _ClassId; }
            set { _ClassId = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "标题：")]
        public string ArticleTitle { get; set; }
        [Display(Name = "作者：")]
        public string ArticleAuthor { get; set; }
        [Display(Name = "简述：")]
        public string ArticleSummary { get; set; }
        [Display(Name = "内容：")]
        public string ArticleContent { get; set; }
        [Display(Name = "封面图片：")]
        public string ArticleImages { get; set; }
        [Display(Name = "相关视频：")]
        public string ArticleVideo { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentCount { get; set; }
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
    //
    #region 文章类别
    public class TeacherArticleClassModel
    {
        private int _ClassId = 0;
        private Nullable<int> _OrderBy = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public int ClassId
        {
            get { return _ClassId; }
            set { _ClassId = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "名称：")]
        public string ClassTitle { get; set; }
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