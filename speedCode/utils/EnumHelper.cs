using System;
using System.ComponentModel;
using System.Reflection;

namespace speedCode.Utils
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}