using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
//
using CheFuZi.DataBaseModel;
using CheFuZi.Function;
using CheFuZi.DataReturn;

namespace CheFuZi.ExternalMethod
{
    public class SmsClass
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="sendPhone">接收短信的手机号</param>
        /// <param name="MsgType">短信类型</param>
        /// <param name="Msg">短信内容</param>
        /// <returns></returns>
        #region 发送短信
        public bool sendSmsMsg(string sendPhone, int MsgType, string Msg)
        {
            //
            bool isOk = false;
            int returnmsg = -1;
            string myMsg = MsgString(MsgType, Msg);
            try
            {
                returnmsg = SendMsg(sendPhone, myMsg);
                if (returnmsg == 0)
                {
                    isOk = true;
                }
            }
            catch
            {
                isOk = false;
            }
            return isOk;
        } 
        #endregion
        #region 消息格式
        private string MsgString(int MsgType, string regplaceMsg)
        {
            StringBuilder ReturnMsg = new StringBuilder();
            //
            switch (MsgType)
            {
                case 0://验证码
                    ReturnMsg.Append("验证码为");
                    ReturnMsg.Append(regplaceMsg);
                    ReturnMsg.Append("(工作人员不会向您索要此验证码，请勿向任何人泄露)，");
                    ReturnMsg.Append("请在页面输入以完成验证，有问题请致电400-6608365");
                    break;
            }
            return ReturnMsg.ToString();
        }
        #endregion
        //
        /// 以 HTTP 的 POST 提交方式 发送短信(ASP.NET的网页或是C#的窗体，均可使用该方法)
        #region 发送短信
        private int SendMsg(string mobile, string msg)
        {
            int SendStatus = -1;//0成功
            //
            string name = "13731168365";
            string pwd = "91F29014BB55C808F1E3482973E1";//登陆web平台 http://sms.1xinxi.cn  在管理中心--基本资料--接口密码（28位） 如登陆密码修改，接口密码会发生改变，请及时修改程序
            string sign = "小鹿芮卡";             //一般为企业简称
            StringBuilder arge = new StringBuilder();

            arge.AppendFormat("name={0}", name);
            arge.AppendFormat("&pwd={0}", pwd);
            arge.AppendFormat("&content={0}", msg);
            arge.AppendFormat("&mobile={0}", mobile);
            arge.AppendFormat("&sign={0}", sign);
            arge.Append("&type=pt");
            string weburl = "http://sms.1xinxi.cn/asmx/smsservice.aspx";

            string resp = PushToWeb(weburl, arge.ToString(), Encoding.UTF8);
            string[] myReturnStatus = resp.Split(',');
            if (myReturnStatus.Length > 0)
            {
                try
                {
                    SendStatus = int.Parse(myReturnStatus[0]);
                }
                catch
                {
                    SendStatus = -1;
                }
            }
            return SendStatus;//是一串 以逗号隔开的字符串。阅读文档查看响应的意思
        } 
        #endregion
        /// HTTP POST方式
        /// 
        /// POST到的网址
        /// POST的参数及参数值
        /// 编码方式
        /// 
        #region POST发送短信
        private string PushToWeb(string weburl, string data, Encoding encode)
        {
            byte[] byteArray = encode.GetBytes(data);

            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(weburl));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream newStream = webRequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();

            //接收返回信息：
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webRequest.GetResponse();
            StreamReader aspx = new StreamReader(response.GetResponseStream(), encode);
            return aspx.ReadToEnd();
        } 
        #endregion
    }
}

