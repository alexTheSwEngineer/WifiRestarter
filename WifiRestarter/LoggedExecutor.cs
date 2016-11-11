using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WifiRestarter
{
    public class ActionLogger
    {
        public ILogger Logger { get; private set;}
        public ActionLogger(ILogger logger)
        {
            Logger = logger;
        }
        public void Log(Action action)
        {
            try
            {
                var methodName = action.Method.Name;
                Logger.Info($"Calling:{methodName}");
                action();
                Logger.Info($"Called:{methodName}");
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
            }
        }

        public Out Log<Out>(Func<Out> action)
        {
           
            try
            {
                var methodName = action.Method.Name;
                Logger.Info($"Calling:{methodName}");
                var res = action();
                Logger.Info($"Called:{methodName} result:{res}");
                return res;
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
                throw;
            }
        }
    }
}
