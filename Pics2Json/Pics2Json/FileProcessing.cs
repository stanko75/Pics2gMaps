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

    public void ProcessDirectory(string targetDirectory, string webPath, string strThumbnailsFolder, List<MyObjectInJson> myFiles)
    {
      // Process the list of files found in the directory.
      string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
      foreach (string fileName in fileEntries)
        ProcessFile(fileName, webPath, strThumbnailsFolder, myFiles);

      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
        ProcessDirectory(subdirectory, webPath, strThumbnailsFolder, myFiles);
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string path, string webPath, string strThumbnailsFolder, List<MyObjectInJson> myFiles)
    {
      if (!System.IO.Directory.Exists(strThumbnailsFolder))
        System.IO.Directory.CreateDirectory(strThumbnailsFolder);

      ImagesProcessing imagesProcessing = new ImagesProcessing();

      imagesProcessing.ResizeImage(path, Path.Combine(strThumbnailsFolder, Path.GetFileName(path)), 200, 200);
      //string thumbNailsFolder = Path.Combine(strThumbnailsFolder, Path.GetFileName(path));
      //imagesProcessing.Resize_Picture(path, thumbNailsFolder, 200, 200, 100);

      Log log = new Log();

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
  }
}
