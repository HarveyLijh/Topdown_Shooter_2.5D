namespace RSM
{
    public class StateMachineEvents
    {
        public delegate void VoidEvent();
        public delegate void GenericEvent<T>(T input);
        public delegate void StateChangeEvent(State from, State to);
    }
}