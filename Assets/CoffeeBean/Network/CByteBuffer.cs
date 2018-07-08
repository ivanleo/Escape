/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:47
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CByteBuffer.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CByteBuffer
    file ext:   cs
    author:     Leo

    purpose:    字节流缓存
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeBean
{
    /// <summary>
    /// 字节流缓存区
    /// </summary>
    public class CByteBuffer
    {
        /// <summary>
        /// 直接长度
        /// </summary>
        private const int LENGTH_BYTE = 1;
        /// <summary>
        /// short长度
        /// </summary>
        private const int LENGTH_SHORT = 2;
        /// <summary>
        /// 整型长度
        /// </summary>
        private const int LENGTH_INT = 4;
        /// <summary>
        /// long 长度
        /// </summary>
        private const int LENGTH_LONG = 8;
        /// <summary>
        /// 浮点数放大范围
        /// </summary>
        private const int FLOAT_SCALE = 100;

        /// <summary>
        /// 数据
        /// </summary>
        private byte[] m_Buffer;
        private int m_Position = 0;
        private int m_Limit = 0;
        private int m_Length = 0;

        /// <summary>
        /// 读写位置
        /// </summary>
        public int Position { get { return m_Position; } }

        /// <summary>
        /// 限制长度
        /// </summary>
        public int Limit { get { return m_Limit; } }

        /// <summary>
        /// 字节流长度
        /// </summary>
        public int Length { get { return m_Position; } }

        /// <summary>
        /// 得到数据缓冲
        /// </summary>
        public byte[] Buffer { get { return m_Buffer; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">字节流长度</param>
        public CByteBuffer( int length )
        {
            m_Buffer = new byte[length];
            m_Position = 0;
            m_Limit = length;
            m_Length = length;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        public CByteBuffer( byte[] data )
        {
            m_Buffer = data;
            m_Position = 0;
            m_Limit = data.Length;
            m_Length = data.Length;
        }

        /// <summary>
        /// 压入字节流
        /// </summary>
        /// <param name="bytes">字节流数组</param>
        public void PushBytes( byte[] bytes )
        {
            PushBytes( bytes, 0, bytes.Length );
        }

        /// <summary>
        /// 压入字节流
        /// </summary>
        /// <param name="bytes">字节流数组</param>
        /// <param name="index">压入的字节流数组的起点</param>
        /// <param name="size">压入多少字节的数据</param>
        public void PushBytes( byte[] bytes, int index, int size )
        {
            if( size != 0 && m_Position + size > m_Buffer.Length + 1 )
            {
                string error = string.Format( "PushBytes Size error! m_Position = {0} PushSize = {1} BufferLength = {2}", m_Position, size, m_Buffer.Length );
                throw new Exception( error );
            }

            for( int i = 0; i < size; i++ )
            {
                m_Buffer[m_Position + i] = bytes[i + index];
            }
            m_Position += size;
            m_Limit -= size;
        }

        /// <summary>
        /// 从字节流中获取一定长度的数据
        /// </summary>
        /// <param name="length">获取的长度</param>
        /// <returns>byte[] 数组</returns>
        public byte[] PopBytes( int length )
        {
            if( length != 0 && m_Position + length > m_Buffer.Length )
            {
                string error = string.Format( "PopBytes Size error!  m_Position = {0} PopLength = {1} BufferLength = {2}", m_Position, length, m_Buffer.Length );
                throw new Exception( error );
            }

            byte[] bytes = new byte[length];
            for( int i = 0; i < length; i++ )
            {
                bytes[i] = m_Buffer[i + m_Position];
            }
            m_Position += length;
            m_Limit -= length;
            return bytes;
        }

        /// <summary>
        /// 重置读写位置
        /// </summary>
        public void ResetPosition()
        {
            m_Position = 0;
            m_Limit = m_Length;
        }

        /// <summary>
        /// 压入一个byte型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushByte( byte value )
        {
            byte[] data = new byte[1];
            data[0] = value;
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个sbyte型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushByte( sbyte value )
        {
            byte[] data = new byte[1];
            data[0] = ( byte )value;
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个bool型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushBoolean( bool value )
        {
            PushByte( ( byte )( value ? 1 : 0 ) );
        }


        /// <summary>
        /// 压入一个short型值
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushShort( short value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个ushort型值
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushShort( ushort value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个int型值
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushInt( int value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个uint型值
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushInt( uint value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个float型值
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushFloat( float value )
        {
            int tempValue = ( int )( value * FLOAT_SCALE );
            PushInt( tempValue );
        }

        /// <summary>
        /// 压入一个long型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushLong( long value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个long型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushLong( ulong value )
        {
            byte[] data = BitConverter.GetBytes( value );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个string型值到body
        /// </summary>
        /// <param name="value">要压入的值</param>
        private void PushString( string value )
        {
            if( value == null )
            {
                PushShort( ( short ) - 1 );
                return;
            }

            byte[] data = Encoding.UTF8.GetBytes( value );
            PushShort( ( short )data.Length );
            PushBytes( data );
        }

        /// <summary>
        /// 压入一个数组的所有值到body
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="data">要压入的值</param>
        /// <param name="msg">数组引用</param>
        public void PushArray<T>( T[] data, ref CMessage msg )
        {
            if( data == null )
            {
                PushShort( ( int ) - 1 );
                return;
            }

            PushShort( ( short )data.Length );
            for( int i = 0; i < data.Length; i++ )
            {
                PushData( data[i], ref msg );
            }
        }

        /// <summary>
        /// 压入一个List容器的所有值到body
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="data">List容器</param>
        /// <param name="msg">要压入的消息引用</param>
        public void PushList<T>( List<T> data, ref CMessage msg )
        {
            if( data == null )
            {
                PushShort( ( short ) - 1 );
                return;
            }

            // 先写入长度
            PushShort( ( short )data.Count );
            for( int i = 0; i < data.Count; i++ )
            {
                T tempData = data[i];
                PushData( tempData, ref msg );
            }
        }

        /// <summary>
        /// 压入数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="data">要压入的值></param>
        /// <param name="msg">要压入的目标消息></param>
        public void PushData<T>( T data, ref CMessage msg )
        {
            Type dataType = typeof( T );

            if( dataType == typeof( byte ) )
            {
                PushByte( ( byte )( Object )data );
                return;
            }

            if ( dataType == typeof( sbyte ) )
            {
                PushByte( ( sbyte )( Object )data );
                return;
            }

            if ( dataType == typeof( bool ) )
            {
                PushBoolean( ( bool )( Object )data );
                return;
            }

            if( dataType == typeof( int ) )
            {
                PushInt( ( int )( Object )data );
                return;
            }

            if ( dataType == typeof( uint ) )
            {
                PushInt( ( uint )( Object )data );
                return;
            }

            if ( dataType == typeof( short ) )
            {
                PushShort( ( short )( Object )data );
                return;
            }

            if ( dataType == typeof( ushort ) )
            {
                PushInt( ( ushort )( Object )data );
                return;
            }

            if ( dataType == typeof( float ) )
            {
                PushFloat( ( float )( Object )data );
                return;
            }

            if( dataType == typeof( long ) )
            {
                PushLong( ( long )( Object )data );
                return;
            }

            if ( dataType == typeof( ulong ) )
            {
                PushLong( ( ulong )( Object )data );
                return;
            }

            if ( dataType == typeof( string ) )
            {
                PushString( ( string )( Object )data );
                return;
            }

            // 如果反射对象是 CPackageBase 的子类
            if( data is CPackageBase )
            {
                CPackageBase tempData = ( CPackageBase )( Object )data;
                if( tempData == null )
                {
                    return;
                }
                tempData.ToMessage( ref msg );
                return;
            }

            // 如果反射对象是 List
            if( dataType.IsGenericType )
            {
                Type[] GenericType = dataType.GetGenericArguments();
                object[] param = { data, msg };
                CFunction.CallGenericArrayFunction( "PushList", this, GenericType, param );
                return;
            }

            // 如果反射对象是数组
            if( dataType.IsArray )
            {
                Type ElementType = dataType.GetElementType();
                object[] param = { data, msg};
                CFunction.CallGenericFunction( "PushArray", this, ElementType, param );
                return;
            }

            CLOG.E( "the type {0} has no serilize methon", dataType.ToString() );
        }

        /// <summary>
        /// 从body中获得一个byte值
        /// </summary>
        private byte PopByte()
        {
            byte[] data = PopBytes( 1 );
            return data[0];
        }

        /// <summary>
        /// 从body中获得一个byte值
        /// </summary>
        private sbyte PopSByte()
        {
            byte[] data = PopBytes( 1 );
            return ( sbyte )data[0];
        }

        /// <summary>
        /// 从body中获得一个bool值
        /// </summary>
        private bool PopBoolean()
        {
            byte data = PopByte();
            return data > 0;
        }

        /// <summary>
        /// 从字节流中获得一个short值
        /// </summary>
        private short PopShort()
        {
            byte[] data = PopBytes( LENGTH_SHORT );
            return BitConverter.ToInt16( data, 0 );
        }

        /// <summary>
        /// 从字节流中获得一个ushort值
        /// </summary>
        private ushort PopUShort()
        {
            byte[] data = PopBytes( LENGTH_SHORT );
            return BitConverter.ToUInt16( data, 0 );
        }

        /// <summary>
        /// 从字节流中获得一个int值
        /// </summary>
        private int PopInt()
        {
            byte[] data = PopBytes( LENGTH_INT );
            return BitConverter.ToInt32( data, 0 );
        }

        /// <summary>
        /// 从字节流中获得一个uint值
        /// </summary>
        private uint PopUInt()
        {
            byte[] data = PopBytes( LENGTH_INT );
            return BitConverter.ToUInt32( data, 0 );
        }

        /// <summary>
        /// 从字节流中获得一个float值
        /// </summary>
        private float PopFloat()
        {
            byte[] data = PopBytes( LENGTH_INT );
            int tempValue = BitConverter.ToInt32( data, 0 );
            float f = float.Parse( ( ( float )tempValue / ( float )FLOAT_SCALE ).ToString( "0.00" ) );
            return f;
        }

        /// <summary>
        /// 从body中获得一个long值
        /// </summary>
        private long PopLong()
        {
            byte[] data = PopBytes( LENGTH_LONG );
            return BitConverter.ToInt64( data, 0 );
        }

        /// <summary>
        /// 从body中获得一个long值
        /// </summary>
        private ulong PopULong()
        {
            byte[] data = PopBytes( LENGTH_LONG );
            return BitConverter.ToUInt64( data, 0 );
        }

        /// <summary>
        /// 从body中获得一个string值
        /// </summary>
        private string PopString()
        {
            short length = PopShort();

            if( length == -1 )
            {
                return null;
            }

            byte[] data = PopBytes( length );
            return Encoding.UTF8.GetString( data );
        }

        /// <summary>
        /// 从body中获得一个数组的
        /// </summary>
        /// <typeparam name="T">泛型，支持各种基本类型</typeparam>
        public T[] PopArray<T>( ref CMessage msg )
        {
            // 先读取长度
            int size = PopShort( );

            if( size == -1 )
            {
                // CLOG.V( "get ArrayData length != supply Array length!" );
                return null;
            }

            T[] data = new T[size];

            for( int i = 0; i < size; i++ )
            {
                data[i] = PopData<T>( ref msg );
            }

            return data;
        }

        // 获得List类型数据，接受方不定长
        /// <summary>
        /// 从body中获取一个List
        /// </summary>
        /// <typeparam name="T">泛型，支持各种基本类型</typeparam>
        public List<T> PopList<T>( ref CMessage msg ) where T : new()
        {
            // 先读取长度
            short size = PopShort();

            if( size == -1 )
            {
                // CLOG.V( "get ArrayData length != supply Array length!" );
                return null;
            }

            List<T> data = new List<T>();

            for( int i = 0; i < size; i++ )
            {
                data.Add( PopData<T>( ref msg ) );
            }

            return data;
        }

        /// <summary>
        /// 从body中获取数据，写入参数中去
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        public T PopData<T>( ref CMessage msg )
        {
            Type TType = typeof( T );

            if( TType == typeof( byte )  )
            {
                return ( T )( Object )PopByte();
            }

            if ( TType == typeof( sbyte ) )
            {
                return ( T )( Object )PopSByte();
            }

            if ( TType == typeof( bool ) )
            {
                return ( T )( Object )PopBoolean();
            }

            if( TType == typeof( short ) )
            {
                return ( T )( Object )PopShort();
            }

            if ( TType == typeof( ushort ) )
            {
                return ( T )( Object )PopUShort();
            }

            if ( TType == typeof( int ) )
            {
                return ( T )( Object )PopInt();
            }

            if ( TType == typeof( uint ) )
            {
                return ( T )( Object )PopUInt();
            }

            if ( TType == typeof( float ) )
            {
                return ( T )( Object )PopFloat();
            }

            if( TType == typeof( long ) )
            {
                return ( T )( Object )PopLong();
            }

            if ( TType == typeof( ulong ) )
            {
                return ( T )( Object )PopULong();
            }

            if ( TType == typeof( string ) )
            {
                return ( T )( Object )PopString();
            }

            // 如果反射对象是 CPackageBase 的子类
            if( TType.IsSubclassOf( typeof( CPackageBase ) ) )
            {
                // 创建反射类型实例
                CPackageBase MemberPackageBase = Activator.CreateInstance( TType ) as CPackageBase;
                MemberPackageBase.FromMessage( ref msg );

                return ( T )( Object )MemberPackageBase;
            }

            // 如果反射对象是 List
            if( TType.IsGenericType )
            {
                Type[] GenericTypes = TType.GetGenericArguments();
                object[] param = { msg };
                return ( T )( Object )CFunction.CallGenericArrayFunction( "PopList", this, GenericTypes, param );
            }

            // 如果反射对象是数组
            if( TType.IsArray )
            {
                Type ElementType = TType.GetElementType();
                object[] param = { msg };
                return ( T )( Object )CFunction.CallGenericFunction( "PopArray", this, ElementType, param );
            }

            CLOG.E( "the type {0} has no serilize methon", TType.ToString() );
            return default( T );
        }

    }
}
