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
            for (int i = 0; i < ChannelCount; i++)
            {
                channels[i] = Matrix<double>.Build.Dense(height, width);
            }

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bitmapData.Scan0;
                    Parallel.For(0, height, y =>
                    {
                        byte* row = ptr + (y * bitmapData.Stride);
                        for (int x = 0; x < width; x++)
                        {
                            int offset = x * 3;
                            channels[0][y, x] = row[offset + 2]; // R
                            channels[1][y, x] = row[offset + 1]; // G
                            channels[2][y, x] = row[offset];     // B
                        }
                    });
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return channels;
        }

        public async Task<Bitmap> CompressAsync(double quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentException("Quality should be between 0 and 100", nameof(quality));

            UpdateProgress(5);

            var compressionTasks = colorChannels
                .Select(channel => Task.Run(() => CompressChannel(channel, quality)))
                .ToArray();

            var compressedChannels = await Task.WhenAll(compressionTasks);

            var result = CreateCompressedImage(compressedChannels);
            UpdateProgress(100);

            return result;
        }

        private Bitmap CreateCompressedImage(double[][,] channels)
        {
            var result = new Bitmap(width, height);
            var resultData = result.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)resultData.Scan0;
                    Parallel.For(0, height, y =>
                    {
                        byte* row = ptr + (y * resultData.Stride);
                        for (int x = 0; x < width; x++)
                        {
                            int offset = x * 3;
                            row[offset + 2] = (byte)Math.Clamp(channels[0][y, x], 0, 255); // R
                            row[offset + 1] = (byte)Math.Clamp(channels[1][y, x], 0, 255); // G
                            row[offset] = (byte)Math.Clamp(channels[2][y, x], 0, 255);     // B
                        }
                    });
                }
            }
            finally
            {
                result.UnlockBits(resultData);
            }

            return result;
        }

        private double[,] CompressChannel(Matrix<double> channel, double quality)
        {
            var svd = ComputeSVD(channel);
            UpdateProgress(progress + 5);

            int k = CalculateComponentCount(svd.S, quality);
            UpdateProgress(progress + 5);

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
            var Sk = DiagonalMatrix.Build.DenseDiagonal(k, k, i => S[i]);
            var VTk = VT.SubMatrix(0, k, 0, VT.ColumnCount);

            var result = Uk.Multiply(Sk).Multiply(VTk);
            UpdateProgress(progress + 5);

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
