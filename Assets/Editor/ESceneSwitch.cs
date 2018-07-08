using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESceneSwitch : EditorWindow
{
    private struct SSceneData
    {
        public string Name;
        public string SceneURL;
    }

    /// <summary>
    /// 场景文件夹
    /// </summary>
    private const string SceneURL = "Assets/Scenes";

    //Key 场景名 value 场景资源路径
    private Dictionary<string, List<SSceneData>> m_GameScenes = null;


    private static ESceneSwitch myWindow = null;
    private Vector2 m_ScrollPosition;

    private string[] ToolBarNames = null;

    private int m_NowSelectToolBar = 0;

    [MenuItem ( "Tools/场景切换工具" )]
    private static void ShowWindow()
    {
        // 弹出窗口
        myWindow = ( ESceneSwitch ) EditorWindow.GetWindow ( typeof ( ESceneSwitch ), false, "场景切换工具", true );
        myWindow.Show();

        return;
    }

    private void Awake()
    {
        Debug.Log ( "Init 场景切换工具 Successful!" );
        PerpareData();
    }

    /// <summary>
    /// 准备数据
    /// </summary>
    public void PerpareData()
    {
        m_GameScenes = new Dictionary<string, List<SSceneData>>();

        DirectoryInfo di = new DirectoryInfo ( SceneURL );
        DirectoryInfo[] dis = di.GetDirectories();

        m_GameScenes.Add ( "NonType", new List<SSceneData>() );
        int i = 0;

        for ( i = 0 ; i < dis.Length ; i++ )
        {
            m_GameScenes.Add ( dis[i].Name, new List<SSceneData>() );
        }

        ToolBarNames = new string[m_GameScenes.Count];
        i = 0;

        foreach ( KeyValuePair<string, List<SSceneData> > item in m_GameScenes )
        {
            ToolBarNames[i++] = item.Key;
        }

        string[] Guids = AssetDatabase.FindAssets ( "t:Scene", new string[] { SceneURL } );

        //转路径
        for (  i = 0; i < Guids.Length; i++ )
        {
            Guids[i] = AssetDatabase.GUIDToAssetPath ( Guids[i] );
            string[] temps = Guids[i].Split ( '/' );

            string className = temps[2];
            Object data = AssetDatabase.LoadAssetAtPath<Object> ( Guids[i] );

            SSceneData ssd = new SSceneData();
            ssd.Name = data.name;
            ssd.SceneURL = Guids[i];

            if ( m_GameScenes.ContainsKey ( className ) )
            {

                m_GameScenes[className].Add ( ssd );
            }
            else
            {
                m_GameScenes["NonType"].Add ( ssd );
            }

            Resources.UnloadAsset ( data );
        }

    }

    /// <summary>
    /// 绘制窗口
    /// </summary>
    private void OnGUI()
    {
        if ( m_GameScenes == null )
        {
            PerpareData();
        }

        int temp = GUILayout.Toolbar ( m_NowSelectToolBar, ToolBarNames, GUILayout.Height ( 30 ) );

        if ( temp != m_NowSelectToolBar )
        {
            PerpareData();
            m_NowSelectToolBar = temp;
        }

        GUILayout.Space ( 10 );

        m_ScrollPosition = EditorGUILayout.BeginScrollView ( m_ScrollPosition );

        List<SSceneData> SceneList = m_GameScenes[ToolBarNames[m_NowSelectToolBar]];

        for ( int i = 0 ; i < SceneList.Count ; i++ )
        {
            if ( GUILayout.Button ( SceneList[i].Name, GUILayout.Height ( 25 ) ) )
            {
                EditorSceneManager.OpenScene ( SceneList[i].SceneURL );
                Debug.Log ( string.Format ( "Switch scene to {0}", SceneList[i].Name ) );
            }
        }

        EditorGUILayout.EndScrollView();

    }

}

