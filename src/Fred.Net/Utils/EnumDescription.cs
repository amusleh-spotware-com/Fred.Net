using System;
using System.ComponentModel;
using System.Reflection;

namespace Fred.Net.Utils
{
    public static class EnumDescription
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

        public static T GetValueFromDescription<T>(string description)
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
    }
}