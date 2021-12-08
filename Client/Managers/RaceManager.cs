using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using static Client.Audio.AudioManager;
using static Client.vData.RAM;
using static Client.Utils.Utils;
using Client.Menus.Lobbys;

namespace Client.Managers
{
    class RaceManager : BaseScript
    {
        public static List<Vector3> cl = new List<Vector3>();
        private static List<Vector3> sl = new List<Vector3>();
        private static List<float> hs = new List<float>();
        //Checks
        //public static List<Checkpoint> cC = new List<Checkpoint>();
        public static string RaceName = "";
        public static string RaceClass = "";
        public static Checkpoint first;
        public static Checkpoint second;
        private static Blip fb;
        private static Blip sb;
        //Modifiers
        public readonly static int CollRadius = 20;
        public static bool OnRace = false;
        public static bool IsOnRace = false;
        public static bool BlockControl = false;
        public static int cIndex = 0;
        public RaceManager()
        {
            Tick += Update;
            EventHandlers["ReceiveRaceFromServer"] += new Action<string, int, int, float, string>(ReceiveRaceFromServer);
            BlockControls();
        }

        private async void BlockControls()
        {
            while (true)
            {
                await Delay(0);
                //DrawHelpText("Press ~b~[F]~s~ to summon an AI",true);
                if (BlockControl)
                {
                    Game.DisableControlThisFrame(0,Control.VehicleExit);
                }
            }
        }

