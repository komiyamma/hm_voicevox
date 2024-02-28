using Microsoft.Win32;

public partial class HmVoiceVoxSpeak
{

    static string GetVoicevoxInstallLocation()
    {
        const string keyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{3BD843E3-5A0C-48B6-A099-7C95833D43E6}_is1";

        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
        {
            if (key != null)
            {
                Object installLocation = key.GetValue("InstallLocation");
                if (installLocation != null)
                {
                    return installLocation.ToString();
                }
            }
        }

        return null;
    }
}