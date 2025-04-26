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
                string name = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
                if (value != null)
                    if (value is DateTime time)
                        ret.Add(name, TimeStampConvertor.DatetimeToLong(time));
                    else if (value is ParsebleToDictionaryBase parseble)
                        ret.Add(name, parseble.ToDict());
                    else if (value is IEnumerable<ParsebleToDictionaryBase> values)
                        ret.Add(name, values.Select(v => v.ToDict()).ToArray());
                    else
                        ret.Add(name, value);
            }
            return ret;
        }
    }
}
