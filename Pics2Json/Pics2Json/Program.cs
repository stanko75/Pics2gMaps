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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

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
            string strThumbnailsFolder = Path.Combine($"{configs[1]}{galleryName}", "thumbs");
            string strPicFolder = Path.Combine($"{configs[1]}{galleryName}", "pics");
            string wwwFolder = Path.Combine($"{configs[1]}{galleryName}", "www");
            string webPath = $"{configs[2]}{galleryName}/pics";
            string jsonFilename = $"{galleryName}.json";

            PrepareTemplates(wwwFolder, galleryName);

            if (Directory.Exists(strPicFolder))
            {
              ProcessDirectory(strPicFolder, webPath, strThumbnailsFolder);

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

      File.Copy(Path.Combine(libTemplatePath, jqueryName), Path.Combine(libFolder, jqueryName), true);
    }

    public static void ProcessDirectory(string targetDirectory, string webPath, string strThumbnailsFolder)
    {
      // Process the list of files found in the directory.
      string[] fileEntries = Directory.GetFiles(targetDirectory);
      foreach (string fileName in fileEntries)
        ProcessFile(fileName, webPath, strThumbnailsFolder);

      // Recurse into subdirectories of this directory.
      string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
      foreach (string subdirectory in subdirectoryEntries)
        ProcessDirectory(subdirectory, webPath, strThumbnailsFolder);
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string path, string webPath, string strThumbnailsFolder)
    {
      if (!Directory.Exists(strThumbnailsFolder))
        Directory.CreateDirectory(strThumbnailsFolder);

      ResizeImage(path, new Size(200, 200), Path.Combine(strThumbnailsFolder, Path.GetFileName(path)));

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

    public static void ResizeImage(
      string fileName,
      Size size,
      string newFn,
      bool preserveAspectRatio = true)
    {
      try
      {
        using (var bmpInput = Image.FromFile(fileName))
        {
          using (var bmpOutput = new Bitmap(bmpInput, size))
          {

            resizeImage(fileName, newFn, 200, 200, bmpInput.Width, bmpInput.Height);

            foreach (var id in bmpInput.PropertyIdList)
              bmpOutput.SetPropertyItem(bmpInput.GetPropertyItem(id));
            bmpOutput.SetResolution(300.0f, 300.0f);
            bmpOutput.Save(newFn, ImageFormat.Jpeg);
          }
        }
        //WriteLog($"Thumbnail from file: {fileName} created in {newFn}");
      }
      catch (Exception e)
      {
        WriteLog($"Error creating thumbnail: {e.Message}");
      }
    }

    private static void resizeImage(string originalFilename, string saveTo,
                         /* note changed names */
                         int canvasWidth, int canvasHeight,
                         /* new */
                         int originalWidth, int originalHeight)
    {
      Image image = Image.FromFile(originalFilename);

      System.Drawing.Image thumbnail =
          new Bitmap(canvasWidth, canvasHeight); // changed parm names
      System.Drawing.Graphics graphic =
                   System.Drawing.Graphics.FromImage(thumbnail);

      graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphic.SmoothingMode = SmoothingMode.HighQuality;
      graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
      graphic.CompositingQuality = CompositingQuality.HighQuality;

      /* ------------------ new code --------------- */

      // Figure out the ratio
      double ratioX = (double)canvasWidth / (double)originalWidth;
      double ratioY = (double)canvasHeight / (double)originalHeight;
      // use whichever multiplier is smaller
      double ratio = ratioX < ratioY ? ratioX : ratioY;

      // now we can get the new height and width
      int newHeight = Convert.ToInt32(originalHeight * ratio);
      int newWidth = Convert.ToInt32(originalWidth * ratio);

      // Now calculate the X,Y position of the upper-left corner 
      // (one of these will always be zero)
      int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
      int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

      graphic.Clear(Color.White); // white padding
      graphic.DrawImage(image, posX, posY, newWidth, newHeight);

      /* ------------- end new code ---------------- */

      System.Drawing.Imaging.ImageCodecInfo[] info =
                       ImageCodecInfo.GetImageEncoders();
      EncoderParameters encoderParameters;
      encoderParameters = new EncoderParameters(1);
      encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                       100L);
      thumbnail.Save(saveTo, info[1],
                       encoderParameters);
    }

    public static void WriteLog(string msg)
    {
      Console.WriteLine(msg);

      try
      {
        File.AppendAllText("log.txt", msg + Environment.NewLine);
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error saving log file: {e.Message}");
      }
    }

  }
}