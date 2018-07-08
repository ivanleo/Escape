using UnityEngine;
namespace CoffeeBean {
    /// <summary>
    /// 音效库
    /// </summary>
    public class CSoundManager : CSingleton<CSoundManager>, IMsgSender
    {
        // 是否允许音乐
        public bool IsEnableMusic { get; set; }

        // 是否允许音效
        public bool IsEnableEffect { get; set; }

        // 音乐播放组件
        private AudioSource m_MusicComp = null;

        // 声音资源路径
        private const string SOUND_URL = "Music/";

        // 背景音乐名
        private const string m_BackgroundMusic = "BackgroundMusic";

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //IsEnableEffect = VUserData.LoadData<int> ( "MUTE" ) == 0;
            IsEnableMusic = IsEnableEffect;

            return;
        }

        /// <summary>
        /// 安全的得到音乐组件
        /// </summary>
        private AudioSource GetMusicComponent()
        {
            if ( m_MusicComp != null )
            {
                return m_MusicComp;
            }

            GameObject MusicOB = new GameObject ( "MusicNode" );
            //记录音源组件
            m_MusicComp = MusicOB.AddComponent<AudioSource>();

            GameObject.DontDestroyOnLoad ( MusicOB );

            return m_MusicComp;
        }

        /// <summary>
        /// 安全的得到音效组件
        /// </summary>
        private AudioSource GetEffectComponent()
        {
            GameObject EffectOB = new GameObject ( "EffectNode" );
            //记录音源组件
            AudioSource AS = EffectOB.AddComponent<AudioSource>();

            return AS;
        }


        /// <summary>
        /// 设置当前音乐音量
        /// </summary>
        /// <param name="Volume">音量 0.0f-1.0f </param>
        public void SetMusicVolume ( float Volume )
        {
            GetMusicComponent().volume = Volume;
        }


        /// <summary>
        /// 停止音乐播放
        /// 播放头回到开始
        /// </summary>
        public void StopMusic()
        {
            GetMusicComponent().Stop();
        }

        /// <summary>
        /// 暂停音乐播放
        /// </summary>
        public void PauseMusic()
        {
            GetMusicComponent().Pause();
        }

        /// <summary>
        /// 播放按钮点击音效
        /// </summary>
        public void PlayButtonClick()
        {
            PlayEffect ( "button" );
        }


        /// <summary>
        /// 恢复音乐播放
        /// </summary>
        public void ResumeMusic()
        {
            GetMusicComponent().Play();
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="IsLoop">是否循环</param>
        public void PlayMusic ( string FileName, bool IsLoop = true )
        {
            if ( !IsEnableMusic )
            {
                return;
            }

            if ( FileName == null || FileName == "" )
            {
                FileName = m_BackgroundMusic;
            }

            AudioSource AS = GetMusicComponent();
            AS.clip = Resources.Load<AudioClip> ( SOUND_URL + FileName );
            AS.loop = IsLoop;
            AS.Play();

        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayBackgroundMusic()
        {
            PlayMusic ( m_BackgroundMusic, true );
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="FileName">文件名</param>
        public void PlayEffect ( string FileName )
        {
            AudioSource AS = GetEffectComponent();
            AS.clip = Resources.Load<AudioClip> ( SOUND_URL + FileName );
            AS.Play();
            GameObject.Destroy ( AS.gameObject, AS.clip.length );
        }

        /// <summary>
        /// 静音
        /// </summary>
        public void Mute()
        {
            GetMusicComponent().mute = true;

            IsEnableEffect = false;
            IsEnableMusic = false;

            //记录静音了
            //VUserData.SaveData<int> ( "MUTE", 1 );

            this.DispatchMessage ( ECustomMessageType.MUTE );
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void MuteCancel()
        {
            GetMusicComponent().mute = false;
            IsEnableEffect = true;
            IsEnableMusic = true;

            //记录静音了
            //VUserData.SaveData<int> ( "MUTE", 0 );
            this.DispatchMessage ( ECustomMessageType.MUTE_CANCEL );
        }

    }

}