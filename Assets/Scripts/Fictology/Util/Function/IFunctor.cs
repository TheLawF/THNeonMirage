using System;

namespace Fictology.Util.Function
{
    public interface IFunctor<TFunctor>
    {
        IFunctor<TResult> Get<TResult>(Func<TFunctor, TResult> getter);
    }
}