using Aprimo.Epi.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Providers
{
    public static class ReflectionHelpers
    {
        public static Dictionary<string, string> GetPropertyNameAndAttributeValue(Type item)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var properties = item.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false)
                    .Where(x => x.GetType() == typeof(AprimoFieldNameAttribute));
                foreach (var attribute in attributes)
                {
                    var fieldName = ((AprimoFieldNameAttribute)attribute).FieldName;
                    if (!string.IsNullOrWhiteSpace(fieldName))
                    {
                        dictionary.Add(property.Name, fieldName);
                    }
                }
            }
            return dictionary;
        }
    }
}