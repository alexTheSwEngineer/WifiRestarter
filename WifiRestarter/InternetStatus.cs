using System;
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
            try
            {
                Ping myPing = new Ping();
                byte[] buffer = new byte[32];
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(GoogleCom, pingTimeOut, buffer, pingOptions);
                return (reply.Status != IPStatus.Success);
            }
            catch (PingException)
            {
                return true;
            }
        }
    }
}
