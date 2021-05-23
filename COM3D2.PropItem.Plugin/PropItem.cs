using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace COM3D2.PropItem.Plugin
{
    [BepInPlugin("COM3D2.PropItem.Plugin", "COM3D2.PropItem.Plugin", "0.21.5.24")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]

    public class PropItem : BaseUnityPlugin
    {
        public static PropItem instance;
        public static ManualLogSource logger;

        public PropItem()
        {
            instance = this;
            PropItem.logger = BepInEx.Logging.Logger.CreateLogSource("PropItem");
            MenuUtill.logger = PropItem.logger;
            SMenuItem.logger = PropItem.logger;
        }

        public void Awake()
        {
            logger.LogMessage("=== Awake ===");
            
        }

        public void OnEnable()
        {
            logger.LogMessage("=== OnEnable ===");
        }

        public void Start()
        {
            logger.LogMessage("=== Start ===");
            GameMain.Instance.StartCoroutine(CheckMenuDatabase());
        }


        private System.Collections.IEnumerator CheckMenuDatabase()
        {
            logger.LogMessage("=== CheckMenuDatabase ===");
            while (!GameMain.Instance.MenuDataBase.JobFinished()) yield return null;//new WaitForSeconds(1f); 

            
            new Thread(() => MenuUtill.InitMenuNative()).Start();
        }

        public void Update()
        {

        }

        private Rect windowRect = new Rect(10, 10, 100f, 100f);
        private int windowId = new System.Random().Next();

        public void OnGUI()
        {
            windowRect = GUILayout.Window(windowId, windowRect, WindowFunction, "My Window " + windowId);
        }

        private void WindowFunction(int id)
        {
            GUI.enabled = true;
        }
    }
}
