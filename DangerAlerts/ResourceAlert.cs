// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DangerAlerts
{
    [Serializable]
    class ResourceAlert : AlertBase
    {

        public string ResourceString;
        public int Percentage;


        private HashSet<Part> activeStageParts = new HashSet<Part>();
        private PartSet partSet = null;

        private bool stagePartsChanged = true;
        public void ClearActiveStageParts()
        {
            activeStageParts.Clear();
            stagePartsChanged = true;
        }

        void checkStageParts()
        {
            if (stagePartsChanged)
            {
                if (partSet == null)
                {
                    partSet = new PartSet(activeStageParts);
                }
                else
                {
                    partSet.RebuildParts(activeStageParts);
                }
                stagePartsChanged = false;
            }
        }


        public ResourceAlert(string resourceString, byte percentage)
        {
            ResourceString = resourceString;
            Percentage = percentage;
            priority = AlertPriorities.HIGH;
        }


        public override bool Triggered(Vessel currentVessel)
        {
            int id = PartResourceLibrary.Instance.GetDefinition(ResourceString).id;
            double amt, maxAmt;
            currentVessel.GetConnectedResourceTotals(id, out amt, out maxAmt);
            if (amt < maxAmt * (Percentage * 0.01))
            {
                return true;
            }

            return false;
        }


    }
}
