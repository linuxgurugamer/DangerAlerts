// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;


namespace DangerAlerts
{
    class DangerAlertSettings
    {
        private static DangerAlertSettings instance = null;
        public static DangerAlertSettings Instance //Singleton interface
        {
            get
            {
                if (instance == null)
                {
                    instance = new DangerAlertSettings();
                }
                return instance;
            }
        }



        private string optionFilePath = KSPUtil.ApplicationRootPath + "GameData/DangerAlerts/options.cfg"; //File path

        //public bool MasterToggle = true; //Master toggle shouldn't be persistent, wouldn't want people turning it off, then filing
                                         //a bug report :)

        //[Persistent] public bool SoundToggle = true;
       // [Persistent] public float MasterVolume = 0.5f;
        [Persistent] public GUIWindow Window = GUIWindow.OPTIONS;

        [Persistent] private string cfgVersionLoaded = DangerAlertUtils.Version;

        public const int WIDTH = 450;
        private const int HEIGHT = 400;

        public Rect GUIPosition = new Rect(Screen.width / 2 - WIDTH / 2, Screen.height / 2 - HEIGHT / 2, WIDTH, HEIGHT);



        public void UpdateFromGui(DangerAlertGUI gui)
            //Updates all the setting classes variables from a DangerAlertGUI's textbox values.
        {
            Window = gui.Window;

            GUIPosition = gui.GetPosition();
        }

        public void UpdateFromCfg()
            //Loads in values from filePath's file.
        {
            if (System.IO.File.Exists(optionFilePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(optionFilePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
            }
            else
            {
                DangerAlertUtils.Log("Options file does not exist; using defaults instead");
            }
        }

        

        public void SaveCfg()
            //Saves all [Persistent] variables to the cfg, in filePath
        {
            string tempVersionLoaded = cfgVersionLoaded;

            cfgVersionLoaded = DangerAlertUtils.Version;

            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(optionFilePath);

            cfgVersionLoaded = tempVersionLoaded;
        }
    }
}
