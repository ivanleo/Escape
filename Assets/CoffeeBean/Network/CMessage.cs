/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:47
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CMessage.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CMessage
    file ext:   cs
    author:     Leo

    purpose:    消息封装类
                ClientObject -> Message -> ByteBuffer -> Server
                Server -> ByteBuffer -> Message -> ClientObject
*********************************************************************/
using System;
using System.Text;

namespace CoffeeBean
{
    /// <summary>
    /// 消息类
    /// 所有的字节流都是先被封装到CMessage对象里
    /// 然后再实现序列化和反序列化变为对象
    /// </summary>
    public class CMessage
    {
        /// <summary>
        /// 最大消息包身体字节长度
        /// </summary>
        private const short MAX_MESSAGE_BODY_SIZE = 2048;

        /// <summary>
        /// 消息ID
        /// </summary>
        private ushort m_OpCode = 0;

        /// <summary>
        /// 包体长度
        /// </summary>
        private ushort m_BodyLength = 0;

        /// <summary>
        /// 包体数据
        /// </summary>
        private CByteBuffer m_Body;

        /// <summary>
        /// 构造默认长度的message对象
        /// </summary>
        public CMessage()
        {
            m_Body = new CByteBuffer( MAX_MESSAGE_BODY_SIZE );
        }

        /// <summary>
        /// 构造一定长度的Message对象
        /// </summary>
        /// <param name="BodySize"></param>
        public CMessage( int BodySize )
        {
            m_Body = new CByteBuffer( BodySize );
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="OpCode"></param>
        /// <param name="data"></param>
        /// <param name="BodyLength"></param>
        public CMessage( int OpCode, byte[] data, int BodyLength = -1  )
        {
            m_OpCode = ( ushort )OpCode;
            m_BodyLength = ( ushort )( BodyLength == -1 ? data.Length : BodyLength );
            m_Body = new CByteBuffer( data );
        }

        /// <summary>
        /// 获取消息包头部长度
        /// </summary>
        /// <returns></returns>
        public ushort GetHeadLength()
        {
            return 4;
        }

        /// <summary>
        /// 获得身体包有效数据长度
        /// </summary>
        public ushort BodyLength
        {
            get { return m_BodyLength; }
            set { m_BodyLength = value; }
        }

        /// <summary>
        /// 协议ID
        /// </summary>
        public ushort OpCode
        {
            get { return m_OpCode; }
            set { m_OpCode = value; }
        }

        /// <summary>
        /// 消息身体
        /// </summary>
        public CByteBuffer Body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        /// <summary>
        /// 得到数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            byte[] finalData = new byte[4 + m_BodyLength];
            int index = 0;

            //压入OpCode
            byte[] data = BitConverter.GetBytes( m_OpCode );
            finalData[index++] = data[0];
            finalData[index++] = data[1];

            CLOG.I( "Get Data OpCode is {0} {1}", m_OpCode, Convert.ToString( m_OpCode, 2 ).PadLeft( 16, '0' ) );

            //压入头长度
            data = BitConverter.GetBytes( m_BodyLength );
            finalData[index++] = data[0];
            finalData[index++] = data[1];

            CLOG.I( "Get Data BodyLength is {0} {1}", m_BodyLength, Convert.ToString( m_BodyLength, 2 ).PadLeft( 16, '0' ) );

            //压入身体
            data = m_Body.Buffer;
            if( data.Length < m_BodyLength )
            {
                CLOG.E( "the ready push body data {0} bytes, but the message object body has only {1} bytes", m_BodyLength, data.Length );
                return null;
            }

            StringBuilder sb = new StringBuilder();
            for ( int i = 0 ; i < m_BodyLength ; i++ )
            {
                finalData[index++] = data[i];
                sb.Append( Convert.ToString( data[i], 2 ).PadLeft( 8, '0' ) + "," );
            }
            CLOG.I( "Get Data is {0}", sb.ToString() );


            return finalData;

        }

        /// <summary>
        /// 重写tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append( "OpCode:" + m_OpCode );
            SB.Append( "  BodyLength:" + m_BodyLength );
            return SB.ToString();
        }

    }
}
