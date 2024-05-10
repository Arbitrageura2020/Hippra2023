using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using System.Linq;

namespace Hippra.Models.Enums
{
    public static class EnumsHelper
    {

        // Helper method to display the name of the enum values.
        public static string GetDisplayName(Enum value)
        {
            if (value.GetType().GetMember(value.ToString()).Count() > 0)
            {
                return value.GetType()?
               .GetMember(value.ToString())?.First()?
               .GetCustomAttribute<DisplayAttribute>()?
               .Name;
            }
            else return "";
        }

        public static T GetValueByShortName<T>(this string shortName)
        {
            var values = from f in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
                         let attribute = Attribute.GetCustomAttribute(f, typeof(DisplayAttribute)) as DisplayAttribute
                         where attribute != null && attribute.Name == shortName
                         select (T)f.GetValue(null);

            if (values.Count() > 0)
            {
                return (T)(object)values.FirstOrDefault();
            }

            return default(T);
        }
    }
}
