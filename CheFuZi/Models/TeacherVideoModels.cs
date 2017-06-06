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
    public class TeacherVideoListModel
    {
        private int _ClassId = 0;
        private int _LessonId = 0;
        private Nullable<int> _Recommended = 0;
        //
        public Nullable<long> VideoId { get; set; }
        public int ClassId
        {
            get
            {
                return _ClassId;
            }
            set
            {
                _ClassId = value;
            }
        }
        public int LessonId
        { 
            get{
                return _LessonId;
            }
            set {
                _LessonId = value;
            }
        }
//
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "视频名称：")]
        public string VideoTitle { get; set; }
        [Display(Name = "封面图片：")]
        public string VideoImage { get; set; }
        [Display(Name = "视频地址：")]
        public string VideoUrl { get; set; }
        [Display(Name = "简介：")]
        public string VideoDes { get; set; }
        [Display(Name = "推荐：")]
        public Nullable<int> Recommended {
            get
            {
                return _Recommended;
            }
            set
            {
                _Recommended = value;
            }
        }
        [Display(Name = "时长：")]
        public Nullable<int> TimeSeconds { get; set; }
        [Display(Name = "时长：")]
        public Nullable<int> TimeMinute { get; set; }
        [Display(Name = "排序：")]
        public Nullable<int> OrderBy { get; set; }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}
