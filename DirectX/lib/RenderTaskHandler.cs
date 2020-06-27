using DirectX.D2D;
using MelloRin.CSd3d.Core;
using SharpDX.Windows;

namespace DirectX.lib
{
    public class RenderTaskHandler
    {
        public D2DFont font { get; protected set; }
        public D2DSprite sprite { get; protected set; }
        public RenderForm targetForm { get; protected set; }
    }
}
