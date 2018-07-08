/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:50
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CPackageBase.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CPackageBase
    file ext:   cs
    author:     Leo

    purpose:    消息包基类
*********************************************************************/
using System;
using System.Reflection;

namespace CoffeeBean
{
    /// <summary>
    /// 消息包基类
    /// 所有的网络消息对象都应该继承本类
    /// </summary>
    public class CPackageBase
    {
        /// <summary>
        /// 检查一个类型是否可以被序列化
        /// 目前支持的序列化类型为
        /// byte short int long float string [] List<>
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private bool CheckCanSerialize( Type tp )
        {
            if ( tp.Equals( typeof( byte ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( bool ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( short ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( int ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( long ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( float ) ) )
            {
                return true;
            }

            if ( tp.Equals( typeof( string ) ) )
            {
                return true;
            }

            if ( tp.IsArray )
            {
                return true;
            }

            if( tp.IsGenericType )
            {
                return true;
            }

            if( tp.IsSubclassOf( typeof( CPackageBase ) ) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 序列化对象，获得值填充CMessage
        /// </summary>
        /// <returns> 待填充的CMessage对象 </returns>
        public void ToMessage( ref CMessage msg )
        {
            // 获得当前实例类型
            Type insType = this.GetType();

            // 获得本实例下所有的公开属性
            FieldInfo[] fis = insType.GetFields( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );

            // 遍历属性
            for( int i = 0 ; i < fis.Length ; i++ )
            {
                // 属性类型
                Type MemberType = fis[i].FieldType;

                if( CheckCanSerialize( MemberType ) )
                {
                    object[] param = { fis[i].GetValue( this ), msg };
                    CFunction.CallGenericFunction( "PushData", msg.Body, MemberType, param );
                }
            }
        }

        /// <summary>
        /// 反序列化对象，利用CMessage填充对象
        /// </summary>
        /// <param name="msg">CMessage对象</param>
        public void FromMessage( ref CMessage msg )
        {
            Type insType = this.GetType();

            // 获得本实例下所有的公开属性
            FieldInfo[] fis = insType.GetFields( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
            for ( int i = 0 ; i < fis.Length ; i++ )
            {
                Type MemberType = fis[i].FieldType;

                if ( CheckCanSerialize( MemberType ) )
                {
                    object[] param = { msg };
                    object PopValue = CFunction.CallGenericFunction( "PopData", msg.Body, MemberType, param );

                    // 设置值
                    fis[i].SetValue( this, PopValue );
                }
            }
        }

        /// <summary>
        /// 通过网络包创建Message对象
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static CMessage CreateMessage( CPackageBase package )
        {
            // 将物理对象转化为Message
            CMessage message = new CMessage();

            //填充OpCode
            Type type = package.GetType();
            Object[] attrs = type.GetCustomAttributes( false );
            foreach ( Object item in attrs )
            {
                // 符合特性的才注册
                if ( item.GetType() == typeof( CAttrPackage ) )
                {
                    message.OpCode = ( item as CAttrPackage ).OpCode;
                    break;
                }
            }

            // 填充身体数据
            package.ToMessage( ref message );

            //设置长度
            message.BodyLength = ( ushort )message.Body.Position;

            return message;
        }
    }
}