        private async void ReceiveRaceFromServer(string JSON, int index, int dimension,float time, string TopName)
        {
            if (!Game.PlayerPed.IsInVehicle()) { Notify(2, "Você Não Está em um Veículo!"); return; }
            LobbyMenu.HideLobby();
            Race race = JsonConvert.DeserializeObject<Race>(JSON);
            RaceName = race.RaceName;
            RaceClass = race.RaceClass;
            race.cl.ForEach((v) => { cl.Add(v); });
            race.sl.ForEach((v) => { sl.Add(v); });
            race.Hs.ForEach((v) => { hs.Add(v); });
            //SetUpRace
            DoScreenFadeOut(1000);
            await Delay(1000);
            SetPlayerDimension(dimension);
            SetupSpawnAndCheks(index);
            Game.PlayerPed.CurrentVehicle.IsPositionFrozen = true;
            BlockControl = true;
            await Delay(2000);
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            foreach (Player player in PositionManager.GetLobbyPlayers().Keys)
            {   
                if (player != Game.Player)
                {   
                    Debug.WriteLine($"Setando Colisão");
                    player.Character.CurrentVehicle.SetNoCollision(veh, false);
                    DisableCamCollisionForEntity(veh.Handle);
                }
            }
            await Delay(200);
            DoScreenFadeIn(1000);
            //do wait
            var a = TimeSpan.FromMilliseconds(time);
            var times = $"{a.Minutes}m:{a.Seconds}s:{a.Milliseconds}ms";
            WaitForOthers("Aguardando Jogadores",$"Melhor Tempo: {TopName} | {times}");
            SetVehicleIsRacing(veh.Handle,true);
            //
            TriggerServerEvent("OnLobbyPlayerStatusChange",PositionManager.GetPlayerLobbyID(Game.Player),true);
            TriggerServerEvent("CheckEveryoneReady", PositionManager.GetPlayerLobbyID(Game.Player));
            //
            OnRace = true;
            IsOnRace = true;
            PositionManager.CheckPlayerPositionOnRace();
        }
        //Race OnTheGo
        private async Task Update()
        {
            if (OnRace == false) { return; }
            Vector3 p = Game.PlayerPed.CurrentVehicle.Position;
            if (cl.Count > 0)
            {
                var d = Vector3.Distance(p, cl[cIndex]);
                if (d < CollRadius)
                {
                    if (cl[cIndex] != cl.Last())
                    {
                        PlayCPSound();
                        DeleteCheckpoint(first.Handle);
                        DeleteCheckpoint(second.Handle);
                        fb.Delete();
                        sb.Delete();
                        cIndex++;
                        if (cIndex < cl.Count)
                        {
                            
                            if (cIndex < cl.Count - 1) 
                            {
                                Debug.WriteLine("Criando Set 1");
                                first = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, cl[cIndex] - new Vector3(0, 0, 3), cl[cIndex + 1], 10, Color.FromArgb(100, 255, 255, 180));
                                second = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, cl[cIndex + 1] - new Vector3(0, 0, 3),Vector3.Zero, 10, Color.FromArgb(100, 255, 255, 180));
                                SetCheckpointCylinderHeight(first.Handle, 13, 13, 10);
                                SetCheckpointCylinderHeight(second.Handle, 13, 13, 10);
                                SetCheckpointIconRgba(first.Handle, 0, 60, 255, 100);
                                SetCheckpointIconRgba(second.Handle, 0, 0, 0, 0);
                                fb = World.CreateBlip(cl[cIndex],10);
                                fb.ShowRoute = true;
                                fb.Alpha = 255;
                                fb.Color = BlipColor.Yellow;
                                //
                                sb = World.CreateBlip(cl[cIndex + 1], 10);
                                sb.Color = BlipColor.Yellow;
                                sb.Alpha = 255;
                            }
                            else 
                            {
                                Debug.WriteLine("Criando Set 2");
                                first = World.CreateCheckpoint(CheckpointIcon.CylinderCheckerboard, cl[cIndex] - new Vector3(0, 0, 3), Vector3.Zero, 10, Color.FromArgb(100, 255, 255, 180));
                                SetCheckpointCylinderHeight(first.Handle, 13, 13, 10);
                                SetCheckpointIconRgba(first.Handle, 0, 60, 255, 100);
                                fb = World.CreateBlip(cl[cIndex]);
                                fb.ShowRoute = true;
                                fb.Alpha = 255;
                                fb.Sprite = BlipSprite.RaceFinish;
                            }
                        }
                    }
                    else if (cl[cIndex] == cl.Last())
                    {
                        IsOnRace = false;
                        DeleteCheckpoint(first.Handle);
                        fb.Delete();
                        CheckFinish();
                    }
                }
            }
            //manage's the race OnTheRun
            await Delay(0);
        }
        //Check if Player Finished
        private async void CheckFinish()
        {
            Debug.WriteLine("Terminou a Corrida!");
            cl.Clear();
            sl.Clear();
            hs.Clear();
            PlayFinishSound();
            var al = RaceTimeManager.StopAndGetRaceTimeCount();
            var a = TimeSpan.FromMilliseconds(al.Tempo);
            var time = $"{a.Minutes}m:{a.Seconds}s:{a.Milliseconds}ms";
            if (a.Milliseconds < 10) { time = $"{a.Minutes}m:{a.Seconds}s:00{a.Milliseconds}ms"; }
            else if (a.Milliseconds < 100) { time = $"{a.Minutes}m:{a.Seconds}s:0{a.Milliseconds}ms"; }
            DrawTimedTextOnScreen($"Chegou em {PositionManager.Position}°", $"Tempo: {time} | + $dinheiro | Loot: Novo(a) peca Desbloqueado(a)!|", 5000);
            var json = JsonConvert.SerializeObject(al);
            TriggerServerEvent("OnFinishRace",PositionManager.GetPlayerLobbyID(Game.Player),PositionManager.Position,json);
            //
            if (PositionManager.Position < PositionManager.GetLobbyPlayers().Count)
            {
                CameraManager.SpectateARandomPlayerFromLobby();
                DisplayRadar(false);
                SpeedManager.ToggleSpeedo(false);
            }
            //
            OnRace = false;
            Vehicle veh = Game.PlayerPed.CurrentVehicle;
            foreach (Player player in Players)
            {
                Debug.WriteLine($"Setando Colisão");
                if (player != Game.Player)
                {
                    player.Character.CurrentVehicle.SetNoCollision(veh, true);
                }
            }
            SetVehicleIsRacing(veh.Handle, false);
            cIndex = 0;
            await Delay(1000);
            veh.IsPositionFrozen = true;
        }
        //Set up Race
        private static void SetupSpawnAndCheks(int spawnIndex)
        {

            //Put Player on the Desired Spot
            Game.PlayerPed.CurrentVehicle.Position = sl[spawnIndex];
            Game.PlayerPed.CurrentVehicle.Heading = hs[spawnIndex];
            //
            first = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, cl[0] - new Vector3(0, 0, 3), cl[0 + 1], 10, Color.FromArgb(100, 255, 255, 180));
            second = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, cl[1] - new Vector3(0, 0, 3), cl[1 + 1], 10, Color.FromArgb(100, 255, 255, 180));
            SetCheckpointCylinderHeight(first.Handle, 13, 13, 10);
            SetCheckpointIconRgba(first.Handle, 0, 60, 255, 100);
            SetCheckpointCylinderHeight(second.Handle, 13, 13, 10);
            SetCheckpointIconRgba(second.Handle, 0, 0,0,0);
            fb = World.CreateBlip(cl[0],10);
            sb = World.CreateBlip(cl[0],10);
            fb.Color = BlipColor.Yellow;
            fb.Sprite = BlipSprite.BigBlip;
            sb.Color = BlipColor.Yellow;
            fb.Alpha = 255;
            sb.Alpha = 255;
        }
    }
}