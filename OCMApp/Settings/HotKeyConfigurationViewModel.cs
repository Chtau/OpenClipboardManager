using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Settings
{
    public class HotKeyConfigurationViewModel
    {
        public enum Modifier
        {
            None,
            Alt,
            Ctrl,
            NoRepeat,
            Shift,
            Win
        }

        public List<KeyValuePair<int, string>> KeyEnum { get; set; }
        public int KeyEnumSelected { get; set; }

        public List<KeyValuePair<int, string>> ModifierEnum { get; set; }
        public int Modifier1EnumSelected { get; set; }
        public int Modifier2EnumSelected { get; set; }

        public HotKeyConfigurationViewModel()
        {
            KeyEnumSelected = (int)OCMHotKey.Enums.Key.None;
            KeyEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(OCMHotKey.Enums.Key));
            ModifierEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(Modifier));
            Modifier1EnumSelected = (int)Modifier.None;
            Modifier2EnumSelected = (int)Modifier.None;
        }

        public void SetKey(OCMHotKey.Enums.Key key)
        {
            KeyEnumSelected = (int)key;
        }

        public OCMHotKey.Enums.Key GetKey()
        {
            return (OCMHotKey.Enums.Key)KeyEnumSelected;
        }

        public void SetModifier(OCMHotKey.Enums.KeyModifier keyModifier)
        {
            bool mod1Used = false;
            if (keyModifier.HasFlag(OCMHotKey.Enums.KeyModifier.Ctrl))
            {
                Modifier1EnumSelected = (int)Modifier.Ctrl;
                mod1Used = true;
            }
            if (keyModifier.HasFlag(OCMHotKey.Enums.KeyModifier.Alt))
            {
                if (mod1Used)
                {
                    Modifier2EnumSelected = (int)Modifier.Alt;
                }
                else
                {
                    Modifier1EnumSelected = (int)Modifier.Alt;
                    mod1Used = true;
                }
            }
            if (keyModifier.HasFlag(OCMHotKey.Enums.KeyModifier.Shift))
            {
                if (mod1Used)
                {
                    Modifier2EnumSelected = (int)Modifier.Shift;
                }
                else
                {
                    Modifier1EnumSelected = (int)Modifier.Shift;
                    mod1Used = true;
                }
            }
            if (keyModifier.HasFlag(OCMHotKey.Enums.KeyModifier.Win))
            {
                if (mod1Used)
                {
                    Modifier2EnumSelected = (int)Modifier.Win;
                }
                else
                {
                    Modifier1EnumSelected = (int)Modifier.Win;
                    mod1Used = true;
                }
            }
            if (keyModifier.HasFlag(OCMHotKey.Enums.KeyModifier.NoRepeat))
            {
                if (mod1Used)
                {
                    Modifier2EnumSelected = (int)Modifier.NoRepeat;
                }
                else
                {
                    Modifier1EnumSelected = (int)Modifier.NoRepeat;
                    mod1Used = true;
                }
            }
        }

        public OCMHotKey.Enums.KeyModifier GetModifier()
        {
            OCMHotKey.Enums.KeyModifier keyModifier = OCMHotKey.Enums.KeyModifier.None;
            var mod1 = Enum.GetName(typeof(Modifier), Modifier1EnumSelected);
            var mod2 = Enum.GetName(typeof(Modifier), Modifier2EnumSelected);
            bool hasMod1 = false;
            if (Enum.TryParse(mod1, out OCMHotKey.Enums.KeyModifier modi1))
            {
                hasMod1 = true;
                keyModifier = modi1;
            }
            if (Enum.TryParse(mod2, out OCMHotKey.Enums.KeyModifier modi2))
            {
                if (hasMod1)
                {
                    keyModifier = modi1 | modi2;
                }
                else
                    keyModifier = modi2;
            }
            return keyModifier;
        }
    }
}
