using System;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Manager;
using THNeonMirage.Util;
using Unity.Netcode;
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
        private ObjectPool<GameObject> pool;
        public static readonly Dictionary<int, string> TileDict = new()
        {
            {0, "失乐园入口"}, {10, "梦乐园"}, {11, "感情的摩天轮"}, {12, "旋转木马"}, {13, "碰碰车"},
            {20, "红魔大酒店"}, {28, "失乐园出口"}, {30, "水上乐园"}, {31, "水滑梯"}, {32, "泳池"}
        };

        public static readonly Dictionary<Range, Func<int, Vector3>> PosInRange = new()
        {
            {..10, index => startPos - new Vector3(index % 10, 0)},
            {10..20, index => startPos - uUnit * 10 - new Vector3(0, index % 10)},
            {20..30, index => startPos - uUnit * 10 - vUnit * 10 + new Vector3(index % 10, 0)},
            {30..40, index => startPos - vUnit * 10 + new Vector3(0, index % 10)}
        };
        public static readonly List<Predicate<int>> IndexGroup = new (new Predicate<int>[]
        {
            x => x > 0 & x < 10,
            x => x > 10 & x < 20,
            x => x > 20 & x < 30,
            x => x > 30 & x < 40,
            _ => true
        });

        public static readonly List<Predicate<int>> TypeIndex = new (new Predicate<int>[]
        {
            x => x == 0,
            x => x == 10,
            x => x == 20,
            x => x >= 4 & x < 7 | x >= 14 & x < 17 | x >= 24 & x < 27 | x >= 34 & x < 37
        });

        private void Start()
        {
            fieldObjects = new List<GameObject>();
            InitField(tilePrefab, 1);

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
            
            var uOffset = new Vector3(index % 10, 0);
            var vOffset = new Vector3(0, index % 10);

            var list = new List<Func<GameObject>>(new Func<GameObject>[]
            {
                () => Instantiate(tp, startPos - uOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - uUnit * 10 - vOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - uUnit * 10 - vUnit * 10 + uOffset, Quaternion.identity),
                () => Instantiate(tp, startPos - vUnit * 10 + vOffset, Quaternion.identity),
                () => Instantiate(tp, Vector3.zero, Quaternion.identity)
            });
            
            var instance = Utils.SwitchByMap(list, index);
            
           // Debug.Log($"编号{index}的地块位于: {instance.transform.position.ToString()}");
           var _ = index switch
            {
                0 => WithFieldType<StartTile>(instance, index, "月虹金融中心", FieldTile.Type.Official),
                10 => WithFieldType<VillageTile>(instance, index, "人里工会", FieldTile.Type.Official),
                20 => WithFieldType<HotelTile>(instance, index, "红魔酒店", FieldTile.Type.Official),
                >= 4 and <= 6 or >= 14 and <= 16 or >= 24 and <= 26 or >= 34 and <= 36 => 
                    WithFieldType<BazaarTile>(instance, index, "默认摊位", FieldTile.Type.Bazaar),
                _ => WithFieldType<BlankTile>(instance, index, "空白地区", FieldTile.Type.Custom)
            };
            
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
        
        public int GetPlayerCountOn(int fieldId)
            => players.Count(player => player.GetComponent<PlayerManager>().Position == fieldId);

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
