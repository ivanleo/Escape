/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:51
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CPackageManager.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CPackageManager
    file ext:   cs
    author:     Leo

    purpose:    消息包回调管理类
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoffeeBean
{
    public delegate void DelegatePackageHandler( CPackageBase package );

    /// <summary>
    /// 网络包管理器
    /// 主要管理网络包ID到类名的映射字典
    /// 启动流程
    ///     1、程序启动
    ///     2、从本dll中获取特定的类型信息
    ///     3、通过反射记录相关信息到字典
    ///
    /// 收包操作流程
    ///     1、收到网络包
    ///     2、解析网络包
    ///     3、得到ActionID
    ///     4、到本类来查找完整类名
    ///     5、反射创建类的实例
    ///     6、将实例交给注册的处理事件
    /// </summary>
    public class CPackageManager : CSingleton<CPackageManager>
    {
        /// <summary>
        /// ActionID - Class String 字典
        /// </summary>
        private Dictionary<int, Type> m_ClassDic = new Dictionary<int, Type>();

        /// <summary>
        /// 类型与处理委托
        /// </summary>
        private Dictionary<Type, DelegatePackageHandler> m_PackageHandlerDic = new Dictionary<Type, DelegatePackageHandler>();

        /// <summary>
        /// 准备所有的网络包反射信息
        /// </summary>
        public void PerpareNetworkPackageReflectionInformation()
        {
            // 获取当前运行程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            //得到所有类型
            Type[] types = assembly.GetTypes();

            //网络包基类
            Type PackageBase = typeof( CPackageBase );

            // 遍历所有类型
            for( int i = 0 ; i < types.Length ; i++ )
            {
                if( types[i].IsSubclassOf( PackageBase ) )
                {
                    // 遍历特性
                    foreach ( var attr in types[i].GetCustomAttributes( false ) )
                    {
                        // 符合特性的才注册
                        if ( attr.GetType() == typeof( CAttrPackage ) )
                        {
                            // 注册ID到类型
                            RegisterActionIDToClass( ( attr as CAttrPackage ).OpCode, types[i] );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册网络消息处理包
        /// </summary>
        /// <param name="clsType">类型</param>
        /// <param name="Handler">处理函数</param>
        public void AddPackageHandler( Type clsType, DelegatePackageHandler Handler )
        {
            if( !m_PackageHandlerDic.ContainsKey( clsType ) )
            {
                m_PackageHandlerDic[clsType] = Handler;
            }
            else
            {
                m_PackageHandlerDic[clsType] += Handler;
            }
        }

        /// <summary>
        /// 根据注册类型得到处理委托
        /// </summary>
        /// <param name="clsType">类型</param>
        /// <returns></returns>
        public DelegatePackageHandler GetPackageHandler( Type clsType )
        {
            if( !m_PackageHandlerDic.ContainsKey( clsType ) )
            {
                return null;
            }

            return m_PackageHandlerDic[clsType];
        }

        /// <summary>
        /// 移除一个消息包的指定委托
        /// </summary>
        /// <param name="clsType">消息包类型</param>
        /// <param name="Handler">指定委托</param>
        public void RemovePackageHandler( Type clsType, DelegatePackageHandler Handler )
        {
            if( !m_PackageHandlerDic.ContainsKey( clsType ) )
            {
                return;
            }

            m_PackageHandlerDic[clsType] -= Handler;
        }

        /// <summary>
        /// 移除一个消息包的所有委托
        /// </summary>
        /// <param name="clsType">消息包类型</param>
        public void RemoveAllPackageHandler( Type clsType )
        {
            if( !m_PackageHandlerDic.ContainsKey( clsType ) )
            {
                return;
            }

            Delegate[] delegates = m_PackageHandlerDic[clsType].GetInvocationList();

            for( int i = 0; i < delegates.Length; i++ )
            {
                m_PackageHandlerDic[clsType] -= delegates[i] as DelegatePackageHandler;
            }
        }


        /// <summary>
        /// 注册ActionID到类名
        /// </summary>
        /// <param name="OpCode">ActionID</param>
        /// <param name="ClassName">类名</param>
        private void RegisterActionIDToClass( int OpCode, Type ClassName )
        {
            m_ClassDic.Add( OpCode, ClassName );
        }

        /// <summary>
        /// 通过ActionID查找类的完整名字
        /// 方便反射创建
        /// </summary>
        /// <param name="ActionID">网络包ActionID</param>
        /// <returns></returns>
        public Type ReflectionClassNameByActionID( int ActionID )
        {
            if( !m_ClassDic.ContainsKey( ActionID ) )
            {
                return null;
            }
            return m_ClassDic[ActionID];
        }
    }
}
