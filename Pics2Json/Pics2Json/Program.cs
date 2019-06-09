using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Directory = System.IO.Directory;
using System.Collections.Specialized;
using System.Configuration;
using Pics2Json;

//using System.Object;

namespace ConsoleApp1
{

  class Program
  {
    static void Main(string[] args)
    {
      Log log = new Log();
      List<FileProcessing.MyObjectInJson> picsJson = new List<FileProcessing.MyObjectInJson>();
      List<FileProcessing.MyObjectInJson> thumbsJson = new List<FileProcessing.MyObjectInJson>();

      try
      {
        //NameValueCollection folders = new NameValueCollection();
        //Console.Write("Enter path of folder where are pictures: ");
        //string strFolder = Console.ReadLine();

        GallerySettingsConfig galeries = (GallerySettingsConfig)ConfigurationManager.GetSection("galeries");
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
        FileProcessing fileProcessing = new FileProcessing();

        if (!(galeries is null))
        {
          foreach (GallerySettingsElement setting in galeries.MilosevBlogInstances)
          {
            string galleryName = setting.GalleryName;
            string rootGalleryFolder = setting.RootGalleryFolder;
            string strThumbnailsFolder = Path.Combine($"{rootGalleryFolder}{galleryName}", "thumbs");
            string strPicFolder = Path.Combine($"{rootGalleryFolder}{galleryName}", "pics");

            string wwwFolder = Path.Combine($"{rootGalleryFolder}{galleryName}", "www");
            string webPath = $"{setting.WebPath}{galleryName}";
            string jsonFilename = $"{galleryName}.json";
            string jsonThumbsFilename = $"{galleryName}Thumbs.json";

            fileProcessing.PrepareTemplates(wwwFolder, galleryName, log);

            if (Directory.Exists(strPicFolder))
            {
              fileProcessing.ProcessDirectory(strPicFolder, webPath, strThumbnailsFolder, picsJson, thumbsJson, log);

              string json = jsonSerialiser.Serialize(picsJson);

              File.WriteAllText(Path.Combine(wwwFolder, jsonFilename), json);

              json = jsonSerialiser.Serialize(thumbsJson);
              File.WriteAllText(Path.Combine(wwwFolder, jsonThumbsFilename), json);
            }
            else
            {
              log.WriteLog($"Folder: {strPicFolder} does not exist");
            }
          }
        }
      }
      catch (Exception e)
      {
        log.WriteLog($"Error: {e.Message}");
      }

      Console.WriteLine("Press any key...");
      Console.ReadKey();
    }

  }
}