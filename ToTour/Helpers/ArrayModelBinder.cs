using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace ToTour.Helpers
{
    public class ArrayModelBinder:IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // 判断是不是enumerable类型           
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // 通过ValueProvider获得输入的值
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // 如果只是空，就返回空
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // 尝试转换成指定类型
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(elementType); // 类型转换工具

            // 通过类型转换工具，将每个GUID字符串转化为GUID类型的对象
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray(); //URL片段中，以 "," 隔开GUID字符串的值

            // 将所有GUID对象通过反射赋值到新的数组中
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            // 返回一个成功的结果，传入Model 
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
