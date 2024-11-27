using ToTour.Services;
using System.Linq.Dynamic.Core;

namespace ToTour.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,string orderBy,Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            // source 合法性检验
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // 映射字符串检验
            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source; //不需要排序直接返回原数据
            }

            string orderByString = string.Empty; // 后面排序时使用，生成 sql 代码
            
            string[] orderAfterSplit = orderBy.Split(',');
            
            foreach (var order in orderAfterSplit)
            {
                var trimmedOrder = order.Trim();
                // 通过字符串 "desc" 来判断升序还是降序
                var orderDescending = trimmedOrder.EndsWith(" desc");

                // 删除升序或降序字符串 "asc" 或 "desc" 来获取属性的名称
                var indexOfFirstSpace = trimmedOrder.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrder : trimmedOrder.Remove(indexOfFirstSpace);
                
                if (!mappingDictionary.ContainsKey(propertyName))
                    throw new ArgumentException($"Key mapping for {propertyName} is missing.");

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                // 使用 dynamic.linq 执行排序操作
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    // 给 IQueryable 添加排序字符串
                    orderByString = orderByString +
                                    (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                                    + destinationProperty
                                    + (orderDescending ? " descending" : " ascending");
                }
            }
            return source.OrderBy(orderByString);
        }
    }
}
