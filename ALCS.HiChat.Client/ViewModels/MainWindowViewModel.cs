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
            StatusMessage = "Welcome to the HiChat application";
        }

        #region ToggleConnect command
        private ICommand toggleConnectCommand;
        public ICommand ToggleConnectCommand
        {
            get
            {
                if (toggleConnectCommand == null)
                {
                    toggleConnectCommand = new RelayCommand((o) => ToggleConnect(o),
                        (o) => IsToggleAllowed);
                }
                return toggleConnectCommand;
            }
        }
        private void ToggleConnect(object o)
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else
            {
                TryConnect();
            }
        }
        #endregion

        private void TryConnect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            if (string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(ServerAddress))
            {
                StatusMessage = "Could not connect, username or server address invalid";
                return;
            }

            try
            {
                IsToggleAllowed = false;
                StatusMessage = "Connecting...";
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
                    StatusMessage = "Could not connect, server refused connection";
                    channel = null;
                    return;
                }
                StatusMessage = "Connection successful";
            }
            catch (Exception e)
            {
                StatusMessage = $"Could not connect, exception: {e.Message}";
                Disconnect();
            }
            finally
            {
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("IsNotConnected");
                IsToggleAllowed = true;
            }
        }
        
        #region PublishMessage command
        private ICommand publishMessageCommand;
        public ICommand PublishMessageCommand
        {
            get
            {
                if (publishMessageCommand == null)
                {
                    publishMessageCommand = new RelayCommand((o) => PublishMessage(),
                    (o) => IsConnected);
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
                StatusMessage = $"Exception when publishing message: {e.Message}";
                Disconnect();
            }
            finally
            {
                OutgoingMessage = string.Empty;
            }
        }
        #endregion

        #region Service callback
        public void RouteMessage(Message message)
        {
            MessageBacklog.Add(message);
        }

        private IHiChatService channel;
        #endregion

        #region MVVM Properties
        public ObservableCollection<Message> MessageBacklog { get; set; }

        private string outgoingMessage;
        public string OutgoingMessage 
        {
            get
            {
                return outgoingMessage;
            }
            set
            {
                outgoingMessage = value;
                OnPropertyChanged("OutgoingMessage");
            }
        }
        
        private string statusMessage;
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                OnPropertyChanged("StatusMessage");
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

        private bool isToggleAllowed = true;
        public bool IsToggleAllowed
        {
            get
            {
                return isToggleAllowed;
            }
            set
            {
                isToggleAllowed = value;
                OnPropertyChanged("IsToggleAllowed");
            }
        }
        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Disconnect()
        {
            IsToggleAllowed = false;
            if (!IsConnected)
            {
                return;
            }
            var user = new User { Name = Username };
            try
            {
                channel.Disconnect(user);
                StatusMessage = "Disconnected";
            }
            catch(Exception e)
            {
                StatusMessage = $"Exception occurred when disconnecting: {e.Message}";   
            }
            finally
            {
                channel = null;
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("IsNotConnected");
                IsToggleAllowed = true;
            }
        }
    }
}
