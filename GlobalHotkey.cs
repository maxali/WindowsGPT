using System;
using NHotkey;
using NHotkey.Wpf;

namespace ChatGptApp
{
    public class GlobalHotkey
    {
        public static void RegisterHotkey(string name, System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys, EventHandler<HotkeyEventArgs> handler)
        {
            HotkeyManager.Current.AddOrReplace(name, key, modifierKeys, handler);
        }
    }
}
