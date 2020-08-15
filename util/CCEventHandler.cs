using System;
using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.util
{
    public class CCEventHandler<T>
    {
        private readonly List<Action<T>> _onEventActions = new List<Action<T>>();
        
        public event Action<T> Events
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _onEventActions.Add(value);
            }
            remove
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                var index = _onEventActions.IndexOf(value);
                if (index != -1)
                    _onEventActions.RemoveAt(index);
            }
        }
        
        // InvokeCallbacksSafe protects both against the callback getting removed while being called
        // and against exceptions being thrown by the callback.

        public  void Invoke(T t)
        {
            foreach (var action in new List<Action<T>>(_onEventActions))
            {
                action(t);
            }
            
        }
    }

}