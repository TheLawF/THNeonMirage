using System;
using System.Collections;
using System.Collections.Generic;

namespace Fictology.Util.Type
{
    [UnionType(typeof(int), typeof(float), typeof(double), typeof(long), typeof(string), typeof(object),
        typeof(List<>), typeof(Hashtable))]
    public class Any: Attribute
    {
        
    }
}