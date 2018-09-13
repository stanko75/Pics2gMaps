using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            NameValueCollection folders = ConfigurationManager.GetSection("folders") as NameValueCollection;

            if (!(folders is null))
            {
                foreach (string folder in folders)
                {
                    string strFolder = folders.Get(folder);
                    if (Directory.Exists(strFolder))
                    {
                        ProcessDirectory(strFolder);

                        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
                        string json = jsonSerialiser.Serialize(myFiles);

                        File.WriteAllText(strFolder + @"\myJosn.json", json);
                    }
                    else
                    {
                        Console.WriteLine($"Folder: {strFolder} does not exist");
                    }
                }
            }

            Console.WriteLine("Press any key...");
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
            try
            {
                IReadOnlyList<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(path);

                GpsDirectory gps = directories.OfType<GpsDirectory>().FirstOrDefault();

                GeoLocation location = gps?.GetGeoLocation();

                if (!(location is null))
                {
                    myFiles.Add(new MyObjectInJson { FileName = path, Latitude = location.Latitude, Longitude = location.Longitude });
                }

                Console.WriteLine("Processed file '{0}'.", path);
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Error extracting GPS location: {e}, file: {path}") ;
            }
        }

    }
}