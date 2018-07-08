/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:49
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network\CPackageAttribute.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Network
    file base:  CPackageAttribute
    file ext:   cs
    author:     Leo

    purpose:    消息包特性
*********************************************************************/
using System;

namespace CoffeeBean
{
    /// <summary>
    /// 协议包特性
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class CAttrPackage : System.Attribute
    {
        private ushort m_OpCode;
        private string m_ClassComment;

        /// <summary>
        /// 无参构造，本特性的使用方法为
        /// [CPackageAttribute( ActionID=0x1001, ClassComment="登陆消息" )]
        /// </summary>
        public CAttrPackage()
        {

        }

        /// <summary>
        /// 带参构造，本特性的使用方法为
        /// [CPackageAttribute(0x1001，"登陆消息")]
        /// </summary>
        /// <param name="actionID">网络包标识id</param>
        /// <param name="classComment">类注释</param>
        public CAttrPackage( ushort OpCode, string classComment )
        {
            m_OpCode = OpCode;
            m_ClassComment = classComment;
        }

        /// <summary>
        /// 包的ActionID
        /// 服务器和客户端之间传输包的唯一标识
        /// </summary>
        public ushort OpCode
        {
            get { return m_OpCode; }
            set { m_OpCode = value; }
        }

        /// <summary>
        /// 类注释，有的人懒得写类注释
        /// 通过特性来约定必须告知用户本类的作用
        /// 本值不具备任何实际意义
        /// 仅作为约束
        /// </summary>
        public string ClassComment
        {
            get { return m_ClassComment; }
            set { m_ClassComment = value; }
        }



    }
}
