using DirectX.D3D;
using FileManager;
using FileManager.util.Directory;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using System;
using System.IO;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace DirectX.util.Sprite
{
    public class SpriteFactory
    {
        private readonly ResourceManageCore resourceCore;

        public SpriteFactory(ResourceManageCore resourceCore)
        {
            this.resourceCore = resourceCore;
        }

        public BitmapBrush makeBitmapBrush(string imgName, RenderTarget renderTarget, bool blankImage = false)
        {
            //TODO : 여기 바꿔라!
            string imageSrc = "";

            if (blankImage)
            {
                SharpDX.Direct2D1.PixelFormat pf = new SharpDX.Direct2D1.PixelFormat()
                {
                    AlphaMode = D3DHandler.ALPHA_MODE,
                    Format = D3DHandler.RENDER_FORMAT
                };

                BitmapRenderTarget pallete = new BitmapRenderTarget(renderTarget, CompatibleRenderTargetOptions.GdiCompatible, pf);
                return new BitmapBrush(renderTarget, pallete.Bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
            if (resourceCore.getFile(imgName, out ResFile resFile))
            {
                ImagingFactory imagingFactory = new ImagingFactory();
                /*NativeFileStream fileStream = new NativeFileStream(imageSrc,
                    NativeFileMode.Open, NativeFileAccess.Read);*/
                MemoryStream ms = new MemoryStream(resFile.rawData);
                BitmapDecoder bitmapDecoder = new BitmapDecoder(imagingFactory, ms, DecodeOptions.CacheOnDemand);

                BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

                FormatConverter converter = new FormatConverter(imagingFactory);
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

                Bitmap bitmap = Bitmap.FromWicBitmap(renderTarget, converter);

                Utilities.Dispose(ref bitmapDecoder);
                Utilities.Dispose(ref imagingFactory);
                Utilities.Dispose(ref converter);

                return new BitmapBrush(renderTarget, bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
            else
            {
                Console.WriteLine("{0} missing", imageSrc);

                SharpDX.Direct2D1.PixelFormat pf = new SharpDX.Direct2D1.PixelFormat()
                {
                    AlphaMode = D3DHandler.ALPHA_MODE,
                    Format = D3DHandler.RENDER_FORMAT
                };
                BitmapRenderTarget pallete = new BitmapRenderTarget(renderTarget, CompatibleRenderTargetOptions.GdiCompatible, new Size2F(30f, 30f), new Size2(1, 1), pf);

                pallete.BeginDraw();
                pallete.Clear(Color.Purple);
                pallete.EndDraw();

                return new BitmapBrush(renderTarget, pallete.Bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
            }
        }
    }
}
