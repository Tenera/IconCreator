//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;

//namespace IconCreator;

///// <summary>
///// Provides methods for creating icons.
///// </summary>
//public static class IconFactory
//{
//    /// <summary>
//    /// Represents the max allowed width of an icon.
//    /// </summary>
//    public const int MaxIconWidth = 256;

//    /// <summary>
//    /// Represents the max allowed height of an icon.
//    /// </summary>
//    public const int MaxIconHeight = 256;

//    private const ushort HeaderReserved = 0;
//    private const ushort HeaderIconType = 1;
//    private const byte HeaderLength = 6;

//    private const byte EntryReserved = 0;
//    private const byte EntryLength = 16;

//    private const byte PngColorsInPalette = 0;
//    private const ushort PngColorPlanes = 1;

//    /// <summary>
//    /// Creates the multi image icon.
//    /// </summary>
//    /// <param name="sourceImage">The path of the source image in a square, 256*256 format or greater.</param>
//    /// <param name="destinationImage">The path for the icon to create.</param>
//    public static void CreateMultiImageIcon(string sourceImage, string destinationImage)
//    {
//        using var source = (Bitmap) Image.FromFile(sourceImage);
        
//        ValidateSize(new Size(source.Width, source.Height));

//        var png256 = source.Width > 256 ? ResizeImage(source, 256, 256) : source;
                
//        using (var png128 = ResizeImage(png256, 128, 128))
//        using (var png64 = ResizeImage(png256, 64, 64))
//        using (var png48 = ResizeImage(png256, 48, 48))
//        using (var png32 = ResizeImage(png256, 32, 32))
//        using (var png16 = ResizeImage(png256, 16, 16))
//        using (var stream = new FileStream(destinationImage, FileMode.Create))
//        {
//            SavePngsAsIcon(new[] {png256, png16, png32, png48, png64, png128,}, stream);
//        }

//        if (png256 != source)
//        {
//            png256.Dispose();
//        }
//    }

//    public static void ValidateSize(Size size)
//    {
//        if (size.Width != size.Height || size.Width < 256)
//        {
//            throw new InvalidOperationException($"The source image must be a square image and at least 256x256 (current dimensions: {size.Width} x {size.Height})");
//        }
//    }

//    /// <summary>
//    /// Creates the favicon.
//    /// </summary>
//    /// <param name="sourceImage">The path of the source image in a square, 256*256 format or greater.</param>
//    /// <param name="destinationImage">The path for the icon to create.</param>
//    public static void CreateFavIcon(string sourceImage, string destinationImage)
//    {
//        using var source = (Bitmap)Image.FromFile(sourceImage);

//        ValidateSize(new Size(source.Width, source.Height));

//        var png16 = source.Width > 16 ? ResizeImage(source, 16, 16) : source;

//        using (var stream = new FileStream(destinationImage, FileMode.Create))
//        {
//            SavePngsAsIcon(new[] { png16 }, stream);
//        }

//        if (png16 != source)
//        {
//            png16.Dispose();
//        }
//    }

//    /// <summary>
//    /// Resize the image to the specified width and height.
//    /// </summary>
//    /// <param name="image">The image to resize.</param>
//    /// <param name="width">The width to resize to.</param>
//    /// <param name="height">The height to resize to.</param>
//    /// <returns>The resized image.</returns>
//    private static Bitmap ResizeImage(Image image, int width, int height)
//    {
//        var destRect = new Rectangle(0, 0, width, height);
//        var destImage = new Bitmap(width, height);

//        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

//        using var graphics = Graphics.FromImage(destImage);
//        graphics.CompositingMode = CompositingMode.SourceCopy;
//        graphics.CompositingQuality = CompositingQuality.HighQuality;
//        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
//        graphics.SmoothingMode = SmoothingMode.HighQuality;
//        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

//        using var wrapMode = new ImageAttributes();
//        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
//        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

//        return destImage;
//    }

//    /// <summary>
//    /// Saves the specified <see cref="Bitmap"/> objects as a single 
//    /// icon into the output stream.
//    /// </summary>
//    /// <param name="images">The bitmaps to save as an icon.</param>
//    /// <param name="stream">The output stream.</param>
//    /// <remarks>
//    /// The expected input for the <paramref name="images"/> parameter are 
//    /// portable network graphic files that have a <see cref="Image.PixelFormat"/> 
//    /// of <see cref="PixelFormat.Format32bppArgb"/> and where the
//    /// width is less than or equal to <see cref="MaxIconWidth"/> and the 
//    /// height is less than or equal to <see cref="MaxIconHeight"/>.
//    /// </remarks>
//    /// <exception cref="InvalidOperationException">
//    /// Occurs if any of the input images do 
//    /// not follow the required image format. See remarks for details.
//    /// </exception>
//    /// <exception cref="ArgumentNullException">
//    /// Occurs if any of the arguments are null.
//    /// </exception>
//    private static void SavePngsAsIcon(IList<Bitmap> images, Stream stream)
//    {
//        ArgumentNullException.ThrowIfNull(images);
//        ArgumentNullException.ThrowIfNull(stream);

//        // validates the pngs
//        //ThrowForInvalidPngs(images);

//        var orderedImages = images.OrderBy(i => i.Width)
//                            .ThenBy(i => i.Height)
//                            .ToArray();

//        using var writer = new BinaryWriter(stream);
//        // write the header
//        writer.Write(HeaderReserved);
//        writer.Write(HeaderIconType);
//        writer.Write((ushort)orderedImages.Length);

//        // save the image buffers and offsets
//        var buffers = new Dictionary<uint, byte[]>();

//        // tracks the length of the buffers as the iterations occur
//        // and adds that to the offset of the entries
//        uint lengthSum = 0;
//        var baseOffset = (uint)(HeaderLength + EntryLength * orderedImages.Length);

//        foreach (var image in orderedImages)
//        {
//            // creates a byte array from an image
//            var buffer = CreateImageBuffer(image);

//            // calculates what the offset of this image will be
//            // in the stream
//            var offset = (baseOffset + lengthSum);

//            // writes the image entry
//            writer.Write(GetIconWidth(image));
//            writer.Write(GetIconHeight(image));
//            writer.Write(PngColorsInPalette);
//            writer.Write(EntryReserved);
//            writer.Write(PngColorPlanes);
//            writer.Write((ushort)Image.GetPixelFormatSize(image.PixelFormat));
//            writer.Write((uint)buffer.Length);
//            writer.Write(offset);

//            lengthSum += (uint)buffer.Length;

//            // adds the buffer to be written at the offset
//            buffers.Add(offset, buffer);
//        }

//        // writes the buffers for each image
//        foreach (var kvp in buffers)
//        {
//            // seeks to the specified offset required for the image buffer
//            writer.BaseStream.Seek(kvp.Key, SeekOrigin.Begin);

//            // writes the buffer
//            writer.Write(kvp.Value);
//        }
//    }

//    private static byte GetIconHeight(Image image)
//    {
//        if (image.Height == MaxIconHeight)
//            return 0;

//        return (byte)image.Height;
//    }

//    private static byte GetIconWidth(Image image)
//    {
//        if (image.Width == MaxIconWidth)
//            return 0;

//        return (byte)image.Width;
//    }

//    private static byte[] CreateImageBuffer(Image image)
//    {
//        using var stream = new MemoryStream();
//        image.Save(stream, ImageFormat.Png);
//        return stream.ToArray();
//    }
//}