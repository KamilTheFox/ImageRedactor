using Images;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageRedactor
{
    public partial class MainMenu : Form
    {
        private Bitmap imageConverable;
        private int progress;
        private const int ChannelCount = 3;
        public MainMenu()
        {
            InitializeComponent();
            UpdateProgressBar();
        }

        private async void UpdateProgressBar()
        {
            while (true)
            {
                await Task.Delay(1000);
                progressBar1.Value = progress;
            }
        }

        private void buttonOpenImage_Click(object sender, EventArgs e)
        {
            progress = 0;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.webp|All Files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(dialog.FileName);
                    bool hasAlpha = bmp.PixelFormat.HasFlag(PixelFormat.Alpha) ||
                    bmp.PixelFormat == PixelFormat.Format32bppArgb ||
                    bmp.PixelFormat == PixelFormat.Format64bppArgb;
                    if (hasAlpha)
                    {
                        var newImage = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
                        using (Graphics g = Graphics.FromImage(newImage))
                        {
                            g.DrawImage(bmp, 0, 0);
                        }
                        bmp.Dispose();
                        bmp = newImage;
                    }
                    pictureBox1.BackgroundImage = bmp;
                    imageConverable = bmp;
                }
            }
        }

        private async void buttonCompress_Click(object sender, EventArgs e)
        {
            using (ImageCompressor imageCompressor = new ImageCompressor(imageConverable))
            {
                int procent = Math.Abs(100 - hScrollBar1.Value);
                imageCompressor.CompressionProgress += (t) => progress = t;
                imageConverable = await imageCompressor.CompressAsync(procent, (progressCount) => progress = progressCount);
            }
            pictureBox1.BackgroundImage = imageConverable;
        }

        private void saveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "JPEG Files(*.jpeg)|*.jpeg|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImageCodecInfo jpegEncoder = GetJpegEncoder();

                    EncoderParameters encoderParameters = new EncoderParameters(1);

                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

                    pictureBox1.BackgroundImage.Save(
                        dialog.FileName,
                        jpegEncoder,
                        encoderParameters
                    );
                }
            }
        }
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            string newText = $"—жатие: {hScrollBar1.Value}";
            labelInfo.Text = newText;
        }
        private ImageCodecInfo GetJpegEncoder()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
        }

        private void buttonNegative_Click(object sender, EventArgs e)
        {
            progress = 0;
            using (NegativeImage negative = new NegativeImage(imageConverable))
            {
                negative.Succsess += (bmp) =>
                {
                    progress = 100;
                    pictureBox1.BackgroundImage = imageConverable = bmp;
                };
                negative.SetNegative();
            }
        }
    }
}