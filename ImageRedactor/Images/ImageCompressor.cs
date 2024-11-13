using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Images
{
    public class ImageCompressor : IDisposable
    {
        private readonly int width;
        private readonly int height;
        private int progress;
        private readonly Matrix<double>[] colorChannels;
        private const int ChannelCount = 3;

        public Bitmap SourceImage { get; private set; }

        public event Action<int> CompressionProgress;

        public ImageCompressor(Bitmap sourceImage)
        {
            width = sourceImage.Width;
            height = sourceImage.Height;
            SourceImage = sourceImage;
            colorChannels = ExtractColorChannels(sourceImage);
        }

        private Matrix<double>[] ExtractColorChannels(Bitmap bitmap)
        {
            var channels = new Matrix<double>[ChannelCount];
            Parallel.For(0, ChannelCount, (ch) =>
            {
                channels[ch] = Matrix<double>.Build.Dense(height, width);
            });

            bitmap.ParallelForFixResult((x, y, r, g, b) =>
            {
                channels[0][y, x] = r;
                channels[1][y, x] = g;
                channels[2][y, x] = b;
            });

            return channels;
        }

        public async Task<Bitmap> CompressAsync(double quality, Action<int> progress)
        {
            UpdateProgress(1);

            if (quality < 0 || quality > 100)
                throw new ArgumentException("Quality should be between 0 and 100", nameof(quality));
            Thread thread = new Thread(() =>
            {
                while (this.progress < 75)
                {
                    UpdateProgress(this.progress + 1);
                    Thread.Sleep(new Random().Next(400,2000));
                }
            });
            thread.Start();
            var compressionTasks = colorChannels
                .Select(channel => Task.Run(() => CompressChannel(channel, quality)))
                .ToArray();
            
            var compressedChannels = await Task.WhenAll(compressionTasks);

            var result = CreateCompressedImage(compressedChannels);
            thread = null;
            UpdateProgress(100);
            CompressionProgress -= progress;
            return result;
        }

        private Bitmap CreateCompressedImage(double[][,] channels)
        {
            return SourceImage.ParallelForFixResult((x, y, indexColor, pixel) =>
            {
                return (byte)Math.Clamp(channels[indexColor][y, x], 0, 255);
            });
        }
        private double[,] CompressChannel(Matrix<double> channel, double quality)
        {
            var svd = ComputeSVD(channel);
            UpdateProgress(progress + 6);

            int k = CalculateComponentCount(svd.S, quality);
            UpdateProgress(progress + 6);

            return ReconstructMatrix(svd.U, svd.S, svd.VT, k);
        }

        private static (Matrix<double> U, Vector<double> S, Matrix<double> VT) ComputeSVD(Matrix<double> matrix)
        {
            var svd = matrix.Svd();
            return (svd.U, svd.S, svd.VT);
        }
        private double[,] ReconstructMatrix(Matrix<double> U, Vector<double> S, Matrix<double> VT, int k)
        {
            var Uk = U.SubMatrix(0, U.RowCount, 0, k);
            UpdateProgress(progress + 6);
            var Sk = DiagonalMatrix.Build.DenseDiagonal(k, k, i => S[i]);
            UpdateProgress(progress + 6);
            var VTk = VT.SubMatrix(0, k, 0, VT.ColumnCount);
            UpdateProgress(progress + 6);
            var result = Uk.Multiply(Sk).Multiply(VTk);
            UpdateProgress(progress + 6);
            return result.ToArray();
        }

        private static int CalculateComponentCount(Vector<double> singularValues, double quality)
        {
            double totalEnergy = singularValues.Sum();
            double targetEnergy = totalEnergy * (quality / 100.0);

            double currentEnergy = 0;
            int k = 0;

            while (k < singularValues.Count && currentEnergy < targetEnergy)
            {
                currentEnergy += singularValues[k];
                k++;
            }

            return Math.Max(1, k);
        }

        private void UpdateProgress(int newProgress)
        {
            progress = newProgress;
            CompressionProgress?.Invoke(progress);
        }

        public void Dispose()
        {
            SourceImage?.Dispose();
        }
    }
}
