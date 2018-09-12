using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Pics2Json
{
    class Program
    {
        public class MyObjectInJson
        {
            public string FileName { get; set; }
        }

        static List<MyObjectInJson> myFiles = new List<MyObjectInJson>();

        static void Main(string[] args)
        {
            if (Directory.Exists(@"C:\tmp"))
            {
                ProcessDirectory(@"C:\tmp");
                //var tt = Directory.GetDirectories(@"D:\stanko");

                JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
                string json = jsonSerialiser.Serialize(myFiles);
                json = json.Replace(@"\\", @"\");

                File.WriteAllText(@"C:\Users\pera\Desktop\myJosn.json", json);
            }

            Console.ReadKey();
        }


        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {

            myFiles.Add(new MyObjectInJson { FileName = path });

            Console.WriteLine("Processed file '{0}'.", path);
        }
    }
}
