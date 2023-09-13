using AVFoundation;
using DineDash.Interface;
using DineDash.iOS.Renderer;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundPlayer_iOS))]
namespace DineDash.iOS.Renderer
{
    public class SoundPlayer_iOS : ISoundPlayer
    {
        public void PlaySound(string soundFileName)
        {
            var player = new AVAudioPlayer(NSUrl.FromFilename(soundFileName), "mp3", out _);
            player.PrepareToPlay();
            player.Play();
        }
    }
}