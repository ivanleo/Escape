#pragma warning disable 1591

namespace CoffeeBean
{
    /// <summary>
    /// 自定义消息事件枚举，在这里注册消息类型
    /// 所有的自定义消息通过一个枚举来区分
    /// </summary>
    public enum ECustomMessageType
    {
        /// <summary>
        /// 游戏开始
        /// </summary>
        [CEnumDesc( Desc = "游戏开始" )]
        MISSION_START,

        /// <summary>
        /// 静音
        /// </summary>
        [CEnumDesc( Desc = "静音" )]
        MUTE,

        /// <summary>
        /// 取消静音
        /// </summary>
        [CEnumDesc( Desc = "取消静音" )]
        MUTE_CANCEL,


        NULL
    }

    /// <summary>
    /// 显示状态枚举
    /// </summary>
    public enum EVisiableType
    {
        [CEnumDesc( Desc = "隐藏状态" )]
        HIDE,
        [CEnumDesc( Desc = "显示状态" )]
        SHOW
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum EDeviceType
    {
        [CEnumDesc( Desc = "IOS环境" )]
        IOS = 1,
        [CEnumDesc( Desc = "Andriod环境" )]
        ANDRIOD = 2,
        [CEnumDesc( Desc = "浏览器环境" )]
        BROWSER = 3,
        [CEnumDesc( Desc = "Unity开发环境" )]
        DEBUG = 4,
        [CEnumDesc( Desc = "Win32环境" )]
        WIN32 = 5,

        NULL
    }

    /// <summary>
    /// 2方向
    /// </summary>
    public enum EDirection_2
    {
        [CEnumDesc( Desc = "向左" )]
        LEFT,
        [CEnumDesc( Desc = "向右" )]
        RIGHT,

        NULL
    }

    /// <summary>
    /// 4方向
    /// </summary>
    public enum EDirection_4
    {
        [CEnumDesc( Desc = "向上" )]
        UP,
        [CEnumDesc( Desc = "向下" )]
        DOWN,
        [CEnumDesc( Desc = "向左" )]
        LEFT,
        [CEnumDesc( Desc = "向右" )]
        RIGHT,

        NULL
    }

    /// <summary>
    /// 弹出框按钮样式
    /// </summary>
    public enum EPopupButtonType
    {
        [CEnumDesc( Desc = "仅有OK按钮" )]
        OK_ONLY,
        [CEnumDesc( Desc = "OK按钮和Cencel按钮都有" )]
        OK_CANCEL,

        UNKNOWN
    }

    /// <summary>
    /// 比率类型
    /// </summary>
    public enum EPrecentType
    {
        [CEnumDesc( Desc = "百分比" )]
        PRECENT_100 = 100,
        [CEnumDesc( Desc = "千分比" )]
        PRECENT_1000 = 1000,
        [CEnumDesc( Desc = "万分比" )]
        PRECENT_10000 = 10000
    }


    /**********************************************/
    /*         下面是本游戏的自定义枚举           */
    /**********************************************/
    #region CustomEnum


    #endregion
}