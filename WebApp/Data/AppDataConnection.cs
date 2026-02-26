using LinqToDB;
using LinqToDB.Data;
using WebApp.Audit.Entities;
using WebApp.Entities;

namespace WebApp.Data;

public class AppDataConnection : DataConnection
{
    public AppDataConnection(DataOptions<AppDataConnection> options)
        : base(options.Options)
    {
    }

    public ITable<ClientEntity> Clients => this.GetTable<ClientEntity>();
    public ITable<OrderEntity> Orders => this.GetTable<OrderEntity>();
    public ITable<OrderItemEntity> OrderItems => this.GetTable<OrderItemEntity>();

    public ITable<ActionLogEntity> ActionLogs => this.GetTable<ActionLogEntity>();
    public ITable<EntityRecordEntity> EntityChanges => this.GetTable<EntityRecordEntity>();
    public ITable<PropertyRecordEntity> PropertyChanges => this.GetTable<PropertyRecordEntity>();
}
