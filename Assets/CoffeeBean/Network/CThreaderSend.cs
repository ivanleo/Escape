/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:52
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CThreaderSend.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CThreaderSend
    file ext:   cs
    author:     Leo

    purpose:    发线程
*********************************************************************/
using System.Collections.Generic;
using System.Threading;

namespace CoffeeBean
{
    /// <summary>
    /// 发线程
    /// </summary>
    public class CThreadSender
    {
        /// <summary>
        /// 网络单例引用
        /// </summary>
        private CNetwork m_NetworkRef;

        // 发线程
        private Thread m_SendThread = null;
        // 该线程是否启动
        private bool m_IsRunning = false;
        // 锁
        private object m_Locker = new object();

        // 待发消息队列
        private Queue<CMessage> m_MessageToSendQueue = new Queue<CMessage>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="networkInstance">网络单例引用</param>
        public CThreadSender( CNetwork networkInstance )
        {
            m_NetworkRef = networkInstance;
        }

        /// <summary>
        /// 开始，启动线程，开始轮询发操作
        /// </summary>
        public void Start()
        {
            if( m_IsRunning )
            {
                return;
            }

            m_IsRunning = true;

            m_SendThread = new Thread( Process );
            m_SendThread.IsBackground = true;
            m_SendThread.Start();
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
        /// 停止发消息线程运行，并释放资源
        /// </summary>
        public void Stop()
        {
            lock( m_Locker )
            {
                m_IsRunning = false;
                m_MessageToSendQueue.Clear();
            }
        }

        /// <summary>
        /// 处理待发消息
        /// </summary>
        private void Process()
        {
            while( m_IsRunning )
            {
                if( m_NetworkRef.IsConnected )
                {
                    CMessage msg = null;
                    // 只要还有包没发出去，就持续发
                    while( ( msg = PopMessageToSend() ) != null )
                    {
                        int errorCode = m_NetworkRef.SocketInstance.SendMessage( msg );
                        if( errorCode < 0 )
                        {
                            // 发送异常，直接断线
                            m_NetworkRef.DisConnect();
                        }
                    }
                }

                Thread.Sleep( 1 );
            }
        }

        /// <summary>
        /// 获取队列中待发的消息
        /// </summary>
        /// <returns>待发消息</returns>
        private CMessage PopMessageToSend()
        {
            CMessage message = null;

            lock( m_Locker )
            {
                if( m_MessageToSendQueue.Count == 0 )
                {
                    return null;
                }

                message = m_MessageToSendQueue.Dequeue();
            }

            return message;
        }

        /// <summary>
        /// 压入一个待发消息准备发到服务器
        /// </summary>
        /// <param name="message">待发消息</param>
        public void PushMessageToSend( CMessage message )
        {
            lock( m_Locker )
            {
                m_MessageToSendQueue.Enqueue( message );
            }
        }

        /// <summary>
        /// 清除所有待发消息
        /// </summary>
        public void ClearMessage()
        {
            lock( m_Locker )
            {
                m_MessageToSendQueue.Clear();
            }
        }
    }
}
