using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WsAdminResidentes.Hubs
{
    public class SingalRHub: Hub
    {
        public async Task SendMessage(string nombreUsuario, string message)
        {
            await Clients.All.SendAsync("Mensaje", nombreUsuario, message);
        }

    }
}
