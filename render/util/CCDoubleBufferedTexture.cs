using UnityEngine;

namespace cc.creativecomputing.render
{
    public class CCDoubleBufferedTexture
    {
        public static RenderTexture CreateTexture(CCTextureSetup theSetup)
        {
            var myResult = new RenderTexture(theSetup.width, theSetup.height, theSetup.depth, theSetup.format)
            {
                enableRandomWrite = true,
                filterMode = theSetup.filterMode,
                wrapMode = theSetup.wrapMode,
                useMipMap = theSetup.useMipMap,
                dimension = theSetup.dimension,
                volumeDepth =  theSetup.volumeDepth
            };
            myResult.Create();
            return myResult;
        }
    
        private RenderTexture _readTex;
        private RenderTexture _writeTex;

        private CCTextureSetup _mySetup;
    
        public CCDoubleBufferedTexture(CCTextureSetup theSetup)
        {
            _mySetup = theSetup;
            Reset();
        }
        public void Swap()
        {
            var tmp = _readTex;
            _readTex = _writeTex;
            _writeTex = tmp;
        }

        public RenderTexture WriteTex => _writeTex;
        public RenderTexture ReadTex => _readTex;

        public void Reset()
        {
            _readTex = CreateTexture(_mySetup);
            _writeTex = CreateTexture(_mySetup);
        }

    
    }
}