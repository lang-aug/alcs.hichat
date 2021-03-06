﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using ALCS.HiChat.Service;

namespace ALCS.HiChat.Server
{
    class Server
    {
        static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(HiChatService)))
            {
                try
                {
                    serviceHost.Open();
                    Console.WriteLine("Service running");
                    Console.ReadLine();
                }
                catch (TimeoutException timeoutException)
                {
                    Console.WriteLine(timeoutException.Message);
                    Console.ReadLine();
                }
                catch (CommunicationException commException)
                {
                    Console.WriteLine(commException.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
