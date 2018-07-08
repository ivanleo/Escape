using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoffeeBean
{

    /// <summary>
    /// 场景加载完毕的委托
    /// </summary>
    public delegate void DelegateSceneLoaded();

    /// <summary>
    /// 场景管理类
    /// </summary>
    public class CSceneManager : CSingletonMono<CSceneManager>
    {
        //异步操作对象
        private AsyncOperation m_AsyncOperator;

        //加载完成回调
        private DelegateSceneLoaded CompleteCallback = null;

        /// <summary>
        /// 立刻切换到目标场景
        /// </summary>
        /// <param name="TargetScene">目标场景</param>
        public void ChangeSceneImmediately( string SceneName )
        {
            try
            {
                CLOG.I( "ready to load scene {0} immediate", SceneName );
                CompleteCallback = null;
                SceneManager.LoadScene( SceneName, LoadSceneMode.Single );
            }
            catch( Exception ex )
            {
                CLOG.E( ( ex.ToString() ) );
            }
        }

        /// <summary>
        /// 显示一个加载场景来加载场景
        /// 等加载完毕后切换到目标场景
        /// </summary>
        /// <param name="TargetScene">目标场景</param>
        /// <param name="LoadingClass">加载界面类</param>
        /// <param name="Callback">加载完毕的回调</param>
        public void ChangeScene( string SceneName, Type LoadingClass, DelegateSceneLoaded Callback = null )
        {
            //记录回调
            CompleteCallback = Callback;

            //加载并显示场景加载UI
            if ( LoadingClass != null )
            {
                MethodInfo func = LoadingClass.GetMethod( "CreateUI" );
                CAssert.AssertIfNull( func );
                func.Invoke( null, null );
            }

            //开始加载
            StartCoroutine( LoadScene( SceneName, LoadingClass == null ) );
        }


        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="SceneName">场景名</param>
        /// <param name="AutoSwitch">是否在加载完成自动切换到目标场景，如果切换场景使用了加载界面，那么一般需要等加载界面完成动画后再来切换，而没有使用加载界面的，可以自动切换场景</param>
        /// <returns></returns>
        IEnumerator LoadScene( string SceneName, bool AutoSwitch )
        {
            CLOG.I( "ready to load scene {0} asyn", SceneName );
            m_AsyncOperator = SceneManager.LoadSceneAsync( SceneName );
            m_AsyncOperator.allowSceneActivation = AutoSwitch;
            yield return m_AsyncOperator;

            if ( CompleteCallback != null && m_AsyncOperator != null )
            {
                yield return new WaitUntil( () => { return m_AsyncOperator.isDone; } );
                CLOG.I( "scene {0} was load complete!", SceneName );
                CompleteCallback();
                m_AsyncOperator = null;
                CompleteCallback = null;
            }
        }


        /// <summary>
        /// 得到当前场景
        /// </summary>
        /// <returns></returns>
        public string GetRunningScene()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>
        /// 得到异步操作对象
        /// 用于获取加载进度
        /// </summary>
        public AsyncOperation AsyncOperator
        {
            get { return m_AsyncOperator; }
        }
    }

}