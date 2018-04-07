﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ALCS.HiChat.Cross.Service {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/ALCS.HiChat.Cross.Models")]
    [System.SerializableAttribute()]
    public partial class User : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Message", Namespace="http://schemas.datacontract.org/2004/07/ALCS.HiChat.Cross.Models")]
    [System.SerializableAttribute()]
    public partial class Message : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ALCS.HiChat.Cross.Service.User DestinationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ALCS.HiChat.Cross.Service.User SenderField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ALCS.HiChat.Cross.Service.User Destination {
            get {
                return this.DestinationField;
            }
            set {
                if ((object.ReferenceEquals(this.DestinationField, value) != true)) {
                    this.DestinationField = value;
                    this.RaisePropertyChanged("Destination");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ALCS.HiChat.Cross.Service.User Sender {
            get {
                return this.SenderField;
            }
            set {
                if ((object.ReferenceEquals(this.SenderField, value) != true)) {
                    this.SenderField = value;
                    this.RaisePropertyChanged("Sender");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Service.IHiChatService", CallbackContract=typeof(ALCS.HiChat.Cross.Service.IHiChatServiceCallback))]
    public interface IHiChatService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHiChatService/Connect", ReplyAction="http://tempuri.org/IHiChatService/ConnectResponse")]
        bool Connect(ALCS.HiChat.Cross.Service.User newUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHiChatService/Connect", ReplyAction="http://tempuri.org/IHiChatService/ConnectResponse")]
        System.Threading.Tasks.Task<bool> ConnectAsync(ALCS.HiChat.Cross.Service.User newUser);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHiChatService/Disconnect")]
        void Disconnect(ALCS.HiChat.Cross.Service.User user);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHiChatService/Disconnect")]
        System.Threading.Tasks.Task DisconnectAsync(ALCS.HiChat.Cross.Service.User user);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHiChatService/PublishMessage")]
        void PublishMessage(ALCS.HiChat.Cross.Service.Message message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHiChatService/PublishMessage")]
        System.Threading.Tasks.Task PublishMessageAsync(ALCS.HiChat.Cross.Service.Message message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHiChatServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHiChatService/RouteMessage")]
        void RouteMessage(ALCS.HiChat.Cross.Service.Message message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHiChatServiceChannel : ALCS.HiChat.Cross.Service.IHiChatService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class HiChatServiceClient : System.ServiceModel.DuplexClientBase<ALCS.HiChat.Cross.Service.IHiChatService>, ALCS.HiChat.Cross.Service.IHiChatService {
        
        public HiChatServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public HiChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public HiChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public HiChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public HiChatServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool Connect(ALCS.HiChat.Cross.Service.User newUser) {
            return base.Channel.Connect(newUser);
        }
        
        public System.Threading.Tasks.Task<bool> ConnectAsync(ALCS.HiChat.Cross.Service.User newUser) {
            return base.Channel.ConnectAsync(newUser);
        }
        
        public void Disconnect(ALCS.HiChat.Cross.Service.User user) {
            base.Channel.Disconnect(user);
        }
        
        public System.Threading.Tasks.Task DisconnectAsync(ALCS.HiChat.Cross.Service.User user) {
            return base.Channel.DisconnectAsync(user);
        }
        
        public void PublishMessage(ALCS.HiChat.Cross.Service.Message message) {
            base.Channel.PublishMessage(message);
        }
        
        public System.Threading.Tasks.Task PublishMessageAsync(ALCS.HiChat.Cross.Service.Message message) {
            return base.Channel.PublishMessageAsync(message);
        }
    }
}
