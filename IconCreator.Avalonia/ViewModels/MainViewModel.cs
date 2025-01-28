using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IconCreator.Avalonia.ViewModels;

public class MainViewModel(TopLevel topLevel) : ViewModelBase
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
                Image = new Bitmap(file.Path.LocalPath);
                IconFactory.ValidateSize(Image.Size);
                var props = await file.GetBasicPropertiesAsync();
                FileName = file.Path.AbsolutePath;
                FileSize = BytesToString(props.Size ?? 0);
                Dimensions = $"{Image.Size.Width} x {Image.Size.Height}";
            }
        }
        catch (Exception ex)
        {
            Image = null;
            FileName = null;
            FileSize = null;
            Dimensions = null;
            var box = MessageBoxManager.GetMessageBoxStandard("Error selecting the image", ex.GetBaseException().Message, ButtonEnum.Ok, Icon.Warning);
            await box.ShowAsync();
        }

        OnPropertyChanged(nameof(HasImage));
    }

    public async Task CreateIcon(string type)
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
                if (type == "favicon")
                {
                    IconFactory.CreateFavIcon(FileName, file.Path.LocalPath);
                }
                else
                {
                    IconFactory.CreateMultiImageIcon(FileName, file.Path.LocalPath);
                }
                
                var box = MessageBoxManager.GetMessageBoxStandard("Icon created", $"Icon '{file.Path.LocalPath}' was created successfully", ButtonEnum.Ok, Icon.Info);
                await box.ShowAsync();
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Error creating icon", ex.GetBaseException().Message, ButtonEnum.Ok, Icon.Warning);
                await box.ShowAsync();
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
