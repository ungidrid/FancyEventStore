﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore
{
    public interface IStoreRegistrar
    {
        public void RegisterStore(IServiceCollection services);
    }
}
