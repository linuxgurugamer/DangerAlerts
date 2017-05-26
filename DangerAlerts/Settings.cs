using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace DangerAlerts
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class DangerAlertsSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Danger Alerts"; } }
        public override string DisplaySection { get { return "Danger Alerts"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Sound Toggle")]
        public bool soundToggle = true;

        [GameParameters.CustomParameterUI("Master Toggle", toolTip = "Disables all checks")]
        public bool masterToggle = true;

        float masterVol = 0.5f;
        [GameParameters.CustomFloatParameterUI("Master Volume (%)", displayFormat = "N0", minValue = 0, maxValue = 100, stepCount = 1, asPercentage = false)]
        public float masterVolume
        {
            get { return masterVol * 100; }
            set { masterVol = value / 100.0f; }
        }


        [GameParameters.CustomIntParameterUI("Tolerance", minValue = 1, maxValue = 99,
            toolTip = "Multiplier used when calculating time until landing")]
        public int collisionTolerance = 7;

        [GameParameters.CustomIntParameterUI("Min Speed (m/s)", minValue = 1, maxValue = 99,
            toolTip = "Minimum speed before alarms start to sound")]
        public int collisionMinimumSpeed = 7;

        [GameParameters.CustomIntParameterUI("Min Vertical Speed (m/s)", minValue = -99, maxValue = -1)]
        public int collisionMinimumVerticalSpeed = -2;

        [GameParameters.CustomParameterUI("Alarm Enabled")]
        public bool collisionEnabled = true;
        
#if false
        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    toolbarEnabled = true;
                    toolbarPopupsEnabled = true;
                    editorMenuPopupEnabled = true;
                    hoverTimeout = 0.5f;
                    break;

                case GameParameters.Preset.Normal:
                    toolbarEnabled = true;
                    toolbarPopupsEnabled = true;
                    editorMenuPopupEnabled = true;
                    hoverTimeout = 0.5f;
                    break;

                case GameParameters.Preset.Moderate:
                    toolbarEnabled = true;
                    toolbarPopupsEnabled = true;
                    editorMenuPopupEnabled = true;
                    hoverTimeout = 0.5f;
                    break;

                case GameParameters.Preset.Hard:
                    toolbarEnabled = true;
                    toolbarPopupsEnabled = true;
                    editorMenuPopupEnabled = true;
                    hoverTimeout = 0.5f;
                    break;
            }
        }
#endif

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            //if (member.Name == "enabled")
            //    return true;

            return true; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
            //            return true; //otherwise return true
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }

    }
}
