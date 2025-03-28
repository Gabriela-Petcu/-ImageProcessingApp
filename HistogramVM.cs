﻿using Emgu.CV;
using Emgu.CV.Structure;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Framework.Model;
using static Framework.Utilities.DataProvider;

using static Algorithms.Utilities.Utils;

namespace Framework.ViewModel
{
    public enum ImageType
    {
        None,
        InitialGray,
        InitialColor,
        ProcessedGray,
        ProcessedColor,
    }

    class HistogramVM : BaseVM
    {
        public void CreateHistogram(ImageType type)
        {
            List<int[]> histograms = new List<int[]>();
            List<string> colors = new List<string>();

            switch (type)
            {
                case ImageType.InitialGray:
                    InitialHistogramOn = true;
                    Title = "Grayscale - Initial Image Histogram";

                    InitHelper(GrayInitialImage, ref histograms, ref colors);
                    break;

                case ImageType.InitialColor:
                    InitialHistogramOn = true;
                    Title = "Color - Initial Image Histogram";

                    InitHelper(ColorInitialImage, ref histograms, ref colors);
                    break;

                case ImageType.ProcessedGray:
                    ProcessedHistogramOn = true;
                    Title = "Grayscale - Processed Image Histogram";

                    InitHelper(GrayProcessedImage, ref histograms, ref colors);
                    break;

                case ImageType.ProcessedColor:
                    ProcessedHistogramOn = true;
                    Title = "Color - Processed Image Histogram";

                    InitHelper(ColorProcessedImage, ref histograms, ref colors);
                    break;
            }

            InitializeHistogram(histograms, colors);
        }

        public void CreateHistogram<T>(List<(T[] Histogram, string PlotColor)> values)
        {
            List<T[]> histograms = values.Select(_ => _.Histogram).ToList();
            List<string> colors = values.Select(_ => _.PlotColor).ToList();

            InitializeHistogram(histograms, colors);
        }

        #region Properties
        public string Title { get; set; } = "Histogram";

        public double Width { get; set; }

        public ObservableCollection<PlotModel> Plots { get; set; } =
            new ObservableCollection<PlotModel>();

        public IThemeMode Theme { get; set; } =
            LimeGreenTheme.Instance;
        #endregion

        #region Helpers
        private void InitializeHistogram<T>(List<T[]> histograms, List<string> colors)
        {
            Width = 400 * histograms.Count;

            for (int index = 0; index < histograms.Count; ++index)
            {
                var plot = new PlotModel();

                plot.Series.Clear();

                plot.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Maximum = 256,
                    Minimum = -1,
                });

                var series = new RectangleBarSeries
                {
                    StrokeThickness = 2.5,
                    StrokeColor = OxyColor.Parse(colors[index]),
                };

                for (int i = 0; i < histograms[index].Length; ++i)
                {
                    dynamic barValue = histograms[index][i];
                    if (barValue != 0)
                    {
                        var bar = new RectangleBarItem(i, 0, i + 0.01, barValue);
                        series.Items.Add(bar);
                    }
                }

                plot.Series.Add(series);
                Plots.Add(plot);
            }
        }

        private void InitHelper(Image<Gray, byte> image, ref List<int[]> histograms, ref List<string> colors)
        {
            histograms.Add(ComputeHistogram(image));
            colors.Add(Brushes.Black.ToString());
        }

        private void InitHelper(Image<Bgr, byte> image, ref List<int[]> histograms, ref List<string> colors)
        {
            histograms.Add(ComputeHistogram(image[0]));
            histograms.Add(ComputeHistogram(image[1]));
            histograms.Add(ComputeHistogram(image[2]));

            colors.Add(Brushes.Blue.ToString());
            colors.Add(Brushes.Green.ToString());
            colors.Add(Brushes.Red.ToString());
        }
        #endregion
    }
}