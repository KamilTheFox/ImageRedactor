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
        private Bitmap bitmap;

        public event Action<string> Errors;

        public event Action<Bitmap> Succsess;

        private bool isSuccsess;
        public NegativeImage(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }
        public void SetNegative()
        {
            try
            {
                bitmap = bitmap.ParallelForFixResult((x, y, indexColor, pixel) =>
                {
                    return (byte)(byte.MaxValue - pixel);
                });
                isSuccsess = true;
            }
            catch (Exception ex)
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
