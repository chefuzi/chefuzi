using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
//
using EntityFramework.Extensions;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
namespace CheFuZi.Controllers.Service
{
    public class PublicController : Controller
    {
        #region 省份列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetProvince()
        {
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                try
                {
                    var myDataTable = myOperating.Sys_Area_Province.OrderBy(p => p.OrderBy).Select(s => new { ProvinceName = s.ProvinceName, ProvinceNum = s.ProvinceNum }).ToList();
                    myStatusData.operateStatus = 200;//登录成功
                    myStatusData.dataTable = myDataTable;
                }
                catch
                {
                    myStatusData.operateStatus = -1;
                }
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 城市列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetCity(string province)
        {
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                try
                {
                    long myLong = 0;
                    bool isNumber = long.TryParse(province, out myLong);
                    if (isNumber)
                    {
                        var myDataTable = myOperating.Sys_Area_City.Where(p => p.ProvinceID == myLong).OrderBy(p => p.OrderBy).Select(s => new { CityName = s.CityName, CityNum = s.CityNum }).ToList();
                        myStatusData.operateStatus = 200;
                        myStatusData.dataTable = myDataTable;
                    }
                    else
                    {
                        myStatusData.operateStatus = 200;
                    }
                }
                catch
                {
                    myStatusData.operateStatus = -1;
                }
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 区县列表
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetDistrict(string city)
        {
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                try
                {
                    long myLong = 0;
                    bool isNumber = long.TryParse(city, out myLong);
                    if (isNumber)
                    {
                        var myDataTable = myOperating.Sys_Area_District.Where(p => p.CityID == myLong).OrderBy(p => p.OrderBy).Select(s => new { DistrictName = s.DistrictName, DistrictNum = s.DistrictNum }).ToList();
                        myStatusData.operateStatus = 200;
                        myStatusData.dataTable = myDataTable;
                    }
                    else
                    {
                        myStatusData.operateStatus = 200;
                    }
                }
                catch
                {
                    myStatusData.operateStatus = -1;
                }
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 性别true男false女
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetSex()
        {
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            SexDataList myData = new SexDataList();
            try
            {
                myStatusData.operateStatus = 200;
                myStatusData.dataTable = myData;
            }
            catch
            {
                myStatusData.operateStatus = -1;
            }
            return Json(myStatusData);
        }
        #endregion
        //
        #region 身份类别
        [HttpPost]
        [OutputCache(CacheProfile = "commoncache")]
        public JsonResult GetRole()
        {
            //
            StatusData myStatusData = new StatusData();//返回状态
            //
            RoleDataList myData = new RoleDataList();
            try
            {
                myStatusData.operateStatus = 200;
                myStatusData.dataTable = myData;
            }
            catch
            {
                myStatusData.operateStatus = -1;
            }
            return Json(myStatusData);
        }
        #endregion
    }
}
