using DirectX.D2D.Sprite;
using DirectX.D3D;
using DirectX.util.Interface;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DirectX.D2D
{
    public class D2DSprite : IDisposable, IListable, IDrawable
    {
        private readonly ConcurrentDictionary<string, SpriteData> _LBackgroundSprite;
        private readonly Dictionary<string, SpriteData> _LSprite;
        private readonly ConcurrentDictionary<string, ClickableSprite> _LClickableSprite;

        public RenderTarget renderTarget { get; private set; }

        public D2DSprite(Texture2D backBuffer)
        {
            _LBackgroundSprite = new ConcurrentDictionary<string, SpriteData>();
            _LSprite = new Dictionary<string, SpriteData>();
            _LClickableSprite = new ConcurrentDictionary<string, ClickableSprite>();


            SharpDX.Direct2D1.Factory d2dFactory = new SharpDX.Direct2D1.Factory();
            Surface d2dSurface = backBuffer.QueryInterface<Surface>();
            renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(D3DHandler.RENDER_FORMAT, D3DHandler.ALPHA_MODE)));

            d2dSurface.Dispose();
            d2dFactory.Dispose();
        }

        public SpriteData getSprite(string tag)
        {
            _LSprite.TryGetValue(tag, out SpriteData value);

            return value;
        }

        public void resetData()
        {
            _LSprite.Clear();
            _LClickableSprite.Clear();
        }

        public void modPoint(string tag, int x, int y)
        {
            if (_LSprite.ContainsKey(tag))
            {
                _LSprite[tag].x = x;
                _LSprite[tag].y = y;
            }
        }

        public void modImage(string tag, BitmapBrush bitmapBrush)
        {
            if (_LSprite.ContainsKey(tag))
            {
                _LSprite[tag].bitmapBrush = bitmapBrush;
            }
        }

        public void draw()
        {
            renderTarget.BeginDraw();

            foreach (string key in _LBackgroundSprite.Keys)
            {
                SpriteData drawTarget = _LBackgroundSprite[key];

                if (_LBackgroundSprite[key].bitmapBrush != null)
                {
                    renderTarget.Transform = Matrix3x2.Translation(drawTarget.x, drawTarget.y);
                    renderTarget.FillRectangle(new RectangleF(0, 0, drawTarget.bitmapBrush.Bitmap.Size.Width, drawTarget.bitmapBrush.Bitmap.Size.Height), drawTarget.bitmapBrush);
                }
            }

            if(_LSprite.Count > 0)
            {
                string[] _LrenderQueue = _LSprite.Keys.ToArray();
                for (int i = 0; i < _LrenderQueue.Length; ++i)
                {
                    try
                    {
                        string key = _LrenderQueue[i];
                        SpriteData drawTarget = _LSprite[key];

                        if (_LSprite[key].bitmapBrush != null)
                        {
                            renderTarget.Transform = Matrix3x2.Translation(drawTarget.x, drawTarget.y);
                            renderTarget.FillRectangle(new RectangleF(0, 0, drawTarget.bitmapBrush.Bitmap.Size.Width, drawTarget.bitmapBrush.Bitmap.Size.Height), drawTarget.bitmapBrush);
                        }
                    }
                    catch (KeyNotFoundException)
                    {

                    }
                }
            }
            

            foreach (string key in _LClickableSprite.Keys)
            {
                SpriteData drawTarget = _LClickableSprite[key];

                if (_LClickableSprite[key].bitmapBrush != null)
                {
                    renderTarget.Transform = Matrix3x2.Translation(drawTarget.x, drawTarget.y);
                    renderTarget.FillRectangle(new RectangleF(0, 0, drawTarget.bitmapBrush.Bitmap.Size.Width, drawTarget.bitmapBrush.Bitmap.Size.Height), drawTarget.bitmapBrush);
                }
            }
            renderTarget.EndDraw();
        }

        public void add(string tag, ListData data)
        {
            if (_LSprite.ContainsKey(tag))
            {
                _LSprite[tag] = (SpriteData)data;
            }
            else
            {
                _LSprite.Add(tag, (SpriteData)data);
            }
        }

        public void setBackground(string tag, ListData data)
        {
            if (_LBackgroundSprite.ContainsKey(tag))
            {
                _LBackgroundSprite[tag] = (SpriteData)data;
            }
            else
            {
                _LBackgroundSprite.TryAdd(tag, (SpriteData)data);
            }
        }

        public void modBackgroundPoint(string tag, int x, int y)
        {
            if (_LBackgroundSprite.ContainsKey(tag))
            {
                _LBackgroundSprite[tag].x = x;
                _LBackgroundSprite[tag].y = y;
            }
        }

        public void modBackgroundImage(string tag, BitmapBrush bitmapBrush)
        {
            if (_LBackgroundSprite.ContainsKey(tag))
            {
                _LBackgroundSprite[tag].bitmapBrush = bitmapBrush;
            }
            else
            {
                _LBackgroundSprite.TryAdd(tag, new SpriteData(bitmapBrush, 0, 0));
            }
        }

        public void addButton(string tag, ListData data)
        {
            if (_LClickableSprite.ContainsKey(tag))
            {
                _LClickableSprite[tag] = (ClickableSprite)data;
            }
            else
            {
                _LClickableSprite.TryAdd(tag, (ClickableSprite)data);
            }
        }

        public void delete(string tag)
        {
            if (_LSprite.ContainsKey(tag))
            {
                _LSprite.Remove(tag);
            }
        }

        public void Dispose()
        {
            foreach (string key in _LSprite.Keys.ToArray())
            {
                SpriteData drawTarget = _LSprite[key];

                drawTarget.Dispose();
            }
            renderTarget.Dispose();
        }
    }
}



/*Bitmap bmp = new Bitmap(imgSource);

BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

if (System.Drawing.Image.IsAlphaPixelFormat(bmp.PixelFormat))
	Console.WriteLine("알파 ok");

DataStream stream = new DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
SharpDX.Direct2D1.PixelFormat pFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
BitmapProperties bmpProps = new BitmapProperties(pFormat);

D2Bitmap bitmap = new D2Bitmap(renderTarget, new Size2(bmp.Width, bmp.Height), stream, bmpData.Stride, bmpProps);

bmp.UnlockBits(bmpData);

stream.Dispose();
bmp.Dispose();

bitmapBrush = new BitmapBrush(renderTarget, bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });

return bitmap;*/
