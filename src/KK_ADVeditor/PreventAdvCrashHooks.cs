﻿using System;
using System.Linq;
using ADV;
using BepInEx.Logging;
using HarmonyLib;

namespace KK_ADVeditor
{
    internal static class PreventAdvCrashHooks
    {
        /// <summary>
        /// Prevent crashes on badly formed commands, should not be relied on outside of development because users will not have this
        /// </summary>
        [HarmonyFinalizer]
        [HarmonyPatch(typeof(CommandList), nameof(CommandList.Add), typeof(ScenarioData.Param), typeof(int))]
        private static Exception PreventAdvCrash(Exception __exception, ScenarioData.Param item)
        {
            string GetCommandStr() => string.Join(" | ", item.Output().Skip(2).ToArray());
            if (__exception != null)
            {
                var commandInfo = AdvCommandInfo.TryGetCommand(item.Command);
                string commandName = item.Command.ToString();
                if (commandInfo != null)
                    commandName = commandInfo.CommandName;

                AdvEditorPlugin.Logger.Log(LogLevel.Error | LogLevel.Message, $"Crash when running command {commandName}, check log for details. State might be corrupted!");
                AdvEditorPlugin.Logger.Log(LogLevel.Error, $"Crash when parsing ADV command:\n{GetCommandStr()}\nSwallowing exception to prevent softlock, *fix this before distributing or people's games will crash*:\n{__exception}");
            }

            return null;
        }
    }
}