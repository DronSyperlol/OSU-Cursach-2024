using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Backend.Tools
{
    public class ParsebleToDictionaryBase
    {
        public virtual Dictionary<string, object> ToDict()
        {
            var ret = new Dictionary<string, object>();
            var properties = GetType().GetProperties().Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);
            foreach (var property in properties)
            {
                object? value = property.GetValue(this);
                if (value != null)
                    if (value is DateTime time)
                        ret.Add(property.Name, TimeStampConvertor.DatetimeToLong(time));
                    else if (value is ParsebleToDictionaryBase parseble)
                        ret.Add(property.Name, parseble.ToDict());
                    else
                        ret.Add(property.Name, value);
            }
            return ret;
        }
    }
}
