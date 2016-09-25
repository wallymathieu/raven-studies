using Raven.Client.Converters;
using SomeBasicRavenApp.Core.Entities;
using System;

namespace SomeBasicRavenApp.Core
{
    public class CustomerIdentityConverter : ITypeConverter
    {
        public bool CanConvertFrom(Type sourceType)
        {
            return sourceType == typeof(CustomerId);
        }

        public string ConvertFrom(string tag, object value, bool allowNull)
        {
            var val = (CustomerId)value;
            if (val.Equals(CustomerId.Empty) && allowNull)
                return null;
            return tag + value.ToString();
        }

        public object ConvertTo(string value)
        {
            return new CustomerId(value);
        }
    }

    public class ProductIdentityConverter : ITypeConverter
    {
        public bool CanConvertFrom(Type sourceType)
        {
            return sourceType == typeof(ProductId);
        }

        public string ConvertFrom(string tag, object value, bool allowNull)
        {
            var val = (ProductId)value;
            if (val.Equals(ProductId.Empty) && allowNull)
                return null;
            return tag + value.ToString();
        }

        public object ConvertTo(string value)
        {
            return new ProductId(value);
        }
    }
}
