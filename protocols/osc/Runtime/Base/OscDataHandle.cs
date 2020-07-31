// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System;
using System.Collections.Generic;

namespace OscJack
{
    //
    // Data handle class that provides offset values to each data element
    // within a shared data buffer
    //
    public sealed class OscDataHandle
    {
        #region Public methods

        public int GetElementCount()
        {
            return _typeTags.Count;
        }

        public int GetElementAsInt(int index)
        {
            if (index >= _typeTags.Count) return 0;
            var tag = _typeTags[index];
            var offs = _offsets[index];
            switch (tag)
            {
                case 'i':
                    return OscDataTypes.ReadInt(_sharedBuffer, offs);
                case 'f':
                    return (int)OscDataTypes.ReadFloat(_sharedBuffer, offs);
                default:
                    return 0;
            }
        }

        public float GetElementAsFloat(int index)
        {
            if (index >= _typeTags.Count) return 0;
            var tag = _typeTags[index];
            var offs = _offsets[index];
            switch (tag)
            {
                case 'f':
                    return OscDataTypes.ReadFloat(_sharedBuffer, offs);
                case 'i':
                    return OscDataTypes.ReadInt(_sharedBuffer, offs);
                default:
                    return 0;
            }
        }

        public string GetElementAsString(int index)
        {
            if (index >= _typeTags.Count) return "";
            var tag = _typeTags[index];
            var offs = _offsets[index];
            switch (tag)
            {
                case 's':
                    return OscDataTypes.ReadString(_sharedBuffer, offs);
                case 'i':
                    return OscDataTypes.ReadInt(_sharedBuffer, offs).ToString();
                case 'f':
                    return OscDataTypes.ReadFloat(_sharedBuffer, offs).ToString();
                default:
                    return "";
            }
        }

        #endregion

        #region Internal method

        internal void Scan(byte[] buffer, int offset)
        {
            // Reset the internal state.
            _sharedBuffer = buffer;
            _typeTags.Clear();
            _offsets.Clear();

            // Read type tags.
            offset++; // ","

            while (true)
            {
                var tag = (char)buffer[offset];
                if (!OscDataTypes.IsSupportedTag(tag)) break;
                _typeTags.Add(tag);
                offset++;
            }

            offset += OscDataTypes.GetStringSize(buffer, offset);

            // Determine the offsets of the each element.
            foreach (var tag in _typeTags)
            {
                _offsets.Add(offset);

                switch (tag)
                {
                    case 'i':
                    case 'f':
                        offset += 4;
                        break;
                    case 's':
                        offset += OscDataTypes.GetStringSize(buffer, offset);
                        break;
                    // tag == 'b'
                    default:
                        offset += 4 + OscDataTypes.ReadInt(buffer, offset);
                        break;
                }
            }
        }

        #endregion

        #region Private members

        private byte[] _sharedBuffer;

        private readonly List<char> _typeTags = new List<char>(8);
        private readonly List<int> _offsets = new List<int>(8);

        #endregion
    }
}
