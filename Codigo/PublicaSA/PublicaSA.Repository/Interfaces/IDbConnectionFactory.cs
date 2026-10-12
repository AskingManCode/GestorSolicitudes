using System.Data;

namespace PublicaSA.Repository.Interfaces
{
    public interface IDBConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
