﻿// DangerAlerts v1.1: A KSP mod. Public domain, do whatever you want, man.
// Author: SpaceIsBig42/Norpo (same person)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DangerAlerts
{
    static class DangerAlertUtils
    {

        static public string Version = "1.1";

        static string tag = "[Danger Alerts] ";

       

        static public void LogError(string message)
        {
            message = tag + message;
            Log.Error(message);
        }

        static public void LogWarning(string message)
        {
            message = tag + message;
            Log.Warning(message);
        }
    }
}
