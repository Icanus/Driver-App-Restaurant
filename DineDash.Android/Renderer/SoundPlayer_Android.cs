using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DineDash.Droid.Renderer;
using DineDash.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundPlayer_Android))]
namespace DineDash.Droid.Renderer
{
    public class SoundPlayer_Android : ISoundPlayer
    {
        public void PlaySound(string soundFileName)
        {
            var player = new MediaPlayer();
            var assetFileDescriptor = Android.App.Application.Context.Assets.OpenFd(soundFileName);
            player.SetDataSource(assetFileDescriptor.FileDescriptor, assetFileDescriptor.StartOffset, assetFileDescriptor.Length);
            player.Prepare();
            player.Start();
        }
    }
}