using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Camera.Helper;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace Camera
{
    [Activity(Label = "Camera", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView ImageView { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            if (!IsThereAnAppToTakePictures()) return;
            
            CreateDirectoryForPictures();

            var button = FindViewById<Button>(Resource.Id.btnCamera);
            ImageView = FindViewById<ImageView>(Resource.Id.imageView);
            button.Click += TakeAPicture;
        }

        private static void CreateDirectoryForPictures()
        {
            App.Dir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "CameraTest");

            if (!App.Dir.Exists())
            {
                App.Dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            var availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            App.File = new File(App.Dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App.File));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            var contentUri = Uri.FromFile(App.File);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            var height = Resources.DisplayMetrics.HeightPixels;
            var width = ImageView.Height;
            App.Bitmap = App.File.Path.LoadAndResizeBitmap(width, height);
            if (App.Bitmap != null)
            {
                ImageView.SetImageBitmap(App.Bitmap);
                App.Bitmap = null;
            }

            GC.Collect();
        }
    }

    public static class App
    {
        public static File File;  
        public static File Dir;
        public static Bitmap Bitmap;
    }
}