using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using Client.Utils;

namespace Client.vData
{
    static class RAM
    {
        public class Race
        {
            public string RaceName = "";
            public string RaceClass = "";
            public List<Vector3> cl = new List<Vector3>();
            public List<Vector3> sl = new List<Vector3>();
            public List<float> Hs = new List<float>();
        }
        public class TopTimes
        {
            private List<TopTime> tl = new List<TopTime>();
            public void InsertTopTime(TopTime time)
            {
                tl.Add(time);
            }
            public List<TopTime> GetTopTimes()
            {
                return tl;
            }
        }

        public struct TopTime
        {
            public string PlayerName { get; set; }
            public string RaceName { get; set; }
            public string Class { get; set; }
            public string Carro { get; set; }
            public float Tempo { get; set; }

            public TopTime(string PlayerName, string RaceName, string Class, string Carro, float Tempo)
            {
                this.PlayerName = PlayerName;
                this.RaceName = RaceName;
                this.Class = Class;
                this.Carro = Carro;
                this.Tempo = Tempo;
            }
        }

        //
        public class ClientData
        {
            public string AccountName = "";
            public string userToken = "";
            public string ServerValitated = "";
            public bool isAdmin = false;
            public bool isSART = false;
            public bool HasAccesToRC = false;
            public List<string[]> RaceList = new List<string[]>();
            public Vehicle CurrentVehicle;
            public string vehs = "";
        }
        //
        public class RadioData
        {
            public List<string> URSL = new List<string>();
            //
            public bool listenOthers = true;
            public bool RadioOnRaces = true;
            public bool SartStream = true;
        }
        //
        public class VehicleData
        {
            public VehicleHash VehicleH;
            public VehicleWheelType WheelType;
            public List<VehicleColor> Colors = new List<VehicleColor>();
            public Dictionary<VehicleModType,int> vehmods = new Dictionary<VehicleModType, int>();
            public Dictionary<int,bool> vehtmods = new Dictionary<int, bool>();
            public Dictionary<VehicleNeonLight, bool> Neons = new Dictionary<VehicleNeonLight, bool>();
            public Dictionary<int, bool> VehicleExtras = new Dictionary<int, bool>();
            public List<int> NeonColorARGB = new List<int>();
            public int HeadlightColor = 0;
        }
        //Instancia Classes Necessárias
        public static ClientData clientData = new ClientData();
        //
    }
}
