using System;
using System.Collections;
using System.Collections.Generic;

namespace Fictology.Util.Type
{
    public interface IUnionType<TUnion> where TUnion : Union { }
    public class UnionType: Attribute
    {
        public readonly System.Type[] ValidTypes;
        public UnionType(params System.Type[] validTypes)
        {
            ValidTypes = validTypes;
        }
    }
    
    public abstract class Union
    {
        public abstract TResult Match<TResult>(params Func<object, TResult>[] matchers);
        public abstract System.Type GetUnionType();
        public abstract object Value { get; }
    }

    public abstract class Union<TUnion, T1, T2> : Union, IUnionType<TUnion>
        where TUnion : Union<TUnion, T1, T2>
    {
        protected abstract object InnerValue { get; }
    
        public override object Value => InnerValue;
    
        public static Union<TUnion, T1, T2> Create(T1 value) => new Case1(value);
        public static Union<TUnion, T1, T2> Create(T2 value) => new Case2(value);
    
        public abstract TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2);
    
        public override TResult Match<TResult>(params Func<object, TResult>[] matchers)
        {
            if (matchers.Length != 2)
                throw new ArgumentException("需要2个匹配函数");
        
            return Match(
                value => matchers[0](value),
                value => matchers[1](value)
            );
        }
    
        public override System.Type GetUnionType() => typeof(TUnion);
    
        // 嵌套类型表示联合的各个分支
        private sealed class Case1 : Union<TUnion, T1, T2>
        {
            private readonly T1 _value;
        
            public Case1(T1 value) => _value = value;
            protected override object InnerValue => _value!;
        
            public override TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2) 
                => case1(_value);
        }
    
        private sealed class Case2 : Union<TUnion, T1, T2>
        {
            private readonly T2 _value;
        
            public Case2(T2 value) => _value = value;
            protected override object InnerValue => _value!;
        
            public override TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2) 
                => case2(_value);
        }
    }
    
    public abstract class Union<TUnion, T1, T2, T3> : Union, IUnionType<TUnion>
    where TUnion : Union<TUnion, T1, T2, T3>
{
    protected abstract object InnerValue { get; }
    
    public override object Value => InnerValue;
    
    public static Union<TUnion, T1, T2, T3> Create(T1 value) => new Case1(value);
    public static Union<TUnion, T1, T2, T3> Create(T2 value) => new Case2(value);
    public static Union<TUnion, T1, T2, T3> Create(T3 value) => new Case3(value);
    
    public abstract TResult Match<TResult>(
        Func<T1, TResult> case1, 
        Func<T2, TResult> case2, 
        Func<T3, TResult> case3);
    
    public override TResult Match<TResult>(params Func<object, TResult>[] matchers)
    {
        if (matchers.Length != 3)
            throw new ArgumentException("需要3个匹配函数");
        
        return Match(
            value => matchers[0](value),
            value => matchers[1](value),
            value => matchers[2](value)
        );
    }
    
    public override System.Type GetUnionType() => typeof(TUnion);
    
    private sealed class Case1 : Union<TUnion, T1, T2, T3>
    {
        private readonly T1 _value;
        public Case1(T1 value) => _value = value;
        protected override object InnerValue => _value!;
        
        public override TResult Match<TResult>(
            Func<T1, TResult> case1, 
            Func<T2, TResult> case2, 
            Func<T3, TResult> case3) => case1(_value);
    }
    
    private sealed class Case2 : Union<TUnion, T1, T2, T3>
    {
        private readonly T2 _value;
        public Case2(T2 value) => _value = value;
        protected override object InnerValue => _value!;
        
        public override TResult Match<TResult>(
            Func<T1, TResult> case1, 
            Func<T2, TResult> case2, 
            Func<T3, TResult> case3) => case2(_value);
    }
    
    private sealed class Case3 : Union<TUnion, T1, T2, T3>
    {
        private readonly T3 _value;
        public Case3(T3 value) => _value = value;
        protected override object InnerValue => _value!;
        
        public override TResult Match<TResult>(
            Func<T1, TResult> case1, 
            Func<T2, TResult> case2, 
            Func<T3, TResult> case3) => case3(_value);
    }
}

// 4. 四个类型的可区分联合
    public abstract class Union<TUnion, T1, T2, T3, T4> : Union, IUnionType<TUnion>
        where TUnion : Union<TUnion, T1, T2, T3, T4>
    {
        protected abstract object InnerValue { get; }

        public override object Value => InnerValue;

        public static Union<TUnion, T1, T2, T3, T4> Create(T1 value) => new Case1(value);
        public static Union<TUnion, T1, T2, T3, T4> Create(T2 value) => new Case2(value);
        public static Union<TUnion, T1, T2, T3, T4> Create(T3 value) => new Case3(value);
        public static Union<TUnion, T1, T2, T3, T4> Create(T4 value) => new Case4(value);

        public abstract TResult Match<TResult>(
            Func<T1, TResult> case1,
            Func<T2, TResult> case2,
            Func<T3, TResult> case3,
            Func<T4, TResult> case4);

        public override TResult Match<TResult>(params Func<object, TResult>[] matchers)
        {
            if (matchers.Length != 4)
                throw new ArgumentException("需要4个匹配函数");

            return Match(
                value => matchers[0](value),
                value => matchers[1](value),
                value => matchers[2](value),
                value => matchers[3](value)
            );
        }

        public override System.Type GetUnionType() => typeof(TUnion);

        private sealed class Case1 : Union<TUnion, T1, T2, T3, T4>
        {
            private readonly T1 _value;
            public Case1(T1 value) => _value = value;
            protected override object InnerValue => _value!;

            public override TResult Match<TResult>(
                Func<T1, TResult> case1,
                Func<T2, TResult> case2,
                Func<T3, TResult> case3,
                Func<T4, TResult> case4) => case1(_value);
        }

        private sealed class Case2 : Union<TUnion, T1, T2, T3, T4>
        {
            private readonly T2 _value;
            public Case2(T2 value) => _value = value;
            protected override object InnerValue => _value!;

            public override TResult Match<TResult>(
                Func<T1, TResult> case1,
                Func<T2, TResult> case2,
                Func<T3, TResult> case3,
                Func<T4, TResult> case4) => case2(_value);
        }

        private sealed class Case3 : Union<TUnion, T1, T2, T3, T4>
        {
            private readonly T3 _value;
            public Case3(T3 value) => _value = value;
            protected override object InnerValue => _value!;

            public override TResult Match<TResult>(
                Func<T1, TResult> case1,
                Func<T2, TResult> case2,
                Func<T3, TResult> case3,
                Func<T4, TResult> case4) => case3(_value);
        }

        private sealed class Case4 : Union<TUnion, T1, T2, T3, T4>
        {
            private readonly T4 _value;
            public Case4(T4 value) => _value = value;
            protected override object InnerValue => _value!;

            public override TResult Match<TResult>(
                Func<T1, TResult> case1,
                Func<T2, TResult> case2,
                Func<T3, TResult> case3,
                Func<T4, TResult> case4) => case4(_value);
        }

    }
}