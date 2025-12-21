using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FlyRabbit.EventCenter;
using Photon.Pun;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.Util;

namespace THNeonMirage.Map
{
    public class BeliefTile : FieldTile
    {
        private readonly List<Func<PlayerManager, int, int, string>> Belives = new()
        {
            Saisen, YouseiFines, YouseiTricks, MansionExplosion, WindowsCrush, BookLoss, BuddhistsCharity, WatchDog,
            BuddhistsDaily, RabbitHop
        };
        private void Start()
        {
            Init();
            canPurchase = false;
            description = "抽取一次信仰效果";
            
            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnPositionChanged, RollFromPool);
        }

        public void RollFromPool(PlayerManager player, int prevPos, int currentPos)
        {
            if(!IsTileValid(currentPos)) return;
            var random = new Random();
            description = Belives[random.Next(Belives.Count - 1)].Invoke(player, prevPos, currentPos);
            player.SendPlayerDataUpdate(player.gameObject.GetPhotonView().ViewID, player.playerData);
        }

        private static string Saisen(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 2000);
            return "参拜博丽神社，贡献2000赛钱";
        }
        
        private static string YouseiTricks(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 2000);
            return "妖精来捣乱了！维修设施花了1500";
        }
        
        private static string YouseiFines(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 1500);
            return "抓住了捣乱的妖精，土下座赔偿了1500";
        }
        
        private static string WatchDog(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 3000);
            return "红师傅由于经常睡觉，仅有的月薪3000都没了";
        }
        
        private static string BookLoss(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 3999);
            return "魔理沙偷走了帕秋莉价值3999的魔法书";
        }
        
        private static string WindowsCrush(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 1000);
            return "偷书贼把窗户打破了，维修费用1000";
        }

        private static string TheWorld(PlayerManager player, int prevPos, int currentPos)
        {
            player.playerData.pauseCount += 1;
            return "砸，瓦鲁多！你被咲夜的时停控制1回合！";
        }
        
        private static string RestInMansion(PlayerManager player, int prevPos, int currentPos)
        {
            player.playerData.pauseCount += 3;
            player.SetPosition(31);
            return "满身疮痍，去永琳的医院休息3回合吧";
        }
        
        private static string MansionExplosion(PlayerManager player, int prevPos, int currentPos)
        {
            player.SetBalance(player.playerData.balance - 10000);
            return "红魔馆爆炸，这得花10000才能修复";
        }

        private static string BuddhistsDaily(PlayerManager player, int prevPos, int currentPos)
        {
            player.playerData.pauseCount += 2;
            return "去命莲寺被硬控2回合！";
        }

        private static string BuddhistsCharity(PlayerManager player, int prevPos, int currentPos)
        {
            player.playerData.pauseCount += 2;
            return "去命莲寺被硬控2回合！";
        }
        
        private static string RabbitHop(PlayerManager player, int prevPos, int currentPos)
        {
            
            var random = new Random();
            var steps = random.Next(3, 5);
            player.SetPosition(player.playerData.position + steps);
            return $"兔子跳，前进3~5格，当前前进{steps}格";
        }
        
        // TODO: 拓展功能，Demo中不展示
        private static string FinancialCrisis(PlayerManager player, int prevPos, int currentPos)
        {
            
            player.SetBalance(player.playerData.balance - 1000);
            return "红魔馆财政危机！每回合扣除300，除非在此期间获得收入";
        }
        
        // TODO: 拓展功能，Demo中不展示
        private static string SantaHatItem(PlayerManager player, int prevPos, int currentPos)
        {
            
            player.SetBalance(player.playerData.balance - 1000);
            return "获得123的圣诞帽，对一名玩家使用使其在红魔大酒店休息一回合";
        }
    }
}