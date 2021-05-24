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
        public string m_strMenuName;
        public MPN m_eColorSetMPN;
        public string m_strMenuNameInColorSet;
        public MaidParts.PARTS_COLOR m_pcMultiColorID;
        public bool m_bMod;
        public float m_fPriority;
        public List<SMenuItem> m_listMember;
        public bool m_bGroupLeader;
        internal bool m_bMember;
        internal SMenuItem m_leaderMenu;
        internal bool m_collabo;

        public Texture2D m_texIconRef
        {
            get
            {
                return (!(this.m_texIcon != null) && !(GameUty.FileSystem == null)) 
                    ? UtillMenu.editItemTextureCache.GetTexter(this.m_nMenuFileRID) : this.m_texIcon;
            }
        }
    }
}