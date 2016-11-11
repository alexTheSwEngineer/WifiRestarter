using System;

namespace WifiRestarter
{
    public interface ILogger
    {
        void Info(string line);
        void Trace(string line);
        void Error(string line);
        void Error(Exception e);
    }
}