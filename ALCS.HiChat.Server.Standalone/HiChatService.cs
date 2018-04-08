using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ALCS.HiChat.Models;
using ALCS.HiChat.Service;

namespace ALCS.HiChat.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, 
        ConcurrencyMode = ConcurrencyMode.Multiple)]
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
                    Console.WriteLine("User with name {0} already exists, not connecting", newUser.Name);
                    return false;
                }

                IHiChatServiceCallback callback =
                    OperationContext.Current.GetCallbackChannel<IHiChatServiceCallback>();
                if (callback == null)
                {
                    Console.WriteLine("Could not get callback channel for user {0}", newUser.Name);
                    return false;
                }

                connectedClients.Add(newUser, callback);
                Console.WriteLine("Connected user {0}", newUser.Name);
            }

            return true;
        }

        public void Disconnect(User user)
        {
            if (user == null)
            {
                return;
            }

            lock (connectedClients)
            {
                Console.WriteLine("Request to disconnect user: {0}", user.Name);
                if (connectedClients.ContainsKey(user))
                {
                    Console.WriteLine("Disconnected user: {0}", user.Name);
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
            Console.WriteLine("From user {0} received message {1}", message.Sender.Name, message.Content);
            lock (connectedClients)
            {
                List<User> invalidClients = new List<User>();
                foreach (var item in connectedClients)
                {
                    var client = item.Key;
                    var callback = item.Value;
                    if (DoRouteMessage(message, client))
                    {
                        try
                        {
                            Console.WriteLine("Sending message: {0} to user: {1}", message.Content, client.Name);
                            callback.RouteMessage(message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: %s", e.Message);
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
