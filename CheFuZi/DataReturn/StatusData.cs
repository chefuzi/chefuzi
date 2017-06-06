using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CheFuZi.DataReturn
{
    #region 返回数据
    public class StatusData
    {
        int _operateStatus = 0;//用户状态
        /* -1：系统错误
         * 1：用户名错误
         * 2：密码错误
         * 3：登录超时，
         * 4：其它用户登录
         * 5：授权码无效，请重新登陆
         * 6：验证码错误
         * 7：已经存在，不能重复
         * 8:没有登录
         * 100：未审核；
         * 400：参数错误
         * 500：已经使用
         * 200:完成
         * 201:锁定，请联系客服
         */
        string _userAuthCode = "";//授权码
        string _dataAccount = "";//操作表的主键值
        int _dataCurrentPate = 0;//当前页
        int _dataRecordCount = 0;//总共记录数
        int _dataPageCount = 0;//总共页数
        string _operateMsg = "";//提示消息
        object _dataTable = null;
        public int operateStatus
        {
            get { return _operateStatus; }
            set { _operateStatus = value; }
        }
        public string userAuthCode
        {
            get { return _userAuthCode; }
            set { _userAuthCode = value; }
        }
        public string dataAccount
        {
            get { return _dataAccount; }
            set { _dataAccount = value; }
        }
        public int dataCurrentPate
        {
            get { return _dataCurrentPate; }
            set { _dataCurrentPate = value; }
        }
        public int dataRecordCount
        {
            get { return _dataRecordCount; }
            set { _dataRecordCount = value; }
        }
        public int dataPageCount
        {
            get { return _dataPageCount; }
            set { _dataPageCount = value; }
        }
        #region 操作提示信息
        public string operateMsg
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_operateMsg))
                {//
                    switch (_operateStatus)
                    {
                        case -1:
                            _operateMsg = "系统错误";
                            break;
                        case 1:
                            _operateMsg = "用户名错误";
                            break;
                        case 2:
                            _operateMsg = "密码错误";
                            break;
                        case 3:
                            _operateMsg = "登录超时";
                            break;
                        case 4:
                            _operateMsg = "其它用户登录";
                            break;
                        case 5:
                            _operateMsg = "授权码无效，请重新登陆";
                            break;
                        case 6:
                            _operateMsg = "验证码错误";
                            break;
                        case 7:
                            _operateMsg = "已经存在，不能重复";
                            break;
                        case 8:
                            _operateMsg = "没有登录";
                            break;
                        case 100:
                            _operateMsg = "未审核";
                            break;
                        case 400:
                            _operateMsg = "参数错误";
                            break;
                        case 500:
                            _operateMsg = "已经使用";
                            break;
                        case 200:
                            _operateMsg = "完成";
                            break;
                        case 201:
                            _operateMsg = "锁定，请联系客服";
                            break;
                    }
                }
                return _operateMsg;

            }
            set { _operateMsg = value; }
        }
        #endregion
        public object dataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }
    }
    #endregion
    //
    #region 性别（固定定义）
    public class SexDataList
    {
        List<SexData> _SexList = new List<SexData>();
        public List<SexData> RoleList
        {
            get
            {
                _SexList.Add(new SexData(true));
                _SexList.Add(new SexData(false));
                return _SexList;
            }
        }
    }
    public class SexData
    {
        bool _sex = true;//false女true男
        string _sexMsg = "男";//
        public SexData(bool mySex = true)
        {
            _sex = mySex;
        }
        public bool sex
        {
            get { return _sex; }
            set { _sex = value; }
        }
        public string sexMsg
        {
            get
            {
                if (_sex)
                {
                    _sexMsg = "男";
                }
                else
                {
                    _sexMsg = "女";
                }
                return _sexMsg;
            }
        }
    }
    #endregion
    //
    #region 身份（根据数据库表Sys_Roles）
    public class RoleDataList
    {
        List<RoleData> _RoleList = new List<RoleData>();
        public List<RoleData> RoleList
        {
            get
            {
                _RoleList.Add(new RoleData(0));
                _RoleList.Add(new RoleData(1));
                _RoleList.Add(new RoleData(2));
                return _RoleList;
            }
        }
    }
    public class RoleData
    {
        int _roleId = 0;//0待定1家长2老师
        string _roleName = "待定";//
        public RoleData(int iniRoleid = 0)
        {
            _roleId = iniRoleid;
        }
        public int roleId
        {
            get { return _roleId; }
            set { _roleId = value; }
        }
        public string roleName
        {
            get
            {
                switch (roleId)
                {
                    case 0: _roleName = "待定";
                        break;
                    case 1: _roleName = "家长";
                        break;
                    case 2: _roleName = "老师";
                        break;
                }

                return _roleName;
            }
        }
    }
    #endregion
}
