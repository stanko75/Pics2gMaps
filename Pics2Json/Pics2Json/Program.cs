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
        GallerySettingsConfig galleries = (GallerySettingsConfig)ConfigurationManager.GetSection("galleries");
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

          foreach (MergedGalleriesSettingsElement setting in mergedGalleries.MergedGalleriesSettingsInstances)
          {
            List<FileProcessing.MyObjectInJson> picsJson = new List<FileProcessing.MyObjectInJson>();
            List<FileProcessing.MyObjectInJson> thumbsJson = new List<FileProcessing.MyObjectInJson>();

            fileProcessing.ProcessDirectory(setting.GalleryName
              , setting.Folder
              , "milosev.com"
              , picsJson
              , thumbsJson
              , false
              , log
              , isAll: true
            );

            if (!System.IO.Directory.Exists(setting.GalleryPath))
              System.IO.Directory.CreateDirectory(setting.GalleryPath);

            //jsonSerialiser.MaxJsonLength

            int maxLength = 10000;

            if (picsJson.Count > maxLength)
            {
              int range = 0;
              do
              {
                range = range + maxLength;
                int rangeIndex = range - maxLength;
                int rangeCount = range < picsJson.Count ? maxLength : picsJson.Count - rangeIndex;
                List<FileProcessing.MyObjectInJson> picsJsonRange = picsJson.GetRange(rangeIndex, rangeCount);
                string json = jsonSerialiser.Serialize(picsJsonRange);
                File.WriteAllText(Path.Combine(setting.GalleryPath, $"{setting.GalleryName}From{rangeIndex}to{rangeIndex + rangeCount}.json"), json);
              } while (range < picsJson.Count);
            }
            else
            {
              string json = jsonSerialiser.Serialize(picsJson);
              File.WriteAllText(Path.Combine(setting.GalleryPath, setting.GalleryName + ".json"), json);
            }

            fileProcessing.PrepareTemplates(setting.GalleryPath
              , setting.GalleryName
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