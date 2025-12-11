using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Fictology.Registry;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class Level : RegistryEntry
    {
        public int PlayerRound
        {
            get => _playerRound;
            set => _playerRound = value > players.Count - 1 ? 0 : value;
        }

        public ObsoleteGameClient client;
        public GameObject settingsPanel;
        public GameObject tilePrefab;
        public GameObject inGamePanel;

        public ObservableList<GameObject> PlayerInstances = new ();
        public List<GameObject> fields = new ();
        public List<PlayerManager> players = new ();

        private Transform m_transform;
        private const float SideLength = 100;
        private int _playerRound;
        private static Vector3 _uUnit = Vector3.right;
        private static Vector3 _vUnit = Vector3.up;
        
        public static Vector3 StartPos = _uUnit * (SideLength / 2) + _vUnit * (SideLength / 2);
        public static Vector3 LeftUp = StartPos - _uUnit * SideLength * 10;
        public static Vector3 LeftDown = StartPos - _uUnit * SideLength * 10 - _vUnit * SideLength * 10;
        public static Vector3 RightDown = StartPos - _vUnit * SideLength * 10;

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
            {..10, index => StartPos - new Vector3(SideLength * (index % 10), 0, 1)},
            {10..20, index => StartPos - _uUnit * 10 * SideLength - new Vector3(0,SideLength * (index % 10), 1)},
            {20..30, index => StartPos - _uUnit * 10 * SideLength - _vUnit * 10 * SideLength + new Vector3(SideLength * (index % 10), 0, 1)},
            {30..40, index => StartPos - _vUnit * 10 * SideLength + new Vector3(0, SideLength * (index % 10), 1)}
        };
        
        private void Start()
        {
            m_transform = GetComponent<Transform>();
            PlayerRound = 1;
        }

        public void CreateLevel()
        {
            Utils.ForAddToList(40, fields, i => InitField(tilePrefab, i));
            fields.ForEach(o => o.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f));
            fields.ForEach(o =>
            {
                var t = o.GetComponent<Transform>();
                var scale = o.GetComponent<Transform>().localScale;
                scale.x = SideLength;
                scale.y = SideLength;
                t.localScale = scale;
            });
            
            var hud = Registries.GetObject(UIRegistry.HUD);
            hud.SetActive(true);
            LoadFieldTexture();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape)) settingsPanel.SetActive(true);
        }

        private void StartCountdown(MonoBehaviour monoBehaviour, ValueEventArgs args)
        {
            if(monoBehaviour is not PlayerManager playerManager) return;
            if(args.Value is not PlayerData playerData) return;
            playerManager.playerData = playerData;
        }

        public GameObject GetTileObject(int index) => fields[index];
        public FieldTile GetTileComponent(int index) => fields[index].GetComponent<FieldTile>();

        private void ShouldRenderTile(int index, bool shouldRender) => fields[index].SetActive(shouldRender);

        /// <summary>
        /// 用代码初始化大富翁地块的位置，这些地块将会围成一个正方形，然后根据索引为不同的地块对象实例动态挂载脚本
        /// </summary>
        /// <param name="tp">Tile Prefab：地块预制件</param>
        /// <param name="index">Index：地块的索引</param>
        /// <returns>实例化且挂载了 FieldTile 的地块对象</returns>
        private GameObject InitField(GameObject tp, int index)
        {
            var uOffset = new Vector3((index % 10) * SideLength, 0);
            var vOffset = new Vector3(0, (index % 10) * SideLength);

            var list = new List<Func<GameObject>>(new Func<GameObject>[]
            {
                () => LevelRegistry.TilePrefab.Instantiate(StartPos - uOffset, Quaternion.identity, m_transform),
                () => LevelRegistry.TilePrefab.Instantiate(LeftUp - vOffset, Quaternion.identity, m_transform),
                () => LevelRegistry.TilePrefab.Instantiate(LeftDown + uOffset, Quaternion.identity, m_transform),
                () => LevelRegistry.TilePrefab.Instantiate(RightDown + vOffset, Quaternion.identity, m_transform),
                () => LevelRegistry.TilePrefab.Instantiate(Vector3.zero, Quaternion.identity, m_transform)
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

        public void LoadFieldTexture()
        {
            Utils.ForAct(fields.Count, index =>
            {
                var spriteRenderer = fields[index].GetComponent<SpriteRenderer>();
                var sprite = index switch
                {
                    0 => Resources.Load<Sprite>("Textures/Mansion"),
                    3 => Resources.Load<Sprite>("Textures/Torii"),
                    7 => Resources.Load<Sprite>("Textures/Mansion"),
                    8 => Resources.Load<Sprite>("Textures/Jeweler_House"),
                    
                    9 => Resources.Load<Sprite>("Textures/Village_House"),
                    11 => Resources.Load<Sprite>("Textures/Shrine"),
                    15 => Resources.Load<Sprite>("Textures/Mansion"),
                    16 => Resources.Load<Sprite>("Textures/Village_House"),
                    
                    17 => Resources.Load<Sprite>("Textures/Jeweler_House"),
                    18 => Resources.Load<Sprite>("Textures/Mansion"),
                    19 => Resources.Load<Sprite>("Textures/Mansion"),
                    20 => Resources.Load<Sprite>("Textures/Jeweler_House"),
                    
                    23 => Resources.Load<Sprite>("Textures/Torii"),
                    29 => Resources.Load<Sprite>("Textures/Jeweler_House"),
                    34 => Resources.Load<Sprite>("Textures/Mansion"),
                    35 => Resources.Load<Sprite>("Textures/Shrine"),
                    
                    38 => Resources.Load<Sprite>("Textures/Hotspring_Inn"),
                    39 => Resources.Load<Sprite>("Textures/Mansion"),
                    _ => Resources.Load<Sprite>("Textures/Mystia_Izakaya"),
                };

                var tileTransform = tilePrefab.GetComponent<Transform>();
                var tileSize = tilePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;
                var size = sprite.bounds.size;

                spriteRenderer.sprite = sprite;
                var sx = tileSize.x / size.x;
                var sy = tileSize.y / size.y;
                var prevScale = tileTransform.localScale;
                tileTransform.localScale = new Vector3(prevScale.x * sx, prevScale.y * sy, 1);
            });
        }

        public int WithFieldData<TF>(GameObject go, int id, FieldProperty fieldProperty) where TF: FieldTile
        {
            go.AddComponent<TF>();
            var ft = go.GetComponent<FieldTile>();
            
            ft.level = 0;
            ft.index = id == -1 ? ft.index : id;
            ft.Property = fieldProperty;
            
            ft.spriteRenderer = go.GetComponent<SpriteRenderer>();
            ft.spriteRenderer.color = ft.backGroundColor;
            ft.inGamePanel = inGamePanel;
            ft.client = client;
            
            return 1;
        }

        public TTile GetTile<TTile>(int index) where TTile : FieldTile
        {
            return fields[index].GetComponent<TTile>();
        }

        public void NextTurn()
        {
            PlayerRound++;
            if (PhotonNetwork.IsConnectedAndReady)
            {
                return;
            }
            var currentRoundPlayer = players[PlayerRound];
            if (currentRoundPlayer.playerData.isBot)
            {
                StartCoroutine(currentRoundPlayer.ExecuteAITask());
            }

            if (currentRoundPlayer.playerData.pauseCount > 0)
                currentRoundPlayer.playerData.pauseCount--;

            if (currentRoundPlayer.playerData.pauseCount < 0)
                currentRoundPlayer.playerData.pauseCount = 0;
            
            // if(photonView.IsMine)
            // {
            //     WaitForOtherPlayer();
            // }
        }
    }
}
