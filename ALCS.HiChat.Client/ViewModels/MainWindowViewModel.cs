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
                    toggleConnectCommand = new RelayCommandAsync(() => ToggleConnectAsync());
                }
                return toggleConnectCommand;
            }
        }
        private async Task ToggleConnectAsync()
        {
            if (IsConnected)
            {
                await DisconnectAsync(true).ConfigureAwait(false);
            }
            else
            {
                await TryConnectAsync().ConfigureAwait(false);
            }
        }
        #endregion

        private async Task TryConnectAsync()
        {
            if (IsConnected)
            {
                await DisconnectAsync(true).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(ServerAddress))
            {
                StatusMessage = "Could not connect, username or server address invalid";
                return;
            }

            try
            {
                StatusMessage = "Connecting...";
                var binding = new WSDualHttpBinding();
                var endpoint = new EndpointAddress(ServerAddress);
                var instanceContext = new InstanceContext(this);
                var channelFactory = new DuplexChannelFactory<IHiChatService>(instanceContext,
                    binding, endpoint);
                channel = channelFactory.CreateChannel();
                var task = Task.Run(() => channel.Connect(Username));
                if (await Task.WhenAny(task, Task.Delay(15000)) != task)
                {
                    StatusMessage = "Connection timed out";
                    channel = null;
                    return;
                }

                if (task.Result == null)
                {
                    StatusMessage = "Could not connect, server refused connection";
                    channel = null;
                }
                else
                {
                    ConnectedUser = task.Result;
                    StatusMessage = "Connection successful";
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Could not connect, exception: {e.Message}";
                await DisconnectAsync(false).ConfigureAwait(false);
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
                    publishMessageCommand = new RelayCommandAsync(() => PublishMessageAsync(),
                    (o) => IsConnected);
                }
                return publishMessageCommand;
            }
        }

        public async Task PublishMessageAsync()
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
                var task = Task.Run(() => channel.PublishMessage(message));
                if (await Task.WhenAny(task, Task.Delay(5000)) != task)
                {
                    StatusMessage = "Sending message timed out";
                    await DisconnectAsync(false).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Exception when publishing message: {e.Message}";
                await DisconnectAsync(false).ConfigureAwait(false);
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

        private User connectedUser;
        private User ConnectedUser
        {
            get
            {
                return connectedUser;
            }
            set
            {
                connectedUser = value;
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("IsNotConnected");
            }
        }

        public bool IsConnected
        {
            get
            {
                return ConnectedUser != null;
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
        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public async Task DisconnectAsync(bool tryCleanDisconnect)
        {
            if (!IsConnected)
            {
                return;
            }
            try
            {
                if (tryCleanDisconnect)
                {
                    var task = Task.Run(() => channel.Disconnect(ConnectedUser));
                    await Task.WhenAny(task, Task.Delay(5000));
                }
                StatusMessage = "Disconnected";
            }
            catch (Exception e)
            {
                StatusMessage = $"Exception occurred when disconnecting: {e.Message}";   
            }
            finally
            {
                channel = null;
                ConnectedUser = null;
            }
        }
    }
}
