using System;
using System.Collections.Generic;
using THNeonMirage.Data;

namespace THNeonMirage.Event
{
    public class RandomEvents
    {
        public static readonly List<Func<PlayerData, string>> BeliefEvents = new()
        {
            HakureiShrine, DrinkWineInTemple
        };
        public static readonly List<Func<PlayerData, string>> LuckyEvents = new()
        {
            WatchOutThief, KnowledgeIsPower
        };

        #region Belief Events

        private static string HakureiShrine(PlayerData data)
        {
            data.balance -= 500;
            return "前往博丽神社参拜，被巫女讹了500月虹币";
        }
        
        private static string DrinkWineInTemple(PlayerData data)
        {
            data.balance -= 1000;
            return "在命莲寺喝酒，罚款1000月虹币";
        }
        
        private static string AnHonestBusiness(PlayerData data)
        {
            data.balance -= 980;
            return "被疫病神怂恿着买了一些用玻璃假冒的宝石制品，失去980月虹币";
        }
        
        private static string PovertyGoddess(PlayerData data)
        {
            data.balance -= 999;
            return "被穷神上身，失去999月虹币";
        }
        
        private static string GambleKing(PlayerData data)
        {
            data.balance -= 770;
            return "驹草山如的赌场上只有庄家才能笑到最后，你失去了770月虹币";
        }
        
        private static string AfraidForYoukai(PlayerData data)
        {
            data.pauseCount++;
            return "由于担心妖怪出没而不敢出门，停一回合";
        }

        private static string SutraLecture(PlayerData data)
        {
            data.balance += 300;
            return "向信众们讲经说法，收获了300功德";
        }

        #endregion

        #region Lucky Events

        private static string WatchOutThief(PlayerData data)
        {
            data.balance -= 860;
            return "小心黑白老鼠！被偷走了860月虹币！";
        }
        
        private static string KnowledgeIsPower(PlayerData data)
        {
            data.balance += 370;
            return "红魔图书馆里的书教会了你一点经商技巧，小赚370月虹币";
        }

        #endregion
    }
}