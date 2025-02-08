namespace THNeonMirage.Event
{
    public class ValueEventArgs : IGameEventArgs
    {
        private object _value;
        public object Value
        {
            get => _value;
        }
        public ValueEventArgs(object value) =>_value = value;
        public IGameEventArgs Of(object type) => new ValueEventArgs(type);
    }
}