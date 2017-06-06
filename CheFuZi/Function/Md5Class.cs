using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace CheFuZi.Function
{
    public class Md5Class
    {
        #region MD5加密
        public static string CreateMd5(string myCode)
        {
            StringBuilder myResult = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(myCode))
            {
                MD5 md5 = MD5.Create();//实例化一个md5对像
                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(myCode));
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                    myResult.Append(s[i].ToString("X"));
                }
            }
            return myResult.ToString();
        } 
        #endregion
    }
}