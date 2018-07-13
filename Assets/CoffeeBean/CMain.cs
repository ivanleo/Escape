using System.Threading;

namespace CoffeeBean {
    /// <summary>
    /// 框架主入口
    /// </summary>
    public static class CMain
    {
        private static bool HasInit = false;

        /// <summary>
        /// 初始化框架
        /// </summary>
        public static void Init()
        {
            if ( HasInit )
            {
                return;
            }

            //激活LOG系统
            CLOG.Instance.Start();

            //开启自动垃圾回收
            CUtilAutoGC.Instance.Begin();

#if UNITY_EDITOR
            //启动帧率
            CFPS.Instance.Begin();
#endif
            //缓存配置
            CConfig.CacheConfigs();

            //缓存预制体
            CPrefabManager.Instance.CacheAllPrefab();

            HasInit = true;
        }
    }
}
