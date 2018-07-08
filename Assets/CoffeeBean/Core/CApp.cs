using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 应用程序状态
    /// </summary>
    public delegate void DelegateAppState();

    /// <summary>
    /// 应用程序类
    /// </summary>
    public class CApp : CSingletonMono<CApp>
    {
        /// <summary>
        /// 切换到前台事件
        /// </summary>
        public event DelegateAppState EventAppSwitchIn = null;

        /// <summary>
        /// 切换到后台事件
        /// </summary>
        public event DelegateAppState EventAppSwitchOut = null;

        /// <summary>
        /// 退出应用程序事件
        /// </summary>
        public event DelegateAppState EventAppQuit = null;

        /// <summary>
        /// 应用程序是否暂停
        /// </summary>
        public bool IsApplecationPause { get; set; }

        /// <summary>
        /// 应用程序是否获得焦点
        /// </summary>
        public bool IsApplecationFocus { get; set; }

        /// <summary>
        /// 应用退出
        /// </summary>
        private void OnApplicationQuit()
        {
            CLOG.I ( "Application Quit" );

            if ( EventAppQuit != null )
            {
                EventAppQuit();
            }
        }

        /// <summary>
        /// 应用程序暂停状态变化
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause ( bool pause )
        {
            CLOG.I ( "Application Pause state {0}", pause );
            IsApplecationPause = pause;

            if ( pause )
            {
                CheckSwitchOut();
            }
            else
            {
                CheckSwitchIn();
            }
        }

        /// <summary>
        /// 应用程序焦点变化
        /// </summary>
        /// <param name="focus"></param>
        private void OnApplicationFocus ( bool focus )
        {
            CLOG.I ( "Application Focus state {0}", focus );
            IsApplecationFocus = focus;

            if ( focus )
            {
                CheckSwitchIn();
            }
            else
            {
                CheckSwitchOut();
            }
        }

        /// <summary>
        /// 检查是否切换到前台
        /// </summary>
        private void CheckSwitchIn()
        {
            if ( IsApplecationFocus == true && IsApplecationPause == false )
            {
                if ( EventAppSwitchIn != null )
                {
                    EventAppSwitchIn();
                }
            }
        }

        /// <summary>
        /// 检查是否切换到前台
        /// </summary>
        private void CheckSwitchOut()
        {
            if ( IsApplecationFocus == false && IsApplecationPause == true )
            {
                if ( EventAppSwitchOut != null )
                {
                    EventAppSwitchOut();
                }
            }
        }
    }

}