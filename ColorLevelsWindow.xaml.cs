using System.Linq;
using System.Windows;

using Framework.ViewModel;
using static Framework.Utilities.DataProvider;

namespace Framework.View
{
    public partial class ColorLevelsWindow : Window
    {
        private readonly ColorLevelsVM _colorLevelsVM;

        public ColorLevelsWindow(MainVM mainVM, CLevelsType type)
        {
            InitializeComponent();

            _colorLevelsVM = ColorLevelsFactory.Produce(type);
            _colorLevelsVM.Theme = mainVM.Theme;
            DataContext = _colorLevelsVM;

            Application.Current.Windows.OfType<MainWindow>().First().Update();
            Update();

            if (ColorInitialImage != null || ColorProcessedImage != null)
            {
                checkBoxBlue.Visibility = Visibility.Visible;
                checkBoxGreen.Visibility = Visibility.Visible;
                checkBoxRed.Visibility = Visibility.Visible;
            }
        }

        private Point LastPoint { get; set; }

        public void Update()
        {
            if (LastPoint != LastMouseClick)
            {
                _colorLevelsVM.XPos = "X: " + ((int)LastMouseClick.X).ToString();
                _colorLevelsVM.YPos = "Y: " + ((int)LastMouseClick.Y).ToString();

                DisplayGray();
                DisplayColor();

                LastPoint = LastMouseClick;
            }

            bool showBlue = (bool)checkBoxBlue.IsChecked;
            bool showGreen = (bool)checkBoxGreen.IsChecked;
            bool showRed = (bool)checkBoxRed.IsChecked;

            SetVisibility(0, showBlue);
            SetVisibility(1, showGreen);
            SetVisibility(2, showRed);
        }

        private void DisplayGray()
        {
            if (GrayInitialImage != null)
                initialImageView.Model = _colorLevelsVM.PlotImage(GrayInitialImage);
            if (GrayProcessedImage != null)
                processedImageView.Model = _colorLevelsVM.PlotImage(GrayProcessedImage);
        }

        private void DisplayColor()
        {
            if (ColorInitialImage != null)
                initialImageView.Model = _colorLevelsVM.PlotImage(ColorInitialImage);
            if (ColorProcessedImage != null)
                processedImageView.Model = _colorLevelsVM.PlotImage(ColorProcessedImage);
        }

        private void SetVisibility(int indexSeries, bool isVisible)
        {
            if (initialImageView.Model != null)
            {
                if (ColorInitialImage != null || (GrayInitialImage != null && indexSeries == 0))
                    initialImageView.Model.Series[indexSeries].IsVisible = isVisible;

                initialImageView.Model.InvalidatePlot(true);
            }

            if (processedImageView.Model != null)
            {
                if (ColorProcessedImage != null || (GrayProcessedImage != null && indexSeries == 0))
                    processedImageView.Model.Series[indexSeries].IsVisible = isVisible;

                processedImageView.Model.InvalidatePlot(true);
            }
        }

        private void UpdateSeriesVisibility(object sender, RoutedEventArgs e)
        {
            bool showSeries;
            if (sender == checkBoxBlue)
            {
                showSeries = (bool)checkBoxBlue.IsChecked;
                SetVisibility(0, showSeries);
            }
            else if (sender == checkBoxGreen)
            {
                showSeries = (bool)checkBoxGreen.IsChecked;
                SetVisibility(1, showSeries);
            }
            else if (sender == checkBoxRed)
            {
                showSeries = (bool)checkBoxRed.IsChecked;
                SetVisibility(2, showSeries);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (_colorLevelsVM.Type)
            {
                case CLevelsType.Row:
                    RowColorLevelsOn = false;
                    break;

                case CLevelsType.Column:
                    ColumnColorLevelsOn = false;
                    break;
            }

            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Update();
        }
    }
}