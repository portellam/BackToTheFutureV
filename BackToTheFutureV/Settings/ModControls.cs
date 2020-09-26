﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using KlangRageAudioLibrary;
using Control = GTA.Control;

namespace BackToTheFutureV.Settings
{
    public static class ModControls
    {
        public static bool UseControlForMainMenu { get; set; } = true;
        public static Keys MainMenu { get; set; } = Keys.F8;

        public static bool CombinationsForInteractionMenu { get; set; } = true;
        public static Control InteractionMenu1 { get; set; } = Control.VehicleHandbrake;
        public static Control InteractionMenu2 { get; set; } = Control.CharacterWheel;

        public static bool LongPressForHover { get; set; } = true;
        public static Control Hover { get; set; } = Control.VehicleFlyTransform;
        public static Control HoverBoost { get; set; } = Control.VehicleHandbrake;
        public static Control HoverVTOL { get; set; } = Control.VehicleAim;
        public static Keys HoverAltitudeHold { get; set; } = Keys.G;

        public static Keys TCToggle { get; set; } = Keys.Add;
        public static Keys CutsceneToggle { get; set; } = Keys.Multiply;
        public static Keys InputToggle { get; set; } = Keys.Divide;

        public static void LoadControls(ScriptSettings settings)
        {
            UseControlForMainMenu = settings.GetValue("Controls", "UseControlForMainMenu", UseControlForMainMenu);
            MainMenu = settings.GetValue("Controls", "MainMenu", MainMenu);

            CombinationsForInteractionMenu = settings.GetValue("Controls", "CombinationsForInteractionMenu", CombinationsForInteractionMenu);
            InteractionMenu1 = settings.GetValue("Controls", "InteractionMenu1", InteractionMenu1);
            InteractionMenu2 = settings.GetValue("Controls", "InteractionMenu2", InteractionMenu2);

            LongPressForHover = settings.GetValue("Controls", "LongPressForHover", LongPressForHover);
            Hover = settings.GetValue("Controls", "Hover", Hover);
            HoverBoost = settings.GetValue("Controls", "HoverBoost", HoverBoost);
            HoverVTOL = settings.GetValue("Controls", "HoverVTOL", HoverVTOL);
            HoverAltitudeHold = settings.GetValue("Controls", "HoverAltitudeHold", HoverAltitudeHold);

            TCToggle = settings.GetValue("Controls", "TCToggle", TCToggle);
            CutsceneToggle = settings.GetValue("Controls", "CutsceneToggle", CutsceneToggle);
            InputToggle = settings.GetValue("Controls", "InputToggle", InputToggle);

            SaveControls(settings);
        }

        public static void SaveControls(ScriptSettings settings)
        {
            settings.SetValue("Controls", "UseControlForMainMenu", UseControlForMainMenu);
            settings.SetValue("Controls", "MainMenu", MainMenu);

            settings.SetValue("Controls", "CombinationsForInteractionMenu", CombinationsForInteractionMenu);
            settings.SetValue("Controls", "InteractionMenu1", InteractionMenu1);
            settings.SetValue("Controls", "InteractionMenu2", InteractionMenu2);

            settings.SetValue("Controls", "LongPressForHover", LongPressForHover);
            settings.SetValue("Controls", "Hover", Hover);
            settings.SetValue("Controls", "HoverBoost", HoverBoost);
            settings.SetValue("Controls", "HoverVTOL", HoverVTOL);
            settings.SetValue("Controls", "HoverAltitudeHold", HoverAltitudeHold);

            settings.SetValue("Controls", "TCToggle", TCToggle);
            settings.SetValue("Controls", "CutsceneToggle", CutsceneToggle);
            settings.SetValue("Controls", "InputToggle", InputToggle);

            settings.Save();
        }
    }
}
