using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;

namespace Algorithms.Utilities
{
    public class Utils
    {
        #region Compute histogram
        public static int[] ComputeHistogram(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];

            Parallel.For(0, inputImage.Height, (int y) =>
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    ++histogram[inputImage.Data[y, x, 0]];
                }
            });

            return histogram;
        }
        #endregion
    }
}