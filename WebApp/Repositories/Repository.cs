using System.Linq.Expressions;
using LinqToDB;
using WebApp.Audit.Events;
using WebApp.Data;
using WebApp.Entities;

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

    public async Task<long> Add(T entity)
    {
        var id = await _db.InsertWithInt64IdentityAsync(entity);
        entity.Id = id;

        await _publisher.PublishAsync(new EntityChangedEvent
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
            await _publisher.PublishAsync(new EntityChangedEvent
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
            await _publisher.PublishAsync(new EntityChangedEvent
            {
                ActionType = ActionType.Deleted,
                OldEntity = oldEntity,
                NewEntity = null
            });
        }

        return rowsAffected;
    }
}
