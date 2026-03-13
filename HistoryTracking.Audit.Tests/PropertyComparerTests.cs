using System.Globalization;
using HistoryTracking.Audit.Configuration;
using HistoryTracking.Audit.Services;
using LinqToDB.Mapping;
using Xunit;

namespace HistoryTracking.Audit.Tests;

public class PropertyComparerTests
{
    [Table("sample")]
    private class SampleEntity
    {
        [PrimaryKey]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("nullable_val")]
        public int? NullableValue { get; set; }

        // No Column attribute — should be ignored
        public string NonMapped { get; set; }
    }

    [Fact]
    public void Compare_BothNull_ReturnsEmpty()
    {
        var result = PropertyComparer.Compare(null!, null!);
        Assert.Empty(result);
    }

    [Fact]
    public void Compare_OldNull_ReturnsAllMappedColumns()
    {
        var newEntity = new SampleEntity { Name = "Alice", Count = 5 };
        var result = PropertyComparer.Compare(null!, newEntity);

        // Name, Count, Amount, CreatedAt, NullableValue — not Id (PK), not NonMapped
        Assert.Contains(result, r => r.PropertyName == "Name");
        Assert.Contains(result, r => r.PropertyName == "Count");
        Assert.Contains(result, r => r.PropertyName == "Amount");
    }

    [Fact]
    public void Compare_NewNull_ReturnsAllMappedColumns()
    {
        var oldEntity = new SampleEntity { Name = "Bob", Count = 3 };
        var result = PropertyComparer.Compare(oldEntity, null!);

        Assert.Contains(result, r => r.PropertyName == "Name");
        Assert.Contains(result, r => r.PropertyName == "Count");
    }

    [Fact]
    public void Compare_SameValues_ReturnsEmpty()
    {
        var a = new SampleEntity { Name = "Same", Count = 1 };
        var b = new SampleEntity { Name = "Same", Count = 1 };
        var result = PropertyComparer.Compare(a, b);
        Assert.Empty(result);
    }

    [Fact]
    public void Compare_DifferentValue_ReturnsChange()
    {
        var old = new SampleEntity { Name = "Old" };
        var @new = new SampleEntity { Name = "New" };
        var result = PropertyComparer.Compare(old, @new);

        var change = Assert.Single(result, r => r.PropertyName == "Name");
        Assert.Equal("Old", change.OldValue);
        Assert.Equal("New", change.NewValue);
    }

    [Fact]
    public void Compare_SkipsNonColumnProperty()
    {
        var old = new SampleEntity { NonMapped = "a" };
        var @new = new SampleEntity { NonMapped = "b" };
        var result = PropertyComparer.Compare(old, @new);

        Assert.DoesNotContain(result, r => r.PropertyName == "NonMapped");
    }

    [Fact]
    public void Compare_SkipsPrimaryKey()
    {
        var old = new SampleEntity { Id = 1 };
        var @new = new SampleEntity { Id = 2 };
        var result = PropertyComparer.Compare(old, @new);

        Assert.DoesNotContain(result, r => r.PropertyName == "Id");
    }

    [Fact]
    public void Compare_BothNullProperty_Skipped()
    {
        var old = new SampleEntity { Name = null };
        var @new = new SampleEntity { Name = null };
        var result = PropertyComparer.Compare(old, @new);

        Assert.DoesNotContain(result, r => r.PropertyName == "Name");
    }

    [Fact]
    public void Compare_AlwaysLog_IncludesUnchanged()
    {
        var entityConfig = new AuditEntityConfig
        {
            Properties = new Dictionary<string, AuditPropertyConfig>
            {
                ["Name"] = new AuditPropertyConfig { IsAlwaysLoged = true }
            }
        };

        var a = new SampleEntity { Name = "Same" };
        var b = new SampleEntity { Name = "Same" };
        var result = PropertyComparer.Compare(a, b, entityConfig);

        Assert.Contains(result, r => r.PropertyName == "Name");
    }

    [Fact]
    public void Compare_DateTime_SerializedCorrectly()
    {
        var dt = new DateTime(2024, 1, 15, 9, 30, 0);
        var old = new SampleEntity { CreatedAt = dt };
        var @new = new SampleEntity { CreatedAt = dt.AddSeconds(1) };
        var result = PropertyComparer.Compare(old, @new);

        var change = Assert.Single(result, r => r.PropertyName == "CreatedAt");
        Assert.Equal("2024-01-15 09:30:00", change.OldValue);
    }

    [Fact]
    public void Compare_NullableInt_UsesUnderlyingType()
    {
        var old = new SampleEntity { NullableValue = 1 };
        var @new = new SampleEntity { NullableValue = 2 };
        var result = PropertyComparer.Compare(old, @new);

        var change = Assert.Single(result, r => r.PropertyName == "NullableValue");
        Assert.Equal("System.Int32", change.PropertyType);
    }

    [Fact]
    public void Compare_Decimal_InvariantCulture()
    {
        var old = new SampleEntity { Amount = 1.5m };
        var @new = new SampleEntity { Amount = 2.5m };
        var result = PropertyComparer.Compare(old, @new);

        var change = Assert.Single(result, r => r.PropertyName == "Amount");
        Assert.Equal("1.5", change.OldValue);
        Assert.Equal("2.5", change.NewValue);
    }
}
