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

      try
      {
        GallerySettingsConfig galeries = (GallerySettingsConfig)ConfigurationManager.GetSection("galeries");
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
        FileProcessing fileProcessing = new FileProcessing();

        if (!(galeries is null))
        {
          foreach (GallerySettingsElement setting in galeries.MilosevBlogInstances)
          {
            List<FileProcessing.MyObjectInJson> picsJson = new List<FileProcessing.MyObjectInJson>();
            List<FileProcessing.MyObjectInJson> thumbsJson = new List<FileProcessing.MyObjectInJson>();

            string galleryName = setting.GalleryName;
            string rootGalleryFolder = setting.RootGalleryFolder;
            string ogTitle = setting.OgTitle;
            string ogDescription = setting.OgDescription;
            string ogImage = setting.OgImage;
            string zoom = setting.Zoom;
            string joomlaThumbsPath = setting.JoomlaThumbsPath;
            string joomlaImgSrcPath = setting.JoomlaImgSrcPath;
            bool resizeImages = setting.ResizeImages;

            string rootGalleryFolderWithGalleryName = Path.Combine(rootGalleryFolder, galleryName);

            string wwwFolder = Path.Combine($"{rootGalleryFolderWithGalleryName}", "www");
            string webPath = $"{setting.WebPath}{galleryName}";
            string jsonFilename = $"{galleryName}.json";
            string jsonThumbsFilename = $"{galleryName}Thumbs.json";

            fileProcessing.PrepareTemplates(wwwFolder, galleryName, ogTitle, ogDescription, ogImage, webPath, zoom, joomlaThumbsPath, joomlaImgSrcPath, log);

            fileProcessing.ProcessDirectory(galleryName, rootGalleryFolderWithGalleryName, webPath, picsJson, thumbsJson, resizeImages, log);

            string json = jsonSerialiser.Serialize(picsJson);

            File.WriteAllText(Path.Combine(wwwFolder, jsonFilename), json);

            json = jsonSerialiser.Serialize(thumbsJson);
            File.WriteAllText(Path.Combine(wwwFolder, jsonThumbsFilename), json);
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