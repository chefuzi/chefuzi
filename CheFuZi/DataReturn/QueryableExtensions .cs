using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Collections.Concurrent;


namespace CheFuZi.DataReturn
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, out int RecordCount, out int PageCount, ref int CurrentPage, int PageSize = 10, bool IsDesc = true, string propertyName2 = "", bool IsDesc2 = true)
        {
            // 
            if (PageSize < 1)
            {
                PageSize = 1;
            }
            //
            RecordCount = queryable.Count();//记录总数
            //
            PageCount = (RecordCount + PageSize - 1) / PageSize;
            //设置当前页
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            if (CurrentPage > PageCount)
            {
                //大于总页数取最大数
                CurrentPage = PageCount;
            }
            if (CurrentPage == 0) CurrentPage = 1;
            //
            dynamic keySelector = QueryableHelper<T>.GetLambdaExpression(propertyName);
            //
            return QueryableHelper<T>.OrderBy(queryable, propertyName, IsDesc, propertyName2, IsDesc2).Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
        //
        #region Lambda排序表达式
        static class QueryableHelper<T>
        {
            private static ConcurrentDictionary<string, LambdaExpression> cache = new ConcurrentDictionary<string, LambdaExpression>();
            public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool desc, string propertyName2, bool desc2)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);

                //Queryable.ThenByDescending
                //Queryable.ThenBy
                if (desc)
                {
                    if (!String.IsNullOrWhiteSpace(propertyName2))
                    {
                        dynamic keySelector2 = GetLambdaExpression(propertyName2);
                        if (desc2)
                        {
                            var tableSourc = Queryable.OrderByDescending(queryable, keySelector);
                            return Queryable.ThenByDescending(tableSourc, keySelector2);
                        }
                        else
                        {
                            var tableSourc = Queryable.OrderByDescending(queryable, keySelector);
                            return Queryable.ThenBy(tableSourc, keySelector2);
                        }
                    }
                    else
                    {
                        return Queryable.OrderByDescending(queryable, keySelector);
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(propertyName2))
                    {
                        dynamic keySelector2 = GetLambdaExpression(propertyName2);
                        if (desc2)
                        {
                            var tableSourc = Queryable.OrderBy(queryable, keySelector);
                            return Queryable.ThenByDescending(tableSourc, keySelector2);
                        }
                        else
                        {
                            var tableSourc = Queryable.OrderBy(queryable, keySelector);
                            return Queryable.ThenBy(tableSourc, keySelector2);
                        }
                    }
                    else
                    {
                        return Queryable.OrderBy(queryable, keySelector);
                    }
                }
            }
            public static LambdaExpression GetLambdaExpression(string propertyName)
            {
                if (cache.ContainsKey(propertyName)) return cache[propertyName];
                var param = Expression.Parameter(typeof(T));
                var body = Expression.Property(param, propertyName);
                var keySelector = Expression.Lambda(body, param);
                cache[propertyName] = keySelector;
                return keySelector;
            }
        }
        #endregion
    }
}