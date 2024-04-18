using System.Reflection;
using ToTour.Dtos;
using ToTour.Models;

namespace ToTour.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        // 属性映射字典列表
        private Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Title", new PropertyMappingValue(new List<string>() {"Title"})},
                {"Rating", new PropertyMappingValue(new List<string>() {"Rating"})},
                {"OriginalPrice", new PropertyMappingValue(new List<string>() {"OriginalPrice"})},
            };

        // 包含 DTO 中字段与字符串名称的对应关系，然后映射给 Model
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(
                new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        // sting:字段字符串名称
        // PropertyMappingValue：目标模型的属性
        // 泛型定义
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // 获得匹配的映射对象
            var matchingMapping =
                _propertyMappings
                    .OfType<PropertyMapping<TSource, TDestination>>(); // 通过在IPropertyMapping 列表中传入数据源类型来匹配映射对象

            if (matchingMapping.Count() == 1)
                return matchingMapping.First()._mappingDictionary;

            throw new Exception(
                $"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}>");
        }

        public bool IsMappingExists<TSource, TDestination>(string? fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields)) 
                return true;

            // 逗号来分隔字段字符串
            var fieldAfterSplit = fields.Split(',');

            foreach (var field in fieldAfterSplit)
            {
                // 去掉空格
                var trimmedField = field.Trim();
                // 获得属性名称字符串
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName)) 
                    return false;
            }

            return true;
        }

        public bool IsPropertiesExists<T>(string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) 
                return true;

            // 逗号分隔字段字符串
            var fieldAfterSplit = fields.Split(',');

            foreach (var field in fieldAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                //  如果 T 中没找到对应的属性
                if (propertyInfo == null) 
                    return false;
            }

            return true;
        }
    }
}
