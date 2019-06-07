using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Pics2Json
{
  class FileProcessing
  {
    string templatePath = $"Json2gMap";

    public class MyObjectInJson
    {
      public string FileName { get; set; }
      public double Latitude { get; set; }
      public double Longitude { get; set; }
    }

    public void ProcessDirectory(string targetDirectory, string webPath, string strThumbnailsFolder, List<MyObjectInJson> picsJson, List<MyObjectInJson> thumbsJson, Log log)
    {
      // Process the list of files found in the directory.
      string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
      foreach (string fileName in fileEntries)
        ProcessFile(fileName, webPath, strThumbnailsFolder, picsJson, thumbsJson, log);

      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
        ProcessDirectory(subdirectory, webPath, strThumbnailsFolder, picsJson, thumbsJson, log);
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string path, string webPath, string strThumbnailsFolder, List<MyObjectInJson> picsJson, List<MyObjectInJson> thumbsJson, Log log)
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

        string picsPath = $"{webPath}pics/";
        string thumbsPath = $"{webPath}thumbs/";

        Uri picsUri = new Uri(picsPath);
        Uri mainPicsUri = new Uri(picsUri, Path.GetFileName(path));

        Uri thumbsUri = new Uri(thumbsPath);
        Uri mainThumbsUri = new Uri(thumbsUri, Path.GetFileName(path));

        if (!(location is null))
        {
          picsJson.Add(new MyObjectInJson { FileName = mainPicsUri.AbsolutePath, Latitude = location.Latitude, Longitude = location.Longitude });
          thumbsJson.Add(new MyObjectInJson { FileName = mainThumbsUri.AbsolutePath, Latitude = location.Latitude, Longitude = location.Longitude });
        }

        log.WriteLog($"Processed file '{path}'.");
      }
      catch (Exception e)
      {
        log.WriteLog($"Error extracting GPS location: {e}, file: {path}");
      }
    }

    private void ReplaceHtml(string replaceKey
      , string replaceString
      , string fileName
      , string origPath
      , string saveToPath
      , Log log
    )
    {
      if (!string.IsNullOrWhiteSpace(origPath))
        origPath = Path.Combine(templatePath, origPath);
      else
        origPath = templatePath;

      string origFileNameWithPath = Path.Combine(origPath, fileName);
      string fileContent = File.ReadAllText(origFileNameWithPath);

      fileContent = fileContent.Replace(replaceKey, replaceString);

      if (!System.IO.Directory.Exists(saveToPath))
        System.IO.Directory.CreateDirectory(saveToPath);

      string saveToFileNameWithPath = Path.Combine(saveToPath, fileName);

      File.WriteAllText(saveToFileNameWithPath, fileContent);
      log.WriteLog($"File {saveToFileNameWithPath} saved.");
    }

    private void CopyFile(string fileName, string origFolder, string saveToPath, Log log)
    {
      saveToPath = Path.Combine(saveToPath, origFolder);
      origFolder = Path.Combine(templatePath, origFolder);

      string saveToFileNameWithPath = Path.Combine(saveToPath, fileName);
      string saveOrigFileNameWithPath = Path.Combine(origFolder, fileName);

      if (!System.IO.Directory.Exists(saveToPath))
        System.IO.Directory.CreateDirectory(saveToPath);

      File.Copy(saveOrigFileNameWithPath, saveToFileNameWithPath, true);

      log.WriteLog($"File {saveOrigFileNameWithPath} copied to: {saveToFileNameWithPath}.");
    }

    public void PrepareTemplates(string wwwFolder, string galleryName, Log log)
    {
      string gapikey = ConfigurationManager.AppSettings.Get("gapikey");
      string scriptsFolder = Path.Combine(wwwFolder, "script");

      ReplaceHtml("/*galleryName*/", galleryName, "index.html", string.Empty, wwwFolder, log);
      ReplaceHtml("/*gapikey*/", gapikey, "index.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*picsJson*/", $"{galleryName}", "thumbnails.js", "script", scriptsFolder, log);
      ReplaceHtml("/*picsJson*/", $"{galleryName}", "pics2maps.js", "script", scriptsFolder, log);

      CopyFile("jquery-3.3.1.js", "lib", wwwFolder, log);
      CopyFile("index.css", "css", wwwFolder, log);

      CopyFile("namespaces.js", "script", wwwFolder, log);
      CopyFile("map.js", "script", wwwFolder, log);
    }
  }
}