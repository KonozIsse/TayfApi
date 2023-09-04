using Entities;
using ResourcesLib;
using System;
using System.ComponentModel;
using System.Linq;

namespace EtayfeWeb.Helpers
{
    public class EnumHelper
    {
        public static void FillEnumDesc(object obj)
        {
            var enumProps = obj.GetType().GetProperties().Where(x => x.PropertyType.IsEnum);
            foreach (var enumProp in enumProps)
            {
                var propValue = (Enum)enumProp.GetValue(obj);

                var enumType = propValue.GetType();

                var enumBindResAttr = enumType.GetCustomAttributes(typeof(Entities.EnumBindResourceAttribute), false).FirstOrDefault();

                var resourceType = typeof(SharedResource);

                if (enumBindResAttr != null)
                {
                    var enumBindResResourceType = ((EnumBindResourceAttribute)enumBindResAttr).ResourceType;
                    if (enumBindResResourceType != null)
                    {
                        resourceType = enumBindResResourceType;
                    }
                }

                System.Reflection.FieldInfo fi = enumType.GetField(propValue.ToString());
                if (fi != null)
                {
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    var resourceKey = attributes != null && attributes.Length > 0 ? attributes[0].Description : propValue.ToString();

                    var props = obj.GetType().GetProperties();
                    foreach (var item in props)
                    {
                        if (item.IsDefined(typeof(Entities.EnumBindAttribute), false))
                        {
                            var enumBidAttr = ((EnumBindAttribute)item.GetCustomAttributes(typeof(Entities.EnumBindAttribute), false).First());
                            if (enumBidAttr.Name == enumProp.Name)
                            {
                              
                                item.SetValue(obj, ResourceHelper.ReadResourceValue(resourceType, resourceKey, System.Threading.Thread.CurrentThread.CurrentCulture));
                            }
                        }
                    }
                }
            }
        }
    }
}
