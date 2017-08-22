// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KSP;
using System.IO;

namespace DangerAlerts
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class DangerAlertCore : MonoBehaviour
    {
        internal static DangerAlertCore Instance;
        internal static string ROOT_PATH = KSPUtil.ApplicationRootPath;
        internal static string GAMEDATA_FOLDER = ROOT_PATH + "GameData/";

        internal const string SOUND_DIR = "DangerAlerts/Sounds/";
        internal static string defaultAlert = "normalAlert";
        internal static string normalAlert = "normalAlert";
        internal AlertSoundPlayer soundplayer = new AlertSoundPlayer();
        DangerAlertGUI dangerAlertGui;

        private bool inDanger = false;

        public bool AlarmActive = false;

        private bool soundActive = true;

        public bool Paused = false;

        void Start()
        {
            Instance = this;
            Log.Info("Danger Alerts started."); //Lets the user know the add-on was started, DEBUG
            Log.Info("Sound file exists: " + GameDatabase.Instance.ExistsAudioClip(SOUND_DIR + normalAlert));
            soundplayer.Initialize(SOUND_DIR + normalAlert); // Initializes the player, does some housekeeping

            dangerAlertGui = gameObject.AddComponent<DangerAlertGUI>();

            GameEvents.onGamePause.Add(OnPause);
            GameEvents.onGameUnpause.Add(OnUnpause);
            GameEvents.onCrash.Add(onCrash);

        }

        void onCrash(EventReport evtRpt)
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<DangerAlertsSettings>().postCrashSound)
                return;
            string PostCrashSound = "FlatlineDeathSound";
            DangerAlertCore.Instance.soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + PostCrashSound, true);
            DangerAlertCore.Instance.soundplayer.PlaySound(true); //Plays sound
        }

        void OnPause()
        {
            Paused = true;
            if (soundplayer == null)
                return;
            if (soundplayer.SoundPlaying())
            {
                soundplayer.StopSound();
            }
        }

        void OnUnpause()
        {
            Paused = false;
        }


        // Use FixedUpdate since it is not called as often as the other Update functions
        void FixedUpdate()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<DangerAlertsSettings>().masterToggle)
            {
                if (HighLogic.LoadedSceneIsFlight && !Paused)
                {
                    Vessel currentVessel = FlightGlobals.ActiveVessel;
                    soundActive = HighLogic.CurrentGame.Parameters.CustomParams<DangerAlertsSettings>().soundToggle;

                    soundplayer.SetVolume(HighLogic.CurrentGame.Parameters.CustomParams<DangerAlertsSettings>().masterVolume);

                    inDanger = false;
                    foreach (AlertBase alert in DangerAlertList.Instance.AlertList)
                    {
                        if (alert.Enabled && alert.Triggered(currentVessel))
                        {
                            if (!AlarmActive) //alarmActive is to make it so the plugin doesn't keep spamming sound
                            {
                                AlarmActive = true;
                                soundplayer.LoadNewSound(DangerAlertCore.SOUND_DIR + alert.Sound(), HighLogic.CurrentGame.Parameters.CustomParams<DangerAlertsSettings>().resourceAlertRepetition);

                                dangerAlertGui.InDanger(true);
                            }
                            if (soundplayer != null && !soundplayer.SoundPlaying()) //If the sound isn't playing, play the sound.
                            {
                                if (soundActive)
                                {
                                    if (soundplayer.altSoundCount-- > 0)
                                        soundplayer.PlaySound(); //Plays sound
                                }
                            }

                            inDanger = true;
                        }
                    }

                    if (!inDanger)
                    {
                        if (AlarmActive)
                        {
                            AlarmActive = false;
                            dangerAlertGui.InDanger(false);
                            soundplayer.StopSound();
                        }
                    }

                }
            }
        }
    }
}
