using DirectX.lib;
using SharpDX.Direct2D1;
using System;

namespace DirectX.D2D.Sprite
{
    public class SpriteData : ListData, IDisposable
    {
        public BitmapBrush bitmapBrush { get; set; }

        public SpriteData(BitmapBrush bitmapBrush, int x, int y)
        {
            base.x = x;
            base.y = y;

            this.bitmapBrush = bitmapBrush;
        }

        public void Dispose()
        {
            if (bitmapBrush != null)
                bitmapBrush.Dispose();
        }
    }
}
