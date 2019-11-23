using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Xync.Web.Hubs
{
    public class PollingHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}