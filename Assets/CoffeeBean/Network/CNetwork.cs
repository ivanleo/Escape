/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:49
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CNetwork.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CNetwork
    file ext:   cs
    author:     Leo

    purpose:    网络单例类，提供消息的发送，派发等
*********************************************************************/
using CoffeeBean;
using System;

namespace CoffeeBean
{
    /// <summary>
    /// 连接委托
    /// </summary>
    public delegate void DelegateConnect();

    /// <summary>
    /// 网络库的主要封装
    /// 实现所有操作
    /// </summary>
    public class CNetwork : CSingleton<CNetwork>
    {
        /// <summary>
        /// Socket实例
        /// </summary>
        private CSocket m_SocketInstance = null;

        /// <summary>
        /// IP地址
        /// </summary>
        private string m_IP = "127.0.0.1";

        /// <summary>
        /// 端口
        /// </summary>
        private int m_Port = 3306;

        /// <summary>
        /// 检测值
        /// </summary>
        private int m_DetectorValue = 123456;

        /// <summary>
        /// 是否已连接到服务器
        /// </summary>
        private bool m_IsConnected = false;

        /// <summary>
        /// 收线程
        /// </summary>
        private CThreadReceiver m_Receiver = null;

        /// <summary>
        /// 发线程
        /// </summary>
        private CThreadSender m_Sender = null;

        /// <summary>
        /// 连接线程
        /// </summary>
        private CThreadConnecter m_Connecter = null;

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool m_HasInit = false;

        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event DelegateConnect OnConnectSuccessEvent = null;

        /// <summary>
        /// 连接失败事件
        /// </summary>
        public event DelegateConnect OnConnectFailedEvent = null;

        /// <summary>
        /// 连接丢失事件
        /// </summary>
        public event DelegateConnect OnConnectLostEvent = null;

        /// <summary>
        /// 带参初始化
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        public void Init( string IP, int Port )
        {
            m_Port = Port;
            m_IP = IP;

            Init();
        }

        /// <summary>
        /// 网络初始化
        /// 创建Socket
        /// 启动各个线程
        /// </summary>
        public void Init()
        {
            CLOG.I( "-- Network init --" );

            // 注册所有网络消息包反射信息
            CPackageManager.Instance.PerpareNetworkPackageReflectionInformation();

            // 创建Socket
            m_SocketInstance = new CSocket();

            // 创建连接线程
            m_Connecter = new CThreadConnecter( this );

            // 创建收线程
            m_Receiver = new CThreadReceiver( this );

            // 创建发送线程
            m_Sender = new CThreadSender( this );

            m_Connecter.Start();
            m_Sender.Start();
            m_Receiver.Start();

            m_HasInit = true;

        }


        /// <summary>
        /// 刷新Network
        /// 注意该操作需要在主线程中完成
        /// </summary>
        public void Update( float dt )
        {
            if( m_SocketInstance == null || m_Sender == null || m_Connecter == null || m_Receiver == null )
            {
                return;
            }

            // 处理缓冲区收到的包
            ProcessReceviedPackage();

            // 处理连接信号
            ProcessConnectSignal();
        }


        /// <summary>
        /// 尝试连接
        /// </summary>
        public void TryConnect()
        {
            //如果已经连接就算了
            if ( m_IsConnected )
            {
                CLOG.W( "you has already connect to server ! so can not try to connect to server!" );
                return;
            }
            CLOG.I( "Try to connect to the server < IP:{0} Port:{1} >", IP, Port );
            m_Connecter.TryConnect();
        }

        /// <summary>
        /// 停止Socket连接并且释放资源
        /// 一般在游戏结束时调用
        /// 这列的主动断线并不一定会出发OnConnectLost消息
        /// </summary>
        public void StopAndRelease()
        {
            if( !m_HasInit )
            {
                return;
            }

            if( m_IsConnected )
            {
                DisConnect();
                m_IsConnected = false;
                m_HasInit = false;
            }

            // 停止所有线程
            if( m_Receiver.IsRunning )
            {
                m_Receiver.Stop();
            }

            if( m_Sender.IsRunning )
            {
                m_Sender.Stop();
            }

            if( m_Connecter.IsRunning )
            {
                m_Connecter.Stop();
            }
        }

