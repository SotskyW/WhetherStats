using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whether.Common;
using Whether.Connectors;

namespace Whether
{
    public static class ConnectorsFactory
    {
        public static IConnector GetConnectorByCity(IWhetherLogger logger)
        {
            return new MeteoByConnector(logger);
        }
    }
}
