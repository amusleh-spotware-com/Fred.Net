using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Fred.Net
{
    public static class Utility
    {
        public static string GetDescription(object obj)
        {
            FieldInfo field = obj.GetType().GetField(obj.ToString());

            DescriptionAttribute descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);

            if (descriptionAttribute != null && !string.IsNullOrEmpty(descriptionAttribute.Description))
            {
                return descriptionAttribute.Description;
            }

            return obj.ToString();
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            Array enumValues = Enum.GetValues(typeof(T));

            foreach (object enumValue in enumValues)
            {
                string enumValueDescription = GetDescription(enumValue);

                if (enumValueDescription.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (T)enumValue;
                }
            }

            throw new InvalidEnumArgumentException("Couldn't find any enum value that matches provided description");
        }

        public static string GetTagNamesSeparatedBySemicolon(List<string> tags)
        {
            string tagNames = string.Empty;

            tags.ForEach(tag => tagNames += tag + ";");

            tagNames = tagNames.Remove(tagNames.Length - 1);

            return tagNames;
        }
    }
}