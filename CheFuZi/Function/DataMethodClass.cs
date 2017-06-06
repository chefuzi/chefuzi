using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
//
using EntityFramework.Extensions;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;
using CheFuZi.ExternalMethod;
namespace CheFuZi.Function
{
    public class DataMethodClass
    {
        #region 检查验证码是否正确
        public bool lookCheckCode(string username, string myCode)
        {
            bool isOk = false;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                DateTime myNowDate = DateTime.Now;
                int isExist = myOperating.User_CheckCode.Count(p => p.MobilePhone == username && p.CheckCode == myCode && p.ExpiredDate > myNowDate);
                if (isExist > 0)
                {
                    isOk = true;
                }
                else
                {
                    isOk = false;
                }
            }
            return isOk;
        }
        #endregion
        //
        #region 获取用户昵称和头像
        public bool GetNickNameAndPic(string username, out string nickname, out string headimage)
        {
            bool isOk = false;
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 昵称和头像
                headimage = StaticVarClass.DefaultHeadImage;//头像
                nickname = "匿名";//昵称
                if (!String.IsNullOrWhiteSpace(username))
                {
                    User_UserName myUserNameTable = myOperating.User_UserName.FirstOrDefault(p => p.MobilePhone == username);
                    if (myUserNameTable != null)
                    {
                        if (!String.IsNullOrWhiteSpace(myUserNameTable.NickName))
                        {//昵称
                            nickname = myUserNameTable.NickName;
                        }
                        else
                        {
                            if (myUserNameTable.MobilePhone.Length > 7)
                            {
                                nickname = myUserNameTable.MobilePhone.Substring(7) + "****";
                            }
                        }
                        //
                        if (!String.IsNullOrWhiteSpace(myUserNameTable.HeadImage))
                        {//头像
                            headimage = StaticVarClass.myDomain+myUserNameTable.HeadImage;
                        }
                        isOk = true;
                    }
                }
                #endregion
            }
            return isOk;
        }
        #endregion
        //
        #region 检查是否收藏
        public bool CollectExist(int classid, long aboutid,bool isTeacherVideo = false)
        {//
            //
            bool isExist = false;
            //
            #region 检查授权
            string mobilePhone = "";//手机号
            AuthCodeInstance myAuthCodeInstance = new AuthCodeInstance();
            AuthCodeClass myAuthCodeClass = new AuthCodeClass();
            bool checkAuthCodeBool = myAuthCodeClass.checkAuthCode(out myAuthCodeInstance);
            #endregion
            if (checkAuthCodeBool)
            {
                mobilePhone = myAuthCodeInstance.mobilePhone;
                //
                using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
                {
                    int myCount = 0;
                    //
                    try
                    {
                        if (isTeacherVideo)
                        {//classid=45679
                            myCount = myOperating.User_Collect.Count(p => p.MobilePhone == mobilePhone && p.IsTeacherVideo == true && p.AboutId == aboutid);
                        }
                        else
                        {
                            myCount = myOperating.User_Collect.Count(p => p.MobilePhone == mobilePhone && p.ClassId == classid && p.AboutId == aboutid);
                        }
                        
                        if (myCount > 0)
                        {
                            isExist = true;
                        }
                    }
                    catch
                    {
                        isExist = false;
                    }
                }
            }
            return isExist;
        }
        #endregion
        //
        #region 获得听一听大类名称
        public string GetAudioClassName(Nullable<int> classid)
        {
            string myClassName = "";
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作
            {
                #region 所属类别名
                try
                {
                    myClassName = myOperating.Child_Audio_Class.FirstOrDefault(p => p.ClassId == classid).ClassTitle;
                }
                catch
                { }
                #endregion
            }
            return myClassName;
        }
        #endregion

    }
}