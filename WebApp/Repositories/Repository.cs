using System.Linq.Expressions;
using System.Reflection;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using WebApp.Data;
using WebApp.Entities;
using WebApp.Events;

namespace WebApp.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDataConnection _db;
    private readonly EntityChangedPublisher _publisher;

    public Repository(AppDataConnection db, EntityChangedPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<T> GetById(long id)
    {
        var entity = await _db.GetTable<T>().FirstOrDefaultAsync(e => e.Id == id);
        return entity;
    }

    public async Task<List<T>> GetAll()
    {
        var entities = await _db.GetTable<T>().ToListAsync();
        return entities;
    }

    public async Task<List<T>> GetAll(Expression<Func<T, bool>> filter)
    {
        var entities = await _db.GetTable<T>().Where(filter).ToListAsync();
        return entities;
    }

    public Task<long> NextIdAsync()
    {
        var tableName = typeof(T).GetCustomAttribute<TableAttribute>()!.Name;
        return _db.ExecuteAsync<long>(
            $"SELECT nextval(pg_get_serial_sequence('{tableName}', 'id'))");
    }

    public async Task<long> Add(T entity)
    {
        long id;

        if (entity.Id > 0)
        {
            var tableName = typeof(T).GetCustomAttribute<TableAttribute>()!.Name;
            var mappedCols = typeof(T).GetProperties()
                .Select(p => (Property: p, Col: p.GetCustomAttribute<ColumnAttribute>()))
                .Where(x => x.Col?.Name != null)
                .ToList();

            var colNames   = string.Join(", ", mappedCols.Select(x => x.Col!.Name));
            var paramNames = string.Join(", ", mappedCols.Select((x, i) => $"@p{i}"));
            var sql = $"INSERT INTO {tableName} ({colNames}) OVERRIDING SYSTEM VALUE VALUES ({paramNames}) RETURNING id";
            var parameters = mappedCols
                .Select((x, i) => new DataParameter($"p{i}", x.Property.GetValue(entity)))
                .ToArray();

            id = await new CommandInfo(_db, sql, parameters).ExecuteAsync<long>();
        }
        else
        {
            id = await _db.InsertWithInt64IdentityAsync(entity);
            entity.Id = id;
        }

        await _publisher.PublishAsync(new EntityChangedEvent<T>
        {
            ActionType = ActionType.Created,
            OldEntity = null,
            NewEntity = entity
        });

        return id;
    }

    public async Task<int> Update(T entity)
    {
        var oldEntity = await _db.GetTable<T>().FirstOrDefaultAsync(e => e.Id == entity.Id);

        var rowsAffected = await _db.UpdateAsync(entity);

        if (rowsAffected > 0)
        {
            await _publisher.PublishAsync(new EntityChangedEvent<T>
            {
                ActionType = ActionType.Updated,
                OldEntity = oldEntity,
                NewEntity = entity
            });
        }

        return rowsAffected;
    }

    public async Task<int> Delete(long id)
    {
        var oldEntity = await _db.GetTable<T>().FirstOrDefaultAsync(e => e.Id == id);

        var rowsAffected = await _db.GetTable<T>().DeleteAsync(e => e.Id == id);

        if (rowsAffected > 0 && oldEntity != null)
        {
            await _publisher.PublishAsync(new EntityChangedEvent<T>
            {
                ActionType = ActionType.Deleted,
                OldEntity = oldEntity,
                NewEntity = null
            });
        }

        return rowsAffected;
    }
}
