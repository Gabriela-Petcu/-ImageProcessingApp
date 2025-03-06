using Emgu.CV;
using Emgu.CV.Structure;

using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;
using Brushes = System.Drawing.Brushes;
using FontFamily = System.Drawing.FontFamily;

using static Framework.Utilities.DataProvider;
using ImageConverter = Framework.Converters.ImageConverter;

namespace Framework.View
{
    public partial class MagnifierWindow : Window
    {
        private Point LastPoint { get; set; }

        public MagnifierWindow()
        {
            InitializeComponent();

            MagnifierOn = true;

            Application.Current.Windows.OfType<MainWindow>().First().Update();
            Update();
        }

        public void Update()
        {
            if (LastPoint != LastMouseClick)
            {
                DisplayGray();
                DisplayColor();

                LastPoint = LastMouseClick;
            }
        }

        private void DisplayColor()
        {
            if (ColorInitialImage != null)
                imageBoxInitial.Source = GetImage(ColorInitialImage, (int)imageBoxInitial.Width, (int)imageBoxInitial.Height);
            if (ColorProcessedImage != null)
                imageBoxProcessed.Source = GetImage(ColorProcessedImage, (int)imageBoxInitial.Width, (int)imageBoxInitial.Height);
        }

        private void DisplayGray()
        {
            if (GrayInitialImage != null)
                imageBoxInitial.Source = GetImage(GrayInitialImage, (int)imageBoxInitial.Width, (int)imageBoxInitial.Height);
            if (GrayProcessedImage != null)
                imageBoxProcessed.Source = GetImage(GrayProcessedImage, (int)imageBoxInitial.Width, (int)imageBoxInitial.Height);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MagnifierOn = false;
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Update();
        }

        private ImageSource GetImage(Image<Bgr, byte> image, int width, int height)
        {
            Bitmap flag = new Bitmap(width, height);
            Graphics flagGraphics = Graphics.FromImage(flag);

            int i = 0;
            int widthStep = width / 9;
            int heightStep = height / 9;
            int yStep = 0;
            for (int x = -4; x <= 4; ++x)
            {
                int xStep = 0;
                int j = 0;
                for (int y = -4; y <= 4; ++y)
                {
                    if (LastMouseClick.Y + y >= 0 && LastMouseClick.X + x >= 0 &&
                        LastMouseClick.Y + y < image.Height && LastMouseClick.X + x < image.Width)
                    {
                        int blueColor = image.Data[(int)LastMouseClick.Y + y,
                            (int)LastMouseClick.X + x, 0];
                        int greenColor = image.Data[(int)LastMouseClick.Y + y,
                            (int)LastMouseClick.X + x, 1];
                        int redColor = image.Data[(int)LastMouseClick.Y + y,
                            (int)LastMouseClick.X + x, 2];

                        string text = redColor + "\n" + greenColor + "\n" + blueColor;
                        Font font = new Font(new FontFamily("Arial"), 7, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
                        Rectangle rectangle = new Rectangle(yStep, xStep, widthStep, heightStep);
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(redColor, greenColor, blueColor)), rectangle);
                        if (blueColor <= 128 & greenColor <= 128 & redColor <= 128)
                            flagGraphics.DrawString(text, font, Brushes.White, rectangle);
                        else
                            flagGraphics.DrawString(text, font, Brushes.Black, rectangle);
                    }
                    else
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)),
                            new Rectangle(yStep, xStep, widthStep, heightStep));
                    xStep += widthStep;
                    ++j;
                }
                yStep += heightStep;
                ++i;
            }

            return ImageConverter.Convert(flag);
        }

        private ImageSource GetImage(Image<Gray, byte> image, int width, int height)
        {
            Bitmap flag = new Bitmap(width, height);
            Graphics flagGraphics = Graphics.FromImage(flag);

            int i = 0;
            int widthStep = width / 9;
            int heightStep = height / 9;
            int yStep = 0;
            for (int x = -4; x <= 4; ++x)
            {
                int xStep = 0;
                int j = 0;
                for (int y = -4; y <= 4; ++y)
                {
                    if (LastMouseClick.Y + y >= 0 && LastMouseClick.X + x >= 0 &&
                        LastMouseClick.Y + y < image.Height && LastMouseClick.X + x < image.Width)
                    {
                        int grayColor = image.Data[(int)LastMouseClick.Y + y,
                            (int)LastMouseClick.X + x, 0];

                        string text = "\n" + grayColor + "\n";
                        Font font = new Font(new FontFamily("Arial"), 7, System.Drawing.FontStyle.Bold, GraphicsUnit.Point);
                        Rectangle rectangle = new Rectangle(yStep, xStep, widthStep, heightStep);
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(grayColor, grayColor, grayColor)),
                            rectangle);
                        if (grayColor <= 128)
                            flagGraphics.DrawString(text, font, Brushes.White, rectangle);
                        else
                            flagGraphics.DrawString(text, font, Brushes.Black, rectangle);
                    }
                    else
                        flagGraphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)),
                            new Rectangle(yStep, xStep, widthStep, heightStep));
                    xStep += widthStep;
                    ++j;
                }
                yStep += heightStep;
                ++i;
            }

            return ImageConverter.Convert(flag);
        }
    }
}