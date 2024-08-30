using System;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Data;
using THNeonMirage.Util;
using UnityEngine;
using Random = System.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class GameMap : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject fieldPrefab;
        
        public List<GameObject> players;
        public List<GameObject> fieldObjects;

        public static Random Random = new Random();

        private void Start()
        {
            fieldObjects = new List<GameObject>();
            CsUtil.ForAddToList(40, fieldObjects, i => InitField(fieldPrefab, i));
            fieldObjects.ForEach(o => o.GetComponent<SpriteRenderer>().color = new Color(
                (float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble()));
        }

        private void Update()
        {
        }

        /// <summary>
        /// 用代码初始化大富翁地块的位置，这些地块将会围成一个正方形
        /// </summary>
        /// <param name="fp">Field Prefab：地块预制件</param>
        /// <param name="index">Index：地块的索引</param>
        /// <returns></returns>
        private GameObject InitField(GameObject fp, int index)
        {
            var nwPos = Vector3.left + Vector3.up;
            var nePos = Vector3.right + Vector3.up;
            var swPos = Vector3.left + Vector3.down;
            var sePos = Vector3.right + Vector3.down;

            var verticalOffset = - new Vector3(0, index % 10);
            var horizontalOffset = - new Vector3(index % 10, 0);

            Debug.Log(index);
            var instance = index switch
            {
                >= 0 and < 5 => Instantiate(fp, nwPos + verticalOffset, Quaternion.identity),
                >= 5 and < 10 => Instantiate(fp, Vector3.left + verticalOffset, Quaternion.identity),
                >= 10 and < 15 => Instantiate(fp, swPos + horizontalOffset, Quaternion.identity),
                >= 15 and < 20 => Instantiate(fp, Vector3.down + horizontalOffset, Quaternion.identity),
                >= 20 and < 25 => Instantiate(fp, nePos + horizontalOffset, Quaternion.identity),
                >= 25 and < 30 => Instantiate(fp, Vector3.right + horizontalOffset, Quaternion.identity),
                >= 30 and < 35 => Instantiate(fp, sePos + horizontalOffset, Quaternion.identity),
                >= 35 and < 40 => Instantiate(fp, Vector3.up + horizontalOffset, Quaternion.identity),
                
                _ => Instantiate(fp, Vector3.zero, Quaternion.identity),
            };
            
            WithComponent(instance, index);
            return instance;
        }

        /// <summary>
        /// 根据索引为不同的地块对象实例动态挂载脚本
        /// </summary>
        /// <param name="go">Game Object：地块对象的实例</param>
        /// <param name="index">Index：地块的索引</param>
        /// <returns></returns>
        public GameObject WithComponent(GameObject go, int index)
        {
            FieldTile fieldTile = index switch
            {
                0 => new StartTile(0, "月虹金融大厦", FieldTile.Type.Official),
                10 => new VillageTile(10, "人里工会", FieldTile.Type.Official),
                20 => new HotelTile(20, "红魔大酒店", FieldTile.Type.Official),
                >= 4 and <= 6 or >= 14 and <= 16 or >= 24 and <= 26 or >= 34 and <= 36 => new BazaarTile(
                    index, "默认摊位", FieldTile.Type.Bazaar),
                _ => new BlankTile(index, "空白地块", FieldTile.Type.Official)
            };
            
            go.AddComponent(fieldTile.GetType());
            return go;
        }

        public int GetPlayerCountOn(int fieldId)
            => players.Count(player => player.GetComponent<Player>().Position == fieldId);

        public static event Action<GameObject, GameObject> OnUserLogin;
        public static void CreateAvatar(GameObject playerPrefab, GameObject mapObj)
        {
            mapObj.GetComponent<GameMap>().players.Add(Instantiate(playerPrefab));
            OnUserLogin?.Invoke(playerPrefab, mapObj);
        }
    }
}
