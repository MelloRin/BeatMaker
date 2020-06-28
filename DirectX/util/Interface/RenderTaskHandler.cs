using DirectX.D2D;
using SharpDX.Windows;

namespace DirectX.util
{
    public class RenderTaskHandler
    {
        public D2DFont font { get; protected set; }
        public D2DSprite sprite { get; protected set; }
        public RenderForm targetForm { get; protected set; }
    }
}
