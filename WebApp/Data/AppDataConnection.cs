using LinqToDB;
using LinqToDB.Data;
using WebApp.Entities;

namespace WebApp.Data;

public class AppDataConnection : DataConnection
{
    public AppDataConnection(DataOptions<AppDataConnection> options)
        : base(options.Options)
    {
    }

    public ITable<ClientEntity> Clients => this.GetTable<ClientEntity>();

    public ITable<ContactEntity> Contacts => this.GetTable<ContactEntity>();
}
