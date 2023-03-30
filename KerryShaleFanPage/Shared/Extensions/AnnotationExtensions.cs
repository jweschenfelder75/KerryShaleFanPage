using System;
using System.Linq;
using System.Reflection;
using KerryShaleFanPage.Shared.Attributes;

namespace KerryShaleFanPage.Shared.Extensions
{
    /// <summary>
    /// https://stackoverflow.com/questions/30467519/about-enum-and-dataannotation
    /// Please keep in mind that this extension class uses Reflection.
    /// </summary>

    public static class AnnotationExtensions
    {
        /// <summary>
        /// 
        /// Uses Reflection.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetFrontColorAttributeFrom(this Enum enumValue, Type enumType)
        {
            string displayName;
            var info = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
            if (info != null && info.CustomAttributes.Any())
            {
                var nameAttr = info.GetCustomAttribute<FrontColorNameAttribute>();
                displayName = nameAttr?.Name ?? string.Empty;
            }
            else
            {
                displayName = enumValue.ToString();
            }
            return displayName;
        }

        /// <summary>
        /// 
        /// Uses Reflection.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetBackColorAttributeFrom(this Enum enumValue, Type enumType)
        {
            string displayName;
            var info = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
            if (info != null && info.CustomAttributes.Any())
            {
                var nameAttr = info.GetCustomAttribute<BackColorNameAttribute>();
                displayName = nameAttr?.Name ?? string.Empty;
            }
            else
            {
                displayName = enumValue.ToString();
            }
            return displayName;
        }
    }
}
