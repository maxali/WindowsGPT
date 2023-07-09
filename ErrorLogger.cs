using System;
using System.IO;

namespace WindowsGPT
{
    public class ErrorLogger
    {
        public void Log(Exception ex)
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: {ex}\n");
        }
    }
}
