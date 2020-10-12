using DirectX.D2D.Font;
using DirectX.D3D;
using DirectX.util.Interface;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
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
            SharpDX.Direct2D1.Factory d2dFactory = new SharpDX.Direct2D1.Factory();
            Surface d2dSurface = backBuffer.QueryInterface<Surface>();


            PixelFormat pixelFormat = new PixelFormat(D3DHandler.RENDER_FORMAT, D3DHandler.ALPHA_MODE);

            var renderTargetProp = new RenderTargetProperties(pixelFormat);
            renderTarget = new RenderTarget(d2dFactory, d2dSurface, renderTargetProp);

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
                        drawTarget.textBox,
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