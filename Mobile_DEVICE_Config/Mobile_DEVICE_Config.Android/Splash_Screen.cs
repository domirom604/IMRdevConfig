using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Felipecsl.GifImageViewLibrary;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Mobile_DEVICE_Config.Droid
{
    [Activity(Theme = "@style/splashTheme", MainLauncher = true, NoHistory =true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Splash_Screen : AppCompatActivity
    {
        protected GifImageView gifImageView; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);

            gifImageView = FindViewById<GifImageView>(Resource.Id.gifImageView);
            Stream input = Assets.Open("loader.gif");
            byte[] bytes = ConvertFileToByteArray(input);
            gifImageView.SetBytes(bytes);
            gifImageView.StartAnimation();

            Timer timer = new Timer();
            
            timer.Interval = 4000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;

            timer.Start();
            //StartActivity(new Intent(this, typeof(MainActivity)));

        }
        private void Timer_Elapsed(object Sender, ElapsedEventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            
        }
        private byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using(MemoryStream ms = new MemoryStream())
            {
                int read;
                while((read = input.Read(buffer, 0 ,buffer.Length)) > 0 )
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}