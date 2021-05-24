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
            UtillMenu.logger = PropItem.logger;
            SMenuItem.logger = PropItem.logger;
            GUIMenu.logger = PropItem.logger;
        }

        public void Awake()
        {
            logger.LogMessage("=== Awake ===");
            UtillMenu.initList();            
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

            
            new Thread(() => UtillMenu.InitMenuNative()).Start();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                GUIMenu.isOnOff = !GUIMenu.isOnOff;
            }

        }


        public void OnGUI()
        {
            GUIMenu.OnGUI();
        }


    }
}
