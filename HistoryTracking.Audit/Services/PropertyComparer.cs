using System.Globalization;
using System.Reflection;

namespace HistoryTracking.Audit.Services;

internal static class PropertyComparer
{
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
                OldValue = oldValueChanged ? Serialize(oldValue) : null,
                NewValue = newValueChanged ? Serialize(newValue) : null
            };

            changes.Add(change);
        }

        return changes;
    }

    private static string Serialize(object value) =>
        value switch
        {
            null => null,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
}
