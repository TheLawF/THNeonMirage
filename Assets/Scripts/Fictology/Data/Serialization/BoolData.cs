using System;
using UnityEngine;

namespace Fictology.Data.Serialization
{
    public class BoolData : ValueTypeData
    {
        private bool Equals(BoolData other)
        {
            return name == other.name && value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoolData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, value);
        }

        public string name;
        public bool value;
        public override ValueTypeData Cast() => this;
        public static BoolData Of(bool data) => new(data);

        public BoolData(bool value) : base(SerializationType.Bool) => this.value = value;
        public BoolData(string name, bool value) : base(SerializationType.Bool)
        {
            this.name = name;
            this.value = value;
        }
        
        public static BoolData operator !(BoolData self) => new (!self.value);
        public static BoolData operator &(BoolData left, BoolData right) => new(left.value & right.value);
        public static BoolData operator |(BoolData left, BoolData right) => new(left.value | right.value);
        public static BoolData operator ^(BoolData left, BoolData right) => new(left.value ^ right.value);
        
        public static BoolData operator ==(BoolData left, BoolData right) => new(left.value == right.value);
        public static BoolData operator !=(BoolData left, BoolData right) => new(left.value != right.value);
        
        public static bool operator false(BoolData self) => !self.value;
        public static bool operator true(BoolData self) => self.value;
        
        public static explicit operator BoolData(IntData intData) => new (intData.value == 0);
        public static explicit operator BoolData(FloatData floatData) => new (floatData.value == 0);
        public static explicit operator BoolData(bool b) => new(b);
    }
}