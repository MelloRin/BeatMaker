// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//source from https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectWrite/CustomFont/ResourceFontFileStream.cs

using SharpDX;
using SharpDX.DirectWrite;
using System;

namespace DirectX.util.Font
{
    public class ResourceFontFileStream : CallbackBase, FontFileStream
    {
        private readonly DataStream _stream;

        public ResourceFontFileStream(DataStream stream)
        {
            _stream = stream;
        }

        void FontFileStream.ReadFileFragment(out IntPtr fragmentStart, long fileOffset, long fragmentSize, out IntPtr fragmentContext)
        {
            lock (this)
            {
                fragmentContext = IntPtr.Zero;
                _stream.Position = fileOffset;
                fragmentStart = _stream.PositionPointer;
            }
        }

        void FontFileStream.ReleaseFileFragment(IntPtr fragmentContext)
        {
            // Nothing to release. No context are used
        }

        long FontFileStream.GetFileSize()
        {
            return _stream.Length;
        }

        long FontFileStream.GetLastWriteTime()
        {
            return 0;
        }
    }
}
