using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Pics2Json
{
  class FileProcessing
  {
    public class MyObjectInJson
    {
      public string FileName { get; set; }
      public double Latitude { get; set; }
      public double Longitude { get; set; }
    }

    public void ProcessDirectory(string targetDirectory, string webPath, string strThumbnailsFolder, List<MyObjectInJson> myFiles, Log log)
    {
      // Process the list of files found in the directory.
      string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
      foreach (string fileName in fileEntries)
        ProcessFile(fileName, webPath, strThumbnailsFolder, myFiles, log);

      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
        ProcessDirectory(subdirectory, webPath, strThumbnailsFolder, myFiles, log);
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string path, string webPath, string strThumbnailsFolder, List<MyObjectInJson> myFiles, Log log)
    {
      if (!System.IO.Directory.Exists(strThumbnailsFolder))
        System.IO.Directory.CreateDirectory(strThumbnailsFolder);

      ImagesProcessing imagesProcessing = new ImagesProcessing();

      imagesProcessing.ResizeImage(path, Path.Combine(strThumbnailsFolder, Path.GetFileName(path)), 200, 200);
      //string thumbNailsFolder = Path.Combine(strThumbnailsFolder, Path.GetFileName(path));
      //imagesProcessing.Resize_Picture(path, thumbNailsFolder, 200, 200, 100);

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

        log.WriteLog($"Processed file '{path}'.");
      }
      catch (Exception e)
      {
        log.WriteLog($"Error extracting GPS location: {e}, file: {path}");
      }
    }

    public void PrepareTemplates(string wwwFolder, string galleryName, Log log)
    {
      string templatePath = $"Json2gMap";

      //index.html
      string indexName = "index.html";
      string indexHtmlPath = Path.Combine(templatePath, indexName);
      string indexHtml = File.ReadAllText(indexHtmlPath);
      indexHtml = indexHtml.Replace("/*galleryName*/", galleryName);

      if (!System.IO.Directory.Exists(wwwFolder))
        System.IO.Directory.CreateDirectory(wwwFolder);

      File.WriteAllText(Path.Combine(wwwFolder, indexName), indexHtml);
      log.WriteLog($"File {Path.Combine(wwwFolder, indexName)} saved.");

      //script\map.js
      string scriptFolderName = "script";
      string mapJsName = $"map.js";
      string thumbnailsJsName = $"thumbnails.js";
      string scriptTemplatePath = Path.Combine(templatePath, scriptFolderName);
      string mapJsPath = Path.Combine(scriptTemplatePath, mapJsName);
      string thumbnailsJsPath = Path.Combine(scriptTemplatePath, thumbnailsJsName);
      string mapJs = File.ReadAllText(mapJsPath);
      mapJs = mapJs.Replace("/*picsJson*/", $"{galleryName}.json");

      string thumbnailsJs = File.ReadAllText(thumbnailsJsPath);
      thumbnailsJs = thumbnailsJs.Replace("/*picsJson*/", $"{galleryName}");

      string scriptFolder = Path.Combine(wwwFolder, scriptFolderName);
      if (!System.IO.Directory.Exists(scriptFolder))
        System.IO.Directory.CreateDirectory(scriptFolder);

      File.WriteAllText(Path.Combine(scriptFolder, mapJsName), mapJs);
      log.WriteLog($"File {Path.Combine(scriptFolder, mapJsName)} saved.");

      File.WriteAllText(Path.Combine(scriptFolder, thumbnailsJsName), thumbnailsJs);
      log.WriteLog($"File {Path.Combine(scriptFolder, thumbnailsJsName)} saved.");

      //copy lib folder
      string libName = "lib";
      string libFolder = Path.Combine(wwwFolder, libName);
      if (!System.IO.Directory.Exists(libFolder))
        System.IO.Directory.CreateDirectory(libFolder);

      string libTemplatePath = Path.Combine(templatePath, libName);

      string jqueryName = "jquery-3.3.1.js";

      File.Copy(Path.Combine(libTemplatePath, jqueryName), Path.Combine(libFolder, jqueryName), true);
      log.WriteLog($"File {Path.Combine(libTemplatePath, jqueryName)} copied to: {Path.Combine(libFolder, jqueryName)}.");
    }
  }
}