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

        public User Connect(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            User newUser = new User { Name = username };
            lock (connectedClients)
            {
                if (connectedClients.ContainsKey(newUser))
                {
                    Console.WriteLine("User with name {0} already exists, not connecting", username);
                    return null;
                }

                IHiChatServiceCallback callback =
                    OperationContext.Current.GetCallbackChannel<IHiChatServiceCallback>();
                if (callback == null)
                {
                    Console.WriteLine("Could not get callback channel for user {0}", username);
                    return null;
                }

                connectedClients.Add(newUser, callback);
                Console.WriteLine("Connected user {0}", username);
                return newUser;
            }
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
            if (message == null || message.Sender == null || String.IsNullOrEmpty(message.Sender.Name))
            {
                Console.WriteLine("Ignoring message without valid sender");
                return;
            }

            Console.WriteLine("From user {0} received message {1}", message.Sender.Name, message.Content);
            lock (connectedClients)
            {
                if (!connectedClients.ContainsKey(message.Sender))
                {
                    Console.WriteLine("Ignoring message from non-connected user: {0}", message.Sender.Name);
                }

                List<User> invalidClients = new List<User>();
                foreach (var item in connectedClients)
                {
                    var client = item.Key;
                    var callback = item.Value;
                    if (DoRouteMessage(message, client))
                    {
                        try
                        {
                            callback.RouteMessage(message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception publishing to user {0}, {1}", client.Name, e.Message);
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
