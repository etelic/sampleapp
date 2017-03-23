using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
namespace GeneratorBase.MVC.Controllers
{
    [HubName("iServerHub")]
    public class LiveMonitoring : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
            //Clients.All.addNewMessageToPage(name, message);
        }
        public static void SendMessage(string msg)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LiveMonitoring>();
            hubContext.Clients.All.broadcastMessage(msg);
        }
 	public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add((Context.QueryString["userid"]));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHandler.ConnectedIds.Remove((Context.QueryString["userid"]));
            return base.OnDisconnected(stopCalled);
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
    public class DataLiveMonitoring
    {
        public string username { get; set; }
        public string entityname { get; set; }
        public string action { get; set; }
        public string anchorid { get; set; }
        public string entityid { get; set; }
        public string entitydisplayname { get; set; }
        public string datetime { get; set; }
    }
}