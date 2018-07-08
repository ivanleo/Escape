using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBean
{
    /// <summary>
    /// 环形缓冲区
    /// </summary>
    public class VRingBuff
    {
        private int MAX_SIZE;

        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private byte[] m_Buff;

        /// <summary>
        /// 读取下标
        /// </summary>
        private int m_ReadIndex;

        /// <summary>
        /// 写入下标
        /// </summary>
        private int m_WriteIndex;

        /// <summary>
        /// 空闲容量
        /// </summary>
        private int m_FreeCapcity;


        /// <summary>
        /// 构造函数
        /// </summary>
        public VRingBuff( int maxSize = 40960 )
        {
            MAX_SIZE = maxSize;
            m_Buff = new byte[MAX_SIZE];
            m_FreeCapcity = MAX_SIZE;
            m_ReadIndex = 0;
            m_WriteIndex = 0;
        }

        /// <summary>
        /// 得到空闲区域
        /// </summary>
        /// <returns></returns>
        public int GetFreeSpace()
        {
            return m_FreeCapcity;
        }

        /// <summary>
        /// 得到非空闲区域
        /// </summary>
        /// <returns></returns>
        public int GetCanReadSpace()
        {
            return MAX_SIZE - m_FreeCapcity;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int WriteBuff( byte[] data, int size )
        {
            if ( size > GetFreeSpace() )
            {
                return 0;
            }

            //可以写得下
            for ( int i = 0 ; i < size ; i++ )
            {
                int index = ( m_WriteIndex + i ) % MAX_SIZE;
                m_Buff[index] = data[i];
            }

            m_FreeCapcity -= size;
            m_WriteIndex = ( m_WriteIndex + size ) % MAX_SIZE;

            return size;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int ReadBuff( ref byte[] data, int size )
        {
            if ( size > GetCanReadSpace() )
            {
                return 0;
            }

            for ( int i = 0 ; i < size ; i++ )
            {
                int index = ( m_ReadIndex + i ) % MAX_SIZE;
                data[i] = m_Buff[index];
            }

            m_FreeCapcity += size;
            m_ReadIndex = ( m_ReadIndex + size ) % MAX_SIZE;

            return size;
        }
    }
}
