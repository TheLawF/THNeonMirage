using UnityEngine;
using UnityEngine.Events;

namespace THNeonMirage.Event
{
    public class ValueListener<T>: MonoBehaviour
    {
        public UnityEvent<T, T> onValueChanged;
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                var oldValue = _value;
                _value = value;
                onValueChanged.Invoke(oldValue, _value);
            }
        }
    }
}