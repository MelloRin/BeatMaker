using DirectX.D2D;
using DirectX.D2D.Font;
using DirectX.lib.Interface;
using DirectX.util;
using DirectX.util.Interface;
using DirectX.util.Task;
using FileManager;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;
using Timer = System.Timers.Timer;

namespace DirectX.D3D
{
    public class D3DHandler : RenderTaskHandle, IDisposable, ITask
    {
        public static readonly Format RENDER_FORMAT = Format.B8G8R8A8_UNorm;
        public static readonly SharpDX.Direct2D1.AlphaMode ALPHA_MODE = SharpDX.Direct2D1.AlphaMode.Premultiplied;

        #region field members
        //D3D device
        private Device _device;
        private SwapChain _swapChain;
        private RenderTargetView _backbufferView;
        private DepthStencilView _zbufferView;
        private DeviceContext _deviceContext;
        private Texture2D _backBufferTexture;
        private SwapChainDescription desc;

        private readonly ConcurrentDictionary<string, IFrameTimeTask> frameTimeWorkList;

        private Timer timer;

        private int frame = 0;

        private readonly ResourceManageCore resourceManageCore;
        #endregion

        public D3DHandler(RenderForm mainForm, ResourceManageCore resourceManageCore, IFrameTimeTask mainformTask)
        {
            this.resourceManageCore = resourceManageCore;
            targetForm = mainForm;

            frameTimeWorkList = new ConcurrentDictionary<string, IFrameTimeTask>();

            addFrameTimeTask("mainform", mainformTask);

            initialize();
        }

        public void initialize()
        {
            timer = new Timer(1000d);
            timer.Elapsed += new ElapsedEventHandler((sender, e) =>
            {
                font.modString("fps", frame + " fps");
                //font.modString("nowTime", "Current Time " + DateTime.Now.ToString());

                frame = 0;
            });

            createDevice();
        }

        public bool addFrameTimeTask(string tag, IFrameTimeTask task)
        {
            return frameTimeWorkList.TryAdd(tag, task);
        }

        public void run(TaskQueue taskQueue)
        {
            Thread _Td3d = new Thread(() =>
            {
                timer.Start();

                RawColor4 clearColor = new RawColor4(0f, 0f, 0f, 0f);

                while (targetForm.Created)
                {
                    try
                    {
                        Clear(clearColor);

                        foreach (string taskName in frameTimeWorkList.Keys)
                        {
                            IFrameTimeTask task = frameTimeWorkList[taskName];

                            task.frameTimeWork();
                        }

                        sprite.draw();
                        font.draw();
                        Present();

                        ++frame;
                    }
                    catch (SharpDXException e)
                    {
                        Console.WriteLine("D3D 에러" + e.ToString());
                        return;
                    }
                }
                Dispose();
            });
            _Td3d.Start();

            taskQueue.runNext();
        }

        private void createDevice()
        {
            int currentWindowWidth = 0;
            int currentWindowHeight = 0;
            IntPtr OutputHandle = IntPtr.Zero;

            targetForm.Invoke(new MethodInvoker(() =>
            {
                currentWindowWidth = targetForm.ClientSize.Width;
                currentWindowHeight = targetForm.ClientSize.Height;
                OutputHandle = targetForm.Handle;
            }));

            desc = new SwapChainDescription()
            {
                BufferCount = 1,//buffer count
                ModeDescription = new ModeDescription(currentWindowWidth, currentWindowHeight, new Rational(60, 1), RENDER_FORMAT),
                //IsWindowed = bool.Parse(GlobalDataManager.settings.getSetting("windowded")),
                IsWindowed = true,
                OutputHandle = OutputHandle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            FeatureLevel[] levels = new FeatureLevel[] { FeatureLevel.Level_11_0 };
            DeviceCreationFlags flag = DeviceCreationFlags.None | DeviceCreationFlags.BgraSupport;

            Device.CreateWithSwapChain(DriverType.Hardware, flag, levels, desc, out _device, out _swapChain);

            _deviceContext = _device.ImmediateContext;

            _backBufferTexture = _swapChain.GetBackBuffer<Texture2D>(0);

            FontLoader fontLoader = new FontLoader(resourceManageCore, "/fonts");
            FontCollection fontCollection = new FontCollection(new SharpDX.DirectWrite.Factory(), fontLoader, fontLoader.Key);

            font = new D2DFont(_backBufferTexture);
            font.add("fps", new FontData(frame + " fps", font.renderTarget, fontCollection, Color.White, fontName: "applemint", fontSize: 20, x: 710));
            font.add("score", new FontData("0", font.renderTarget, fontCollection, Color.White, fontName: "applemint", fontSize: 20, x: 710, y:25));
            sprite = new D2DSprite(_backBufferTexture);

            _backbufferView = new RenderTargetView(_device, _backBufferTexture);
            _backBufferTexture.Dispose();

            var _zbufferTexture = new Texture2D(_device, new Texture2DDescription()
            {
                Format = Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                //Width = int.Parse(GlobalDataManager.settings.getSetting("width")),
                //Height = int.Parse(GlobalDataManager.settings.getSetting("height")),
                Width = 1280,
                Height = 720,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            // Create the depth buffer view
            _zbufferView = new DepthStencilView(_device, _zbufferTexture);
            _zbufferTexture.Dispose();

            _deviceContext.Rasterizer.SetViewport(0, 0, targetForm.ClientSize.Width, targetForm.ClientSize.Height);
            _deviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);

            //GlobalDataManager.deviceCreated = true;
        }

        private void Clear(RawColor4 color)
        {
            _deviceContext.ClearRenderTargetView(_backbufferView, color);
            _deviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
        }

        private void Present() { _swapChain.Present(1, PresentFlags.None); }

        public void Dispose()
        {
            if (_device != null)
            {
                _device.Dispose();
            }

            if (font != null)
            {
                font.Dispose();
            }

            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}
