using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WifiRestarter
{
    class Program
    {
        public const string RestartScript = "Disable-NetAdapter –Name \"Wi-Fi\" –Confirm:$False; Enable-NetAdapter -Name \"Wi-Fi\" -Confirm:$False";
        public const string GoogleCom = "www.google.com";
        public const int pingTimeOut = 1000;
        public const int DelaySeconds = 70;
        private HttpClient client;
        private Program()
        {
            client = new HttpClient();
        }

        public async Task Run()
        {
            var waitTask = Task.Delay(new TimeSpan(0, 0, DelaySeconds));
            var cmd = new PowerShellExecutor()
                    .WhenOutputLog(LogPSObject)
                    .WhenError(inputObject =>
                    {
                        LogPSObject(inputObject);
                        throw new Exception("Something went wrong" + inputObject.BaseObject);
                    });

            while (true)
            {
                if (await IsInternetDown())
                {
                    Console.Write("Restarting...");
                    cmd.Execute(RestartScript);
                }
                await waitTask;
            }
        }


        public static void Main(string[] args)
        {
            try
            {
                var app = new Program();
                AsyncContext.Run(app.Run);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error was encountered " + e);
                Console.Out.Write(e.StackTrace);
                Console.Write("Press any key to Exit");
                var res = Console.ReadKey();
            }
        }
        
        public void LogPSObject(PSObject obj)
        {
            Console.Write("PowerShell::" + obj.BaseObject.ToString());
        }

        public async Task<bool> IsInternetDown()
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
