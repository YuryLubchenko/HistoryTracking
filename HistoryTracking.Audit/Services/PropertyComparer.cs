using System.Globalization;
using System.Reflection;
using LinqToDB.Mapping;

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
            if (!IsMappedColumn(property))
                continue;

            var oldValue = oldEntity != null ? property.GetValue(oldEntity) : null;
            var newValue = newEntity != null ? property.GetValue(newEntity) : null;

            if (oldValue == null && newValue == null)
                continue;

            if (oldEntity != null && newEntity != null)
            {
                if (Equals(oldValue, newValue))
                    continue;
            }

            var oldValueSerialized = Serialize(oldValue);
            var newValueSerialized = Serialize(newValue);

            var effectiveType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var change = new PropertyChange
            {
                PropertyName = property.Name,
                PropertyType = effectiveType.FullName ?? effectiveType.Name,
                OldValue = oldValueSerialized,
                NewValue = newValueSerialized
            };

            changes.Add(change);
        }


        return changes;
    }

    private static bool IsMappedColumn(PropertyInfo property)
    {
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        if (columnAttribute == null || columnAttribute is NotColumnAttribute)
            return false;

        if (property.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            return false;

        return true;
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
