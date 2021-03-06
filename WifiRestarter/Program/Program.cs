﻿using Nito.AsyncEx;
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
        private ActionLogger _actionLogger;
        private ILogger _logger { get { return _actionLogger.Logger; } }
        private Program()
        {
            //wire powerShell to write console
            _actionLogger = new ActionLogger(new ConsoleLogger());
            _cmd = new PowerShellExecutor()
                   .WhenOutputLog(LogPSObject)
                   .WhenError(inputObject =>
                   {
                       LogPSObject(inputObject);
                       throw new Exception("Something went wrong in the script " + inputObject.BaseObject);
                   });
        }

        public async Task Run()
        {
            Console.WriteLine();
            for (int consecutiveRestarts = 0; consecutiveRestarts < 4;)
            {
                var isInternetDown = _actionLogger.Log(InternetStatus.IsDown);
                if (isInternetDown)
                {
                    _actionLogger.Log(Restart);
                    _logger.Trace("Internet down. Wifi was restarted ");
                    consecutiveRestarts++;
                }
                else
                {
                    consecutiveRestarts = 0;
                }
                await Task.Delay(new TimeSpan(0, 0, DelaySeconds));
            }


            _logger.Trace("Maximum number of restarts exceeded. Press any key to exit");
            Console.ReadKey();
            
        }

        public void Restart()
        {      
            _cmd.Execute(RestartScript);
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
            _logger.Trace("PowerShell::" + obj.BaseObject.ToString());
        }

              
    }
}
