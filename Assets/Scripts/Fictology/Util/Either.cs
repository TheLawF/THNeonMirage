using System;
using Fictology.Data.Serialization;

namespace Fictology.Util
{
    public class Either<F,S> where F : ISynchronizable where S : ISynchronizable
    {
        private F _first;
        private S _second;
        public ISynchronizable Current;

        public Either(F first, S second)
        {
            _first = first;
            _second = second;
            Current = first;
        }
        
        public static Either<F, S> Or(F defaultValue, S another)
        {
            return new Either<F, S>(defaultValue, another);
        }

        public void SwitchToAnother()
        {
            Current = Current switch
            {
                F => _second,
                S => _first,
                _ => Current
            };
        }
    }
}