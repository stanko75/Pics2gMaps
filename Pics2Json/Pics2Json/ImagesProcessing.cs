using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

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

        int OrientationKey = 0x0112;
        const int NotSpecified = 0;
        const int NormalOrientation = 1;
        const int MirrorHorizontal = 2;
        const int UpsideDown = 3;
        const int MirrorVertical = 4;
        const int MirrorHorizontalAndRotateRight = 5;
        const int RotateLeft = 6;
        const int MirorHorizontalAndRotateLeft = 7;
        const int RotateRight = 8;

        if (image.PropertyIdList.Contains(OrientationKey))
        {
          if (image.PropertyIdList.Contains(OrientationKey))
          {
            var orientation = (int)image.GetPropertyItem(OrientationKey).Value[0];
            switch (orientation)
            {
              case NotSpecified: // Assume it is good.
              case NormalOrientation:
                // No rotation required.
                break;
              case MirrorHorizontal:
                thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipX);
                break;
              case UpsideDown:
                thumbnail.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
              case MirrorVertical:
                thumbnail.RotateFlip(RotateFlipType.Rotate180FlipX);
                break;
              case MirrorHorizontalAndRotateRight:
                thumbnail.RotateFlip(RotateFlipType.Rotate90FlipX);
                break;
              case RotateLeft:
                thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
                break;
              case MirorHorizontalAndRotateLeft:
                thumbnail.RotateFlip(RotateFlipType.Rotate270FlipX);
                break;
              case RotateRight:
                thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);
                break;
              default:
                throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
            }
          }
        }

        //thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
        thumbnail.Save(saveTo, info[1], encoderParameters);
        log.WriteLog($"Thumbnail from file: {originalFilename} created in {saveTo}");
      }
      catch (Exception e)
      {
        log.WriteLog($"Error creating thumbnail: {e.Message}");
      }
    }
  }
}