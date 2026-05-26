namespace CyberSecurityAwarenessBot.Helpers
{
    
    /// Plays the WAV voice greeting on startup.
    
    public class AudioHelper
    {
        private const string WavFile = "voiceGreeting.wav";

        public void PlayVoiceGreeting()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, WavFile);
                if (!File.Exists(path)) return;
                if (OperatingSystem.IsWindows()) PlayWav(path);
            }
            catch { /* fail silently */ }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void PlayWav(string path)
        {
            using var p = new System.Media.SoundPlayer(path);
            p.PlaySync();
        }
    }
}
