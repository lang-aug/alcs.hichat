using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using ALCS.HiChat.Cross.Models;
using ALCS.HiChat.Cross.Service;

namespace ALCS.HiChat.Cross
{
    class HiChatServiceCallback : IHiChatServiceCallback
    {
        public event RoutedMessageEventHandler RoutedMessage;

        public void RouteMessage(Message message)
        {
            RoutedMessage?.Invoke(this, new RoutedMessageEventArgs(message));    
        }
    }

    public delegate void RoutedMessageEventHandler(object sender, RoutedMessageEventArgs e);

    public class RoutedMessageEventArgs : EventArgs
    {
        private readonly Message message;

        public RoutedMessageEventArgs(Message message)
        {
            this.message = message;
        }

        public Message Message { get { return message; } }
    }

    class Program
    {
        static void OnMessage(object sender, RoutedMessageEventArgs e)
        {            
            System.Console.WriteLine(e.Message.Content);
        }

        static void Main(string[] args)
        {
            var callback = new HiChatServiceCallback();
            callback.RoutedMessage += Program.OnMessage;
            var instanceContext = new InstanceContext(callback);
            using (var client = new Service.HiChatServiceClient(instanceContext))
            {
                client.Open();
                var user = new User();
                user.Name = "Joe";
                client.Connect(user);
                var message = new Message();
                message.Sender = user;
                message.Content = "Hello World";
                client.PublishMessage(message);
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
