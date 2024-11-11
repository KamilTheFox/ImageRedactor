using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Images
{
    public class NegativeImage : IDisposable
    {
        private readonly Bitmap bitmap;

        private readonly BitmapData data;

        public event Action<string> Errors;

        public event Action<Bitmap> Succsess;

        private bool isSuccsess;
        public NegativeImage(Bitmap bitmap)
        {
            this.bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            data = this.bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        }
        public void SetNegative()
        {
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;
                    Parallel.For(0, data.Height, y =>
                    {
                        byte* row = ptr + (y * data.Stride);
                        for (int x = 0; x < data.Width; x++)
                        {
                            int offset = x * 3;
                            row[offset + 2] = (byte)(byte.MaxValue - row[offset + 2]);
                            row[offset + 1] = (byte)(byte.MaxValue - row[offset + 1]);
                            row[offset] = (byte)(byte.MaxValue - row[offset]);
                        }
                    });
                }
                isSuccsess = true;
                bitmap.UnlockBits(data);
            }
            catch(Exception ex)
            {
                Errors?.Invoke(ex.Message);
            }
        }
        public void Dispose()
        {
            if(isSuccsess)
                Succsess?.Invoke(bitmap);
            isSuccsess = false;
            Errors = null;
            Succsess = null;
        }
    }
}
