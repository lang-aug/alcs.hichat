using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ALCS.HiChat.Cross.Service;
using ALCS.HiChat.Cross.Models;

namespace ALCS.HiChat.Cross.Service
{
    public class HiChatService : IHiChatService
    {
        private Dictionary<User, IHiChatServiceCallback> connectedClients =
            new Dictionary<User, IHiChatServiceCallback>();

        public bool Connect(User newUser)
        {
            if (newUser == null)
            {
                return false;
            }

            lock (connectedClients)
            {
                if (connectedClients.ContainsKey(newUser))
                {
                    return false;
                }
                IHiChatServiceCallback callback = 
                    OperationContext.Current.GetCallbackChannel<IHiChatServiceCallback>();
                if (callback != null)
                {
                    return false;
                }
                connectedClients.Add(newUser, callback);
            }

            return true;
        }

        public void Disconnect(User user)
        {
            lock(connectedClients)
            {
                if (connectedClients.ContainsKey(user))
                {
                    connectedClients.Remove(user);
                }
            }
        }

        public void PublishMessage(Message message)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(o => { BroadcastMessage(message); }), null);
        }

        private void BroadcastMessage(Message message)
        {
            lock(connectedClients)
            {
                List<User> invalidClients = new List<User>();
                foreach(var item in connectedClients)
                {
                    var client = item.Key;
                    var callback = item.Value;
                    if (DoRouteMessage(message, client))
                    {
                        try
                        {
                            callback.RouteMessage(message);
                        }
                        catch(Exception)
                        {
                            invalidClients.Add(client);
                        }
                    }
                }

                foreach (var client in invalidClients)
                {
                    connectedClients.Remove(client);
                }
            }
        }

        private bool DoRouteMessage(Message message, User client)
        {
            return true;
        }
    }
}
