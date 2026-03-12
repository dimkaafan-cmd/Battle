using System;
using UnityEngine;

namespace Scripts.Touches
{
    public class BattleTouchHandler : ITouchHandler
    {

        public event Action<Vector3, Vector3> OnTouchMoveEvent;
        public event Action<Vector3> OnTouchStartEvent;
        public event Action<float> OnTrabslateByZEvent;


        public bool TouchBegin(TouchData[] touches)
        {
            if (touches.Length == 1)
            {
                OnTouchStartEvent?.Invoke(touches[0].MousePosition);
            }
            return true;
        }

        public void TouchMove(TouchData[] touches)
        {
            if (touches.Length == 1)
            {
               OnTouchMoveEvent?.Invoke(touches[0].MousePosition, touches[0].MoveDistance);
            }
        }

        public void TouchEnd(TouchData[] touches)
        {

        }

        public bool OnWheel(float delta, Vector3 mouseScreenPosition)
        {
            OnTrabslateByZEvent?.Invoke(delta);
            return true;
        }
    }
}
