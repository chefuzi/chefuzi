using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheFuZi.Function
{
    public static class CreateNewGuidClass
    {
        #region 生成唯一Guid
        public static string CreateNewGuid()
        {//生成唯一Guid
            return Guid.NewGuid().ToString("N");
        } 
        #endregion
    }
}
