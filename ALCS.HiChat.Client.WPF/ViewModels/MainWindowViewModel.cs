using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ALCS.HiChat.Models;
using ALCS.HiChat.Service;
using ALCS.HiChat.Client.Commands;

namespace ALCS.HiChat.Client.ViewModels
{
    public class MainWindowViewModel : IHiChatServiceCallback, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            MessageBacklog = new ObservableCollection<Message>();
        }

        #region Commands
        private ICommand tryConnectCommand;
        public ICommand TryConnectCommand
        {
            get
            {
                if (tryConnectCommand == null)
                {
                    tryConnectCommand = new RelayCommand((o) => TryConnect(o));
                }
                return tryConnectCommand;
            }
        }

        public void TryConnect(object o)
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

        public void RouteMessage(Message message)
        {
            MessageBacklog.Add(message);
        }

        private ICommand publishMessageCommand;
        public ICommand PublishMessageCommand
        {
            get
            {
                if (publishMessageCommand == null)
                {
                    publishMessageCommand = new RelayCommand((o) => PublishMessage());
                }
                return publishMessageCommand;
            }
        }

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
            catch (Exception e)
            {
                Disconnect();
            }
            finally
            {
                OutgoingMessage = string.Empty;
            }
        }
        #endregion



        private IHiChatService channel;

        public ObservableCollection<Message> MessageBacklog { get; set; }

        public string OutgoingMessage 
        {
            get; set;
        }
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
    }
}
