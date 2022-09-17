using System.Data;

namespace FancyEventStore.ReadModel
{
    public interface IReadModelContext
    {
        IDbConnection Connection { get; }
    }
}
