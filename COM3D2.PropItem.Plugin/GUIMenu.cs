using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;

namespace COM3D2.PropItem.Plugin
{
    class GUIMenu
    {
        public static ManualLogSource logger;

        public static bool isOnOff = false;

        private static Rect windowRect = new Rect(10, 10, 600f, 600f);
        private static int windowId = new System.Random().Next();



        public static void OnGUI()
        {
            GUI.skin = null;

            // 화면 밖으로 안나가게 조정
            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 20, Screen.width - 20);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 20, Screen.height - 20);

            windowRect = GUILayout.Window(windowId, windowRect, GUIMenu.WindowFunction, "PropItem");
        }

        private const long w=50L;
        private static Vector2 scrollPosition;
        private static Vector2 scrollPosition2;
        private static Rect position=new Rect(0,0,w,w);
        private static long index;
        private static long count = 550L / w;

        //public static Dictionary<int, SMenuItem> m_menuRidDic=new   Dictionary<int, SMenuItem>();
        private static MPN now;

        public static void WindowFunction(int id)
        {
            if (UtillMenu.isLoadMenuFiles && isOnOff)
            {
                return;
            }

            GUI.enabled = true;

            GUILayout.BeginHorizontal();


            #region left

            GUILayout.BeginVertical(GUILayout.Width(50));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var item in UtillMenu.menuRidDic)
            {
                if (GUILayout.Button(item.Key.ToString()))
                    now = item.Key;
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical( );

            #endregion

            #region right

            GUILayout.BeginVertical(GUILayout.Width(550));

            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);

            //foreach (KeyValuePair<int, SMenuItem> item in UtillMenu.m_menuRidDic)
            if (UtillMenu.menuRidDic.ContainsKey(now))
            foreach (var item in UtillMenu.menuRidDic[now])
            //foreach (var item in UtillMenu.m_menuRidDic)
            {
                position.y = Math.DivRem(index++, count, out long x) * w;
                position.x = x * w;
                /*
                bool flag3 = GUILayout.Button(item.Value.m_texIconRef,  GUILayout.Width(50), GUILayout.Height(50));
                */
                if (GUI.Button(position, item.m_texIconRef))
                {
                    logger.LogMessage(item.m_mpn + " , " + item.m_strMenuFileName + " , " + item.m_strCateName);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            #endregion

            GUILayout.EndHorizontal();

            GUI.DragWindow();
            GUI.enabled = true;
        }
    }
}
