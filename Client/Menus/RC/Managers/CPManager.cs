using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Drawing;

namespace Client.Menus.RC.Managers
{
    class CPManager : BaseScript
    {
        public static List<Vector3> checks = new List<Vector3>();
        public static List<Checkpoint> cl = new List<Checkpoint>();
        private static List<Blip> bl = new List<Blip>();

        public CPManager()
        { }
        public static void ClearAllCheckPoints()
        {
            for (int i = 0; i < cl.Count; i++)
            {
                int ch = cl[i].Handle;
                int bh = bl[i].Handle;
                DeleteCheckpoint(ch);
                RemoveBlip(ref bh);
            }
            cl.Clear();
            bl.Clear();
            checks.Clear();
        }

        public static void ClearLastCheckPoint()
        {
            if (cl.Count == 1)
            {
                int g = cl[0].Handle;
                int n = bl[0].Handle;
                DeleteCheckpoint(g);
                RemoveBlip(ref n);
                cl.Clear();
                bl.Clear();
                checks.Clear();
                return;
            }
            int c = cl.Last().Handle;
            int b = bl.Last().Handle;
            DeleteCheckpoint(c);
            RemoveBlip(ref b);
            cl.Remove(cl.Last());
            bl.Remove(bl.Last());
            checks.Remove(checks.Last());
            
        }

        public static void NewCheckPoint()
        {
            var Player = Game.PlayerPed;
            var p = Player.Position - new Vector3(0, 0, 2);
            if (Player.IsInVehicle())
            {
                Checkpoint c = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, p, Vector3.Zero, 10, Color.FromArgb(50, 255, 255, 180));
                checks.Add(p);
                cl.Add(c);
                Blip b = World.CreateBlip(p, 10);
                b.Color = BlipColor.Blue;
                bl.Add(b);
            }
        }

        public static void ReceiveCheckPoint(Vector3 p)
        {
            Checkpoint c = World.CreateCheckpoint(CheckpointIcon.CylinderDoubleArrow, p, Vector3.Zero, 10, Color.FromArgb(50, 255, 255, 180));
            checks.Add(p);
            cl.Add(c);
            Blip b = World.CreateBlip(p, 10);
            b.Color = BlipColor.Blue;
            bl.Add(b);
        }
    }
}
