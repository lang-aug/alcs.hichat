using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ALCS.HiChat.Cross.Models;
using ALCS.HiChat.Service;
using System.ComponentModel;

namespace ALCS.HiChat.Client.WPF
{
    public class ViewModel : IHiChatServiceCallback, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {      
        }
        
        private IHiChatService channel;

        private string incomingMessage;

        public string OutgoingMessage 
        {
            get; set;
        }

        public string IncomingMessage
        {
            get
            {
                return incomingMessage;
            }
            private set
            {
                incomingMessage = value;
                OnPropertyChanged("IncomingMessage");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void RouteMessage(Message message)
        {
            IncomingMessage = message.Content;   
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }
            var user = new User { Name = Username };
            try
            {
                channel.Disconnect(user);
            }
            catch(Exception e)
            {
                
            }
            channel = null;
        }

        public void TryConnect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            if (string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(ServerAddress))
            {
                return;
            }

            try
            {
                var binding = new WSDualHttpBinding();
                var endpoint = new EndpointAddress(ServerAddress);
                var instanceContext = new InstanceContext(this);
                var channelFactory = new DuplexChannelFactory<IHiChatService>(instanceContext,
                    binding, endpoint);
                channel = channelFactory.CreateChannel();
                User user = new User { Name = Username };
                bool hasConnected = channel.Connect(user);
                if (!hasConnected)
                {
                    channel = null;
                }
                return;
            }
            catch (Exception e)
            {
                Disconnect();
                return;
            }
        }

        public string ServerAddress { get; set; }

        public bool IsConnected
        {
            get
            {
                return channel != null;
            }
        }

        public bool IsNotConnected
        {
            get
            {
                return !IsConnected;
            }
        }

        public string Username { get; set; }

        public void PublishMessage()
        {
            if (!IsConnected) 
            {
                return;
            }

            if (string.IsNullOrEmpty(OutgoingMessage))
            {
                return;
            }

            var user = new User { Name = Username };
            var message = new Message { Sender = user, Content = OutgoingMessage };
            try
            {
                channel.PublishMessage(message);
            }
            catch(Exception e)
            {
                Disconnect();
            }
            finally
            {
                OutgoingMessage = string.Empty;
            }
        }
    }
}
