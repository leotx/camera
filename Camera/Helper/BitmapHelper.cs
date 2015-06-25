using Android.Graphics;

namespace Camera.Helper
{
    public static class BitmapHelper
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            var options = new BitmapFactory.Options {InJustDecodeBounds = true};
            BitmapFactory.DecodeFile(fileName, options);

            var outHeight = options.OutHeight;
            var outWidth = options.OutWidth;
            var inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                    ? outHeight/height
                    : outWidth/width;
            }

            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            var resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}