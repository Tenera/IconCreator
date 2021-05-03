using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Brush = System.Drawing.Brush;

namespace IconCreator
{
    public partial class MainWindow
    {
        private string _fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = "All image files|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.exif;*.bmp;*.tiff;*.tif;*.gif|PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif,*.exif)|*.jpg;*.jpeg;*.jpe;*.jfif;*.exif|BMP (*.bmp)|*.bmp|TIFF (*.tiff, *.tif)|*.tiff;*.tif|GIF (*.gif)|*.gif",
                    Multiselect = false,
                    Title = "Select image file"
                };
                if (dialog.ShowDialog() == true)
                {
                    var image = new BitmapImage(new Uri(dialog.FileName));
                    ImageBox.Source = image;
                    _fileName = dialog.FileName;
                    var fileInfo = new FileInfo(_fileName);
                    Filename.Text = fileInfo.FullName;
                    FileSize.Text = BytesToString(fileInfo.Length);
                    Dimensions.Text = $"{image.PixelWidth} x {image.PixelHeight}";
                }
            }
            catch (Exception ex)
            {
                ImageBox.Source = null;
                _fileName = null;
                Filename.Clear();
                FileSize.Clear();
                Dimensions.Clear();
                MessageBox.Show(ex.Message, "Error reading image", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            //IconFactory.CreateMultiImageIcon("C:\\Users\\peter.van.dender\\Pictures\\Icons\\File.png", "LogViewer.ico");
            //CreateImage("SplashScreen.scale-200.png", 1240, 600, new SolidBrush(Color.White), (Bitmap)Image.FromFile("C:\\Data\\Git\\Service\\Lansweeper.TestTools.App\\Images\\Lansweeper.png"));
        }

        private void CreateIcon_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".ico",
                Filter = "Icon file|*.ico",
                Title = "Select icon file",
                OverwritePrompt = true
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IconFactory.CreateMultiImageIcon(_fileName, dialog.FileName);
                    MessageBox.Show($"Icon '{dialog.FileName}' was created successfully", "Icon created", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error creating icon", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        
        private void CreateFavicon_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".ico",
                Filter = "Icon file|*.ico",
                Title = "Select icon file",
                OverwritePrompt = true
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IconFactory.CreateFavIcon(_fileName, dialog.FileName);
                    MessageBox.Show($"Icon '{dialog.FileName}' was created successfully", "Icon created", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error creating icon", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0) return "0" + suf[0];

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Math.Sign(byteCount) * num + suf[place];
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
