// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System.Collections.Generic;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace protocols.tuio
{
    [AddComponentMenu("TUIO/ TUIO Receiver")]
    public sealed class CCTuioEventReceiver : MonoBehaviour
    {
        public abstract class CCTuioEntity
        {
            public int Id { get; private set; }
            public float X { get; internal set; }
            public float Y { get; internal set; }
            public float VelocityX { get; internal set; }
            public float VelocityY { get; internal set; }
            public float Acceleration { get; internal set; }

            protected CCTuioEntity(int id, float x = 0, float y = 0, float velocityX = 0, float velocityY = 0, float acceleration = 0)
            {
                Id = id;
                X = x;
                Y = y;
                VelocityX = velocityX;
                VelocityY = velocityY;
                Acceleration = acceleration;
            }
            public void Update(float x, float y, float velocityX, float velocityY, float acceleration)
            {
                X = x;
                Y = y;
                VelocityX = velocityX;
                VelocityY = velocityY;
                Acceleration = acceleration;
            }
        }

        public class CCTuioCursor : CCTuioEntity
        {
            public CCTuioCursor(int id, float x = 0, float y = 0, float velocityX = 0, float velocityY = 0, float acceleration = 0)
                : base(id, x, y, velocityX, velocityY, acceleration)
            {
            }

    
        }
        
        public delegate void Cursor(CCTuioCursor theCursor);

        

        #region Editable fields

        [SerializeField] private int udpPort = 3333;
        [SerializeField] private string oscAddress = "/tuio/2Dcur";
        [SerializeField] private int frameNumber;
        #endregion

        #region Internal members

        private List<Cursor> CursorAdded = new List<Cursor>();
        private List<Cursor> CursorUpdated = new List<Cursor>();
        private List<Cursor> CursorRemoved = new List<Cursor>();
        
        // Used to store values on initialization
        private int _currentPort;
        private string _currentAddress;

        #endregion

        #region MonoBehaviour implementation

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(oscAddress))
            {
                _currentAddress = null;
                return;
            }

            var server = OscMaster.GetSharedServer(udpPort);
            server.MessageDispatcher.AddCallback(oscAddress, OnDataReceive);

            _currentPort = udpPort;
            _currentAddress = oscAddress;
        }

        private void OnDisable()
        {
            if (string.IsNullOrEmpty(_currentAddress)) return;

            var server = OscMaster.GetSharedServer(_currentPort);
            server.MessageDispatcher.RemoveCallback(_currentAddress, OnDataReceive);

            _currentAddress = null;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            OnDisable();
            OnEnable();
        }

        void Update()
        {
            
        }

        #endregion

        #region OSC event callback

        public Dictionary<int, CCTuioCursor> cursors = new Dictionary<int, CCTuioCursor>();

        private void OnDataReceive(string address, OscDataHandle data)
        {
            var addedCursors = new List<int>();
            var updatedCursors = new List<CCTuioCursor>();
            var removedCursors = new List<int>();
            var command = data.GetElementAsString(0);
       
            switch (command)
            {
                case "set":
                    if (data.GetElementCount() < 7) return;
                    
                    var id = data.GetElementAsInt(0);
                    
                    if (!cursors.TryGetValue(id, out var cursor)) cursor = new CCTuioCursor(id);
                    cursor.Update(
                        data.GetElementAsFloat(2), 
                        data.GetElementAsFloat(3), 
                        data.GetElementAsFloat(4),
                        data.GetElementAsFloat(5),
                        data.GetElementAsFloat(6)
                        );
                    updatedCursors.Add(cursor);
                    break;
                case "alive":
                    for (var i = 1; i < data.GetElementCount(); i++)
                    {
                        var aliveId = data.GetElementAsInt(i);
                        addedCursors.Add(aliveId);
                    }

                    foreach (var value in cursors)
                    {
                        if (!addedCursors.Contains(value.Key))
                        {
                            removedCursors.Add(value.Key);
                        }

                        addedCursors.Remove(value.Key);
                    }

                    break;
                case "fseq":
                    if (data.GetElementCount() < 2) return;
                    frameNumber = data.GetElementAsInt(1);
                    var count = updatedCursors.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var updatedCursor = updatedCursors[i];
                        if (addedCursors.Contains(updatedCursor.Id) && !cursors.ContainsKey(updatedCursor.Id))
                        {
                            cursors.Add(updatedCursor.Id, updatedCursor);
                            CursorAdded.ForEach(c => c.Invoke(updatedCursor));
                        }
                        else
                        {
                            CursorUpdated.ForEach(c => c.Invoke(updatedCursor));
                        }
                    }

                    count = removedCursors.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var cursorId = removedCursors[i];
                        cursor = cursors[cursorId];
                        cursors.Remove(cursorId);
                        CursorRemoved.ForEach(c => c.Invoke(cursor));
                    }
                    break;
            }
        }

        #endregion
    }
}
