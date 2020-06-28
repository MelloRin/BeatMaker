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

//source from https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectWrite/CustomFont/ResourceFontLoader.cs

//some parts are moded by MelloRin

using DirectX.util.Font;
using FileManager;
using FileManager.util.Directory;
using SharpDX;
using SharpDX.DirectWrite;
using System.Collections.Generic;

namespace DirectX.D2D.Font
{
    public partial class FontLoader : CallbackBase, FontCollectionLoader, FontFileLoader
    {
        private readonly List<ResourceFontFileStream> _fontStreams = new List<ResourceFontFileStream>();
        private readonly List<ResourceFontFileEnumerator> _enumerators = new List<ResourceFontFileEnumerator>();
        private readonly Factory _factory;

        public FontLoader(/*Factory factory*/ResourceManageCore resourceCore, string searchDir)
        {
            _factory = new Factory();
            //_factory = factory;

            FileManagerCore.logger.Info(this, "Loading Fonts from " + searchDir);
            if (resourceCore.getDirectory(out ResDirectory resDirectory, searchDir))
            {
                foreach (string name in resDirectory.getChildFileList())
                {
                    if (name.EndsWith(".ttf"))
                    {
                        FileManagerCore.logger.Info(this, "Fonts file found! " + name);
                        resourceCore.getFile(out ResFile resfile, searchDir + "/" + name);

                        DataStream stream = new DataStream(resfile.rawData.Length, true, true);
                        stream.Write(resfile.rawData, 0, resfile.rawData.Length);
                        stream.Position = 0;
                        _fontStreams.Add(new ResourceFontFileStream(stream));
                    }
                }
            }
            FileManagerCore.logger.Info(this, "Loading Fonts Success!");

            Key = new DataStream(sizeof(int) * _fontStreams.Count, true, true);
            for (int i = 0; i < _fontStreams.Count; i++)
            {
                Key.Write(i);
            }

            Key.Position = 0;

            _factory.RegisterFontFileLoader(this);
            _factory.RegisterFontCollectionLoader(this);
        }

        public DataStream Key { get; }

        FontFileEnumerator FontCollectionLoader.CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
        {
            var enumerator = new ResourceFontFileEnumerator(factory, this, collectionKey);
            _enumerators.Add(enumerator);

            return enumerator;
        }

        FontFileStream FontFileLoader.CreateStreamFromKey(DataPointer fontFileReferenceKey)
        {
            var index = Utilities.Read<int>(fontFileReferenceKey.Pointer);
            return _fontStreams[index];
        }
    }
}