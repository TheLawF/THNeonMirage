using System;
using System.Collections.Generic;
using UnityEngine;

namespace THNeonMirage.Util
{
    public class CsUtil
    {
        public static void ForAct(int count, Action action)
        {
            for (var j = 0; j < count; j++) action.Invoke();
        }

        public static void ForAddToList(int count, List<GameObject> listToAdd, Func<int, GameObject> func) 
            => ForAct(count, () => listToAdd.Add(func.Invoke(count)));
    }
}