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
        private static int windowId = new System.Random().Next();

        private static Rect windowRect = new Rect(10, 10, 650f, 620f);
        private static Vector2 scrollPosition;
        private static Vector2 scrollPosition2;
        private static Rect position = new Rect(0, 0, 50L, 50L);
        private static Rect positionRight = new Rect(160, 20, 480, 600);
        private static Rect viewRectRight = new Rect(0, 0, 460, 600);
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

            #region left

            scrollPosition = GUILayout.BeginScrollView(scrollPosition,false,true, GUILayout.Width(150), GUILayout.Height(500));

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

            #endregion

            #region right

            if (UtillMenu.menuRidDic.ContainsKey(now))
            {
                GUILayout.BeginArea();

                viewRectRight.height = UtillMenu.menuRidDic[now].Count / count + 1;
                scrollPosition2 = GUI.BeginScrollView(positionRight,scrollPosition2, viewRectRight, false,true);//, GUILayout.Width(500), GUILayout.Height(600)
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

                GUI.EndScrollView();
            }

            #endregion

            GUI.DragWindow();
            GUI.enabled = true;
        }
    }
}
