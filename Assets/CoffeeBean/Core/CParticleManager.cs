using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 特效管理器
    /// </summary>
    public static class CParticleManager
    {
        /// <summary>
        /// 播放一个特效在指定位置
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="pos"></param>
        /// <param name="CostTime"></param>
        /// <returns></returns>
        private static GameObject PlayEffect ( string Name, Vector3 pos, float CostTime = 1f )
        {
            GameObject Par = CPrefabManager.Instance.CreatePrefabInstance ( Name );
            Par.transform.position = pos;
            GameObject.Destroy ( Par, CostTime );
            return Par;
        }

        public static void PlaySmoke ( Vector3 Pos )
        {
            PlayEffect ( "Smoke", Pos, 1f );
        }

        public static void PlayHit ( Vector3 Pos )
        {
            PlayEffect ( "Hit", Pos, 0.5f );
        }

        public static void PlayRelife ( Vector3 Pos )
        {
            PlayEffect ( "ReLife", Pos, 4f );
        }

        public static GameObject PlayNeedHelp ( Vector3 Pos )
        {
            return PlayEffect ( "NeedHelp", Pos, 15f );
        }
    }
}
