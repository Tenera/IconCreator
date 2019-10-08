using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace IconCreator
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IconFactory.CreateMultiImageIcon("C:\\Users\\peter.van.dender\\Pictures\\Icons\\File.png", "LogViewer.ico");
            //CreateImage("SplashScreen.scale-200.png", 1240, 600, new SolidBrush(Color.White), (Bitmap)Image.FromFile("C:\\Data\\Git\\Service\\Lansweeper.TestTools.App\\Images\\Lansweeper.png"));
            MessageBox.Show("Done");
        }

        private static void CreateImage(string filename, int width, int height, Brush background, Image icon)
        {
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(icon.HorizontalResolution, icon.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.FillRectangle(background, new Rectangle(0, 0, width, height));

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var iconsize = Math.Min(width, height) / 2;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    var destRect = new Rectangle((width - iconsize)/2, (height-iconsize)/2, iconsize, iconsize);
                    graphics.DrawImage(icon, destRect, 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            using (var s = File.Create(filename))
            {
                destImage.Save(s, ImageFormat.Png);
                s.Close();
            }
        }
    }
}
