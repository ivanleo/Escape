﻿/********************************************************************
    created:    2017/10/26
    created:    26:10:2017   20:53
    filename:   D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Utils\CUtilFunction.cs
    file path:  D:\Work\NanJingMaJiang\trunk\Project\CoffeeBean\Utils
    file base:  CUtilFunction
    file ext:   cs
    author:     Leo

    purpose:    函数工具类
*********************************************************************/
using System;
using System.Reflection;

namespace CoffeeBean
{
    /// <summary>
    /// 函数工具类
    /// </summary>
    public static class CFunction
    {
        /// <summary>
        /// 在一个对象身上通过字符串调用公开的方法
        /// 形如下列的成员方法可用本函数执行反射调用
        ///     public int TestFunc( int a ,int b ){return a+b;}
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Target">目标对象</param>
        /// <param name="Param">参数列表</param>
        /// <returns>函数的返回值</returns>
        public static object CallFunction( string FunctionName, object Target, params object[] Param )
        {

            CAssert.AssertIfNull( FunctionName );
            CAssert.AssertIfNull( Target );

            // 定义泛型类型
            MethodInfo func = Target.GetType().GetMethod( FunctionName );
            CAssert.AssertIfNull( func );

            return func.Invoke( Target, Param );
        }

        /// <summary>
        /// 在一个类型里通过字符串调用一个公开的静态方法
        /// 形如下列的成员方法可用本函数执行反射调用
        ///     public static int TestFunc( int a ,int b ){return a+b;}
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="TargetClass">目标类型</param>
        /// <param name="Param">参数列表</param>
        /// <returns>函数的返回值</returns>
        public static object CallStaticFunction( string FunctionName, Type TargetClass, params object[] Param )
        {
            CAssert.AssertIfNull( FunctionName );
            CAssert.AssertIfNull( TargetClass );

            // 定义泛型类型
            MethodInfo func = TargetClass.GetMethod( FunctionName );

            CAssert.AssertIfNull( func );

            return func.Invoke( null, Param );
        }

        /// <summary>
        /// 在一个对象身上通过字符串调用公开的泛型方法
        /// 形如下列的成员方法可用本函数执行反射调用
        ///     public T TestFunc<T>( T a ,T b ){return a+b;}                                      省略个warning</T>
        /// </summary>
        /// <param name="FunctionName">方法名称</param>
        /// <param name="Target">调用目标</param>
        /// <param name="GenericType">泛型类型</param>
        /// <param name="Param">参数列表</param>
        /// <returns>方法的返回值</returns>
        public static object CallGenericFunction( string FunctionName, object Target, Type GenericType, params object[] Param )
        {
            CAssert.AssertIfNull( FunctionName );
            CAssert.AssertIfNull( Target );
            CAssert.AssertIfNull( GenericType );

            // 定义泛型类型
            MethodInfo func = Target.GetType().GetMethod( FunctionName );

            CAssert.AssertIfNull( func );

            func = func.MakeGenericMethod( GenericType );
            return func.Invoke( Target, Param );
        }


        /// <summary>
        /// 在一个对象身上通过字符串调用公开的泛型数组方法
        /// 形如下列的成员方法可用本函数执行反射调用
        ///     public T TestFunc<T>( T[] a ,T[] b ){return a+b;}                                      省略个warning</T>
        /// </summary>
        /// <param name="FunctionName">方法名称</param>
        /// <param name="Target">调用目标</param>
        /// <param name="GenericType">泛型数组类型</param>
        /// <param name="Param">参数列表</param>
        /// <returns>方法的返回值</returns>
        public static object CallGenericArrayFunction( string FunctionName, object Target, Type[] GenericType, params object[] Param )
        {
            CAssert.AssertIfNull( FunctionName );
            CAssert.AssertIfNull( Target );
            CAssert.AssertIfNull( GenericType );

            // 定义泛型类型
            MethodInfo func = Target.GetType().GetMethod( FunctionName );

            CAssert.AssertIfNull( func );

            func = func.MakeGenericMethod( GenericType );
            return func.Invoke( Target, Param );
        }

        /// <summary>
        /// 在一个类型里通过字符串调用公开的静态泛型方法
        /// 形如下列的成员方法可用本函数执行反射调用
        ///     public static T TestFunc<T>( T a ,T b ){return a+b;}                                       省略个warning</T>
        /// </summary>
        /// <param name="FunctionName">方法名</param>
        /// <param name="TargetClass">目标类型</param>
        /// <param name="GenericType">泛型类型</param>
        /// <param name="Param">参数列表</param>
        /// <returns>函数的返回值</returns>
        public static object CallStaticGenericFunction( string FunctionName, Type TargetClass, Type GenericType, params object[] Param )
        {
            CAssert.AssertIfNull( FunctionName );
            CAssert.AssertIfNull( TargetClass );
            CAssert.AssertIfNull( GenericType );

            // 定义泛型类型
            MethodInfo func = TargetClass.GetMethod( FunctionName );

            CAssert.AssertIfNull( func );

            func = func.MakeGenericMethod( GenericType );
            return func.Invoke( null, Param );
        }
    }
}