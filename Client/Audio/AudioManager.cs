using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Client.Audio
{
    class AudioManager
    {
        AudioManager()
        {
            
        }

        public static void PlayCPSound() { PlaySoundFrontend(-1, "CHECKPOINT_AHEAD", "HUD_MINI_GAME_SOUNDSET", false); }
        public static void PlayFinishSound() { PlaySoundFrontend(-1, "CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET", false); }
        
    }
}
