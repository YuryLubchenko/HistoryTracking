using LinqToDB.Mapping;

namespace WebApp.Entities;

public abstract class BaseEntity
{
    [PrimaryKey, Identity]
    [Column("id")]
    public virtual long Id { get; set; }
}
