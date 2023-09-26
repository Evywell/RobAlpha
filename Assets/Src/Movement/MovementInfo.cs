using System;

namespace UnityClientSources.Movement {
    public class MovementInfo {
        public MovementType Type
        { get; private set; }

        public bool IsJumping
        { get; private set; }

        public MovementInfo(MovementType type, bool isJumping) {
            Type = type;
            IsJumping = isJumping;
        }

        public bool IsMovementInProgress()
        {
            return Type != MovementType.None;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                MovementInfo m = (MovementInfo) obj;

                return (Type == m.Type) && (IsJumping == m.IsJumping);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum MovementType {
        Forward,
        Backward,
        TurnLeft,
        TurnRight,
        None,
    }
}