using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RandomSelector.Common
{
    /// <summary>
    /// 列挙型の拡張メソッドクラス
    /// </summary>
    public static class EnumExtention
    {
        /// <summary>
        /// 列挙値の <see cref="DisplayAttribute.Name"/> を取得します。
        /// </summary>
        /// <param name="value">列挙値</param>
        /// <returns></returns>
        /// <remarks>存在しない場合は列挙値の.ToString()を返します。</remarks>
        public static string ToDisplayName(this Enum value)
        {
            var fld = value.GetType().GetField(value.ToString());
            var attr = value.GetDisplayAttr();

            return attr?.GetName() ?? value.ToString();
        }

        /// <summary>
        /// 列挙値の <see cref="DisplayAttribute"/> を取得します。
        /// </summary>
        /// <param name="value">列挙値</param>
        /// <returns><see cref="DisplayAttribute"/></returns>
        /// <remarks>存在しない場合はNull</remarks>
        public static DisplayAttribute GetDisplayAttr(this Enum value)
        {
            var fld = value.GetType().GetField(value.ToString());
            var attr = (Attribute.GetCustomAttributes(fld, typeof(DisplayAttribute), true) as DisplayAttribute[]).FirstOrDefault();

            return attr;
        }
    }

    /// <summary>
    /// 列挙型のユーティリティクラス
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// 指定した列挙型が持つ値の内、<see cref="DisplayAttribute"/> を持つ値のみの一覧を返します。
        /// </summary>
        /// <typeparam name="T">列挙型</typeparam>
        /// <returns>列挙値一覧</returns>
        /// <exception cref="InvalidOperationException">ジェネリック型が列挙型ではない場合</exception>
        public static IEnumerable<T> GetDisplayValues<T>()
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