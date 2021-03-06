﻿/********************************************************************
    created:    2018/06/21
    created:    21:6:2018   9:04
    filename:   D:\Work\PushCoin\trunk\PushCoin\Assets\CoffeeBean\Core\CEncryptInt.cs
    file path:  D:\Work\PushCoin\trunk\PushCoin\Assets\CoffeeBean\Core
    file base:  CEncryptInt
    file ext:   cs
    author:     Leo

    purpose:    加密数字
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBean {
    public class CEncryptInt: System.Object
    {
        private string _data = null;
        private static string _key = "PuShCoIn";
        private static char[] _key_chars = _key.ToCharArray();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Num"></param>
        public CEncryptInt ( int Num )
        {
            Encryption ( Num );
        }

        /// <summary>
        /// 得到加密后的字符串
        /// </summary>
        /// <returns></returns>
        public string GetEncryptionData()
        {
            return _data;
        }

        /// <summary>
        /// 得到原文
        /// </summary>
        /// <returns></returns>
        public int GetDecryptionData()
        {
            return Decryption();
        }

        /// <summary>
        /// 重置归0
        /// </summary>
        public void ResetZero()
        {
            Encryption ( 0 );
        }

        /// <summary>
        /// 加密一个数字
        /// </summary>
        /// <param name="number"></param>
        private void Encryption ( int number )
        {
            char[] EnStr = number.ToString().ToCharArray();

            for ( int i = 0 ; i < EnStr.Length ; i++ )
            {
                EnStr[i] ^= _key_chars[i % _key_chars.Length];
            }

            _data = new string ( EnStr );
        }

        /// <summary>
        /// 解密一个数字
        /// </summary>
        /// <param name="number"></param>
        private int Decryption()
        {
            char[] EnStr = _data.ToCharArray();

            for ( int i = 0 ; i < EnStr.Length ; i++ )
            {
                EnStr[i] ^= _key_chars[i % _key_chars.Length];
            }

            try
            {
                int result = int.Parse ( new string ( EnStr ) );
                return result;
            }
            catch ( Exception ex )
            {
                CLOG.E ( ex.ToString() );
                return -1;
            }
        }

        #region  Operation
        public static CEncryptInt operator ++ ( CEncryptInt lhs )
        {
            return new CEncryptInt ( lhs.Decryption() + 1 );
        }

        public static CEncryptInt operator + ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() + rhs );
            return lhs;
        }

        public static CEncryptInt operator -- ( CEncryptInt lhs )
        {
            return new CEncryptInt ( lhs.Decryption() - 1 );
        }

        public static CEncryptInt operator - ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() - rhs );
            return lhs;
        }

        public static CEncryptInt operator * ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() * rhs );
            return lhs;
        }

        public static CEncryptInt operator / ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() / rhs );
            return lhs;
        }

        public static CEncryptInt operator % ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() % rhs );
            return lhs;
        }

        public static CEncryptInt operator & ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() & rhs );
            return lhs;
        }

        public static CEncryptInt operator | ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() | rhs );
            return lhs;
        }

        public static CEncryptInt operator ^ ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() ^ rhs );
            return lhs;
        }

        public static CEncryptInt operator << ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() << rhs );
            return lhs;
        }

        public static CEncryptInt operator >> ( CEncryptInt lhs, int rhs )
        {
            lhs.Encryption ( lhs.Decryption() >> rhs );
            return lhs;
        }

        public static bool operator == ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() == rhs;
        }

        public static bool operator != ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() != rhs;
        }

        public static bool operator > ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() > rhs;
        }

        public static bool operator < ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() < rhs;
        }

        public static bool operator >= ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() >= rhs;
        }

        public static bool operator <= ( CEncryptInt lhs, int rhs )
        {
            return lhs.Decryption() <=  rhs;
        }

        public override bool Equals ( Object obj )
        {
            if ( obj == null )
            {
                return false;
            }

            if ( obj is CEncryptInt )
            {
                return Decryption() == ( obj as CEncryptInt ).Decryption();
            }

            if ( obj is int )
            {
                return Decryption() == ( int ) obj;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Decryption();
        }

        public override string ToString()
        {
            return Decryption().ToString();
        }


        public static CEncryptInt operator + ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() + rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator - ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() - rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator * ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() * rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator / ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() / rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator % ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() % rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator & ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() & rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator | ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() | rhs.Decryption() );
            return lhs;
        }

        public static CEncryptInt operator ^ ( CEncryptInt lhs, CEncryptInt rhs )
        {
            lhs.Encryption ( lhs.Decryption() ^ rhs.Decryption() );
            return lhs;
        }

        public static bool operator == ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() == rhs.Decryption();
        }

        public static bool operator != ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() != rhs.Decryption();
        }

        public static bool operator > ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() > rhs.Decryption();
        }

        public static bool operator < ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() < rhs.Decryption();
        }

        public static bool operator >= ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() >= rhs.Decryption();
        }

        public static bool operator <= ( CEncryptInt lhs, CEncryptInt rhs )
        {
            return lhs.Decryption() <= rhs.Decryption();
        }

        #endregion


    }
}
