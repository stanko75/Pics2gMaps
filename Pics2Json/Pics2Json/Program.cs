using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = System.IO.Directory;
using System.Collections.Specialized;
using System.Configuration;

//using System.Object;

namespace ConsoleApp1
{

    class Program
    {
        private class MyObjectInJson
        {
            public string FileName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        static List<MyObjectInJson> myFiles = new List<MyObjectInJson>();

        static void Main(string[] args)
        {
            try
            {
                NameValueCollection folders = ConfigurationManager.GetSection("folders") as NameValueCollection;
                JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();

                if (!(folders is null))
                {
                    foreach (string config in folders)
                    {
                        string[] configs = folders.Get(config).Split(';');
                        string strFolder = configs[0];
                        string webPath = configs[1];
                        string jsonFilename = configs[2];

                        if (Directory.Exists(strFolder))
                        {
                            ProcessDirectory(strFolder, webPath);

                            string json = jsonSerialiser.Serialize(myFiles);

                            File.WriteAllText(Path.Combine(strFolder, jsonFilename), json);
                        }
                        else
                        {
                            WriteLog($"Folder: {strFolder} does not exist");
                        }
                    }

                    string completeJson = jsonSerialiser.Serialize(myFiles);
                    File.WriteAllText("c:" + @"\myJosn.json", completeJson);
                }
            }
            catch (Exception e)
            {
                WriteLog($"Error: {e.Message}");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        public static void ProcessDirectory(string targetDirectory, string webPath)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, webPath);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, webPath);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, string webPath)
        {
            try
            {
                IReadOnlyList<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(path);

                GpsDirectory gps = directories.OfType<GpsDirectory>().FirstOrDefault();

                GeoLocation location = gps?.GetGeoLocation();

                if (!webPath.EndsWith("/"))
                {
                    webPath = webPath + '/';
                }

                Uri baseUri = new Uri(webPath);
                Uri mainUri = new Uri(baseUri, Path.GetFileName(path));

                if (!(location is null))
                {
                    myFiles.Add(new MyObjectInJson { FileName = mainUri.AbsoluteUri, Latitude = location.Latitude, Longitude = location.Longitude });
                }

                WriteLog($"Processed file '{path}'.");
            } 
            catch (Exception e)
            {
                WriteLog($"Error extracting GPS location: {e}, file: {path}") ;
            }
        }

        public static void WriteLog(string msg)
        {
            Console.WriteLine(msg);

            try
            {
                File.AppendAllText("log.txt", msg);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving log file: {e.Message}");
            }
        }

    }
}