using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Configuration;
//
using EFCache;
//
using System.Data.Entity.Core.Metadata.Edm;
//
using CheFuZi.DataBaseModel;

namespace CheFuZi.Function
{
    public class EFCachClear
    {
        #region 清空缓存
        public static void ClearTable(string tableName, bool clearAll = false)
        {
            if (!String.IsNullOrWhiteSpace(tableName))
            {
                tableName = tableName.ToLower();
            }
            using (chefuzi_dataEntities myOperating = new chefuzi_dataEntities())//数据库操作objectContext.DefaultContainerName
            {
                var _metadataWorkspace = myOperating.ObjectContext.MetadataWorkspace;
                var container = _metadataWorkspace.GetEntityContainer("chefuzi_dataModelStoreContainer", DataSpace.SSpace);
                IEnumerable<string> myEntitySets;
                if (clearAll)
                {
                    myEntitySets = container.EntitySets.Select(e => e.Name);
                    if (myEntitySets != null)
                    {
CheFuZi.DataBaseModel.Configuration.DataCache.InvalidateSets(myEntitySets);
CheFuZi.DataBaseModel.Configuration.DataCache.Purge();
                    }
                }
                else if (!String.IsNullOrWhiteSpace(tableName))
                {
                    myEntitySets = container.EntitySets.Where(p => p.Name.ToLower() == tableName).Select(e => e.Name);
                    if (myEntitySets != null)
                    {
CheFuZi.DataBaseModel.Configuration.DataCache.InvalidateSets(myEntitySets);
                        CheFuZi.DataBaseModel.Configuration.DataCache.Purge();
                    }
                }
            }
        }
        #endregion
    }
}