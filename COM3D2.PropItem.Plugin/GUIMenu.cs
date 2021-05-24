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

        public static bool isOnGUI = false;

        private static Rect windowRect = new Rect(10, 10, 650f, 600f);
        private static int windowId = new System.Random().Next();

        public GUIMenu()
        {

        }

        public static void init()
        {
            style.fixedWidth = 50;
            style.fixedHeight= 50;
            style.active.background = Texture2D.blackTexture;
            style.onActive.background = Texture2D.blackTexture;
        }

        public static void OnGUI()
        {
            GUI.skin = null;

            // 화면 밖으로 안나가게 조정
            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 20, Screen.width - 20);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 20, Screen.height - 20);

            windowRect = GUI.Window(windowId, windowRect, GUIMenu.WindowFunction, "PropItem");
        }
        //
        // private const long w=50L;
        // private const long w2=500L;
        // private const long w3=100L;
        private static Vector2 scrollPosition;
        private static Vector2 scrollPosition2;
        private static Rect position = new Rect(0, 0, 50L, 50L);
        private static Rect position1 = new Rect(150, 0, 500, 600);
        private static Rect position2 = new Rect(0, 0, 500, 600);
        private static long index;
        private static long count = 9;

        //public static Dictionary<int, SMenuItem> m_menuRidDic=new   Dictionary<int, SMenuItem>();
        private static MPN now;
        private static int selected;
        private static Texture2D[] texture2D;
        private static GUIStyle  style=new GUIStyle();

        public static void WindowFunction(int id)
        {
            if (UtillMenu.isLoadMenuFiles && !isOnGUI)
            {
                return;
            }

            GUI.enabled = true;

            GUILayout.BeginHorizontal(GUILayout.Width(650f));


            #region left

            //GUILayout.BeginVertical(GUILayout.Width(100));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(150), GUILayout.Height(600));

            foreach (var item in UtillMenu.menuRidDic)
            {
                if (GUILayout.Button(item.Key.ToString()))
                {
                    logger.LogMessage(item.Key + " , " + item.Value.Count);
                    now = item.Key;
                    texture2D=UtillMenu.menuRidDic[now].Select(x => x.m_texIconRef).ToArray();
                }
            }

            GUILayout.EndScrollView();

            //GUILayout.EndVertical( );

            #endregion

            #region right

            //GUILayout.BeginVertical(GUILayout.Width(500));


            //foreach (KeyValuePair<int, SMenuItem> item in UtillMenu.m_menuRidDic)
            if (UtillMenu.menuRidDic.ContainsKey(now))
            {
                position2.height = UtillMenu.menuRidDic[now].Count / count + 1;
                scrollPosition2 = GUI.BeginScrollView(position1,scrollPosition2, position2, false,true);//, GUILayout.Width(500), GUILayout.Height(600)
                index = 0;
                foreach (var item in UtillMenu.menuRidDic[now])
                //foreach (var item in UtillMenu.m_menuRidDic)
                {
                    position.y = Math.DivRem(index++, count, out long x) * 50L;
                    position.x = x * 50L;
                    /*
                    bool flag3 = GUILayout.Button(item.Value.m_texIconRef,  GUILayout.Width(50), GUILayout.Height(50));
                    */
                    if (GUI.Button(position, item.m_texIconRef))
                    {
                        logger.LogMessage(item.m_mpn + " , " + item.m_strMenuFileName + " , " + item.m_strCateName);
                    }
                }

                //selected= GUILayout.SelectionGrid(selected, texture2D, 9,style, GUILayout.Width(500));
                //if (GUI.changed)
                //{
                //    logger.LogMessage(selected + " , " + UtillMenu.menuRidDic[now].ElementAt(selected).m_strMenuName + " , " + UtillMenu.menuRidDic[now].ElementAt(selected).m_strCateName);
                //}
                GUI.EndScrollView();
            }


            //GUILayout.EndVertical();

            #endregion

            GUILayout.EndHorizontal();

            GUI.DragWindow();
            GUI.enabled = true;
        }
    }
}
