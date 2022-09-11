namespace RSM
{
    public class TransitionCondition
    {
        public bool isTrue;
        public bool isTrigger = false;
        public bool shouldTrigger;

        public TransitionCondition(bool isTrue, bool isTrigger = false)
        {
            this.isTrue = isTrue;
            this.isTrigger = isTrigger;
        }

        public static implicit operator bool(TransitionCondition condition)
        {
            if (condition.isTrigger && condition.isTrue)
            {
                condition.isTrue = false;
                return true;
            }
            return condition.isTrue;
        }

        public void Trigger()
        {
            if (isTrigger) isTrue = true;
        }

        public void CancelTrigger()
        {
            if (isTrigger) isTrue = false;
        }
    }
}