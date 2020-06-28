﻿using DirectX.D2D.Font;
using DirectX.util.Interface;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Concurrent;

namespace DirectX.D2D
{
    public class D2DFont : IDisposable, IDrawable, IListable
    {
        public RenderTarget renderTarget { get; private set; }
        public static ConcurrentDictionary<string, FontData> _Ldraw = new ConcurrentDictionary<string, FontData>();

        public static void resetData()
        {
            foreach (string key in _Ldraw.Keys)
            {
                if (!key.Equals("fps"))
                {
                    _Ldraw.TryRemove(key, out FontData value);
                }
            }
        }

        public D2DFont(Texture2D backBuffer)
        {
            var d2dFactory = new SharpDX.Direct2D1.Factory();
            var d2dSurface = backBuffer.QueryInterface<Surface>();
            renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

            d2dSurface.Dispose();
            d2dFactory.Dispose();
        }

        public void add(string tag, ListData data)
        {
            if (_Ldraw.ContainsKey(tag))
            {
                _Ldraw[tag] = (FontData)data;
            }
            else
            {
                _Ldraw.TryAdd(tag, (FontData)data);
            }
        }

        public void delete(string tag)
        {
            if (_Ldraw.ContainsKey(tag))
            {
                _Ldraw.TryRemove(tag, out FontData temp);
            }
        }

        public void modString(string tag, string text)
        {
            if (_Ldraw.ContainsKey(tag))
            {
                _Ldraw[tag].text = text;
            }
        }

        public void modPoint(string tag, int x, int y)
        {
            if (_Ldraw.ContainsKey(tag))
            {
                _Ldraw[tag].x = x;
                _Ldraw[tag].y = y;
            }
        }

        public void draw()
        {
            renderTarget.BeginDraw();

            foreach (string key in _Ldraw.Keys)
            {
                try
                {
                    FontData drawTarget = _Ldraw[key];
                    renderTarget.DrawText(drawTarget.text, drawTarget._directWriteTextFormat,
                        new RawRectangleF(drawTarget.x, drawTarget.y, 1280, 720),
                        drawTarget._directWriteFontColor);
                    /*new RawRectangleF(drawTarget.x, drawTarget.y, float.Parse(GlobalDataManager.settings.getSetting("width")), float.Parse(GlobalDataManager.settings.getSetting("width"))),
                    drawTarget._directWriteFontColor);*/

                }
                catch (Exception) { }
            }
            renderTarget.EndDraw();
        }

        public void Dispose()
        {
            foreach (string key in _Ldraw.Keys)
            {
                FontData drawTarget = _Ldraw[key];

                drawTarget.Dispose();
            }

            renderTarget.Dispose();
        }
    }
}