        /// <summary>
        /// 主动断线
        /// </summary>
        public void DisConnect( bool NeedReconnect = true )
        {
            if( !m_IsConnected )
            {
                return;
            }

            CLOG.I( "DisConnect!" );
            m_IsConnected = false;

            if( m_SocketInstance != null )
            {
                m_SocketInstance.CloseConnect();
            }

            if( m_Sender != null )
            {
                m_Sender.ClearMessage();
            }

            if( m_Receiver != null )
            {
                m_Receiver.ClearMessage();
            }

            // 如果需要主动重连，那么压入连接丢失信号
            // 由应用层检测到断线进行处理
            // 否则直接断线即可
            if( NeedReconnect )
            {
                m_Connecter.PushConnectSignal( EConnectSignal.CONNECT_LOST );
            }
        }

        /// <summary>
        /// 发送消息到服务器
        /// </summary>
        /// <param name="package">要发送的消息</param>
        public void Send( CPackageBase package )
        {
            if( m_Sender != null )
            {
                CMessage message = CPackageBase.CreateMessage( package );

                // 加入待发队列
                m_Sender.PushMessageToSend( message );
            }
        }


        /// <summary>
        /// 增加一个消息包的处理委托
        /// </summary>
        /// <param name="clsType">包类类型</param>
        /// <param name="handler">处理委托</param>
        public void AddPackageHandler( Type clsType, DelegatePackageHandler handler )
        {
            CPackageManager.Instance.AddPackageHandler( clsType, handler );
        }

        /// <summary>
        /// 移除指定消息包的指定处理委托
        /// </summary>
        /// <param name="clsType">消息包类型</param>
        /// <param name="handler">处理委托</param>
        public void RemovePackageHandler( Type clsType, DelegatePackageHandler handler )
        {
            CPackageManager.Instance.RemovePackageHandler( clsType, handler );
        }

        /// <summary>
        /// 移除一个消息类类型下的所有处理委托
        /// </summary>
        /// <param name="clsType">消息类类型</param>
        public void RemovePackageHandler( Type clsType )
        {
            CPackageManager.Instance.RemoveAllPackageHandler( clsType );
        }

        /// <summary>
        /// 给一个消息包重设一个新的处理委托
        /// 该操作相当于
        /// RemovePackageHandler( clsType );
        ///  AddPackageHandler( clsType, handler );
        /// </summary>
        /// <param name="clsType">网络包类型</param>
        /// <param name="Handler">处理委托</param>
        public void ResetPackageHandler( Type clsType, DelegatePackageHandler Handler )
        {
            RemovePackageHandler( clsType );
            AddPackageHandler( clsType, Handler );
        }


        /// <summary>
        /// 处理信号
        /// 本函数要在主线程的Update中调用
        /// </summary>
        private void ProcessConnectSignal()
        {
            EConnectSignal State;
            while( ( State = m_Connecter.PopConnectSingal() ) != EConnectSignal.NULL )
            {
                switch( State )
                {
                    case EConnectSignal.CONNECT_SUESSFUL:
                        OnConnected();
                        break;
                    case EConnectSignal.CONNECT_FAILED:
                        OnConnectFailed();
                        break;
                    case EConnectSignal.CONNECT_LOST:
                        OnConnectLost();
                        break;
                }
            }
        }

        /// <summary>
        /// 伪造一个对象发送到收线程来伪造服务器消息
        /// 一般情况下
        /// 收线程收到字节流 -> 反序列化到CMessage -> 反射创建对象 -> FromMessage填充数据 -> 执行委托
        /// 这里是
        /// 伪造一个对象 -> ToMessage()得到CMessage -> 反射创建对象 -> FromMessage填充数据 -> 执行委托
        /// </summary>
        /// <param name="package">消息对象</param>
        /// <param name="Length">消息数据长度
        ///      正常收消息时
        ///      字节流的大小由包头提供了
        ///      属于精确的分配内存空间
        ///      不会有问题
        ///
        ///      而伪造消息进行收取时
        ///      没有包头提供消息大小
        ///      因此提供默认的第二个参数来指定消息大小
        ///      如果伪造的消息过大
        ///      可以增大本值来确保不会产生数组越界的问题
        /// </param>
        public void FakePackageToReceive( CPackageBase package )
        {
            CMessage msg = CPackageBase.CreateMessage( package );

            // 加入待收队列
            FakeMessageToReceive( ref msg );
        }

