using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Expansion
{
    public static Bitmap ParallelForFixResult(this Bitmap bitmap,Action<int, int, byte, byte, byte> getRGB)
    {
        return bitmap.ParallelForFixResult(null, getRGB);
    }
    public static Bitmap ParallelForFixResult(this Bitmap bitmap, Func<int, int, byte, byte, byte> FixResult = null, Action<int,int,byte,byte,byte> getRGB = null)
    {
        int width = bitmap.Width;
        int height = bitmap.Height;
        var result = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
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
                        if(getRGB != null)
                            getRGB(x, y, row[offset + 2], row[offset + 1], row[offset]);

                        if (FixResult != null)
                        {
                            row[offset + 2] = FixResult(x, y, 0, row[offset + 2]);
                            row[offset + 1] = FixResult(x, y, 1, row[offset + 1]);
                            row[offset] = FixResult(x, y, 2, row[offset]);
                        }
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
}
