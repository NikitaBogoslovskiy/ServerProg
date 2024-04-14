using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace RazorPages
{
    public static class Utils
    {
        #pragma warning disable CA1416

        public static bool ClipToCircle(string imagePath)
        {
            var srcImage = Image.FromFile(imagePath);
            var srcCenterX = srcImage.Width / 2f;
            var srcCenterY = srcImage.Height / 2f;
            var radius = Math.Min(srcCenterX, srcCenterY);
            var sideLength = (int)radius * 2;
            Image dstImage = new Bitmap(sideLength, sideLength, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(dstImage))
            {
                RectangleF r = new RectangleF(0, 0, sideLength, sideLength);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Brush br = new SolidBrush(Color.Transparent))
                {
                    g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
                }

                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(r);
                g.SetClip(path);
                g.DrawImage(srcImage, -(srcImage.Width - sideLength) / 2f, -(srcImage.Height - sideLength) / 2f);
            }

            srcImage.Dispose();
            File.Delete(imagePath);
            dstImage.Save(imagePath);
            return true;
        }
    }
}
