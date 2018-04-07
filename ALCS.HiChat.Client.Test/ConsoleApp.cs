using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ALCS.HiChat.Models;
using ALCS.HiChat.Service;
using System.Threading;

namespace ALCS.HiChat.Client.Test
{
    class HiChatServiceCallback : IHiChatServiceCallback
    { 
        public void RouteMessage(Message message)
        {
            Console.WriteLine(message.Content);
        }
    }

    class ConsoleApp
    {
        static void Main(string[] args)
        {
            var binding = new WSDualHttpBinding();
            Console.WriteLine("Created binding");
            var endpointAddress = 
            new EndpointAddress("http://localhost:8733/Design_Time_Addresses/ALCS.HiChat.Service/HiChatService/");
            Console.WriteLine("Created endpoint");
            var callback = new HiChatServiceCallback();
            Console.WriteLine("Created callback");
            var instanceContext = new InstanceContext(callback);
            Console.WriteLine("Created instance context");
            var channelFactory = new DuplexChannelFactory<IHiChatService>(instanceContext, 
            binding, endpointAddress);
            Console.WriteLine("Created factory, state: {0}", channelFactory.State);
            IHiChatService channel = channelFactory.CreateChannel();
            Console.WriteLine("Created channel, state: {0}", channelFactory.State);
            User user = new User();
            user.Name = "Abc";
            Message message = new Message();
            message.Sender = user;
            message.Content = "MyMessage";
            bool hasConnected = channel.Connect(user);
            Console.WriteLine("Has connected: {0}", hasConnected);
            channel.PublishMessage(message);
            Console.WriteLine("Published message");

            User otherUser = new User();
            otherUser.Name = "Abc";

            bool operatorResult = (user == otherUser);
            Console.WriteLine("Operator {0}, equals {1}", operatorResult, user.Equals(otherUser));

            Console.ReadLine();
        }
    }
}
