namespace CheFuZi.DataBaseModel
{
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using EFCache;
    using System.Data.Entity.Core.Common;
    public class Configuration : DbConfiguration
    {
        internal static readonly InMemoryCache DataCache = new InMemoryCache();
        public Configuration()
        {
            var transactionHandler = new CacheTransactionHandler(DataCache);
            AddInterceptor(transactionHandler);
            var cachingPolicy = new CachingPolicy();
            Loaded +=
              (sender, args) => args.ReplaceService<DbProviderServices>(
                (s, _) => new CachingProviderServices(s, transactionHandler,
                  cachingPolicy));
        }
    }
}
