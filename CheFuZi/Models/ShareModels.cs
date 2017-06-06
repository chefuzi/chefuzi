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
    #region 分享的内容
    public class ShareModel
    {
        private int _ClassType = 0;
        //
        private long _AboutId = 0;
        private int _ClassId = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public long AboutId
        {
            get { return _AboutId; }
            set { _AboutId = value; }
        }
        public int ClassType
        {
            get { return _ClassType; }
            set { _ClassType = value; }
        }
        public int ClassId
        {
            get { return _ClassId; }
            set { _ClassId = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "标题：")]
        public string AboutTitle { get; set; }
        [Display(Name = "作者：")]
        public string AboutAuthor { get; set; }
        [Display(Name = "简述：")]
        public string AboutSummary { get; set; }
        [Display(Name = "内容：")]
        public string AboutContent { get; set; }
        [Display(Name = "图片：")]
        public string AboutImages { get; set; }
        [Display(Name = "视频：")]
        public string AboutVideo { get; set; }
        [Display(Name = "音乐：")]
        public string AboutAudio { get; set; }
        public Nullable<int> ReadTimes { get; set; }
        public Nullable<int> CommentCount { get; set; }
        public System.DateTime AddDate
        {
            get { return _AddDate; }
            set { _AddDate = value; }
        }
    } 
    #endregion
}