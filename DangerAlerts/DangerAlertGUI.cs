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
        private Rect windowPosition = new Rect(Screen.width / 2 - WIDTH / 2, Screen.height / 2 - HEIGHT / 2, WIDTH, HEIGHT);
        private bool visible = false; //Inbuilt "visible" boolean, in case I need it for something else.

        private Texture2D safeTexture;
        private Texture2D dangerTexture;

        string[] dirEntries;
        List<string> dirEntriesList;


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
               (ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW), safeTexture);

            var ls = new LocalSettings();
            ls.LoadSettings();
            DangerAlertCore.normalAlert = ls.selectedSound;
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
            dirEntries = Directory.GetFiles(DangerAlertCore.GAMEDATA_FOLDER + DangerAlertCore.SOUND_DIR);
            dirEntriesList = new List<string>();
            for (int x = 0; x < dirEntries.Count(); x++)
            {
                string s = dirEntries[x].Substring(0, dirEntries[x].LastIndexOf('.'));
                s = s.Substring(s.LastIndexOf('/') + 1);
                dirEntriesList.Add(s);
            }
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

        void SaveSettings()
        {
            var ls = new LocalSettings();
            ls.selectedSound = DangerAlertCore.normalAlert;
            ls.SaveSettings();
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowPosition = GUILayout.Window(10, windowPosition, OnWindow, "Danger Alerts");
            }
        }

        Vector2 soundFileSelScrollVector;
        int lastSelectedSoundIdx = -1;
        string lastSelectedSoundFile = "";
        void SoundSelectionWindow(int id)
        {
            // Log.Info("DisplayHtmlTemplateSelectionWindow");
            GUIStyle toggleStyle = new GUIStyle(HighLogic.Skin.label);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sound Selection");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            soundFileSelScrollVector = GUILayout.BeginScrollView(soundFileSelScrollVector);
            int cnt = 0;
            foreach (string fileName in dirEntries)
            {
                string s = fileName.Substring(0, fileName.LastIndexOf('.'));
                s = s.Substring(s.LastIndexOf('/') + 1);

                GUILayout.BeginHorizontal();
                if (lastSelectedSoundIdx == -1 && DangerAlertCore.normalAlert == s)
                {
                    toggleStyle.normal.textColor = Color.green;
                    lastSelectedSoundIdx = cnt;
                    lastSelectedSoundFile = s;
                }
                else
                {
                    if (lastSelectedSoundIdx != cnt)
                        toggleStyle.normal.textColor = Color.red;
                    else
                        toggleStyle.normal.textColor = Color.green;
                }

                if (GUILayout.Button(s, toggleStyle))
                {

                    lastSelectedSoundIdx = cnt;
                    lastSelectedSoundFile = s;
                }
                cnt++;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!DangerAlertCore.Instance.soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
            {
                if (GUILayout.Button("Play Alarm"))
                {
                    DangerAlertCore.Instance.soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + lastSelectedSoundFile, true);
                    DangerAlertCore.Instance.soundplayer.PlaySound(true); //Plays sound
                }
            }
            else
            {
                if (GUILayout.Button("Stop Alarm"))
                    DangerAlertCore.Instance.soundplayer.StopSound();
            }



            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", GUILayout.Width(90)))
            {
                DangerAlertCore.normalAlert = lastSelectedSoundFile;

                DangerAlertCore.Instance.soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + DangerAlertCore.normalAlert);
                GuiOff();
                var ls = new LocalSettings();
                ls.selectedSound = DangerAlertCore.normalAlert;
                ls.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(90)))
            {
                lastSelectedSoundIdx = -1;
                GuiOff();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
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


        public const int WIDTH = 650;
        private const int HEIGHT = 400;

        void ShowResourceGUI()
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            textFieldStyle = new GUIStyle(GUI.skin.textField);

            toDel = null;
            textFieldStyle.normal.textColor = Color.green;
            GUILayout.BeginHorizontal(GUILayout.Width(WIDTH));
            GUILayout.Label("Descent Warning:  ");
            GUILayout.Label(DangerAlertCore.normalAlert, textFieldStyle, GUILayout.Width(120));
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                int x = dirEntriesList.FindIndex(s => s == DangerAlertCore.normalAlert);
                if (x == 0)
                    x = dirEntriesList.Count - 1;
                else
                    x--;
                DangerAlertCore.normalAlert = dirEntriesList[x];
            }
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                int x = dirEntriesList.FindIndex(s => s == DangerAlertCore.normalAlert);
                if (x == dirEntriesList.Count - 1)
                    x = 0;
                else
                    x++;
                DangerAlertCore.normalAlert = dirEntriesList[x];
            }
            GUILayout.FlexibleSpace();
            if (!DangerAlertCore.Instance.soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
            {
                if (GUILayout.Button("Preview", GUILayout.Width(60)))
                {
                    DangerAlertCore.Instance.soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + DangerAlertCore.normalAlert, true);
                    DangerAlertCore.Instance.soundplayer.PlaySound(true); //Plays sound
                }
            }
            else
            {
                if (GUILayout.Button("Stop", GUILayout.Width(60)))
                    DangerAlertCore.Instance.soundplayer.StopSound();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);



            GUILayout.BeginHorizontal(GUILayout.Width(WIDTH));
            if (GUILayout.Button("Add Resource"))
            {
                DangerAlertList.Instance.AddAlert(new ResourceAlert("ElectricCharge", 20, DangerAlertCore.defaultAlert));
                resourceIndex = DangerAlertList.Instance.ResourceAlertList.Count - 1; //sets index to last one
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Resource", GUILayout.Width(120));
            GUILayout.Space(75);
            GUILayout.Label("Alarm Sound", GUILayout.Width(150));


            GUILayout.Space(5 + 120);
            GUILayout.Label("%", GUILayout.Width(35));
            GUILayout.FlexibleSpace();
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
                    if (x == resourceList.Count - 1)
                        x = 0;
                    else
                        x++;
                    resourceAlert.ResourceString = resourceList[x];
                }
                GUILayout.FlexibleSpace();



                GUILayout.Label(resourceAlert.alertSound, textFieldStyle, GUILayout.Width(120));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    int x = dirEntriesList.FindIndex(s => s == resourceAlert.alertSound);
                    if (x == 0)
                        x = dirEntriesList.Count - 1;
                    else
                        x--;
                    resourceAlert.alertSound = dirEntriesList[x];
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    int x = dirEntriesList.FindIndex(s => s == resourceAlert.alertSound);
                    if (x == dirEntriesList.Count - 1)
                        x = 0;
                    else
                        x++;
                    resourceAlert.alertSound = dirEntriesList[x];
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
                if (!DangerAlertCore.Instance.soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
                {
                    if (GUILayout.Button("Preview", GUILayout.Width(60)))
                    {
                        DangerAlertCore.Instance.soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + resourceAlert.alertSound, true);
                        DangerAlertCore.Instance.soundplayer.PlaySound(true); //Plays sound
                    }
                }
                else
                {
                    if (GUILayout.Button("Stop", GUILayout.Width(60)))
                        DangerAlertCore.Instance.soundplayer.StopSound();
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndScrollView();
            if (toDel != null)
                DangerAlertList.Instance.ResourceAlertList.Remove(toDel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save and Close"))
            {
                SaveSettings();
                GuiOff();
            }
            GUILayout.EndHorizontal();
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

            SaveSettings();
            //DangerAlertList.Instance.SaveAlertsDat();
        }
    }
}
