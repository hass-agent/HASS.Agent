using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Text;

namespace HASS.Agent.Shared.Functions
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;

        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                var displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? $"[[{_resourceKey}]]"
                    : displayName;
            }
        }
    }

    public static class EnumExtensions
    {
        public static string GetLocalizedDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }

        public static (int key, string description) GetLocalizedDescriptionAndKey(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            var description = attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();

            var enumIndex = Array.IndexOf(Enum.GetValues(enumValue.GetType()), enumValue);
            var key = (int)Enum.GetValues(enumValue.GetType()).GetValue(enumIndex);

            return (key, description);
        }
    }
}
