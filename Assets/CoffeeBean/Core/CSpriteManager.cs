
using UnityEngine;
using UnityEngine.U2D;

namespace CoffeeBean {
    /// <summary>
    /// 精灵管理器
    /// </summary>
    public class CSpriteManager : CSingleton<CSpriteManager> {
        private const string ALTAS_PATH = "SpriteAltas/";
        /// <summary>
        /// 从精灵图集得到精灵图
        /// </summary>
        /// <param name="AltasName"></param>
        /// <param name="SpriteName"></param>
        /// <returns></returns>
        public Sprite GetSpriteFromAltas ( string AltasName, string SpriteName )
        {
            SpriteAtlas SA = Resources.Load<SpriteAtlas> ( ALTAS_PATH + AltasName );
            return SA.GetSprite ( SpriteName );
        }

    }
}

