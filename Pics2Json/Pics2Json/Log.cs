using System;
using System.IO;

namespace Pics2Json
{
  class Log
  {
    public void WriteLog(string msg)
    {
      Console.WriteLine(msg);

      try
      {
        File.AppendAllText("log.txt", msg + Environment.NewLine);
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error saving log file: {e.Message}");
      }
    }
  }
}
