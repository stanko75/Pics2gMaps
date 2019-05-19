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
      List<FileProcessing.MyObjectInJson> myFiles = new List<FileProcessing.MyObjectInJson>();

      try
      {
        //NameValueCollection folders = new NameValueCollection();
        //Console.Write("Enter path of folder where are pictures: ");
        //string strFolder = Console.ReadLine();

        NameValueCollection folders = ConfigurationManager.GetSection("folders") as NameValueCollection;
        JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
        FileProcessing fileProcessing = new FileProcessing();

        if (!(folders is null))
        {
          foreach (string config in folders)
          {
            string[] configs = folders.Get(config).Split(';');
            string galleryName = configs[0];
            string strThumbnailsFolder = Path.Combine($"{configs[1]}{galleryName}", "thumbs");
            string strPicFolder = Path.Combine($"{configs[1]}{galleryName}", "pics");
            string wwwFolder = Path.Combine($"{configs[1]}{galleryName}", "www");
            string webPath = $"{configs[2]}{galleryName}/pics";
            string jsonFilename = $"{galleryName}.json";

            PrepareTemplates(wwwFolder, galleryName);

            if (Directory.Exists(strPicFolder))
            {
              fileProcessing.ProcessDirectory(strPicFolder, webPath, strThumbnailsFolder, myFiles);

              string json = jsonSerialiser.Serialize(myFiles);

              File.WriteAllText(Path.Combine(wwwFolder, jsonFilename), json);
            }
            else
            {
              log.WriteLog($"Folder: {strPicFolder} does not exist");
            }
          }

          //string completeJson = jsonSerialiser.Serialize(myFiles);
          //File.WriteAllText("c:" + @"\myJosn.json", completeJson);
        }
      }
      catch (Exception e)
      {
        log.WriteLog($"Error: {e.Message}");
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

      File.Copy(Path.Combine(libTemplatePath, jqueryName), Path.Combine(libFolder, jqueryName), true);
    }

  }
}