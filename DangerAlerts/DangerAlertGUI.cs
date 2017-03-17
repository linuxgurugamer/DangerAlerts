// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)

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
    enum GUIWindow
    {
        OVERVIEW,
        OPTIONS,
        COLLISION,
        RESOURCE
    }

    class DangerAlertGUI : MonoBehaviour
    {
        public GUIWindow Window = GUIWindow.OPTIONS;

        int resourceIndex = 0;

        string resourceName = "ElectricCharge";

        string resourcePercentage = "20";

        bool resourceEnabled = true;

        private ApplicationLauncherButton dangerAlertButton;
        private Rect windowPosition = DangerAlertSettings.Instance.GUIPosition;

        private bool visible = false; //Inbuilt "visible" boolean, in case I need it for something else.

        private Texture2D safeTexture;
        private Texture2D dangerTexture;

        void Start()
        {
            //Thank youuuuuu, github!
            safeTexture = new Texture2D(36, 36, TextureFormat.RGBA32, false);
            string safeTextureFile = KSPUtil.ApplicationRootPath + "GameData/DangerAlerts/Icons/safeicon.png";
            safeTexture.LoadImage(File.ReadAllBytes(safeTextureFile));

            dangerTexture = new Texture2D(36, 36, TextureFormat.RGBA32, false);
            string dangerTextureFile = KSPUtil.ApplicationRootPath + "GameData/DangerAlerts/Icons/dangericon.png";
            dangerTexture.LoadImage(File.ReadAllBytes(dangerTextureFile));

            dangerAlertButton = ApplicationLauncher.Instance.AddModApplication(GuiOn, GuiOff, null, null, null, null,
               (ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW), safeTexture);

            DangerAlertList.Instance.UpdateAlertsFromDat();

            DangerAlertSettings.Instance.UpdateFromGui(this);

            //collisionTolerance = DangerAlertList.Instance.CollisionAlertList.First().Tolerance.ToString(); //that's a mouthful
            // collisionMinimumSpeed = DangerAlertList.Instance.CollisionAlertList.First().MinimumSpeed.ToString();
            // collisionMinimumVerticalSpeed = DangerAlertList.Instance.CollisionAlertList.First().MinimumVerticalSpeed.ToString();
            //collisionEnabled = DangerAlertList.Instance.CollisionAlertList.First().Enabled; //This chunk of code is ugly. I should see about cleaning
        }

        public void InDanger(bool danger)
        {
            if (danger)
            {
                dangerAlertButton.SetTexture(dangerTexture);
            }
            else
            {
                dangerAlertButton.SetTexture(safeTexture);
            }
        }

        public void GuiOn()
        {
            visible = true;
            Add();
        }

        public void GuiOff()
        {
            visible = false;
        }
        public List<string> resourceList = new List<string>();

        public void Add()
        {
            resourceList = new List<string>();
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                foreach (PartResource r in p.Resources)
                {
                    if (!resourceList.Contains(r.resourceName))
                        resourceList.Add(r.resourceName);
                }
            }

        }

        private void OnGUI()
        {
            if (visible)
            {
                windowPosition = GUILayout.Window(10, windowPosition, OnWindow, "Danger Alerts");
                DangerAlertSettings.Instance.UpdateFromGui(this);
            }
                
        }

        void OnWindow(int windowId)
        {
            if (visible)
            {
                ShowResourceGUI();
                GUI.DragWindow();
            }
        }

        Vector2 scrollPos;
        ResourceAlert resourceAlert;
        GUIStyle buttonStyle;
        GUIStyle toggleStyle;
        GUIStyle textFieldStyle;
        ResourceAlert toDel;

        void ShowResourceGUI()
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            textFieldStyle = new GUIStyle(GUI.skin.textField);

            toDel = null;

            GUILayout.BeginHorizontal(GUILayout.Width(DangerAlertSettings.WIDTH));
            if (GUILayout.Button("Add Resource"))
            {
                DangerAlertList.Instance.AddAlert(new ResourceAlert("ElectricCharge", 20));
                resourceIndex = DangerAlertList.Instance.ResourceAlertList.Count - 1; //sets index to last one
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Resource", GUILayout.Width(120));
            GUILayout.Space(50);
            GUILayout.FlexibleSpace();
            GUILayout.Label("%", GUILayout.Width(35));
            GUILayout.Space(100);
            GUILayout.EndHorizontal();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < DangerAlertList.Instance.ResourceAlertList.Count; i++)
            {
                
                GUILayout.BeginHorizontal();
                resourceAlert = DangerAlertList.Instance.ResourceAlertList[i];
                if (resourceAlert.Enabled)
                {
                    buttonStyle.normal.textColor = Color.green;
                    toggleStyle.normal.textColor = Color.green;
                    textFieldStyle.normal.textColor = Color.green;
                }
                else
                {
                    buttonStyle.normal.textColor = Color.red;
                    toggleStyle.normal.textColor = Color.red;
                    textFieldStyle.normal.textColor = Color.red;

                }
                GUILayout.Label(resourceAlert.ResourceString, textFieldStyle, GUILayout.Width(120));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    int x = resourceList.FindIndex(s => s == resourceAlert.ResourceString);
                    if (x == 0)
                        x = resourceList.Count - 1;
                    else
                        x--;
                    resourceAlert.ResourceString = resourceList[x];
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    int x = resourceList.FindIndex(s => s == resourceAlert.ResourceString);
                    if (x ==resourceList.Count - 1)
                        x = 0;
                    else
                        x++;
                    resourceAlert.ResourceString = resourceList[x];
                }
                GUILayout.FlexibleSpace();

                float f = resourceAlert.Percentage;
                GUILayout.Label(f.ToString() + "%", GUILayout.Width(35));
                f = GUILayout.HorizontalSlider(f, 0f, 99f, GUILayout.Width(100));
                resourceAlert.Percentage = (int)f;
                resourceAlert.Enabled = GUILayout.Toggle(resourceAlert.Enabled, "", toggleStyle);
                GUILayout.Space(5);
                if (GUILayout.Button("X", GUILayout.Width(20))) 
                    toDel = resourceAlert;
                GUILayout.Space(5);

                GUILayout.EndHorizontal();

            }
            GUILayout.EndScrollView();
            if (toDel != null)
                DangerAlertList.Instance.ResourceAlertList.Remove(toDel);
        }

        bool ResourceValueCheck()
        {
            bool changed = false;

            if (resourceIndex < 0)
            {
                resourceIndex = 0;
                changed = true;
            }
            try
            {
                if (Int32.Parse(resourcePercentage) < 0 || Int32.Parse(resourcePercentage) > 100)
                {
                    resourcePercentage = "50";
                    changed = true;
                }
            }
            catch (FormatException e)
            {
                resourcePercentage = "50";
                changed = true;
            }

            return changed;
        }

        void UpdateResourceAlertVariables()
        {
            ResourceAlert currentAlert = DangerAlertList.Instance.ResourceAlertList[resourceIndex];
            resourceName = currentAlert.ResourceString;
            resourceEnabled = currentAlert.Enabled;
            resourcePercentage = currentAlert.Percentage.ToString();
        }

        public Rect GetPosition()
        {
            return windowPosition;
        }

        void OnDestroy()
        {
            DangerAlertUtils.Log("DangerAlertGUI is being destroyed");
            ApplicationLauncher.Instance.RemoveModApplication(dangerAlertButton);
            DangerAlertList.Instance.SaveAlertsDat();
        }
    }
}
