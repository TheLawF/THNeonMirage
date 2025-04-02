using System;
using System.Collections.Generic;

namespace THNeonMirage.Util
{
    public class ActionTree: TreeNode<Action<List<object>>>
    {
        public ActionTree(string name, Action<List<object>> value) : base(name, value)
        {
        }
    }
}