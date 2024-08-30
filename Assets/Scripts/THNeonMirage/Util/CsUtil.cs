using System;
using System.Collections.Generic;
using UnityEngine;

namespace THNeonMirage.Util
{
    public class CsUtil
    {
        public static void ForAct(int count, Action<int> action)
        {
            for (var index = 0; index < count; index++) action.Invoke(index);
        }

        public static void ForAddToList(int count, List<GameObject> listToAdd, Func<int, GameObject> indexToObjFunc) 
            => ForAct(count, i => listToAdd.Add(indexToObjFunc.Invoke(i)));
        
    }
}