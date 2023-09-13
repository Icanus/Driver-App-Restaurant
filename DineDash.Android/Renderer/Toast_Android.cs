using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using DineDash.Droid.Renderer;
using DineDash.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_Android))]
namespace DineDash.Droid.Renderer
{
    public class Toast_Android : Toast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, Android.Widget.ToastLength.Short).Show();
        }
    }
}