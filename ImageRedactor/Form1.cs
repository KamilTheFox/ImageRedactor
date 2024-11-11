using Images;
using System.Drawing.Imaging;

namespace ImageRedactor
{
    public partial class Form1 : Form
    {
        private ImageCompressor imageCompressor;

        private int progress;
        public Form1()
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
                dialog.Filter = "JPEG Files(*.jpeg)|*.jpeg|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(dialog.FileName);
                    bool hasAlpha = bmp.PixelFormat.HasFlag(PixelFormat.Alpha) ||
                    bmp.PixelFormat == PixelFormat.Format32bppArgb ||
                    bmp.PixelFormat == PixelFormat.Format64bppArgb;
                    if(hasAlpha)
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
                    imageCompressor = new ImageCompressor(bmp);
                }
            }
        }

        private async void buttonCompress_Click(object sender, EventArgs e)
        {
            double procent = 100;
            if(!double.TryParse(procentCompress.Text, out procent))
            {
                MessageBox.Show("Введите процент сжатия");
                return;
            }
            Bitmap map = await imageCompressor.CompressAsync(procent);
            pictureBox1.BackgroundImage = map;
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
        private ImageCodecInfo GetJpegEncoder()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
        }
    }
}