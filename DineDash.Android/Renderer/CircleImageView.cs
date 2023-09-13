using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DineDash.Droid.Renderer
{
    public class CircleImageView : ImageView
    {
        public CircleImageView(Context context) : base(context)
        {
            Initialize();
        }

        public CircleImageView(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public CircleImageView(Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            SetScaleType(ScaleType.CenterCrop);
        }

        protected override void OnDraw(Canvas canvas)
        {
            BitmapDrawable drawable = Drawable as BitmapDrawable;
            if (drawable != null)
            {
                using (Bitmap bitmap = drawable.Bitmap)
                {
                    if (bitmap != null)
                    {
                        int width = Width;
                        int height = Height;
                        int minSize = Math.Min(width, height);

                        // Create a square bitmap from the original image (centered)
                        Bitmap squareBitmap = Bitmap.CreateBitmap(bitmap, (bitmap.Width - minSize) / 2, (bitmap.Height - minSize) / 2, minSize, minSize);

                        // Create a BitmapShader to draw the circular image
                        BitmapShader shader = new BitmapShader(squareBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

                        Paint paint = new Paint();
                        paint.SetShader(shader);
                        paint.AntiAlias = true;

                        float radius = minSize / 2.0f;
                        canvas.DrawCircle(width / 2.0f, height / 2.0f, radius, paint);
                    }
                }
            }
            else
            {
                base.OnDraw(canvas);
            }
        }
    }
}