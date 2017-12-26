using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RandomSelector.Common
{
    public static class EnumExtention
    {
        public static string ToDisplayName(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var fld = type.GetField(value.ToString());
            var attr = (Attribute.GetCustomAttributes(fld, typeof(DisplayAttribute), true) as DisplayAttribute[]).FirstOrDefault();

            return attr?.GetName() ?? value.ToString();
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetDisplayValuea<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var values = Enum.GetValues(type).Cast<T>();
            return values.Where(x => Attribute.IsDefined(type.GetField(x.ToString()), typeof(DisplayAttribute)));
        }
    }
}