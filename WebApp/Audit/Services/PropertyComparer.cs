using System.Reflection;
using System.Text.Json;

namespace WebApp.Audit.Services;

public class PropertyChange
{
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}

public static class PropertyComparer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public static List<PropertyChange> Compare(object oldEntity, object newEntity)
    {
        var changes = new List<PropertyChange>();

        if (oldEntity == null && newEntity == null)
        {
            return changes;
        }

        var entityType = (oldEntity ?? newEntity)!.GetType();
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name == "Id")
            {
                continue;
            }

            var oldValue = oldEntity != null ? property.GetValue(oldEntity) : null;
            var newValue = newEntity != null ? property.GetValue(newEntity) : null;

            var oldValueChanged = oldEntity != null;
            var newValueChanged = newEntity != null;

            if (oldEntity != null && newEntity != null)
            {
                var areEqual = Equals(oldValue, newValue);
                if (areEqual)
                {
                    continue;
                }
            }

            var change = new PropertyChange
            {
                PropertyName = property.Name,
                PropertyType = property.PropertyType.FullName ?? property.PropertyType.Name,
                OldValue = oldValueChanged ? SerializeToJsonb(oldValue) : null,
                NewValue = newValueChanged ? SerializeToJsonb(newValue) : null
            };

            changes.Add(change);
        }

        return changes;
    }

    private static string SerializeToJsonb(object value)
    {
        if (value == null)
        {
            return JsonSerializer.Serialize(new { value = (object)null }, JsonOptions);
        }

        var wrapper = new Dictionary<string, object> { { "value", value } };
        return JsonSerializer.Serialize(wrapper, JsonOptions);
    }
}
