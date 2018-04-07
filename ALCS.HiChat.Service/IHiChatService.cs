using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ALCS.HiChat.Cross.Models;

namespace ALCS.HiChat.Service
{
    [ServiceContract(CallbackContract = typeof(IHiChatServiceCallback))]
    public interface IHiChatService
    {
        [OperationContract]
        bool Connect(User newUser);

        [OperationContract(IsOneWay = true)]
        void Disconnect(User user);

        [OperationContract(IsOneWay = true)]
        void PublishMessage(Message message);
    }

    public interface IHiChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RouteMessage(Message message);
    }
}
