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
    #region 类别
    public class VideoClassModel
    {
        [Range(1, int.MaxValue)]
        public Nullable<int> ClassId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "类别名称：")]
        public string ClassTitle { get; set; }
        [Display(Name = "排序：")]
        public Nullable<int> OrderBy { get; set; }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }

    } 
    #endregion
    //
    #region 专辑
    public class VideoAlbumModel
    {
         [Range(1, int.MaxValue)]
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> AlbumId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "专辑名称：")]
        public string AlbumTitle { get; set; }
        [Display(Name = "专辑封面：")]
        public string AlbumImage { get; set; }
        [Display(Name = "专辑简介：")]
        public string AlbumDescrib { get; set; }
        [Display(Name = "专辑排序：")]
        public Nullable<int> OrderBy { get; set; }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    } 
    #endregion
    //
    #region 内容
    public class VideoListModel
    {
        private int _AlbumId = 0;
        //
        public Nullable<long> VideoId { get; set; }
        public int AlbumId { 
            get{
                return _AlbumId;
            }
            set {
                _AlbumId = value;
            }
        }
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
        [Display(Name = "时长：")]
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> TimeMinute { get; set; }
        [Display(Name = "排序：")]
        public Nullable<int> OrderBy { get; set; }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}
