using UnityEngine;

namespace Scripts.Touches
{
    public struct TouchData
    {
        public int TouchId;
        public Vector3 MousePosition;
        public Vector3 MoveDistance;
    }

    public interface ITouchHandler
    {
        public const float DOUBLE_CKICK_TIME = 0.5f;

        bool TouchBegin(TouchData[] touches);

        void TouchMove(TouchData[] touches);

        void TouchEnd(TouchData[] touches);
        bool OnWheel(float delta, Vector3 mouseScreenPosition);
    }
}


