using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DapperProductionStore
{
    public interface IDbContext
    {
        IDbConnection Connection { get; }
        void EnsureCreated();
    }
}
