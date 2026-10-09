using System.Data;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
