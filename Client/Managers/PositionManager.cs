using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using static Client.Utils.Utils;

namespace Client.Managers
{
    class PositionManager:BaseScript
    {
        private static Dictionary<Player, int> players = new Dictionary<Player, int>();
        private static Dictionary<Ped, Tuple<int, float>> srtdlist = new Dictionary<Ped, Tuple<int, float>>();
        public static int Position = 0;
        //Render Pos Only
        static Vector2 vp = new Vector2(0.5f,0.5f);
        static Vector2 vp2 = new Vector2(0.5f,0.55f);
        static Vector2 vp3 = new Vector2(0.5f,0.6f);
        static float[] s = { 0.5f, 0.5f };
        static int[] r = { 255, 255, 255 ,255};
        static int[] e = { 1, 255, 255, 255 };
        public PositionManager()
        {
            EventHandlers["SendLobbyPlayerList"] += new Action<string>(SetRacePlayerList);
            EventHandlers["SetFinished"] += new Action<int>(SetFinished);
        }

        private void SetFinished(int id)
        {
            var plrped = Players[id].Character;
            var newedit = new Tuple<int, float>(RaceManager.cl.Count,0);
            srtdlist[plrped] = newedit;
        }

        private void SetRacePlayerList(string pl )
        {
            players.Clear();
            List<int[]> ps = JsonConvert.DeserializeObject<List<int[]>>(pl);
            ps.ForEach((p)=> { players.Add(Players[p[0]],p[1]);});
        }

        public static int GetPlayerLobbyID(Player player)
        {
            return players[player];
        }
        
        public static Dictionary<Player,int> GetLobbyPlayers()
        {
            return players;
        }
        /// <summary>
        /// Calcula o Offset do Player Baseado na Referência do Próximo CheckPoint
        /// </summary>
        /// <param name="pos">Posição Atual do Veículo</param>
        /// <param name="index">Index Atual</param>
        /// <returns>Retorna float[] [0]= Distancia do Check Atual| [1]= Distancia do próximo checkpoint => retorna 0 caso o próximo chechpoint n exista</returns>
        private static float[] GetDistanceByPositionOffset(Vector3 pos,int index)
        {
            var dis = Vector3.Distance(pos, RaceManager.cl[index]);
            if (index + 1 < RaceManager.cl.Count)
            {
                var next = RaceManager.cl[index + 1];
                var maxd = Vector3.Distance(pos, next);
                float[] res = { dis, maxd};
                return res;
            }
            else
            {
                float[] res = {dis,0,0};
                return res;
            }
        }
        private static void ResetFromCheckPoint()
        {   
            if(RaceManager.cl.Count == 0) { return; }
            var veh = Game.PlayerPed.CurrentVehicle;
            var check = RaceManager.cl[RaceManager.cIndex];
            var dir =  check - veh.Position;
            var dis = Vector3.Distance(veh.Position,check);
            if(veh.IsInWater)
            {
                veh.Rotation = GameMath.DirectionToRotation(dir,0);
                veh.Velocity = Vector3.Zero;
                veh.Position += veh.ForwardVector * (dis - 30);
                veh.PlaceOnGround();
                veh.Position += new Vector3(0,0,4);
            }
        }
        public static async void CheckPlayerPositionOnRace()
        {   
            var MyCar = Game.PlayerPed.CurrentVehicle;
            srtdlist.Clear();
            foreach (var p in players)
            {
                srtdlist.Add(p.Key.Character, new Tuple<int, float>(0, 0));
            }
            while (RaceManager.IsOnRace == true)
            {
                await Delay(0);
                ResetFromCheckPoint();
                foreach (var p in players)
                {
                    var Otherplayer = p.Key.Character;
                    var OtherCar = Otherplayer.CurrentVehicle;
                    var index = srtdlist[p.Key.Character].Item1;
                    if (index <= RaceManager.cl.Count)
                    {
                        var dis = GetDistanceByPositionOffset(OtherCar.Position, index);
                        if (srtdlist.ContainsKey(Otherplayer))
                        {
                            if ((dis[0] < RaceManager.CollRadius || dis[0] > dis[1]) && srtdlist[Otherplayer].Item1 < RaceManager.cl.Count - 1)
                            {
                                var lastEdit = srtdlist[Otherplayer];
                                if (Otherplayer != Game.PlayerPed)
                                {
                                    var newedit = new Tuple<int, float>(lastEdit.Item1 + 1, dis[0]); srtdlist[Otherplayer] = newedit;
                                }
                                else
                                {
                                    var newedit = new Tuple<int, float>(RaceManager.cIndex, dis[0]); srtdlist[Otherplayer] = newedit;
                                }
                            }
                            else
                            { 
                                var lastEdit = srtdlist[Otherplayer];
                                var newedit = new Tuple<int, float>(lastEdit.Item1, dis[0]);
                                srtdlist[Otherplayer] = newedit;
                            };
                        }
                    }
                }
                var sorted = srtdlist.OrderByDescending(x => x.Value.Item1).ThenBy(x => x.Value.Item2).ToDictionary(x => x.Key,x => x.Value);
                var res = sorted.Keys.ToList().IndexOf(Game.PlayerPed) + 1;
                Position = res;
                DrawLabel(vp, s, 0, true, r, e, $"{Position}/{players.Count}");
                DrawLabel(vp2, s, 0, true, r, e, $"INDEX: {sorted[Game.PlayerPed].Item1}  Posição Retida do Seu Player : {sorted[Game.PlayerPed].Item2} ");
                foreach (var sort in sorted){if(sort.Key != Game.PlayerPed) { DrawLabel(vp3, s, 0, true, r, e, $"INDEX: {sorted[sort.Key].Item1}  Posição Retida do Outro Player : {sorted[sort.Key].Item2}");}}
            }
        }
    }
}