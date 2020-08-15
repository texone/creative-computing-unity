using System;
using System.Collections.Generic;
using System.Linq;
using cc.creativecomputing.util;
using UnityEngine;

namespace cc.creativecomputing.input
{
    public class CCHand : CCCorrelatedEntry
    {
        public readonly List<Touch> Fingers = new List<Touch>();
            
        public CCHand(int theId, bool theIsCorrelated, Vector2 thePosition) : base(theId, theIsCorrelated, thePosition)
        {
        }

        public CCHand() : base(-1, false, new Vector2())
        {
        }
    }
    
    public class CCTouchInput : UnityEngine.MonoBehaviour
    {
        public CCEventHandler<Touch> OnFingerDown = new CCEventHandler<Touch>();
        public CCEventHandler<Touch> OnFingerMove = new CCEventHandler<Touch>();
        public CCEventHandler<Touch> OnFingerUp = new CCEventHandler<Touch>();
        
        

        public float maxHandDistance = 10;
        public float maxCorrelationDistance = 10;
        
        private CCCorrelateEntries<CCHand> _myCorrelatedEntries = new CCCorrelateEntries<CCHand>();
        
        public CCEventHandler<CCHand> OnHandDown => _myCorrelatedEntries.OnNewEntry;
        public CCEventHandler<CCHand> OnHandMove => _myCorrelatedEntries.OnMoveEntry;
        public CCEventHandler<CCHand> OnHandUp => _myCorrelatedEntries.OnLostEntry;

        public static List<Touch> Touches
        {
         get
         {
             var result = Input.touches.ToList();
             if (!Input.GetMouseButton(0) || Input.touches.Length > 0) return result;
             var touch = new Touch
             {
                 fingerId = 0, 
                 position = Input.mousePosition
             };
             result.Add(touch);

             return result;
         }   
        }
        
        private List<CCHand> DetectHands()
        {
            // a list of all touch points present on screen.
            var fingers = Touches;
            
            //a list of detected hands on the screen and the associated fingers.
            var hands = new List<CCHand>();
            
            while(fingers.Count > 0){
                var f1 = fingers[0];
                fingers.RemoveAt(0);

                

                var hand = new CCHand();
                hands.Add(hand);
                hand.Fingers.Add(f1);
                
                foreach (var f2 in new List<Touch>(fingers))
                {
                    if (hand.Fingers.Any(fH =>
                    {
                        Debug.Log(Vector2.Distance(fH.position, f2.position));
                        return Vector2.Distance(fH.position, f2.position) < maxHandDistance;
                    }))
                    {
                        hand.Fingers.Add(f2);
                        fingers.Remove(f2);
                    }

                    if (hand.Fingers.Count >= 5){
                        break;
                    }
                }
            }
            
            hands.ForEach(hand => {  hand.Position = hand.Fingers.Aggregate(new Vector2(), (current, finger) => current + finger.position) * 1f / hand.Fingers.Count;});

            return hands;
        }
        
        private static Vector3 ScreenToModel(Vector3 theInput)
        {
            return  new Vector3((theInput.x/ 7560 -0.5f) * 47.95f,(theInput.y / 1920 - 0.5f) * 12.18f,0);
        }

        private void Start()
        {
            //OnHandDown.Events += hand => Debug.Log("Hand down " + hand.id);
            //OnHandMove.Events += hand => Debug.Log("Hand move " + hand.id);
            //OnHandUp.Events += hand => Debug.Log("Hand up " + hand.id);
        }

        public void Update()
        {
            

            var hands = DetectHands();
            _myCorrelatedEntries.Update(hands, maxCorrelationDistance);
        }
        
        public bool drawDebug = true;
        
        public Color fingerColor = Color.red;
        
        public float handRadius = 1f;
        public Color handColor = Color.blue;

        
        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            if (!drawDebug) return;
            
            var g = new CCGraphics();
            g.BeginDraw();
            
            
            var fingerRadius = maxHandDistance / 2;
            
            
            foreach (var touch in Touches)
            {
                g.Color(fingerColor);
                g.Ellipse(touch.position,fingerRadius,fingerRadius, true);
                g.Text(touch.fingerId,touch.position.x - fingerRadius,touch.position.y + fingerRadius);
            }

            foreach (var hand in _myCorrelatedEntries.Entries())
            {
                g.Color(handColor);
                g.Ellipse(hand.Position,handRadius,handRadius, true);
                g.Text(hand.id,hand.Position.x - handRadius,hand.Position.y + handRadius);
            }
            g.EndDraw();
        }

        public List<CCHand> Hands => new List<CCHand>(_myCorrelatedEntries.Entries());
    }
}