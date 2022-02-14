/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;

namespace InfinityCode.uContext
{
    [Serializable]
    public class ProjectBookmark : BookmarkItem
    {
        public override bool isProjectItem
        {
            get { return true; }
        }

        public ProjectBookmark()
        {

        }

        public ProjectBookmark(UnityEngine.Object obj):base(obj)
        {

        }
    }
}