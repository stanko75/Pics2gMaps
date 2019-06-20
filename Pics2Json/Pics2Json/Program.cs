using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Configuration;
using Pics2Json;
using System.Diagnostics;

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
        GallerySettingsConfig galleries = (GallerySettingsConfig)ConfigurationManager.GetSection("Galleries");
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
        FileProcessing fileProcessing = new FileProcessing();

        if (!(galleries is null))
        {
          foreach (GallerySettingsElement setting in galleries.GallerySettingsInstances)
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

        MergedGalleriesSettingsConfig mergedGalleries = (MergedGalleriesSettingsConfig)ConfigurationManager.GetSection("mergedGalleries");
        /*
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
        FileProcessing fileProcessing = new FileProcessing();
        */

        if (!(mergedGalleries is null))
        {
          List<FileProcessing.MyObjectInJson> picsJson = new List<FileProcessing.MyObjectInJson>();
          List<FileProcessing.MyObjectInJson> thumbsJson = new List<FileProcessing.MyObjectInJson>();

          foreach (MergedGalleriesSettingsElement setting in mergedGalleries.MergedGalleriesSettingsInstances)
          {
            fileProcessing.ProcessDirectory("all"
              , setting.Folder
              , "milosev.com"
              , picsJson
              , thumbsJson
              , false
              , log
              , isAll: true
            );
          }

          string json = jsonSerialiser.Serialize(picsJson);
          File.WriteAllText(Path.Combine(@"C:\projects\gallery\all\all.json"), json);
          fileProcessing.PrepareTemplates(@"C:\projects\gallery\all"
            , "all"
            , "List of all places"
            , "List of all places without images"
            , string.Empty
            , "www.milosev.com"
            , "2"
            , string.Empty
            , string.Empty
            , log);
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