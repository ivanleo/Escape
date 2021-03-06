﻿
using UnityEngine;
using System.Collections;

namespace CoffeeBean
{
    /// <summary>
    /// HTTP位置
    /// </summary>
    public enum EHttpLocation
    {
        /// <summary>
        /// 本地
        /// </summary>
        [CEnumDesc( Desc = "本地" )]
        LOCAL,
        /// <summary>
        /// 远程
        /// </summary>
        [CEnumDesc( Desc = "远程" )]
        REMOTE,

        UNKNOWN
    }

    /// <summary>
    /// HTTP请求完成时的回调
    /// 内涵加载的数据
    /// </summary>
    public delegate void DelegateHttpLoadComplete( WWW HttpObject, bool isSuccess );

    /// <summary>
    /// HTTP请求时的回调
    /// 内涵加载进度
    /// </summary>
    public delegate void DelegateHttpLoading( WWW HttpObject );

    /// <summary>
    /// URL Image加载完毕回调
    /// </summary>
    public delegate void DelegateURLImageLoadComplete( bool isSuccess );

    /// <summary>
    /// HTTP任务类
    /// 特点：主线程异步下载，不阻塞主线程
    /// <code>
    ///  <para>CHttp.Instance.CreateHttpLoader( "http://zhongwei-info.com/apk/MoneyFruit1.2_165_sign.apk", Loaded, Loading );</para>
    ///  <para>CHttp.Instance.CreateHttpLoader( "http://zhongwei-info.com/apk/wuhh1.4.apk ", Loaded, Loading );</para>
    /// </code>
    /// </summary>

    public partial class CHttp : CSingletonMono<CHttp>
    {
        [SerializeField]
        private int m_NowRunningLoaderCount = 0;

        //private Dictionary<string, Coroutine> MissionList = new Dictionary<string, Coroutine>();

        /// <summary>
        /// 创建一个HTTP下载任务
        /// </summary>
        /// <param name="URL"> URL </param>
        /// <param name="CompleteCallBack"> 完成回调 </param>
        /// <param name="LoadingCallBack"> 下载中回调 </param>
        /// <param name="EHT"> URL位置是本地还是网络 </param>
        public void CreateHttpLoader( string URL, DelegateHttpLoadComplete CompleteCallBack = null, DelegateHttpLoading LoadingCallBack = null, EHttpLocation EHT = EHttpLocation.REMOTE )
        {
            StartCoroutine( StartLoad( URL, CompleteCallBack, LoadingCallBack, EHT ) );
        }

        /// <summary>
        /// 开始读取
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="CompleteCallBack"></param>
        /// <param name="LoadingCallBack"></param>
        /// <param name="EHT"> URL位置是本地还是网络 </param>
        /// <returns>协程</returns>
        private IEnumerator StartLoad( string URL, DelegateHttpLoadComplete CompleteCallBack = null, DelegateHttpLoading LoadingCallBack = null, EHttpLocation EHT = EHttpLocation.REMOTE )
        {
            if( EHT == EHttpLocation.LOCAL )
            {
                URL = "file:///" + Application.persistentDataPath + "/" + URL;
                CLOG.I( "read local file uri={0}", URL );
            }
            else
            {
                CLOG.I( "read remote file url={0}", URL );
            }

            WWW HttpObj = new WWW( URL );
            m_NowRunningLoaderCount++;

            yield return StartCoroutine( Loading( HttpObj, CompleteCallBack, LoadingCallBack ) );

            m_NowRunningLoaderCount--;
            HttpObj.Dispose();
        }

        /// <summary>
        /// 读取中
        /// </summary>
        /// <returns></returns>
        public IEnumerator Loading( WWW HttpObj, DelegateHttpLoadComplete CompleteCallBack = null, DelegateHttpLoading LoadingCallBack = null )
        {
            while( !HttpObj.isDone )
            {
                if( LoadingCallBack != null )
                {
                    LoadingCallBack( HttpObj );
                }
                yield return 1;
            }

            if( HttpObj.error != null )
            {
                // CLOG.W( "Http Loading Error: URL:{0} Error:{1}", HttpObj.url, HttpObj.error );
                CompleteCallBack( HttpObj, false );
            }
            else
            {
                CompleteCallBack( HttpObj, true );
            }
        }
    }
}
