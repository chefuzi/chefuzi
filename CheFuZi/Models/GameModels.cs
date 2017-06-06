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
    public class GameModel
    {
        private long _GameId = 0;
        private Nullable<long> _OrderBy = 0;
        private DateTime _AddDate = DateTime.Now;
        //
        public long GameId
        {
            get { return _GameId; }
            set { _GameId = value; }
        }
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        [Display(Name = "名称：")]
        public string GameName { get; set; }
        [Display(Name = "游戏封面图：")]
        public string GameImage { get; set; }
        [Display(Name = "简介：")]
        public string GameDescribe { get; set; }
        [Display(Name = "游戏URL地址：")]
        public string GameUrl { get; set; }

        [Display(Name = "排序：")]
        public Nullable<long> OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }
        //操作类型add,save,edit,editsave
        public string Operate { get; set; }
    }
    #endregion
}