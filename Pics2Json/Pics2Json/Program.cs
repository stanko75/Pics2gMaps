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
using System.Reflection;

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
        //NameValueCollection folders = new NameValueCollection();
        //Console.Write("Enter path of folder where are pictures: ");
        //string strFolder = Console.ReadLine();

        NameValueCollection folders = ConfigurationManager.GetSection("folders") as NameValueCollection;
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();

        if (!(folders is null))
        {
          foreach (string config in folders)
          {
            string[] configs = folders.Get(config).Split(';');
            string galleryName = configs[0];
            string strPicFolder = Path.Combine($"{configs[1]}{galleryName}", "pics");
            string wwwFolder = Path.Combine($"{configs[1]}{galleryName}", "www");
            string webPath = $"{configs[2]}{galleryName}/pics";
            string jsonFilename = $"{galleryName}.json";

            PrepareTemplates(wwwFolder, galleryName);

            if (Directory.Exists(strPicFolder))
            {
              ProcessDirectory(strPicFolder, webPath);

              string json = jsonSerialiser.Serialize(myFiles);

              File.WriteAllText(Path.Combine(wwwFolder, jsonFilename), json);
            }
            else
            {
              WriteLog($"Folder: {strPicFolder} does not exist");
            }
          }

          //string completeJson = jsonSerialiser.Serialize(myFiles);
          //File.WriteAllText("c:" + @"\myJosn.json", completeJson);
        }
      }
      catch (Exception e)
      {
        WriteLog($"Error: {e.Message}");
      }

      Console.WriteLine("Press any key...");
      Console.ReadKey();
    }

    private static void PrepareTemplates(string wwwFolder, string galleryName)
    {
      string templatePath = $"..\\..\\..\\..\\Json2gMap\\";

      //index.html
      string indexName = "index.html";
      string indexHtmlPath = $"{templatePath}{indexName}";
      string indexHtml = File.ReadAllText(indexHtmlPath);
      indexHtml = indexHtml.Replace("/*galleryName*/", galleryName);

      if (!Directory.Exists(wwwFolder))
        Directory.CreateDirectory(wwwFolder);

      File.WriteAllText(Path.Combine(wwwFolder, indexName), indexHtml);

      //script\map.js
      string scriptFolderName = "script";
      string mapJsName = $"map.js";
      string scriptTemplatePath = Path.Combine(templatePath, scriptFolderName);
      string mapJsPath = Path.Combine(scriptTemplatePath, mapJsName);
      string mapJs = File.ReadAllText(mapJsPath);
      mapJs = mapJs.Replace("/*picsJson*/", $"{galleryName}.json");

      string scriptFolder = Path.Combine(wwwFolder, scriptFolderName);
      if (!Directory.Exists(scriptFolder))
        Directory.CreateDirectory(scriptFolder);

      File.WriteAllText(Path.Combine(scriptFolder, mapJsName), mapJs);

      //copy lib folder
      string libName = "lib";
      string libFolder = Path.Combine(wwwFolder, libName);
      if (!Directory.Exists(libFolder))
        Directory.CreateDirectory(libFolder);

      string libTemplatePath = Path.Combine(templatePath, libName);

      string jqueryName = "jquery-3.3.1.js";

      File.Copy(Path.Combine(libTemplatePath, jqueryName), Path.Combine(libFolder, jqueryName));
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
        WriteLog($"Error extracting GPS location: {e}, file: {path}");
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