        /// <summary>
        /// 压入一个包到收线程来伪造服务器消息
        /// </summary>
        /// <param name="msg">消息对象</param>
        public void FakeMessageToReceive( ref CMessage msg )
        {
            m_Receiver.PushReadyToHandlerPackage( ref msg );
        }

        /// <summary>
        /// 处理收到的包
        /// </summary>
        private void ProcessReceviedPackage()
        {
            CMessage message = null;
            while( ( message = m_Receiver.PopReadyToHandlerPackage() ) != null )
            {
                // 如果是伪造的消息，则创建时会导致读写位置到包的末尾了
                // 因此需要在这里重置归0
                message.Body.ResetPosition();

                // 包ID
                int OpCode = message.OpCode;

                // 反射出类型
                Type type = CPackageManager.Instance.ReflectionClassNameByActionID( OpCode );

                //安全处理
                if( type == null )
                {
                    CLOG.E( "Reflection class name by OpCode error! OpCode={0} reflection class name = null!! please check the code", OpCode );
                    return;
                }

                // 得到该类型的处理委托
                DelegatePackageHandler DP = CPackageManager.Instance.GetPackageHandler( type );

                // 创建反射类型实例
                CPackageBase package = Activator.CreateInstance( type ) as CPackageBase;

                //安全处理
                if( package == null )
                {
                    CLOG.E( "create package instance error! OpCode = {0} type = {1}", OpCode, type.ToString() );
                    return;
                }

                // 从message的身体中获取数据实例化对象
                try
                {
                    package.FromMessage( ref message );
                }
                catch( Exception ex )
                {
                    CLOG.E( "from message error! Exception!! OpCode = {0} type={1} message={2} ", OpCode, type.ToString(), message.ToString() );
                    CLOG.E( ex.ToString() );
                    return;
                }

                // 调用委托，传入参数
                if( DP != null )
                {
                    DP( package );
                }
                else
                {
                    CLOG.W( "ths OpCode {0} was not register handler!", OpCode );
                }

            }
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        private void OnConnected()
        {
            if( m_IsConnected )
            {
                return;
            }

            m_IsConnected = true;

            // 派发连接成功事件
            if( OnConnectSuccessEvent != null )
            {
                OnConnectSuccessEvent();
            }
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        private void OnConnectFailed()
        {
            if( m_IsConnected )
            {
                return;
            }

            m_IsConnected = false;

            // 派发连接失败事件
            if( OnConnectFailedEvent != null )
            {
                OnConnectFailedEvent();
            }

        }


        /// <summary>
        /// 连接丢失了
        /// </summary>
        private void OnConnectLost()
        {
            if( m_IsConnected )
            {
                return;
            }

            m_IsConnected = false;

            // 派发连接丢失事件
            if( OnConnectLostEvent != null )
            {
                OnConnectLostEvent();
            }
        }

        /// <summary>
        /// 获取或者设置IP地址
        /// </summary>
        public string IP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }


        /// <summary>
        /// 获取或者设置端口号
        /// </summary>
        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        /// <summary>
        /// 得到Socket,只读
        /// </summary>
        public CSocket SocketInstance
        {
            get { return m_SocketInstance; }
        }

        /// <summary>
        /// 是否连接上了,只读
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        /// <summary>
        /// 设置网络收包探测值
        /// 默认为123456
        /// 该值必须与服务器的一止
        /// </summary>
        public int DetectorValue
        {
            get { return m_DetectorValue; }
            set { m_DetectorValue = value; }
        }

        /// <summary>
        /// Network是否已经初始化了
        /// </summary>
        public bool HasInit
        {
            get { return m_HasInit; }
        }
    }
}
