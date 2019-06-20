using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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

    public void ProcessDirectory(string galleryName
      , string targetDirectory
      , string webPath
      , List<MyObjectInJson> picsJson
      , List<MyObjectInJson> thumbsJson
      , bool resizeImages
      , Log log
      , string subdirectoryName = ""
      , bool isAll = false)
    {
      // Process the list of files found in the directory.

      string picsFolder = Path.Combine(targetDirectory, "pics");
      string thumbnailsFolder = Path.Combine(targetDirectory, "thumbs");
      if (!isAll && System.IO.Directory.Exists(picsFolder))
      {
        string getFilesFrom = isAll ?  targetDirectory : picsFolder;
        string[] fileEntries = System.IO.Directory.GetFiles(getFilesFrom);
        foreach (string fileName in fileEntries)
          ProcessFile(galleryName, fileName, webPath, thumbnailsFolder, picsJson, thumbsJson, resizeImages, log, subdirectoryName);
      }

      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
      {
        string[] directoryNamesInPath = subdirectory.Split('\\');
        subdirectoryName = Path.Combine(subdirectoryName, directoryNamesInPath[directoryNamesInPath.Length - 1]);
        ProcessDirectory(galleryName, subdirectory, webPath, picsJson, thumbsJson, resizeImages, log, subdirectoryName);
      }
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string galleryName
      , string path
      , string webPath
      , string strThumbnailsFolder
      , List<MyObjectInJson> picsJson
      , List<MyObjectInJson> thumbsJson
      , bool resizeImages
      , Log log
      , string subdirectoryName
      , bool isAll = false)
    {
      if (!isAll)
      {
        if (!System.IO.Directory.Exists(strThumbnailsFolder))
          System.IO.Directory.CreateDirectory(strThumbnailsFolder);
      }

      ImagesProcessing imagesProcessing = new ImagesProcessing();

      if (resizeImages)
        imagesProcessing.ResizeImage(path, Path.Combine(strThumbnailsFolder, Path.GetFileName(path)), 200, 200);

      try
      {
        IReadOnlyList<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(path);

        GpsDirectory gps = directories.OfType<GpsDirectory>().FirstOrDefault();

        GeoLocation location = gps?.GetGeoLocation();

        if (!(location is null))
        {
          int indexOfGalleryNameInPath = path.IndexOf(galleryName);
          indexOfGalleryNameInPath = indexOfGalleryNameInPath + galleryName.Length;
          string relativeSysPathFileName = path.Substring(indexOfGalleryNameInPath, path.Length - indexOfGalleryNameInPath);
          relativeSysPathFileName = relativeSysPathFileName.Substring(0, relativeSysPathFileName.LastIndexOf(@"\") + 1);

          string picsPath = relativeSysPathFileName.Replace(@"\", "/");

          string thumbsPath = string.Empty;
          if (picsPath.IndexOf("pics") > 0)
          {
            thumbsPath = picsPath.Substring(0, picsPath.IndexOf("pics")) + "thumbs/";
          }

          string jsonPicsPath = $"..{picsPath}{Path.GetFileName(path)}";
          string jsonThumbsPath = $"..{thumbsPath}{Path.GetFileName(path)}";

          if (!webPath.EndsWith("/"))
          {
            webPath = webPath + '/';
          }

          if (!string.IsNullOrWhiteSpace(subdirectoryName))
          {
            if (!subdirectoryName.EndsWith("/"))
            {
              subdirectoryName = subdirectoryName + '/';
            }

            webPath = webPath + subdirectoryName;
          }

          picsJson.Add(new MyObjectInJson { FileName = isAll ? path : jsonPicsPath, Latitude = location.Latitude, Longitude = location.Longitude });
          thumbsJson.Add(new MyObjectInJson { FileName = jsonThumbsPath, Latitude = location.Latitude, Longitude = location.Longitude });
        }
        else
        {
          log.WriteLog($"Location is null.");
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

    public void PrepareTemplates(string wwwFolder
      , string galleryName
      , string ogTitle
      , string ogDescription
      , string ogImage
      , string webPath
      , string zoom
      , string joomlaThumbsPath
      , string joomlaImgSrcPath
      , Log log
    )
    {
      string gapikey = ConfigurationManager.AppSettings.Get("gapikey");
      string scriptsFolder = Path.Combine(wwwFolder, "script");

      ReplaceHtml("/*galleryName*/", galleryName, "index.html", string.Empty, wwwFolder, log);
      ReplaceHtml("/*gapikey*/", gapikey, "index.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*ogTitle*/", ogTitle, "index.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*ogDescription*/", ogDescription, "index.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*ogImage*/", $"{webPath}/{ogImage}", "index.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*ogUrl*/", $"{webPath}/www/index.html", "index.html", wwwFolder, wwwFolder, log);

      ReplaceHtml("/*picsJson*/", galleryName, "thumbnails.js", "script", scriptsFolder, log);
      ReplaceHtml("/*zoom*/", zoom, "thumbnails.js", wwwFolder + "\\" + "script", scriptsFolder, log);

      ReplaceHtml("/*picsJson*/", galleryName, "pics2maps.js", "script", scriptsFolder, log);
      ReplaceHtml("/*zoom*/", zoom, "pics2maps.js", wwwFolder + "\\" + "script", scriptsFolder, log);

      ReplaceHtml("/*ogTitle*/", ogTitle, "joomlaPreview.html", string.Empty, wwwFolder, log);
      ReplaceHtml("/*galleryName*/", galleryName, "joomlaPreview.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*joomlaThumbsPath*/", joomlaThumbsPath, "joomlaPreview.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*joomlaImgSrcPath*/", joomlaImgSrcPath, "joomlaPreview.html", wwwFolder, wwwFolder, log);
      ReplaceHtml("/*ogImage*/", ogImage, "joomlaPreview.html", wwwFolder, wwwFolder, log);

      CopyFile("jquery-3.3.1.js", "lib", wwwFolder, log);
      CopyFile("index.css", "css", wwwFolder, log);

      CopyFile("namespaces.js", "script", wwwFolder, log);
      CopyFile("map.js", "script", wwwFolder, log);
    }
  }
}