/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:51
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CSocket.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CSocket
    file ext:   cs
    author:     Leo

    purpose:    Socket封装
*********************************************************************/
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace CoffeeBean
{
    /// <summary>
    /// Socket封装
    /// </summary>
    public class CSocket
    {
        private Socket m_Socket = null;

        /// <summary>
        /// Socket对象
        /// </summary>
        public Socket Socket
        {
            get { return m_Socket; }
            set { m_Socket = value; }
        }

        /// <summary>
        /// 解析一个域名地址
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns>IP地址字符串</returns>
        private static string DomainToIP( string host )
        {
            IPAddress ipAdress;
            if( IPAddress.TryParse( host, out ipAdress ) )
            {
                return ipAdress.ToString();
            }
            else
            {
                IPHostEntry hostinfo = Dns.GetHostEntry( host );
                IPAddress[] aryIP = hostinfo.AddressList;
                string ip = null;
                for( int i = 0 ; i < aryIP.Length ; i++ )
                {
                    ip = aryIP[i].ToString();
                    if( IPCheck( ip ) )
                    {
                        return ip;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// IP正则检查
        /// </summary>
        /// <param name="ip">IP地址字符串</param>
        /// <returns>是否符合IP地址格式</returns>
        public static bool IPCheck( string ip )
        {
            string num = @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))";
            return Regex.IsMatch( ip, num );
        }



        /// <summary>
        /// 连接到指定IP和端口
        /// </summary>
        /// <param name="host">ip或者域名</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public bool Connect( string host, int port )
        {
            m_Socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            string ip = DomainToIP( host );
            IPAddress ipAddr = IPAddress.Parse( ip );
            IPEndPoint ipe = new IPEndPoint( ipAddr, port );
            try
            {
                m_Socket.Connect( ipe );
                return true;
            }
            catch( Exception ex )
            {
                CLOG.E( ex.ToString() );
                return false;
            }
        }

        /// <summary>
        /// 在当前Socket对象上收取一定长度的数据放入缓冲区
        /// </summary>
        /// <param name="packageBuff">缓冲区</param>
        /// <param name="buffOffset">放入缓冲区的偏移位置</param>
        /// <param name="length">收取的数据长度</param>
        /// <param name="flag">Socekt收发行为</param>
        /// <returns>收到的字节数</returns>
        public int ReceiveData( ref byte[] packageBuff, int buffOffset, int length, SocketFlags flag = SocketFlags.None )
        {
            return m_Socket.Receive( packageBuff, buffOffset, length, flag );
        }

        /// <summary>
        /// 在当前Socekt上发送CMessage消息
        /// </summary>
        /// <param name="message">消息，可由任意继承了PackageBase的对象调用 ToMessage获得</param>
        /// <returns>发送的字节数</returns>
        public int SendMessage( CMessage message )
        {
            try
            {
                int nowSend = 0;
                byte[] data = message.GetData();
                while( nowSend < data.Length )
                {
                    int sendSize = m_Socket.Send( data, nowSend, 1024, SocketFlags.None );
                    nowSend += sendSize;
                }
                return nowSend;
            }
            catch( ArgumentNullException e )
            {
                CLOG.E( e.ToString() );
                return -1;
            }
            catch( SocketException e )
            {
                CLOG.E( e.ToString() );
                return -2;
            }
            catch( ObjectDisposedException e )
            {
                CLOG.E( e.ToString() );
                return -3;
            }
        }

        /// <summary>
        /// 关闭Socket连接
        /// </summary>
        public void CloseConnect()
        {
            if( m_Socket == null )
            {
                return;
            }

            m_Socket.Shutdown( SocketShutdown.Both );
            m_Socket.Disconnect( true );

            m_Socket = null;
        }
    }
}
