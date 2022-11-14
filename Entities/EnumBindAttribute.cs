using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnumBindAttribute : Attribute
    {
        public EnumBindAttribute()
        {
        }

        public EnumBindAttribute(string name, Type resourceType = null)
        {
            Name = name;
            ResourceType = resourceType;
        }

        public string Name { get; set; }
        public Type ResourceType { get; set; }
    }
    
    public class EnumBindResourceAttribute : Attribute
    {
        public EnumBindResourceAttribute()
        {
        }

        public EnumBindResourceAttribute(Type resourceType = null)
        {
            ResourceType = resourceType;
        }
        public Type ResourceType { get; set; }
    }
}
