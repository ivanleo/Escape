using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 数据管理类
    /// </summary>
    public static class CConfig
    {
        private static Dictionary<string, float> m_Config = null;

        /// <summary>
        /// 缓存数据
        /// </summary>
        public static void CacheConfigs()
        {
            if ( m_Config == null )
            {
                m_Config = new Dictionary<string, float>();
            }

            TextAsset conText = Resources.Load<TextAsset> ( "Config" );
            JObject json = JObject.Parse ( conText.text );

            foreach ( JToken child in json.Children() )
            {
                var property1 = child as JProperty;
                m_Config.Add ( property1.Name, property1.Value.ToObject<float>() );
            }
        }

        /// <summary>
        /// 得到缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetValue ( string key )
        {
            if ( m_Config.ContainsKey ( key ) )
            {
                return m_Config[key];
            }

            return int.MinValue;
        }
    }

}