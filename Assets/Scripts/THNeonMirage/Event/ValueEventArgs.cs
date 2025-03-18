namespace THNeonMirage.Event
{
    public class ValueEventArgs : IGameEventArgs
    {
        private object _value;
        public object Value => _value;
        public ValueEventArgs(object value) =>_value = value;
        public static ValueEventArgs Of(object type) => new ValueEventArgs(type);
    }
}