using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Crop Image

        //variabile pt stocarea coordonatelor clickurilor
        private static System.Windows.Point? firstClick = null;
        private static System.Windows.Point? secondClick = null;

        public static void SetSelectionPoint(System.Windows.Point clickPosition)
        {
            System.Diagnostics.Debug.WriteLine($"Click detectat la: X={clickPosition.X}, Y={clickPosition.Y}");

            //daca este prima selectie dintr o noua actiune, resetam ambele puncte
            if (firstClick == null || secondClick != null)
            {
                firstClick = clickPosition;
                secondClick = null;
                System.Diagnostics.Debug.WriteLine("Prima selecție a fost resetată și setată.");
            }
            else
            {
                secondClick = clickPosition;
                System.Diagnostics.Debug.WriteLine("Zona de crop selectată!");
            }
        }


        // return dreptunghiul selectat
        public static Int32Rect GetSelectionRect(BitmapSource sourceBitmap)
        {
            if (firstClick == null || secondClick == null) return new Int32Rect();

            int imgWidth = sourceBitmap.PixelWidth;
            int imgHeight = sourceBitmap.PixelHeight;

            //det coord corecte ale zonei selectate
            int x1 = (int)Math.Max(0, Math.Min(firstClick.Value.X, secondClick.Value.X));
            int y1 = (int)Math.Max(0, Math.Min(firstClick.Value.Y, secondClick.Value.Y));
            int width = (int)Math.Min(imgWidth - x1, Math.Abs(secondClick.Value.X - firstClick.Value.X));
            int height = (int)Math.Min(imgHeight - y1, Math.Abs(secondClick.Value.Y - firstClick.Value.Y));

            //System.Diagnostics.Debug.WriteLine($"Crop Coords Adjusted: X1={x1}, Y1={y1}, Width={width}, Height={height}");

            return new Int32Rect(x1, y1, width, height);
        }


        // crop pe imagine
        public static Bitmap CropImage(BitmapSource sourceBitmap, Int32Rect selection)
        {
            if (sourceBitmap == null || selection.Width <= 0 || selection.Height <= 0) return null;

            // crop doar daca e valida
            CroppedBitmap croppedBitmap = new CroppedBitmap(sourceBitmap, selection);
            return ImageSourceToBitmap(croppedBitmap);
        }


        // conversie ImageSource -> Bitmap
        //conversia imaginilor wpf intr un format compatibil cu procesarea
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                var encoder = new PngBitmapEncoder(); //utilizez png pt a evita pierderile de calitate
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new System.IO.MemoryStream()) //salvam imaginea intr un stream de memorie
                {
                    encoder.Save(stream);
                    return new Bitmap(stream); //creez un bitmap din fluxul de date
                }
            }
            return null;
        }

        // conversie Bitmap -> ImageSource
        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap(); //se obtine un handle catre bitmap
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()); //se creeaza ImageSource din handle

            return wpfBitmap;
        }
       

        //calc media si abaterea pt fiecare canal
        public static void ComputeAndDisplayStatistics(Bitmap croppedBitmap)
        {
            if (croppedBitmap == null)
            {
                System.Windows.MessageBox.Show("Eroare: Nu există o zonă validă pentru analiză!");
                return;
            }

            int width = croppedBitmap.Width;
            int height = croppedBitmap.Height;
            int numPixels = width * height;

            double sumR = 0, sumG = 0, sumB = 0;
            double sumSquaredR = 0, sumSquaredG = 0, sumSquaredB = 0;

            //se parcurge fiecare pixel si se aduna valorile pentru fiecare canal (R, G, B)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixel = croppedBitmap.GetPixel(x, y);


                    sumR += pixel.R;
                    sumG += pixel.G;
                    sumB += pixel.B;

                    sumSquaredR += pixel.R * pixel.R;
                    sumSquaredG += pixel.G * pixel.G;
                    sumSquaredB += pixel.B * pixel.B;
                }
            }

            //calc media pt fiecare canal
            double meanR = sumR / numPixels;
            double meanG = sumG / numPixels;
            double meanB = sumB / numPixels;

            //calc abaterea standard pt fiecare canal
            double stdDevR = Math.Sqrt(sumSquaredR / numPixels - meanR * meanR);
            double stdDevG = Math.Sqrt(sumSquaredG / numPixels - meanG * meanG);
            double stdDevB = Math.Sqrt(sumSquaredB / numPixels - meanB * meanB);

            //rezultatele
            string statsMessage = $"📊 Statistici zonă selectată:\n" +
                                  $"🔴 R (Roșu): Medie = {meanR:F2}, Abatere medie = {stdDevR:F2}\n" +
                                  $"🟢 G (Verde): Medie = {meanG:F2}, Abatere medie = {stdDevG:F2}\n" +
                                  $"🔵 B (Albastru): Medie = {meanB:F2}, Abatere medie = {stdDevB:F2}";

            System.Windows.MessageBox.Show(statsMessage, "Statistici imagine", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        #endregion

        #region Rotate image
        //roteste o imagine la 90 grade in sens orar sau anti orar
        public static Bitmap RotateImage(Bitmap sourceBitmap, bool clockwise)
        {
            if (sourceBitmap == null) return null;

            try
            {
                //se cloneaza imaginea pt a evita modificarea directa asupra imaginii originale,
                // deoarece `RotateFlip` modifica imaginea sursa in loc sa creeze una noua
                Bitmap clonedBitmap = new Bitmap(sourceBitmap);

                // aplicam rotirea
                clonedBitmap.RotateFlip(clockwise ? RotateFlipType.Rotate90FlipNone : RotateFlipType.Rotate270FlipNone);

                return clonedBitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la rotire: {ex.Message}");
                return null;
            }
        }
        #endregion


        // imaginea oglindita fata de verticala
        public static Bitmap MirrorImage(Bitmap sourceBitmap)
            {
                if (sourceBitmap == null)
                    throw new ArgumentNullException(nameof(sourceBitmap), "Imaginea sursă nu poate fi null!");

                Bitmap mirroredBitmap = new Bitmap(sourceBitmap); //se creeaza o copie a imaginii originale sub forma unui Bitmap nou
                mirroredBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX); //aplica oglindirea pe axa x
                return mirroredBitmap;
            }
        

    }
}