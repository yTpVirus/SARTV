using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Client.Utils.Utils;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using static Client.vData.RAM;
using static Client.Audio.AudioManager;
using Newtonsoft.Json;

namespace Client.Menus.RC.Managers
{
    public class RCTestManager : BaseScript
    {
        //Validations
        public static int CollRadius = 20;
        //List Vectors
        private static List<Vector3> tv = new List<Vector3>();
        private static List<Vector3> sv = new List<Vector3>();
        private static List<Checkpoint> cC = new List<Checkpoint>();
        //Privats
        private static bool OnTest = false;
        public int cIndex = 0;
        public RCTestManager()
        {
            Tick += RaceTick;
        }

        public static void SetupRace()
        {
            foreach (Vector3 v in CPManager.checks)
            {
                tv.Add(v);
            }
            foreach (Vector3 v in SPManager.vl)
            {
                sv.Add(v);
            }
            SPManager.ClearAllSpawnPoints();
            CPManager.ClearAllCheckPoints();
            SetupSpawnAndCheks();
            OnTest = true;
        }

        private async Task RaceTick()
        {
            if(OnTest == false) { return; }
            Vector3 p = Game.PlayerPed.Position;
            if (tv.Count > 0)
            {
                var d = Vector3.Distance(p, tv[cIndex]);
                if (d < CollRadius)
                {
                    if (cC[cIndex] != cC.Last())
                    {
                        PlayCPSound();   
                        DeleteCheckpoint(cC[cIndex].Handle);
                        cIndex++;
                    }
                    else if (cC[cIndex] == cC.Last())
                    {
                        PlayCPSound();
                        DeleteCheckpoint(cC[cIndex].Handle);
                        CheckFinish();
                    }
                    
                }
            }
            await Delay(50);
        }

        private void CheckFinish()
        {
            Debug.WriteLine("Terminou a Corrida");
            tv.Clear();
            cC.Clear();
            sv.Clear();
            cIndex = 0;
            SetPlayerDimension(0);
            ReloadLastSavedRace();
        }

        private static void SetupSpawnAndCheks()
        {
            Game.PlayerPed.CurrentVehicle.Position = sv[0];
            
            for (int i = 0; i < tv.Count; i++)
            {
                if (tv[i] == tv.Last()) { Checkpoint f = World.CreateCheckpoint(CheckpointIcon.CylinderCheckerboard, tv[i],Vector3.Zero, 10, Color.FromArgb(50, 255, 255, 180)); cC.Add(f); return; }
                Checkpoint c = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, tv[i], tv[i + 1], 10, Color.FromArgb(50, 255, 255, 180));
                cC.Add(c);
            }
        }
    }
}
