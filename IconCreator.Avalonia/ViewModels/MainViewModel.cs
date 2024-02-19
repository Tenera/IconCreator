using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;

namespace IconCreator.Avalonia.ViewModels;

public partial class MainViewModel(TopLevel topLevel) : ViewModelBase
{
    private string? _fileName;
    private string? _fileSize;
    private string? _dimensions;
    private Bitmap? _image;

    public bool HasImage => _image != null;

    public string? FileName
    {
        get => _fileName;
        set
        {
            if (value == _fileName) return;
            _fileName = value;
            OnPropertyChanged();
        }
    }

    public string? FileSize
    {
        get => _fileSize;
        set
        {
            if (value == _fileSize) return;
            _fileSize = value;
            OnPropertyChanged();
        }
    }

    public string? Dimensions
    {
        get => _dimensions;
        set
        {
            if (value == _dimensions) return;
            _dimensions = value;
            OnPropertyChanged();
        }
    }

    public Bitmap? Image
    {
        get => _image;
        set
        {
            if (Equals(value, _image)) return;
            _image = value;
            OnPropertyChanged();
        }
    }

    public async Task SelectImage()
    {
        try
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = new FilePickerFileType[]
                {
                    new("All image files") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.jpe", "*.jfif", "*.exif", "*.bmp", ".tiff", "*.tif", "*.gif"]},
                    new("PNG (*.png)") { Patterns = ["*.png"]},
                    new("JPEG (*.jpg, *.jpeg, *.jpe, *.jfif,*.exif)") { Patterns = ["*.jpg", "*.jpeg", "*.jpe", "*.jfif", "*.exif" ]},
                    new("BMP (*.bmp)") { Patterns = ["*.bmp"]},
                    new("TIFF (*.tiff, *.tif)") { Patterns = [".tiff", "*.tif"]},
                    new("GIF (*.gif)") { Patterns = ["*.gif"]}
                },
                Title = "Select image file",
                AllowMultiple = false
            });

            if (files.Count == 1)
            {
                var file = files[0];
                var props = await file.GetBasicPropertiesAsync();
                Image = new Bitmap(file.Path.LocalPath);
                FileName = file.Path.AbsolutePath;
                FileSize = BytesToString(props.Size ?? 0);
                Dimensions = $"{Image.Size.Width} x {Image.Size.Height}";
                IconFactory.ValidateSize(Image.Size);
            }
        }
        catch (Exception ex)
        {
            Image = null;
            _fileName = null;
            FileName = null;
            FileSize = null;
            Dimensions = null;
            //MessageBox.Show(ex.Message, "Error selecting the image", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        OnPropertyChanged(nameof(HasImage));
    }

    public async Task CreateIcon()
    {
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Icon File",
            DefaultExtension =".ico",
            FileTypeChoices = new FilePickerFileType[] { new("Icon file") { Patterns = ["*.ico"] } },
            ShowOverwritePrompt = true
        });

        if (file is not null)
        {
            try
            {
                IconFactory.CreateMultiImageIcon(_fileName, file.Path.LocalPath);
                
                //MessageBox.Show($"Icon '{dialog.FileName}' was created successfully", "Icon created", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error creating icon", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
    
    public async Task CreateFavicon()
    {
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Icon File",
            DefaultExtension =".ico",
            FileTypeChoices = new FilePickerFileType[] { new("Icon file") { Patterns = ["*.ico"] } },
            ShowOverwritePrompt = true
        });

        if (file is not null)
        {
            try
            {
                IconFactory.CreateFavIcon(_fileName, file.Path.LocalPath);
                //MessageBox.Show($"Icon '{dialog.FileName}' was created successfully", "Icon created", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error creating icon", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }

    private static string BytesToString(ulong byteCount)
    {
        string[] suf = ["B", "KB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB
        if (byteCount == 0) return "0" + suf[0];

        var place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
        var num = Math.Round(byteCount / Math.Pow(1024, place), 1);
        return num + suf[place];
    }
}
