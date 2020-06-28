﻿using DirectX.D2D.Sprite;
using DirectX.lib.Interface;
using DirectX.util.Interface;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace DirectX.D2D
{
    public class D2DSprite : IDisposable, IListable, IDrawable
    {
        public static ConcurrentDictionary<string, SpriteData> _LBackgroundSprite { get; private set; }
        public static Dictionary<string, SpriteData> _LSprite { get; private set; }
        public static ConcurrentDictionary<string, ClickableSprite> _LClickableSprite { get; private set; }

        public RenderTarget renderTarget { get; private set; }

        public D2DSprite(Texture2D backBuffer)
        {
            _LBackgroundSprite = new ConcurrentDictionary<string, SpriteData>();
            _LSprite = new Dictionary<string, SpriteData>();
            _LClickableSprite = new ConcurrentDictionary<string, ClickableSprite>();


            var d2dFactory = new SharpDX.Direct2D1.Factory();
            var d2dSurface = backBuffer.QueryInterface<Surface>();
            renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

            d2dSurface.Dispose();
            d2dFactory.Dispose();
        }

        public static void resetData()
        {
            _LSprite = new Dictionary<string, SpriteData>();
            _LClickableSprite = new ConcurrentDictionary<string, ClickableSprite>();
        }

        public static BitmapBrush makeBitmapBrush(RenderTarget renderTarget, string imgName, bool blankImage = false)
        {
            //TODO : 여기 바꿔라!
            string imageSrc = "";

            if (blankImage)
            {
                var pf = new SharpDX.Direct2D1.PixelFormat()
                {
                    AlphaMode = SharpDX.Direct2D1.AlphaMode.Premultiplied,
                    Format = Format.B8G8R8A8_UNorm
                };

                BitmapRenderTarget pallete = new BitmapRenderTarget(renderTarget, CompatibleRenderTargetOptions.GdiCompatible, pf);
                return new BitmapBrush(renderTarget, pallete.Bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
            if (File.Exists(imageSrc))
            {
                ImagingFactory imagingFactory = new ImagingFactory();
                NativeFileStream fileStream = new NativeFileStream(imageSrc,
                    NativeFileMode.Open, NativeFileAccess.Read);
                BitmapDecoder bitmapDecoder = new BitmapDecoder(imagingFactory, fileStream, DecodeOptions.CacheOnDemand);

                BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

                FormatConverter converter = new FormatConverter(imagingFactory);
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

                Bitmap bitmap = Bitmap.FromWicBitmap(renderTarget, converter);

                Utilities.Dispose(ref bitmapDecoder);
                Utilities.Dispose(ref fileStream);
                Utilities.Dispose(ref imagingFactory);
                Utilities.Dispose(ref converter);

                return new BitmapBrush(renderTarget, bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
            else
            {
                Console.WriteLine("{0} missing", imageSrc);

                var pf = new SharpDX.Direct2D1.PixelFormat()
                {
                    AlphaMode = SharpDX.Direct2D1.AlphaMode.Premultiplied,
                    Format = Format.B8G8R8A8_UNorm
                };
                BitmapRenderTarget pallete = new BitmapRenderTarget(renderTarget, CompatibleRenderTargetOptions.GdiCompatible, new Size2F(30f, 30f), new Size2(1, 1), pf);

                pallete.BeginDraw();
                pallete.Clear(Color.Purple);
                pallete.EndDraw();

                return new BitmapBrush(renderTarget, pallete.Bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
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
