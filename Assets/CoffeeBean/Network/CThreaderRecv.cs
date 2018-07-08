
/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:52
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CThreaderRecv.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CThreaderRecv
    file ext:   cs
    author:     Leo

    purpose:    收线程
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace CoffeeBean {
    /// <summary>
    /// 收线程
    /// </summary>
    public class CThreadReceiver {
        /// <summary>
        /// 网络单例引用
        /// </summary>
        private CNetwork m_NetworkRef;

        // 发线程
        private Thread m_RecvThread = null;

        // 该线程是否启动
        private bool m_IsRunning = false;

        // 锁
        private object m_Locker = new object();

        // 已经接受的消息队列
        private Queue<CMessage> m_ReceviedMessageQueue = new Queue<CMessage>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="networkInstance">网络单例引用</param>
        public CThreadReceiver ( CNetwork networkInstance )
        {
            m_NetworkRef = networkInstance;
        }

        /// <summary>
        /// 开始，启动收包线程
        /// </summary>
        public void Start()
        {
            if ( m_IsRunning )
            {
                return;
            }

            m_IsRunning = true;

            m_RecvThread = new Thread ( Process );
            m_RecvThread.IsBackground = true;
            m_RecvThread.Start();
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
        /// 结束本线程并且释放所有资源！！
        /// </summary>
        public void Stop()
        {
            lock ( m_Locker )
            {
                m_IsRunning = false;
                m_ReceviedMessageQueue.Clear();
            }
        }

        /// <summary>
        /// 处理收包
        /// </summary>
        private void Process()
        {
            while ( m_IsRunning )
            {
                if ( m_NetworkRef.IsConnected )
                {
                    // 处理收到的包
                    ProcessReceivingPacket();
                }
                else
                {
                    Thread.Sleep ( 1 );
                }
            }
        }

        /// <summary>
        /// 处理收到的包
        /// </summary>
        private void ProcessReceivingPacket()
        {
            CMessage message = null;

            // 收数据
            if ( ( message = RecevieData( ) ) != null )
            {
                PushReadyToHandlerPackage ( ref message );
            }
        }

        /// <summary>
        /// 返回收到的数据
        /// </summary>
        /// <returns></returns>
        private CMessage RecevieData( )
        {
            // 得到Socket对象
            Socket _Socket = m_NetworkRef.SocketInstance.Socket;

            if ( _Socket == null || !m_NetworkRef.IsConnected )
            {
                return null;
            }

            byte[] Buff = new byte[4];
            int length;

            // 收4个字节的Head
            try
            {
                length = _Socket.Receive ( Buff );
            }
            catch ( Exception ex )
            {
                CLOG.V ( ex.ToString() );
                CNetwork.Instance.DisConnect();
                return null;
            }

            if ( 4 != length )
            {
                CLOG.V ( "receive packge length error! length = {0} ", length );
                return null;
            }

            //取OpCode
            ushort Opcode = BitConverter.ToUInt16 ( Buff, 0 );
            //取BodyLength
            ushort BodyLength = BitConverter.ToUInt16 ( Buff, 2 );

            //新建包体缓存区
            Buff = new byte[BodyLength];
            int HasReceivedLength = 0;

            while ( HasReceivedLength < BodyLength )
            {
                try
                {
                    int recSize = _Socket.Receive ( Buff, HasReceivedLength, BodyLength - HasReceivedLength, SocketFlags.None );
                    HasReceivedLength += recSize;
                }
                catch ( Exception ex )
                {
                    CLOG.V ( ex.ToString() );
                    CNetwork.Instance.DisConnect();
                    //主动断网
                    return null;
                }
            }

            CLOG.V ( "package length is {0} has received {1} data", BodyLength, HasReceivedLength );

            if ( BodyLength != HasReceivedLength || BodyLength != Buff.Length )
            {
                CLOG.E ( "receivce data error!" );
                return null;
            }

            CMessage message = new CMessage ( Opcode, Buff, BodyLength );
            return message;
        }


        /// <summary>
        /// 将收到的数据包存入待处理队列
        /// 由主线程轮询执行
        /// </summary>
        /// <param name="message">待处理的包</param>
        public void PushReadyToHandlerPackage ( ref CMessage message )
        {
            lock ( m_Locker )
            {
                m_ReceviedMessageQueue.Enqueue ( message );
            }
        }

        /// <summary>
        /// 获取一个待处理的网络包
        /// </summary>
        /// <returns></returns>
        public CMessage PopReadyToHandlerPackage()
        {
            CMessage message = null;

            if ( m_ReceviedMessageQueue.Count == 0 )
            {
                return message;
            }

            lock ( m_Locker )
            {
                message = m_ReceviedMessageQueue.Dequeue();
            }

            return message;
        }

        /// <summary>
        /// 清除消息
        /// </summary>
        public void ClearMessage()
        {
            lock ( m_Locker )
            {
                m_ReceviedMessageQueue.Clear();
            }
        }
    }

}