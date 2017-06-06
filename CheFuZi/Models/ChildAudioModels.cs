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
    #region 专辑
    public class AudioAlbumModel
    {
        public Nullable<int> AlbumId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "专辑名称：")]
        public string AlbumTitle { get; set; }
        [Display(Name = "主播：")]
        public string Anchor { get; set; }
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
    public class AudioListModel
    {
        private int _AlbumId = 0;
        //
        public Nullable<long> AudioId { get; set; }
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
        [Display(Name = "专辑名称：")]
        public string AudioTitle { get; set; }
        [Display(Name = "音频地址：")]
        public string AudioUrl { get; set; }
        [Display(Name = "内容/歌词：")]
        public string AudioWords { get; set; }
        [Display(Name = "时长：")]
        public Nullable<int> TimeSeconds { get; set; }
        public Nullable<int> TimeMinute { get; set; }
        [Display(Name = "专辑排序：")]
        public Nullable<int> OrderBy { get; set; }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}
