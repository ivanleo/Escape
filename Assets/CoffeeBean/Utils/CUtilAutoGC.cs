
using UnityEngine;

namespace CoffeeBean
{
    /// <summary>
    /// 自动垃圾回收
    /// 每隔60帧垃圾回收1次
    /// </summary>
    public class CUtilAutoGC : CSingletonMono<CUtilAutoGC>
    {
        //开始自动垃圾回收
        public void Begin()
        {

        }

        //多少帧自动垃圾回收
        public const int GarbageCollectorFrameCount = 60;
        private void Update()
        {
            if ( Time.frameCount % GarbageCollectorFrameCount == 0 )
            {
                System.GC.Collect();
            }
        }
    }

}
