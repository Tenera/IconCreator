using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace IconCreator;

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
                IconFactory.ValidateSize(new SixLabors.ImageSharp.Size(image.PixelWidth, image.PixelHeight));
            }
        }
        catch (Exception ex)
        {
            ImageBox.Source = null;
            _fileName = null;
            Filename.Clear();
            FileSize.Clear();
            Dimensions.Clear();
            MessageBox.Show(ex.Message, "Error selecting the image", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        CreateIcon.Visibility = ImageBox.Source == null ? Visibility.Collapsed : Visibility.Visible;
        CreateFavicon.Visibility = ImageBox.Source == null ? Visibility.Collapsed : Visibility.Visible;
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
        if (dialog.ShowDialog() != true) return;

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
        
    private void CreateFavicon_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            DefaultExt = ".ico",
            Filter = "Icon file|*.ico",
            Title = "Select icon file",
            OverwritePrompt = true
        };
        if (dialog.ShowDialog() != true) return;

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

    private static string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0) return "0" + suf[0];

        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return Math.Sign(byteCount) * num + suf[place];
    }
}