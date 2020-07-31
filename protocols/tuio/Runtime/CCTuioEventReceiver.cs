// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System.Collections.Generic;
using System.Linq;
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

        private readonly Queue<CCTuioCursor> _addQueue = new Queue<CCTuioCursor>();
        private readonly Queue<CCTuioCursor> _removeQueue = new Queue<CCTuioCursor>();
        private readonly Queue<CCTuioCursor> _updateQueue = new Queue<CCTuioCursor>();
        
        private readonly List<Cursor> _cursorAdded = new List<Cursor>();
        private readonly List<Cursor> _cursorUpdated = new List<Cursor>();
        private readonly List<Cursor> _cursorRemoved = new List<Cursor>();

        public void OnAdd(Cursor theEvent)
        {
            _cursorAdded.Add(theEvent);
        }
        
        public void OnUpdate(Cursor theEvent)
        {
            _cursorUpdated.Add(theEvent);
        }
        
        public void OnRemove(Cursor theEvent)
        {
            _cursorRemoved.Add(theEvent);
        }

        public Dictionary<int, CCTuioCursor>.ValueCollection ActiveCursors => cursors.Values;

        // Used to store values on initialization
        //private int _currentPort;

        #endregion

        #region MonoBehaviour implementation

        private void Start()
        {
            if (string.IsNullOrEmpty(oscAddress))
            {
                return;
            }

            var server = OscMaster.GetSharedServer(udpPort);
            server.MessageDispatcher.AddCallback(oscAddress, OnDataReceive);
            
            Debug.Log(server);
        }

        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(oscAddress)) return;

            var server = OscMaster.GetSharedServer(udpPort);
            server.MessageDispatcher.RemoveCallback(oscAddress, OnDataReceive);
        }

        private static void SendCursorEvents(List<Cursor> theEvents, Queue<CCTuioCursor> theCursors)
        {
            lock (theCursors)
            {
                while (theCursors.Count > 0)
                {
                    var myCursor = theCursors.Dequeue();
                    theEvents.ForEach(c => c.Invoke(myCursor));
                }
            }
        }

        private void Update()
        {
            SendCursorEvents(_cursorAdded, _addQueue);
            SendCursorEvents(_cursorRemoved, _removeQueue);
            SendCursorEvents(_cursorUpdated, _updateQueue);
            
        }

        #endregion

        #region OSC event callback

        public Dictionary<int, CCTuioCursor> cursors = new Dictionary<int, CCTuioCursor>();

        private readonly List<int> _addedCursors = new List<int>();
        private readonly List<CCTuioCursor> _updatedCursors = new List<CCTuioCursor>();
        private readonly List<int> _removedCursors = new List<int>();
        
        private void OnDataReceive(string address, OscDataHandle data)
        {
            
            var command = data.GetElementAsString(0);
            switch (command)
            {
                case "alive":
                    for (var i = 1; i < data.GetElementCount(); i++)
                    {
                        var aliveId = data.GetElementAsInt(i);
                        _addedCursors.Add(aliveId);
                    }

                    
                    foreach (var value in cursors.Where(value => !_addedCursors.Contains(value.Key)))
                    {
                        _removedCursors.Add(value.Key);
                    }

                    break;
                case "set":
                    if (data.GetElementCount() < 7) return;
                    
                    var id = data.GetElementAsInt(1);
                    
                    if (!cursors.TryGetValue(id, out var cursor)) cursor = new CCTuioCursor(id);
                    cursor.Update(
                        data.GetElementAsFloat(2), 
                        data.GetElementAsFloat(3), 
                        data.GetElementAsFloat(4),
                        data.GetElementAsFloat(5),
                        data.GetElementAsFloat(6)
                        );
                    
                    _updatedCursors.Add(cursor);
                    break;
                case "fseq":
                    if (data.GetElementCount() < 2) return;
                    frameNumber = data.GetElementAsInt(1);
                    
                    _updatedCursors.ForEach(updatedCursor =>
                    {
                        _addedCursors.ForEach(i => Debug.Log(i));
                        if (_addedCursors.Contains(updatedCursor.Id) && !cursors.ContainsKey(updatedCursor.Id))
                        {
                            
                            cursors.Add(updatedCursor.Id, updatedCursor);
                            _addQueue.Enqueue(updatedCursor);
                        }
                        else
                        {
                            _updateQueue.Enqueue(updatedCursor);
                        }
                    });

                    _removedCursors.ForEach(cursorId =>
                    {
                        _removeQueue.Enqueue(cursors[cursorId]);
                        cursors.Remove(cursorId);
                    });
                    _addedCursors.Clear();
                    _removedCursors.Clear();
                    _updatedCursors.Clear();
                    break;
            }
        }

        #endregion
    }
}
