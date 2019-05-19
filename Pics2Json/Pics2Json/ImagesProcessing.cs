using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Pics2Json
{
  class ImagesProcessing
  {
    public void ResizeImage(string originalFilename, string saveTo,
                     int canvasWidth, int canvasHeight)
    {
      Log log = new Log();

      try
      {
        Image image = Image.FromFile(originalFilename);

        Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
        Graphics graphic = Graphics.FromImage(thumbnail);

        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphic.SmoothingMode = SmoothingMode.HighQuality;
        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphic.CompositingQuality = CompositingQuality.HighQuality;

        int originalWidth = image.Width;
        int originalHeight = image.Height;

        double ratioX = canvasWidth / (double)originalWidth;
        double ratioY = canvasHeight / (double)originalHeight;

        double ratio = ratioX < ratioY ? ratioX : ratioY;

        int newHeight = Convert.ToInt32(originalHeight * ratio);
        int newWidth = Convert.ToInt32(originalWidth * ratio);

        int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
        int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

        graphic.Clear(Color.White); // white padding

        graphic.DrawImage(image, posX, posY, newWidth, newHeight);

        ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
        EncoderParameters encoderParameters;
        encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                         100L);
        thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
        thumbnail.Save(saveTo, info[1], encoderParameters);
        log.WriteLog($"Thumbnail from file: {originalFilename} created in {saveTo}");
      }
      catch (Exception e)
      {
        log.WriteLog($"Error creating thumbnail: {e.Message}");
      }
    }

    public void Resize_Picture(string Org, string Des, int FinalWidth, int FinalHeight, int ImageQuality)
    {
      System.Drawing.Bitmap NewBMP;
      System.Drawing.Graphics graphicTemp;
      System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Org);

      int iWidth;
      int iHeight;
      if ((FinalHeight == 0) && (FinalWidth != 0))
      {
        iWidth = FinalWidth;
        iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
      }
      else if ((FinalHeight != 0) && (FinalWidth == 0))
      {
        iHeight = FinalHeight;
        iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
      }
      else
      {
        iWidth = FinalWidth;
        iHeight = FinalHeight;
      }

      NewBMP = new System.Drawing.Bitmap(iWidth, iHeight);
      graphicTemp = System.Drawing.Graphics.FromImage(NewBMP);
      graphicTemp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
      graphicTemp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
      graphicTemp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      graphicTemp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      graphicTemp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
      graphicTemp.DrawImage(bmp, 0, 0, iWidth, iHeight);
      graphicTemp.Dispose();
      System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
      System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ImageQuality);
      encoderParams.Param[0] = encoderParam;
      System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
      for (int fwd = 0; fwd <= arrayICI.Length - 1; fwd++)
      {
        if (arrayICI[fwd].FormatDescription.Equals("JPEG"))
        {
          NewBMP.Save(Des, arrayICI[fwd], encoderParams);
        }
      }

      NewBMP.Dispose();
      bmp.Dispose();
    }

  }
}