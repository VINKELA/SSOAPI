using System;

namespace SSOService.Extensions
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }
    }

    public static class EnumExtensions
    {
        public static string DisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return Attribute.GetCustomAttribute(field, attributeType: typeof(EnumDisplayNameAttribute)) is not EnumDisplayNameAttribute attribute ? value.ToString() : attribute.DisplayName;
        }
    }
}
