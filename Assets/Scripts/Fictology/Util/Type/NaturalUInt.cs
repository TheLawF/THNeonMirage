using System;
using Fictology.Data.Record;
using Fictology.Util.Type;

namespace Fictology.Util.Type
{
    public delegate Func<int, int> Natural<T>(Func<int, int> func);

    public class NaturalUInt
    {
        public static readonly Natural<int> Zero = _ => nat => nat;
        public static readonly Natural<int> One = suc => suc;
        public static readonly Natural<int> Two = suc => nat => suc(suc(nat));
        public static readonly Func<Natural<int>, Natural<int>> Suc = nat => func => i => func(nat(func)(i));
        
        public static readonly Func<Natural<int>, Natural<int>, Natural<int>> Add =
            (left, right) => func => nat => left(func)(right(func)(nat));
        
        public static readonly Func<Natural<int>, Natural<int>, Natural<int>> Mul =
            (left, right) => func => left(right(func));

        public static Natural<int> AsNatural(int integer)
        {
            if (integer < 0) throw new ArgumentException("整数必须非负。");
            var result = Zero;
            for (var i = 0; i < integer; i++)
            {
                result = Suc(result);
            }

            return result;
        }
        
        public static Natural<int> AsNatural(uint integer)
        {
            var result = Zero;
            for (var i = 0; i < integer; i++)
            {
                result = Suc(result);
            }
            return result;
        }

        public static int ToInteger(Natural<int> natural)
        {
            // 将Church numeral转换为整数
            // 对0应用n次+1操作
            return natural(x => x + 1)(0);
        }
    }
}