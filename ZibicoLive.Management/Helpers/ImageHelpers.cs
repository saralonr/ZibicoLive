using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management.Helpers
{
    public class ImageHelpers
    {
        public static FileUploadResult Upload(string type, HttpPostedFileBase postedFile, int? width = null, int? height = null)
        {
            try
            {
                WebImage img = new WebImage(postedFile.InputStream);
                FileInfo photoInfo = new FileInfo(postedFile.FileName);
                if (width == null || height == null)
                {
                    width = img.Width;
                    height = img.Height;
                }
                Random rnd = new Random();
                int random = rnd.Next(1000, 9999);
                string filename = postedFile.FileName.Remove(postedFile.FileName.Length - photoInfo.Extension.Length, photoInfo.Extension.Length);
                string imgname = DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString() + "_" + Guid.NewGuid() + "_" + filename + "_" + random;
                string newPhoto = imgname + photoInfo.Extension;

                img.Save("~/uploads/temp/" + newPhoto);

                ImageHelpers.ImageResize("uploads/temp/" + newPhoto, "uploads/" + type + "/", newPhoto, (int)width, (int)height);

                return new FileUploadResult() { Status = true, URL = "/uploads/" + type + "/" + newPhoto };
            }
            catch (Exception ex)
            {
                return new FileUploadResult() { Status = false };
            }
        }
        public static void ImageResize(string sourcePath, string path, string name, int canvasWidth, int canvasHeight)
        {
            Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/" + sourcePath));
            System.Drawing.Image thumbnail = new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / originalHeight;
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White);
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);


            System.Drawing.Imaging.ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            thumbnail.Save(HttpContext.Current.Server.MapPath("~/" + path + name), info[1], encoderParameters);
            image.Dispose();
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/" + sourcePath))) System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/" + sourcePath));

        }
    }
}