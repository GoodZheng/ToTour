using System.Dynamic;
using System.Reflection;

namespace ToTour.Helpers
{
    // 集合数据塑形帮助类
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string? fields)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var expandObjectList = new List<ExpandoObject>();
            // 避免在列表中便利数据，创建一个属性信息列表
            var propertyInfoList = new List<PropertyInfo>(); // PropertyInfo在 System.Reflection 命名空间下，包含对象属性所有的信息
            if (string.IsNullOrWhiteSpace(fields))
            {
                // 希望返回动态类型对象 ExpandoObject 所有的属性
                var propertyInfos = typeof(TSource) // 输入对象的类型
                    .GetProperties(BindingFlags.IgnoreCase
                                   | BindingFlags.Public | BindingFlags.Instance); // 获取数据源 TSource 一系列属性信息
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // 用逗号来分隔字段字符串
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    // 去掉首尾多余的空格，获得属性名称
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase
                        | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                        throw new Exception($"属性 {propertyName} 找不到" + $" {typeof(TSource)}");
                    
                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (TSource sourceObject in source)
            {
                //  创建动态类型对象，创建数据塑形对象
                var dataShapeObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    //  获得对应属性的真实数据
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    ((IDictionary<string, object>)dataShapeObject).Add(propertyInfo.Name, propertyValue);
                }

                expandObjectList.Add(dataShapeObject);
            }

            return expandObjectList;
        }
    }
}
