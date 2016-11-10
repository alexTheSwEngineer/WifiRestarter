using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WifiRestarter
{
    public class PowerShellExecutor
    {
        private Action<PSObject> OnError;
        private Action<PSObject> OnLog;
        private List<string> Scripts;
        
        public PowerShellExecutor()
        {
            Scripts = new List<string>();
        }
        public  PowerShellExecutor WhenError(Action<PSObject> action)
        {
            OnError = action;
            return this;
        }

        public PowerShellExecutor WhenOutputLog(Action<PSObject> action)
        {
            OnLog = action;
            return this;
        }

        public PowerShellExecutor AddScript(string script)
        {
            Scripts.Add(script);
            return this;
        }

        public void Execute(Action<PowerShell> work)
        {
            using (PowerShell cmd = PowerShell.Create())
            {
                PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                outputCollection.DataAdded += (source, e) =>
                {
                    OnLog?.Invoke(outputCollection[e.Index]);
                };
                cmd.Streams.Error.DataAdded += (source, e) =>
                {
                    OnError?.Invoke(outputCollection[e.Index]);
                };


                work(cmd);


                cmd.Invoke();
            }
        }

        public void Execute(string script)
        {
            Execute(x => x.AddScript(script));
        }
    }
}
