﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WifiRestarter
{
    class InternetStatus
    {
        public const string RestartScript = "Disable-NetAdapter –Name \"Wi-Fi\" –Confirm:$False; Enable-NetAdapter -Name \"Wi-Fi\" -Confirm:$False";
        public const string GoogleCom = "www.google.com";
        public const int pingTimeOut = 1000;

        public static  bool IsDown()
        {
            ConsoleExtensions.OverwriteLine("Checking Internet....");            
            try
            {
                Ping myPing = new Ping();
                byte[] buffer = new byte[32];
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(GoogleCom, pingTimeOut, buffer, pingOptions);
                var internetDown = (reply.Status != IPStatus.Success);
                if (internetDown)
                {
                    ConsoleExtensions.WriteLine("Internet is down at " + DateTime.Now);
                }
                else
                {
                    ConsoleExtensions.OverwriteLine("Internet is up at" + DateTime.Now);
                }
                
                return internetDown;
            }
            catch (PingException)
            {
                ConsoleExtensions.WriteLine("Internet is down at " + DateTime.Now);
                return true;
            }
        }
    }
}
