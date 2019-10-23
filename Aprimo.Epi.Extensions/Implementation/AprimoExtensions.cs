using Aprimo.Epi.Extensions.API.Assets;
using System;
using System.Linq;

namespace Aprimo.Epi.Extensions.Implementation
{
    public static class AprimoExtensions
    {
        public static string GetFieldValue(this Asset asset, string fieldName)
        {
            if (asset.Embedded.Fields.Items.Any())
            {
                var field = asset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (field != null && field.LocalizedValues.Any())
                {
                    var thumbFxValue = field.LocalizedValues.FirstOrDefault()?.Value;
                    if (!string.IsNullOrWhiteSpace(thumbFxValue))
                        return thumbFxValue;
                }
            }
            return string.Empty;
        }
    }
}