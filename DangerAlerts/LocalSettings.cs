using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;

namespace DangerAlerts
{
    public class LocalSettings
    {
        string SAVEDIR = DangerAlertCore.GAMEDATA_FOLDER + "/DangerAlerts/PluginData/";
        string SAVEFILE = "settings.cfg";
        const string NODENAME = "DangerAlerts";

        internal string selectedSound;

        public void SaveSettings()
        {
            if (!System.IO.Directory.Exists(SAVEDIR))
                System.IO.Directory.CreateDirectory(SAVEDIR);
            ConfigNode logFile = new ConfigNode();
            ConfigNode log = new ConfigNode();
            logFile.SetNode(NODENAME, log, true);
           
            log.AddValue("selectedSound", selectedSound);

            foreach (var ra in DangerAlertList.Instance.ResourceAlertList)
            {
                ConfigNode raNode = new ConfigNode();
                raNode.AddValue("resourceString", ra.ResourceString);
                raNode.AddValue("percentage", ra.Percentage);
                raNode.AddValue("alertSound", ra.alertSound);
                log.AddNode("Resource", raNode);
            }


            logFile.Save(SAVEDIR + SAVEFILE);
        }

         string SafeLoad(string value, string oldvalue)
        {
            if (value == null)
                return oldvalue;
            return value;
        }
          string SafeLoad(string value, int oldvalue)
        {
            if (value == null)
                return oldvalue.ToString();
            return value;
        }
        public void LoadSettings()
        {
            ConfigNode log;
            ConfigNode logFile = ConfigNode.Load(SAVEDIR + SAVEFILE);

            if (logFile != null)
            {
                log = logFile.GetNode(NODENAME);
                if (log == null)
                    return;
                selectedSound = SafeLoad(log.GetValue("selectedSound"), DangerAlertCore.normalAlert);
                DangerAlertCore.normalAlert = selectedSound;

                ConfigNode[] resourceAlerts = log.GetNodes();
                if (resourceAlerts.Count() > 0)
                    DangerAlertList.Instance.ResourceAlertList.Clear();
                foreach (var entry in resourceAlerts)
                {
                    var resourceString = SafeLoad(entry.GetValue("resourceString"), "");
                    var percentage = int.Parse(SafeLoad(entry.GetValue("percentage"), 0));
                    var alertSound = SafeLoad(entry.GetValue("alertSound"), DangerAlertCore.normalAlert);
                    
                    DangerAlertList.Instance.AddAlert(new ResourceAlert(resourceString, (byte)percentage, alertSound));
                    
                }
            }
            else
                selectedSound = DangerAlertCore.normalAlert;
            
        }
    }
}
