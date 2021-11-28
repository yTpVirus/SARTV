using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuAPI;
using CitizenFX.Core;
using System.Threading.Tasks;
using static Client.Utils.Utils;
using static CitizenFX.Core.Native.API;

namespace Client.Menus.RC.Managers
{
    class SPManager : BaseScript
    {
        public static List<Vehicle> vghostl = new List<Vehicle>();
        public static List<Vector3> vl = new List<Vector3>();
        public static List<float> Hs = new List<float>();
        public SPManager()
        {
        }
        public static void ClearAllSpawnPoints()
        {
            foreach (var car in vghostl)
            {
                int c = car.Handle;
                DeleteVehicle(ref c);
            }
            vghostl.Clear();
            vl.Clear();
        }

        public static void ClearLastCheckPoint()
        {
            int c = vghostl.Last().Handle;
            DeleteVehicle(ref c);
            vghostl.Remove(vghostl.Last());
            vl.Remove(vl.Last());
        }

        public static async void NewSpawnPoint()
        {
            var player = Game.PlayerPed;
            if (player.IsInVehicle())
            {
                if (CPManager.cl.Count > 0)
                {
                    Vehicle mv = player.CurrentVehicle;
                    var vghost = await World.CreateVehicle(mv.Model, mv.Position, mv.Heading);
                    while (!vghost.Model.IsLoaded) {await Delay(0);}
                    vghostl.Add(vghost);
                    vl.Add(mv.Position);
                    Hs.Add(mv.Heading);
                    vghost.IsPositionFrozen = true;
                    vghost.IsCollisionEnabled = false;
                }
                else
                {
                    Notify(1, "Você Não Pode Criar um SpawnPoint Sem ao Menos Ter um Checkpoint Ativo");
                }
            }
        }
    }
}
