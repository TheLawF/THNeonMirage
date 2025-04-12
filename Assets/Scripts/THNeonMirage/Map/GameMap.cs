using System;
using System.Collections.Generic;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Manager.UI;
using THNeonMirage.Util;
using UnityEngine;
using Random = System.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class GameMap : MonoBehaviour
    {
        public static ScriptEventHandler<RoundEventArgs> OnRoundEnd;
        public static Action OnRoundStart;
        public static int Activity
        {
            get => _activity;
            set
            {
                if (Activity > GameHost.MaxPlayersPerRoom) Activity = 1;
                OnRoundStart?.Invoke();
                _activity = value;
                // OnRoundEnd?.Invoke(prevPlayer.GetComponent<PlayerManager>(), new RoundEventArgs(prevPlayer, nextPlayer, _activity));
            }
        }

        public GameClient client;
        public GameObject settingsPanel;
        public GameObject tilePrefab;
        public GameObject inGamePanel;
        public GameObject hudPanel;
        
        public static List<GameObject> Players = new ();
        public static List<GameObject> Fields = new ();

        private static int _activity;
        private const float Side = 10;
        private static Vector3 _uUnit = Vector3.right;
        private static Vector3 _vUnit = Vector3.up;
        
        public static Vector3 StartPos = _uUnit * (Side / 2) + _vUnit * (Side / 2);
        public static Vector3 LeftUp = StartPos - _uUnit * Side;
        public static Vector3 LeftDown = StartPos - _uUnit * Side - _vUnit * Side;
        public static Vector3 RightDown = StartPos - _vUnit * Side;

        public static Random Random = new ();
        private static readonly Price CheapPrice = new (8000, 6000, 500, 4000, 10000, 20000);
        private static readonly Price NormalPrice = new (10000, 6000, 500, 4000, 10000, 20000);
        private static readonly Price MediumPrice = new (12000, 6000, 500, 4000, 10000, 20000);
        private static readonly Price ExpensivePrice = new (15000, 6000, 500, 4000, 10000, 20000);
        private static readonly Price UnaffordablePrice = new (18000, 6000, 500, 4000, 10000, 20000);
        private static readonly Price BankruptPrice = new (20000, 6000, 500, 4000, 10000, 20000);

        public static readonly List<FieldProperty> Properties = new()
        {
            new FieldProperty("梦乐园城堡", FieldTile.Type.DreamWorld,     new Price()),  
            new FieldProperty("寺子屋", FieldTile.Type.Village,           CheapPrice),
            new FieldProperty("铃奈庵", FieldTile.Type.Village,           NormalPrice),          
            new FieldProperty("【信仰】地藏像", FieldTile.Type.Other,      new Price()),
            new FieldProperty("稗田邸", FieldTile.Type.Village,           CheapPrice),          
            new FieldProperty("雾雨魔法店", FieldTile.Type.MagicForest,    BankruptPrice),
            new FieldProperty("爱丽丝的家", FieldTile.Type.MagicForest,    UnaffordablePrice),   
            new FieldProperty("村纱的海盗船", FieldTile.Type.DreamWorld,   new Price()),
            new FieldProperty("火焰猫碰碰车", FieldTile.Type.DreamWorld,   new Price(40000, 10000, 1500, 8000, 15000, 40000)),  
            new FieldProperty("人里工会", FieldTile.Type.Other, new Price()),
            
            new FieldProperty("守矢神社", FieldTile.Type.YoukaiMountain,  ExpensivePrice), 
            new FieldProperty("【祈福】风神的祝福", FieldTile.Type.Other,   new Price()),
            new FieldProperty("九天瀑布", FieldTile.Type.YoukaiMountain,  MediumPrice), 
            new FieldProperty("玄武之泽", FieldTile.Type.YoukaiMountain,  MediumPrice),
            new FieldProperty("天狗村落", FieldTile.Type.YoukaiMountain,  NormalPrice), 
            new FieldProperty("妖怪过山车", FieldTile.Type.DreamWorld,    new Price(60000, 6000, 1500, 8000, 15000, 40000)),
            new FieldProperty("感情的摩天轮", FieldTile.Type.DreamWorld,   new Price(60000, 6000, 1500, 8000, 15000, 40000)),  
            new FieldProperty("转转木马", FieldTile.Type.DreamWorld,      new Price(40000, 10000, 1500, 8000, 15000, 40000)),
            new FieldProperty("梦乐园舞台", FieldTile.Type.DreamWorld,    new Price()),   
            new FieldProperty("红魔馆大酒店", FieldTile.Type.Other,       new Price()),
            
            new FieldProperty("云山大摆锤", FieldTile.Type.DreamWorld,    new Price(50000, 8000, 1500, 8000, 15000, 40000)),   
            new FieldProperty("妖怪兽道", FieldTile.Type.Fairies,         CheapPrice),
            new FieldProperty("废弃的电波塔", FieldTile.Type.Fairies,     NormalPrice),    
            new FieldProperty("【信仰】土著神的信仰", FieldTile.Type.Other, new Price()),
            new FieldProperty("三妖精的大树", FieldTile.Type.Fairies,      MediumPrice),    
            new FieldProperty("三途川", FieldTile.Type.Higan,             BankruptPrice),
            new FieldProperty("是非曲直厅", FieldTile.Type.Higan,         UnaffordablePrice),        
            new FieldProperty("白玉楼", FieldTile.Type.Nether,           UnaffordablePrice),
            new FieldProperty("西行妖", FieldTile.Type.Nether,           BankruptPrice),          
            new FieldProperty("八云紫的隙间", FieldTile.Type.Other,       new Price()),
            
            new FieldProperty("妹红小屋", FieldTile.Type.BambooForest,   ExpensivePrice),  
            new FieldProperty("永远亭", FieldTile.Type.BambooForest,     NormalPrice),
            new FieldProperty("蓬莱药局", FieldTile.Type.BambooForest,   BankruptPrice),  
            new FieldProperty("【祈福】龙神像", FieldTile.Type.Other,     new Price()),
            new FieldProperty("幻想风穴", FieldTile.Type.AncientHell,    CheapPrice),   
            new FieldProperty("旧都", FieldTile.Type.AncientHell,       NormalPrice),
            new FieldProperty("地灵殿", FieldTile.Type.AncientHell,     NormalPrice),     
            new FieldProperty("核融合中心", FieldTile.Type.AncientHell, ExpensivePrice),
            new FieldProperty("旧地狱温泉", FieldTile.Type.Other,       new Price()),       
            new FieldProperty("旧地狱水上乐园", FieldTile.Type.Other,    new Price())
        };

        public static readonly Dictionary<Range, Func<int, Vector3>> PosInRange = new()
        {
            {..10, index => StartPos - new Vector3(index % 10, 0)},
            {10..20, index => StartPos - _uUnit * 10 - new Vector3(0, index % 10)},
            {20..30, index => StartPos - _uUnit * 10 - _vUnit * 10 + new Vector3(index % 10, 0)},
            {30..40, index => StartPos - _vUnit * 10 + new Vector3(0, index % 10)}
        };

        public void CreateMap()
        {
            Players = new List<GameObject>();
            Fields = new List<GameObject>();
            // InitField(tilePrefab, 1);

            Utils.ForAddToList(40, Fields, i => InitField(tilePrefab, i));
            Fields.ForEach(o => o.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f));
            inGamePanel.GetComponent<InGamePanelHandler>().client = client;
            
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape)) settingsPanel.SetActive(true);
            if (Activity >= Players.Count) Activity = 0;
        }

        private void ShouldRenderTile(int index, bool shouldRender) => Fields[index].SetActive(shouldRender);

        /// <summary>
        /// 用代码初始化大富翁地块的位置，这些地块将会围成一个正方形，然后根据索引为不同的地块对象实例动态挂载脚本
        /// </summary>
        /// <param name="tp">Tile Prefab：地块预制件</param>
        /// <param name="index">Index：地块的索引</param>
        /// <returns>实例化且挂载了 FieldTile 的地块对象</returns>
        private GameObject InitField(GameObject tp, int index)
        {
            var uOffset = new Vector3(index % Side, 0);
            var vOffset = new Vector3(0, index % Side);

            var list = new List<Func<GameObject>>(new Func<GameObject>[]
            {
                () => Instantiate(tp, StartPos - uOffset, Quaternion.identity, transform),
                () => Instantiate(tp, LeftUp - vOffset, Quaternion.identity, transform),
                () => Instantiate(tp, LeftDown + uOffset, Quaternion.identity, transform),
                () => Instantiate(tp, RightDown + vOffset, Quaternion.identity, transform),
                () => Instantiate(tp, Vector3.zero, Quaternion.identity, transform)
            });

            var instance = Utils.SwitchByMap(list, index);
            var _ = index switch
            {
                0 => WithFieldData<DreamCastle>(instance, index, Properties[index]),
                3 => WithFieldData<BeliefTile>(instance, index, Properties[index]),
                7 => WithFieldData<PirateShip>(instance, index, Properties[index]),
                8 => WithFieldData<BumperCar>(instance, index, Properties[index]),
                
                9 => WithFieldData<VillageTile>(instance, index, Properties[index]),
                11 => WithFieldData<LuckyTile>(instance, index, Properties[index]),
                15 => WithFieldData<RollerCoaster>(instance, index, Properties[index]),
                16 => WithFieldData<FerrisWheel>(instance, index, Properties[index]),
                
                17 => WithFieldData<MerryGoRound>(instance, index, Properties[index]),
                18 => WithFieldData<StageTile>(instance, index, Properties[index]),
                19 => WithFieldData<HotelTile>(instance, index, Properties[index]),
                20 => WithFieldData<BigPendulum>(instance, index, Properties[index]),

                23 => WithFieldData<BeliefTile>(instance, index, Properties[index]),
                29 => WithFieldData<GapTile>(instance, index, Properties[index]),
                34 => WithFieldData<LuckyTile>(instance, index, Properties[index]),
                35 => WithFieldData<BeliefTile>(instance, index, Properties[index]),
                
                38 => WithFieldData<HotSpring>(instance, index, Properties[index]),
                39 => WithFieldData<ExitTile>(instance, index, Properties[index]),
                _ => WithFieldData<FieldTile>(instance, index, Properties[index])
            };
            
            return instance;
        }

        public int WithFieldData<TF>(GameObject go, int id, FieldProperty fieldProperty) where TF: FieldTile
        {
            go.AddComponent<TF>();
            var ft = go.GetComponent<FieldTile>();
            
            ft.level = 0;
            ft.id = id == -1 ? ft.id : id;
            ft.Property = fieldProperty;
            ft.spriteRenderer = tilePrefab.GetComponent<SpriteRenderer>();
            ft.inGamePanel = inGamePanel;
            ft.client = client;
            ft.Init();
            
            return 1;
        }

        public Vector3 Next(GameObject prefab, Vector3 prevPos, float u, float v)
        {
            return prevPos + new Vector3(u, v);
        }
    }
}
