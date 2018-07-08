﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBean
{
    public static class CTime
    {
        /// <summary>
        /// 时间戳转时间字符串
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="formatString">时间格式串
        /// 常见的有 yyyy/MM/dd HH:mm:ss:ffff
        ///          yyyy/MM/dd
        ///          HH:mm:ss
        /// </param>
        /// <returns></returns>
        public static string TimeStampToTimeString( int timeStamp, string formatString )
        {
            //开始时间
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime( 1970, 1, 1 ) ); // 当地时区
            DateTime dt = startTime.AddSeconds( timeStamp );
            return dt.ToString( formatString );
        }

        /// <summary>
        /// 得到当前时间戳
        /// </summary>
        /// <param name="needseconds">为真获取秒时间戳，为假获取毫秒时间戳</param>
        /// <returns></returns>
        public static long GetNowTimeStamp( bool needseconds = true )
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
            long ret;
            if ( needseconds )
            {
                ret = Convert.ToInt64( ts.TotalSeconds );
            }
            else
            {
                ret = Convert.ToInt64( ts.TotalMilliseconds );
            }
            return ret;
        }
    }
}