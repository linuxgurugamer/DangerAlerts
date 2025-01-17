﻿// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using KSP;

namespace DangerAlerts
{
    internal class AlertSoundPlayer
    {
        public GameObject dangeralertplayer = new GameObject("dangeralertplayer"); //Makes the GameObject
        public FXGroup source; //The source to be added to the object
        public AudioClip loadedClip;
        public AudioClip alternativeClip;
        public int altSoundCount;

        public void PlaySound(bool alternative = false)
        {
            if (alternative)
                source.audio.clip = alternativeClip;
            else
                source.audio.clip = loadedClip;
            source.audio.Play();
        }
        public void SetVolume(float vol)
        {
            source.audio.volume = vol / 100;            
        }
        public void StopSound()
        {
            source.audio.Stop();
        }
        public bool SoundPlaying() //Returns true if sound is playing, otherwise false
        {
            if (source != null && source.audio != null)
            {
                return source.audio.isPlaying;
            }
            else
            {
                return false;
            }
        }
        public void LoadNewSound(string soundPath, int cnt)
        {
            altSoundCount = cnt;
            LoadNewSound(soundPath, true);
        }
        public void LoadNewSound(string soundPath, bool alternative = false)
        {
            if (alternative)
                alternativeClip = GameDatabase.Instance.GetAudioClip(soundPath);
            else
                loadedClip = GameDatabase.Instance.GetAudioClip(soundPath);
        }
        public void Initialize(string soundPath)
        {
            //Initializing stuff;
           // dangeralertplayer = new GameObject("dangeralertplayer");
           source = new FXGroup("dangeralertplayer");
            source.audio = dangeralertplayer.AddComponent<AudioSource>();
            loadedClip = GameDatabase.Instance.GetAudioClip(soundPath);
            Log.Info("Did file stuff.");

            source.audio.volume = 0.5f; 
            source.audio.spatialBlend = 0;
            Log.Info("Initialized Danger Alert Player");
        }
        
    }
}
