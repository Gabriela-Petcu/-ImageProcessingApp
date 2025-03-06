using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Algorithms.Sections
{
    public static class Thresholding
    {

        //binarizarea pe o imagine grayscale, folosind 2 praguri. pixelii<treshold devin negri(0), pixelii>treshold devin albi(255), iar restul gri(127).
        public static Bitmap ApplyThresholding(Bitmap sourceBitmap, int threshold1, int threshold2)
        {
            //creeaza o imagine noua de aceleasi dimensiuni pt rezultat
            Bitmap binarizedBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            //se parcurge fiecare pixel al imaginii sursa
            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    //se obtine culoarea pixelului curent
                    System.Drawing.Color pixelColor = sourceBitmap.GetPixel(x, y);

                    //se converteste pixelul in nuante de gri folosind ponderile standard pt R,G,B
                    byte gray = (byte)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);

                    //aplicam pragurile
                    byte newGray;
                    if (gray < threshold1)
                        newGray = 0;       // negru
                    else if (gray > threshold2)
                        newGray = 255;     // alb
                    else
                        newGray = 127;     // gri 


                    //se seteaza pixelul procesat in imaginea rezultata
                    binarizedBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(newGray, newGray, newGray));
                }
            }
            //return imagine binarizata
            return binarizedBitmap;
        }

        //se converteste un obiect ImageSource intr un Bitmap utilizabil pt procesare ulterioara
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                //se creeaza un encoder png pt a salva imaginea intr un stream de memorie
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return new Bitmap(stream); //bitmap ul rezultat
                }
            }
            return null;
        }

        //se converteste bitmap ul intr un obiect de tip ImageSource pt a putea fi utilizat in WPF
        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            //se obtine handle ul imaginii bitmap
            IntPtr hBitmap = bitmap.GetHbitmap();

            //se converteste imaginea intr un ImageSource utilizabil in WPF
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return wpfBitmap;
        }
    }
}
