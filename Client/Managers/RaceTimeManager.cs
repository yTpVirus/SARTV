using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;
using static Client.vData.RAM;
using CitizenFX.Core;
namespace Client.Managers
{
    class RaceTimeManager : BaseScript
    {
        private static float RaceTime = 0;
        private static bool Counting = false;
        //Counter
        static Vector2 v = new Vector2(0.16f,0.95f);
        static float[] s = {0.5f,0.5f};
        static int[] r = {255,255,255,255};
        static int[] e = {2,0,0,0,255};
        //

        public RaceTimeManager()
        { CountRaceTime(); }

        public static void StartRaceTimer()
        {
            Counting = true;
            RaceTime = 0;
        }

        private static async void CountRaceTime()
        {
            while (true)
            {   
                await Delay(0);
                if (Counting)
                {
                    RaceTime += Game.LastFrameTime;
                    var a = TimeSpan.FromSeconds(RaceTime);
                    if (a.Milliseconds < 100) { DrawLabel(v, s, 0, true, r, e, $"T: {a.Minutes}m: {a.Seconds}s: 0{a.Milliseconds}ms"); }
                    else { DrawLabel(v, s, 0, true, r, e, $"T: {a.Minutes}m: {a.Seconds}s: {a.Milliseconds}ms"); }
                }
            }
        }

        public static TopTime StopAndGetRaceTimeCount()
        {
            Counting = false;
            var veh = Game.PlayerPed.CurrentVehicle;
            TimeSpan a = TimeSpan.FromSeconds(RaceTime);
            TopTime top = new TopTime(Game.Player.Name,RaceManager.RaceName,veh.ClassLocalizedName,veh.LocalizedName,(float)a.TotalMilliseconds);
            return top;
        }
    }
}
