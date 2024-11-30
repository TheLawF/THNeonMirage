using System;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Data;
using THNeonMirage.Manager;
using THNeonMirage.Util;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = System.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class GameMap : NetworkBehaviour
    {
        public GameObject settingsPanel;
        public GameObject tilePrefab;
        
        public List<PlayerManager> players;
        public List<GameObject> fieldObjects;

        private static Vector3 uUnit = Vector3.right;
        private static Vector3 vUnit = Vector3.up;
        private static Vector3 startPos = Vector3.right * 5 + Vector3.up * 5;

        public static Random Random = new ();
        public static bool FirstUse = true;
        
        public const string Castle = "梦乐园城堡";
        public const string Ship = "村纱的海盗船";
        public const string Wheel = "感情的摩天轮";
        private const string Bazaar = "摊位";

        public static readonly List<FieldData> Fields = new()
        {
            new FieldData("梦乐园城堡",null, 0, 0, 0, -10000, typeof(StartTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData("村纱的海盗船", null,50000, 60000, 80000, -12000, typeof(PirateShip)), 
            new FieldData("感情的摩天轮", null,60000, 80000,  100000, -20000, typeof(FerrisWheel)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
            new FieldData(Bazaar, null,0, 0, 0, 0, typeof(BazaarTile)),
        };

        public static readonly Dictionary<Range, Func<int, Vector3>> PosInRange = new()
        {
            {..10, index => startPos - new Vector3(index % 10, 0)},
            {10..20, index => startPos - uUnit * 10 - new Vector3(0, index % 10)},
            {20..30, index => startPos - uUnit * 10 - vUnit * 10 + new Vector3(index % 10, 0)},
            {30..40, index => startPos - vUnit * 10 + new Vector3(0, index % 10)}
        };

        public static readonly Dictionary<int, Action> TileActions = new()
        {
            { 0, () => {} }, { 9, ToggleHandler.DisplayPanel }
        };

        private void Start()
        {
            fieldObjects = new List<GameObject>();
            // InitField(tilePrefab, 1);

            Utils.ForAddToList(40, fieldObjects, i => InitField(tilePrefab, i));
            fieldObjects.ForEach(o => o.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f));
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                settingsPanel.SetActive(true);
            }
        }

        private void ShouldRenderTile(int index, bool shouldRender) => fieldObjects[index].SetActive(shouldRender);

        /// <summary>
        /// 用代码初始化大富翁地块的位置，这些地块将会围成一个正方形，然后根据索引为不同的地块对象实例动态挂载脚本
        /// </summary>
        /// <param name="tp">Tile Prefab：地块预制件</param>
        /// <param name="index">Index：地块的索引</param>
        /// <returns>实例化且挂载了 FieldTile 的地块对象</returns>
        private GameObject InitField(GameObject tp, int index)
        {
            const int side = 10;
            var uOffset = new Vector3(index % side, 0);
            var vOffset = new Vector3(0, index % side);

            var list = new List<Func<GameObject>>(new Func<GameObject>[]
            {
                () => Instantiate(tp, startPos - uOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - uUnit * side - vOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - uUnit * side - vUnit * side + uOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - vUnit * side + vOffset, Quaternion.identity),
                () => Instantiate(tp, Vector3.zero, Quaternion.identity)
            });
            
            var instance = Utils.SwitchByMap(list, index);
            // var _ = index switch
            // {
            //     0 => WithFieldData<StartTile>(instance, index, Fields[index]),
            //     10 => WithFieldData<FerrisWheel>(instance, index, Fields[index]),
            //     20 => WithFieldData<PirateShip>(instance, index, Fields[index]),
            //     >= 4 and <= 6 or >= 14 and <= 16 or >= 24 and <= 26 or >= 34 and <= 36 => 
            //         WithFieldData<BlankTile>(instance, index, Fields[index]),
            //     _ => WithFieldData<BazaarTile>(instance, index, Fields[index])
            // };
            var _ = index switch
            {
                0 => WithFieldType<StartTile>(instance, index, "月虹金融中心", FieldTile.Type.Official),
                10 => WithFieldType<VillageTile>(instance, index, "人里工会", FieldTile.Type.Official),
                20 => WithFieldType<HotelTile>(instance, index, "红魔酒店", FieldTile.Type.Official),
                >= 4 and <= 6 or >= 14 and <= 16 or >= 24 and <= 26 or >= 34 and <= 36 => 
                    WithFieldType<BazaarTile>(instance, index, "默认摊位", FieldTile.Type.Bazaar),
                _ => WithFieldType<BlankTile>(instance, index, "空白地区", FieldTile.Type.Other)
            };
            // WithFieldData(instance, index, Fields[index], Fields[index].FieldType);
            return instance;
        }

        public int WithFieldType<TFt>(GameObject go, int id, string fieldName, FieldTile.Type fieldType)
            where TFt: FieldTile
        {
            go.AddComponent<TFt>();

            var ft = go.GetComponent<FieldTile>();
            ft.id = id == -1 ? ft.id : id;
            ft.fieldName = fieldName;
            ft.fieldType = fieldType;
            
            return 1;
        }
        
        public int WithFieldData<TF>(GameObject go, int id, FieldData fieldData) where TF: FieldTile
        {
            go.AddComponent<TF>();
            var ft = go.GetComponent<FieldTile>();
            
            ft.id = id == -1 ? ft.id : id;
            ft.price1 = fieldData.FirstBid;
            ft.price2 = fieldData.SecondBid;
            ft.price3 = fieldData.ThirdBid;

            ft.tolls = fieldData.Tolls;
            ft.fieldName = fieldData.Name;
            ft.fieldType = FieldTile.Type.Other;
            
            return 1;
        }
        
        public int GetPlayerCountOn(int fieldId)
            => players.Count(player => player.GetComponent<PlayerManager>().PlayerData.Position == fieldId);

        public int StartServer() => 
            NetworkManager.Singleton.StartServer() ? Utils.Info("服务器启动成功") : Utils.Error("服务器启动失败");

        public int StartClient() =>
            NetworkManager.Singleton.StartClient() ? Utils.Info("客户端启动成功") : Utils.Error("客户端启动失败");

        public int StartHost()
        {
            return NetworkManager.Singleton.StartHost() ? Utils.Info("主机启动成功") : Utils.Error("主机启动失败");
        }
        public void ShutDown() => NetworkManager.Singleton.Shutdown();
    }
}
