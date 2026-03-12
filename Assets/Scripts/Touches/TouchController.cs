using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Touches
{
    public class TouchController : MonoBehaviour
    {
        private readonly List<ITouchHandler> _activeHandlers = new ();
        private readonly SortedDictionary<int, List<ITouchHandler>> _touchHandlers = new ();
        private readonly List<TouchData> _allTouches = new (10);
        private Vector3 _lastMousePosition = Vector3.one;

        public event Action<float> OnWheel;

        
        public bool IsBlocked { get; set; } = false;
        public void AddHandler(ITouchHandler handler, int priority = 125)
        {
           if (handler == null)
           {
              return;
           }
            if (_touchHandlers.TryGetValue(priority, out var handlers))
            {
                handlers.Add(handler);
            }
            else
            {
                _touchHandlers.Add(priority, new List<ITouchHandler> { handler });
            }
        }

        public void RemoveHandler(ITouchHandler handler)
        {
            foreach(var handlers in _touchHandlers.Values)
            {
                handlers.RemoveAll(it => it == handler);
            }
        }
        

        private void Update()
        {
            if (Pointer.current.press.wasPressedThisFrame)
            {
                TouchBegin(AllTouches());
            }
            else if (Pointer.current.press.wasReleasedThisFrame)
            {
                TouchEnd();
            }
            else if (Pointer.current.press.isPressed)
            {
                TouchMove();
            }

            else 
            {
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    float scrollY = mouse.scroll.ReadValue().y;
                    if (Mathf.Abs(scrollY) > float.Epsilon)
                    {
                        OnMouseWheel(scrollY, mouse.position.ReadValue());
                    }
                }
            }

        }
        
       private void TouchBegin(TouchData[] touches)
       {
          if (IsBlocked)
          {
             return;
          }

          _activeHandlers.Clear();
          if (touches == null || touches.Length == 0)
          {
             return;
          }


         _lastMousePosition = touches[0].MousePosition;

          foreach (var it in _touchHandlers.Values)
          {
                foreach (var handler in it)
                {
                    if (handler.TouchBegin(touches))
                    {
                        _activeHandlers.Add(handler);
                        return;
                    }
                }
          }
       }

       private void TouchMove()
       {
           var allTouches = AllTouches();
           foreach (var it in _activeHandlers)
           {
              it.TouchMove(allTouches);
           }
       }

      private void TouchEnd()
      {
         var touches = _allTouches.ToArray();
         _activeHandlers.ForEach(it => it.TouchEnd(touches));
         _activeHandlers.Clear();
      }

      private TouchData[] AllTouches()
      {
         _allTouches.Clear();

         if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
         {
            foreach (var touch in Touchscreen.current.touches.Where(it => it.isInProgress))
            {
                    _allTouches.Add(new TouchData
                    {
                        TouchId = touch.touchId.ReadValue(),
                        MousePosition = touch.position.ReadValue(),
                        MoveDistance = touch.delta.ReadValue()
                    });
            }
         }
         else
         {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            _allTouches.Add(new TouchData {
                                        TouchId = 1
                                        , MousePosition = currentPos
                                        , MoveDistance = (Vector3)currentPos - _lastMousePosition
                                     });
            _lastMousePosition = currentPos;
         }

         return _allTouches.ToArray();
      }


         private void OnMouseWheel(float delta, Vector3 mouseScreenPosition)
         {
            // Делим на 120, так как в новой системе значения прокрутки обычно кратны 120
            float normalizedDelta = delta / 120f; 
            OnWheel?.Invoke(delta);
            foreach (var it in _touchHandlers.Values)
            {
                foreach (var handler in it)
                {
                    if (handler.OnWheel(normalizedDelta, mouseScreenPosition))
                    {
                        return;
                    }
                }
            }
         }
    }
}