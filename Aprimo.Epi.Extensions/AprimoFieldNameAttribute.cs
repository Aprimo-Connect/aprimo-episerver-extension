using System;

namespace Aprimo.Epi.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AprimoFieldNameAttribute : Attribute
    {
        public AprimoFieldNameAttribute()
        {
        }

        public AprimoFieldNameAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }
}