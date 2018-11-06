using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

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

        public static string GetStringSeparatedBySemicolon(List<string> tags)
        {
            string result = string.Empty;

            tags.ForEach(tag => result += tag + ";");

            result = result.Remove(result.Length - 1);

            return result;
        }

        public static T Deserialize<T>(string xml)
        {
            StringReader reader = new StringReader(xml);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T result = (T)serializer.Deserialize(reader);

            return result;
        }

        public static string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd");

        public static string FormatTime(DateTime time) => time.ToString("yyyyMMddHHmm");

        public static string GetDatesSeparatedByComma(List<DateTime> dates)
        {
            string result = string.Empty;

            dates.ForEach(date => result += FormatDate(date) + ",");

            result = result.Remove(result.Length - 1);

            return result;
        }
    }
}