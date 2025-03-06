using System.Windows;

using Framework.ViewModel;
using static Framework.Utilities.DataProvider;

namespace Framework.View
{
    public partial class HistogramWindow : Window
    {
        private readonly HistogramVM _histogramVM;
        private readonly ImageType _imageType = ImageType.None;

        public HistogramWindow(MainVM mainVM, ImageType type)
        {
            InitializeComponent();

            HistogramOn = true;

            _histogramVM = new HistogramVM();
            _histogramVM.Theme = mainVM.Theme;

            if (type != ImageType.None)
            {
                _histogramVM.CreateHistogram(_imageType = type);
            }

            DataContext = _histogramVM;
        }

        public HistogramWindow(MainVM mainVM, string title, dynamic values)
        {
            InitializeComponent();

            _histogramVM = new HistogramVM();
            _histogramVM.Theme = mainVM.Theme;
            _histogramVM.Title = title;

            if (values != null)
            {
                _histogramVM.CreateHistogram(values);
            }

            DataContext = _histogramVM;
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize == e.NewSize)
                return;

            var w = SystemParameters.PrimaryScreenWidth;
            var h = SystemParameters.PrimaryScreenHeight;

            Left = (w - e.NewSize.Width) / 2;
            Top = (h - e.NewSize.Height) / 2;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_imageType == ImageType.InitialGray || _imageType == ImageType.InitialColor)
                InitialHistogramOn = false;
            else if (_imageType == ImageType.ProcessedGray || _imageType == ImageType.ProcessedColor)
                ProcessedHistogramOn = false;
            else HistogramOn = false;
        }
    }
}