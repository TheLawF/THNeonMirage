using System.Collections.Generic;
using THNeonMirage.Util;

namespace THNeonMirage.Data
{
    public record NeoPlayerData(string UserName, int Position, int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields)
    {
        
    }
}