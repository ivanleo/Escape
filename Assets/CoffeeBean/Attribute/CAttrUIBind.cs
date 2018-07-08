using System;
using System.Text;

namespace CoffeeBean {
    /// <summary>
    /// UI特性
    /// 指定一个UI类的预制体
    /// 通过UI模组创建该UI时
    /// 会自动处理
    /// </summary>
    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class CAttrUIBind : Attribute {
        private string m_PrefabName;
        private bool m_IsSigleton;
        private string m_Description;


        /// <summary>
        /// 无参构造，本特性的使用方法为
        /// [CUIPrefabBind( ActionID=0x1001, ClassComment="登陆消息" )]
        /// </summary>
        public CAttrUIBind()
        {

        }

        /// <summary>
        /// 带参构造，本特性的使用方法为
        /// [CUIPrefabBind(0x1001，"登陆消息")]
        /// </summary>
        /// <param name="prefab">预制体名字id</param>
        /// <param name="sigleton">是否单例子</param>
        public CAttrUIBind ( string prefab, bool sigleton )
        {
            m_PrefabName = prefab;
            m_IsSigleton = sigleton;
        }

        /// <summary>
        /// 带参构造，本特性的使用方法为
        /// [CUIPrefabBind(0x1001，"登陆消息")]
        /// </summary>
        /// <param name="prefab">预制体名字id</param>
        /// <param name="sigleton">是否单例子</param>
        /// <param name="description">描述</param>
        public CAttrUIBind ( string prefab, bool sigleton, string description )
        {
            m_PrefabName = prefab;
            m_IsSigleton = sigleton;
            m_Description = description;
        }

        /// <summary>
        /// 预制体名字
        /// 指定UI目录下的预制体名字，不要带后缀
        /// </summary>
        public string PrefabName
        {
            get { return m_PrefabName; }
            set { m_PrefabName = value; }
        }

        /// <summary>
        /// 是否单例UI
        /// 单例UI会保证在屏幕上只存在一个UI，如特定界面等
        /// 非单例UI则允许被创建多个来显示，如滚动区的节点，MessageBox等
        /// </summary>
        public bool IsSigleton
        {
            get { return m_IsSigleton; }
            set { m_IsSigleton = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
    }
}
