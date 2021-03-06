﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

namespace CoffeeBean
{
    /// <summary>
    /// Image的URL Loader
    /// 加载的图片会自动以MD5的形式保存本地
    /// 下次加载会直接读取本地文件提高速度
    /// <code>
    ///     Image image = .......
    ///     <para>image.LoadURLImage( "http://61.183.69.235:7001/hx/uploadFiles/default/subject/20151019081139869433.jpg" );</para>
    /// </code>
    /// </summary>
    public static class CImageURLLoader
    {
        /// <summary>
        /// Image this扩展，读取图片
        /// </summary>
        /// <param name="image">Imagte组件</param>
        /// <param name="URL">网络URL头像地址</param>
        /// <param name="LoadCompleteCallBack">加载完毕回调</param>
        public static void LoadURLImage( this Image image, string URL, DelegateURLImageLoadComplete LoadCompleteCallBack = null )
        {
            if( URL == null || URL == "" )
            {
                return;
            }

            //文件类型
            string TargetFileType = GetFileType( URL );

            //得到文件名
            string TargetFileName = GetMD5HeadName( URL ) + "." + TargetFileType;

            //本地文件名，用于判断文件是否存在
            string LoacalFileName = Application.persistentDataPath + "/" + TargetFileName;

            //判断本地文件是否存在
            FileInfo FI = new FileInfo( LoacalFileName );

            //存在就读取本地文件，不存在就读取网络文件
            EHttpLocation EHT = FI.Exists ? EHttpLocation.LOCAL : EHttpLocation.REMOTE;

            //读取完毕回调
            DelegateHttpLoadComplete OnComplete = ( WWW HttpObj, bool isSuccess ) =>
            {
                try
                {
                    //加载失败时的回调
                    if ( !isSuccess )
                    {
                        if ( LoadCompleteCallBack != null )
                        {
                            LoadCompleteCallBack( isSuccess );
                        }

                        return;
                    }

                    //加载成功，但是头像已销毁
                    if ( image == null )
                    {
                        return;
                    }

                    //得到Texture
                    Texture2D texture = HttpObj.texture;

                    if ( texture == null )
                    {
                        return;
                    }

                    //设置Image图片精灵
                    image.sprite = Sprite.Create( texture, new Rect( 0, 0, texture.width, texture.height ), Vector2.zero );

                    //只有远程文件才需要保存
                    if ( EHT == EHttpLocation.REMOTE )
                    {
                        //得到字节流
                        byte[] ImageData = null;
                        if ( TargetFileType == "jpg" || TargetFileType == "jpeg" )
                        {
                            ImageData = texture.EncodeToJPG();
                        }
                        else if ( TargetFileType == "png" )
                        {
                            ImageData = texture.EncodeToPNG();
                        }

                        //保存路径
                        string SaveURl = Application.persistentDataPath + "/" + TargetFileName;
                        CLOG.I( "read http remote data complete , Start save image to {0}!", SaveURl );

                        //保存文件
                        if ( ImageData != null )
                        {
                            File.WriteAllBytes( SaveURl, ImageData );
                        }
                        else
                        {
                            CLOG.E( "Write file {0} error!", SaveURl );
                        }
                    }

                    //执行回调
                    if ( LoadCompleteCallBack != null )
                    {
                        LoadCompleteCallBack( isSuccess );
                    }
                }
                catch ( Exception ex )
                {
                    CLOG.I( "**********  don't warry it's OK  ************" );
                    CLOG.E( ex.ToString() );
                    CLOG.I( "*********************************************" );
                }

            };

            //读取中回调
            DelegateHttpLoading OnLoading = null;
            /* (WWW HttpObj) =>
             {

             };
            */

            //创建HTTP加载器
            if( EHT == EHttpLocation.REMOTE )
            {
                //加载网络头像
                CHttp.Instance.CreateHttpLoader( URL, OnComplete, OnLoading, EHT );
            }
            else if( EHT == EHttpLocation.LOCAL )
            {
                //加载本地文件
                CHttp.Instance.CreateHttpLoader( TargetFileName, OnComplete, OnLoading, EHT );
            }
        }


        /// <summary>
        /// 得到网络路径的文件路径
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private static string GetMD5HeadName( string URL )
        {
            return URL.MD5();
        }

        /// <summary>
        /// 得到文件类型
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private static string GetFileType( string URL )
        {
            int LastSlash = URL.LastIndexOf( '.' );
            return URL.Substring( LastSlash + 1 );
        }

    }
}
