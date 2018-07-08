/********************************************************************
    created:    2017/10/30
    created:    30:10:2017   9:59
    filename:   D:\Work\NanJingMaJiang\trunk\Project\NanJing\Assets\Code\Common\DataManager\VPrefabManager.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\NanJing\Assets\Code\Common\DataManager
    file base:  VPrefabManager
    file ext:   cs
    author:     Leo

    purpose:    预制体管理类
*********************************************************************/

using System;
using System.Collections.Generic;

using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 预制体管理类
    /// </summary>
    public class CPrefabManager : CSingleton<CPrefabManager>
    {
        /// <summary>
        /// 预制体缓存
        /// </summary>
        private Dictionary<string, GameObject> m_PrefabCache = new Dictionary<string, GameObject>();

        //世界地图路径
        private const string PREFAB_PATH = "Prefab";

        /// <summary>
        /// 预加载所有需要用到的预制体
        /// </summary>
        /// <returns>加载成功与否</returns>
        public bool CacheAllPrefab()
        {
            try
            {
                //读取所有预制体
                UnityEngine.Object[] AllPrefabs = Resources.LoadAll ( PREFAB_PATH );

                for ( int i = 0 ; i < AllPrefabs.Length ; i++ )
                {
#if UNITY_EDITOR

                    if ( m_PrefabCache.ContainsKey ( AllPrefabs[i].name ) )
                    {
                        CLOG.E ( "the key {0} has already add to prefab cache", AllPrefabs[i].name );
                        return false;
                    }

#endif
                    m_PrefabCache.Add ( AllPrefabs[i].name, AllPrefabs[i] as GameObject );
                }

                return true;
            }
            catch ( Exception ex )
            {
                CLOG.E ( ex.ToString() );
                return false;
            }
        }

        /// <summary>
        /// 缓存一个预制体
        /// </summary>
        /// <param name="name"></param>
        public bool CachePrefab ( string name )
        {
            //读取所有预制体
            UnityEngine.Object Prefab = Resources.Load ( name );

#if UNITY_EDITOR

            if ( m_PrefabCache.ContainsKey ( Prefab.name ) )
            {
                CLOG.E ( "the key {0} has already add to prefab cache", Prefab.name );
                return false;
            }

#endif
            m_PrefabCache.Add ( Prefab.name, Prefab as GameObject );
            return true;
        }

        /// <summary>
        /// 得到缓存的预制体
        /// </summary>
        /// <param name="Name">预制体名字</param>
        /// <returns>得到的预制体</returns>
        public GameObject GetPrefab ( string Name )
        {
            if ( m_PrefabCache.ContainsKey ( Name ) )
            {
                return m_PrefabCache[Name];
            }

            return null;
        }

        /// <summary>
        /// 创建预制体实例
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GameObject CreatePrefabInstance ( string Name )
        {
            return GameObject.Instantiate ( GetPrefab ( Name ) );
        }

    }



}