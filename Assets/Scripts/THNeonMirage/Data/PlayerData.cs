using System.Collections.Generic;
using JetBrains.Annotations;
using THNeonMirage.Util;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    public record PlayerData(string UserName, int Position);
}