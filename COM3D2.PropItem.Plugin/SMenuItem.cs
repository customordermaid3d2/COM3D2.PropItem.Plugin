using BepInEx.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.PropItem.Plugin
{
    internal class SMenuItem
    {
        public static ManualLogSource logger;

        public string m_strMenuFileName;
        public int m_nMenuFileRID;
        public Texture2D m_texIcon;
        public bool m_bMan;
        public MPN m_mpn;
        public string m_strCateName;
        internal string m_strMenuName;
        internal MPN m_eColorSetMPN;
        internal string m_strMenuNameInColorSet;
        internal MaidParts.PARTS_COLOR m_pcMultiColorID;
        internal bool m_bMod;
        internal float m_fPriority;
        internal List<SMenuItem> m_listMember;
        internal bool m_bGroupLeader;

        public Texture2D m_texIconRef
        {
            get
            {
                return (!(this.m_texIcon != null) && !(SceneEdit.Instance == null)) ? SceneEdit.Instance.editItemTextureCache.GetTexter(this.m_nMenuFileRID) : this.m_texIcon;
            }
        }
    }
}