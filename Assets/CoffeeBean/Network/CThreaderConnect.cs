/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:52
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CThreaderConnect.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CThreaderConnect
    file ext:   cs
    author:     Leo

    purpose:    连接线程
*********************************************************************/
using CoffeeBean;
using System.Collections.Generic;
using System.Threading;

namespace CoffeeBean
{

    /// <summary>
    /// 连接信号枚举
    /// </summary>
    public enum EConnectSignal
    {
        [CEnumDesc( Desc = "连接成功" )]
        CONNECT_SUESSFUL = 0,
        [CEnumDesc( Desc = "连接失败" )]
        CONNECT_FAILED,
        [CEnumDesc( Desc = "连接丢失" )]
        CONNECT_LOST,

        NULL
    }

    /// <summary>
    /// 连接线程
    /// </summary>
    public class CThreadConnecter
    {
        /// <summary>
        /// 网络单例引用
        /// </summary>
        private CNetwork m_NetworkRef;

        // 连接线程
        private Thread m_ConnectThread = null;

        // 该线程是否启动
        private bool m_IsRunning = false;

        // 锁
        private object m_Locker = new object();

        // 消息队列
        private Queue<EConnectSignal> m_SingalQueue = new Queue<EConnectSignal>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="networkInstance">网络单例引用</param>
        public CThreadConnecter( CNetwork networkInstance )
        {
            m_NetworkRef = networkInstance;
        }

        /// <summary>
        /// 开始，启动连接线程
        /// </summary>
        public void Start()
        {
            if( m_IsRunning )
            {
                return;
            }

            m_IsRunning = true;

            m_ConnectThread = new Thread( Process );
            m_ConnectThread.IsBackground = true;
            m_ConnectThread.Start();
        }

        /// <summary>
        /// 返回本连接线程是否处于工作状态
        /// </summary>
        /// <returns></returns>
        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        /// <summary>
        /// 停止连接信号检测
        /// </summary>
        public void Stop()
        {
            lock( m_Locker )
            {
                m_IsRunning = false;
                m_SingalQueue.Clear();
            }
        }
        /// <summary>
        /// 让连接线程尝试连接
        /// </summary>
        public void TryConnect()
        {
            lock( m_Locker )
            {
                Monitor.Pulse( m_Locker );
            }
        }

        /// <summary>
        /// 线程轮询
        /// </summary>
        private void Process()
        {
            while( m_IsRunning )
            {
                Wait();
                // 网络没有连接上
                if( !m_NetworkRef.IsConnected )
                {
                    // 尝试连接
                    if( ProcessConnect() )
                    {
                        PushConnectSignal( EConnectSignal.CONNECT_SUESSFUL );
                    }
                    else
                    {
                        PushConnectSignal( EConnectSignal.CONNECT_FAILED );
                    }
                }
            }
        }

        /// <summary>
        /// 线程挂起，等待唤醒
        /// </summary>
        private void Wait()
        {
            lock( m_Locker )
            {
                Monitor.Wait( m_Locker );
            }
        }

        /// <summary>
        /// 进行Socket连接
        /// </summary>
        /// <returns>返回连接成功与否</returns>
        private bool ProcessConnect()
        {
            bool ret = m_NetworkRef.SocketInstance.Connect( m_NetworkRef.IP, m_NetworkRef.Port );
            return ret;
        }

        /// <summary>
        /// 压入一个连接信号到队列
        /// 方便主线程update来获取信号
        /// </summary>
        /// <param name="ECS"></param>
        public void PushConnectSignal( EConnectSignal ECS )
        {
            lock( m_Locker )
            {
                m_SingalQueue.Enqueue( ECS );
            }
        }

        /// <summary>
        /// 从信号队列中获取连接信号
        /// </summary>
        /// <returns></returns>
        public EConnectSignal PopConnectSingal()
        {
            lock( m_Locker )
            {
                // 没有信号直接返回
                if( m_SingalQueue.Count == 0 )
                {
                    return EConnectSignal.NULL;
                }

                return m_SingalQueue.Dequeue();
            }
        }

    }
}
