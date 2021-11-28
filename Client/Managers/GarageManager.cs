using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;
using Client.Utils;
using static Client.vData.RAM;
using Newtonsoft.Json;
using Client.Menus.Tuning_Admin_;

namespace Client.Managers
{
    class GarageManager : BaseScript
    {
        //garagePos 
        public static List<Vehicle> VehiclesOnSpot = new List<Vehicle>();
        //
        private static Vector3 Outside = new Vector3(1001.5f, -1859.6f, 30.5f);
        //
        Blip Garage = World.CreateBlip(Outside);
        public static bool IsOnGarage = false;
        //
        private Vector4[] spots =
        {
            new Vector4(978.10f,-3002.00f,-40.20f,270),
            //
            new Vector4(955.15f,-3018.32f,-40.27f,270),
            new Vector4(955.15f,-3023.60f,-40.27f,270),
            new Vector4(955.15f,-3028.70f,-40.27f,270),
            new Vector4(959.40f,-3035.30f,-40.27f,000),
            new Vector4(963.50f,-3035.30f,-40.27f,000),
            new Vector4(967.70f,-3035.30f,-40.27f,000),
            new Vector4(971.86f,-3035.30f,-40.27f,000),
            new Vector4(976.00f,-3035.30f,-40.27f,000),
            new Vector4(980.14f,-3035.30f,-40.27f,000),
        };

        public static string[] LDCarro = {"2","t_I","Ligar/Desligar Veículo "};
        public static string[] AFCarro = {"3","~INPUT_JUMP~","Abrir/Fechar Portas "};
        public static string[] E =       {"4","t_E","(Câmera)Zoom + "};
        public static string[] Q =       {"5","t_Q","(Câmera)Zoom - "};
        public static string[] WASD =    {"6","","(WASD) Movimentar Câmera "};
        public static List<string[]> Instructions = new List<string[]>();
        public GarageManager()
        {
            RegisterCommand("int",new Action(EntrarG),false);
            //KeyMaps
            KeyManager.RegisterKeyMap("Ligar/Desligar Veículo (Garagem)","i",new Action(ToggleVehicleEngine));
            KeyManager.RegisterKeyMap("Abrir/Fechar Portas do Veículo (Garagem)","space",new Action(ToggleVehicleDoors));
            Instructions.Add(AFCarro);
            Instructions.Add(LDCarro);
            Instructions.Add(WASD);
            Instructions.Add(E);
            Instructions.Add(Q);
            //Instructions.Add(W);
            //Instructions.Add(A);
            //Instructions.Add(S);
            //Instructions.Add(D);
            //
            Garage.Sprite = BlipSprite.Garage;
            Garage.Name = "Sua Mãe";
            //
            EventHandlers["teste"] += new Action<string>(StartGarage);
            //
            Veryfier();
        }

        private async void Veryfier()
        {
            while (true)
            {
                await Delay(0);
                if(RaceManager.IsOnRace == false && IsOnGarage == false)
                {
                    Garage.Alpha = 255;
                }
                else { Garage.Alpha = 0; }
            }
        }

        private void ToggleVehicleDoors()
        {
            if (!CameraManager.isOnFirstSlot) { return; }
            foreach (var door in VehiclesOnSpot[0].Doors)
            {
                if (!door.IsOpen) { door.Open(false, false);}
                else { door.Close();}
            }
        }

        private void ToggleVehicleEngine()
        {
            if(CameraManager.isOnFirstSlot == false) { return; }
            var car = VehiclesOnSpot[0];
            if (!car.IsEngineRunning) { SetVehicleEngineOn(car.Handle, true, false, false); }
            else { SetVehicleEngineOn(car.Handle, false, false, false); }
        }

        private void SairG()
        {
            SendPlayerToOpenWorld();
            IsOnGarage = false;
        }

        private async void EntrarG()
        {   
            DoScreenFadeOut(500);
            await Delay(1000);
            IsOnGarage = true;
            if (Game.PlayerPed.IsInVehicle()) { Game.PlayerPed.CurrentVehicle.Delete(); }
            TriggerServerEvent("LOAD", clientData.userToken);
        }

        private static void clear()
        {
            foreach (var car in VehiclesOnSpot)
            {
                if (car != VehiclesOnSpot.First()){ car.Delete(); }
            }
            CameraManager.ToggleCamerasRenderOFF();
            CameraManager.DestroyAllCams();
            VehiclesOnSpot.Clear();
        }
        private async void StartGarage(string json)
        {
            clientData.vehs = json;
            Game.PlayerPed.Position = new Vector3(994.5925f, -3002.594f, -39.64699f);
            List<string> list = JsonConvert.DeserializeObject<List<string>>(json);
            VehicleManager.LoadVehicleFromJSON(list, spots);
            while (VehiclesOnSpot.Count != list.Count)
            {
                await Delay(0);
            }
            CameraManager.CreateCams();
            await Delay(500);
            //
            Game.PlayerPed.SetIntoVehicle(VehiclesOnSpot[0],VehicleSeat.Driver);
            SpeedManager.ToggleSpeedo(false);
            //
            CameraManager.FadeCameraIntoDefaultPosition();
            if(VehiclesOnSpot.Count < 2) { Menus.Garage.GarageMenu.changeVeh.Enabled = false; };
            Menus.Garage.GarageMenu.OpenGMenu();
        }

        public static async void SendPlayerToOpenWorld()
        {
            DoScreenFadeOut(500);
            await Delay(600);
            clientData.CurrentVehicle = VehiclesOnSpot[0];
            clear();
            TriggerServerEvent("SetLastUsed",clientData.userToken,clientData.CurrentVehicle.Model.Hash);
            IsOnGarage = false;
            Game.PlayerPed.SetIntoVehicle(clientData.CurrentVehicle, VehicleSeat.Driver);
            Game.PlayerPed.CurrentVehicle.Position = Outside;
            Game.PlayerPed.CurrentVehicle.IsPositionFrozen = true;
            Game.PlayerPed.CurrentVehicle.Heading = 173.00f;
            await Delay(3000);
            Game.PlayerPed.CurrentVehicle.IsPositionFrozen = false;
            SpeedManager.ToggleSpeedo(true);
            SetPlayerDimension(0);
            RemoveHelpInstructions();
            await Delay(3000);
            clientData.CurrentVehicle.ApplyForce(new Vector3(0,0,1));
            DoScreenFadeIn(500);
        }

        public static async void ChangeHostVehicle(int i)
        {
            await Delay(500);
            var v = VehiclesOnSpot[i];
            var vv = VehiclesOnSpot[0];
            //
            var vp = v.Position;
            var vh = v.Heading;
            var vvp = vv.Position;
            var vvh = vv.Heading;
            //
            v.IsPositionFrozen = true;
            vv.IsPositionFrozen = true;
            //
            v.Position = vvp;
            v.Heading = vvh;
            //
            vv.Position = vp;
            vv.Heading = vh;
            //Troca de Lugares
            VehiclesOnSpot.RemoveAt(0);
            VehiclesOnSpot.RemoveAt(i-1);
            //
            VehiclesOnSpot.Insert(0,v);
            VehiclesOnSpot.Insert(i,vv);
            //
            v.IsPositionFrozen = false;
            vv.IsPositionFrozen = false;
            //SetPlayerOnNewVehicle
            Game.PlayerPed.SetIntoVehicle(v,VehicleSeat.Driver);
        }
    }
}
