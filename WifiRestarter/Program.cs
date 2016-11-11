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
        public const string RestartScript = "Disable-NetAdapter –Name \"Wi-Fi\" –Confirm:$False; Enable-NetAdapter -Name \"Wi-Fi\" ";
        public const int DelaySeconds = 30;
        private PowerShellExecutor _cmd;
        private Program()
        {
            //wire powerShell to write console
             _cmd = new PowerShellExecutor()
                        .WhenOutputLog(LogPSObject)
                        .WhenError(inputObject =>
                        {
                            LogPSObject(inputObject);
                            throw new Exception("Something went wrong" + inputObject.BaseObject);
                        });
        }

        public async Task Run()
        {
            Console.WriteLine();
            for (int consecutiveRestarts = 0; consecutiveRestarts < 4;)
            {
                if (InternetStatus.IsDown())
                {
                    Restart();
                    consecutiveRestarts++;
                }
                else
                {
                    consecutiveRestarts = 0;
                }
                await Task.Delay(new TimeSpan(0, 0, DelaySeconds));
            }
            ConsoleExtensions.WriteLine("Maximum number of restarts. Press any key to exit");
            Console.ReadKey();
            
        }

        public void Restart()
        {
            ConsoleExtensions.OverwriteLine("Restarting... at "+DateTime.Now);            
            _cmd.Execute(RestartScript);
            ConsoleExtensions.WriteLine("Restarted successfully at " + DateTime.Now);
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
                Console.ReadKey();
            }
        }
        
        public void LogPSObject(PSObject obj)
        {
            Console.Write("PowerShell::" + obj.BaseObject.ToString());
        }

              
    }
}
