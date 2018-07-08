using System;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 数据管理类
    /// </summary>
    public static class CData
    {
        public static string UserID { get; set; }

        /// <summary>
        /// 获得前缀，有的游戏需要根据用户来存储
        /// </summary>
        private static string GetKey ( string Key )
        {
            return UserID == "" ? Key : UserID + "_" + Key;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveInt ( string Key, int value )
        {
            PlayerPrefs.SetInt ( GetKey ( Key ), value );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveFloat ( string Key, float value )
        {
            PlayerPrefs.SetFloat ( GetKey ( Key ), value );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveString ( string Key, string value )
        {
            PlayerPrefs.SetString ( GetKey ( Key ), value );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveBool ( string Key, bool value )
        {
            PlayerPrefs.SetInt ( GetKey ( Key ), value ? 1 : 0 );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static int LoadInt ( string Key, int Default = 0 )
        {
            return PlayerPrefs.GetInt ( GetKey ( Key ), Default );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static float LoadFloat ( string Key, float Default = 0f )
        {
            return PlayerPrefs.GetFloat ( GetKey ( Key ), Default );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static string LoadString ( string Key, string Default = "" )
        {
            return PlayerPrefs.GetString ( GetKey ( Key ), Default );
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static bool LoadBool ( string Key, bool Default = false )
        {
            return PlayerPrefs.GetInt ( GetKey ( Key ), Default ? 1 : 0 ) == 1 ? true : false;
        }

    }
}
