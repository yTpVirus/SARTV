using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;
using Newtonsoft.Json;
using Client.Managers;
using static Client.vData.RAM;
using System.Drawing;

namespace Client.Managers
{
    class VehicleManager : BaseScript
    {

        public VehicleManager()
        {
            RegisterCommand("Save",new Action(()=> { if (Game.PlayerPed.IsInVehicle()) { SaveVehicleIntoJSON(Game.PlayerPed.CurrentVehicle,true);} }),false);
        }

        private static Dictionary<int,bool> GetVehicleExtras(Vehicle veh)
        {
            Dictionary<int, bool> extras = new Dictionary<int, bool>();
            for (int i = 1; i < 30; i++)
            {
                if (veh.ExtraExists(i)) { extras.Add(i, veh.IsExtraOn(i)); };
            }
            return extras;
        }


        public static void SaveVehicleIntoJSON(Vehicle veh,bool wasCommand)
        {
            if (veh.Exists())
            {
                var car = veh;
                //Gerencia os Parametros Para Serem Salvos
                //Aplica Parametros Importantes antes de Salvar (Holder)
                //
                VehicleData vehicleData = new VehicleData();
                vehicleData.vehmods.Clear();
                vehicleData.vehtmods.Clear();
                vehicleData.Colors.Clear();
                vehicleData.Neons.Clear();
                //Parametros Que Não Podem ser Iterados
                vehicleData.VehicleH = (VehicleHash)car.Model.Hash;
                vehicleData.Colors.Add(car.Mods.TrimColor);//
                vehicleData.Colors.Add(car.Mods.SecondaryColor);//
                vehicleData.Colors.Add(car.Mods.PrimaryColor);//
                vehicleData.Colors.Add(car.Mods.DashboardColor);//
                vehicleData.Colors.Add(car.Mods.PearlescentColor);//
                vehicleData.Colors.Add(car.Mods.RimColor);//
                vehicleData.HeadlightColor = GetVehicleHeadlightsColour(car.Handle);
                //Wheelt
                vehicleData.WheelType = car.Mods.WheelType;
                //Toggles
                vehicleData.vehtmods.Add(18, IsToggleModOn(car.Handle, 18));
                vehicleData.vehtmods.Add(20, IsToggleModOn(car.Handle, 20));
                vehicleData.vehtmods.Add(22, IsToggleModOn(car.Handle, 22));
                //Iteração
                foreach (VehicleMod mod in car.Mods.GetAllMods())
                {
                    vehicleData.vehmods.Add(mod.ModType,mod.Index);
                }
                foreach (var extra in GetVehicleExtras(car))
                {
                    vehicleData.VehicleExtras.Add(extra.Key,extra.Value);
                }
                if (car.Mods.HasNeonLights)
                {
                    vehicleData.Neons.Add(VehicleNeonLight.Front,car.Mods.IsNeonLightsOn(VehicleNeonLight.Front));
                    vehicleData.Neons.Add(VehicleNeonLight.Right,car.Mods.IsNeonLightsOn(VehicleNeonLight.Right));
                    vehicleData.Neons.Add(VehicleNeonLight.Left,car.Mods.IsNeonLightsOn(VehicleNeonLight.Left));
                    vehicleData.Neons.Add(VehicleNeonLight.Back,car.Mods.IsNeonLightsOn(VehicleNeonLight.Back));
                    //Col
                    var c = car.Mods.NeonLightsColor;
                    vehicleData.NeonColorARGB.Add(c.A);
                    vehicleData.NeonColorARGB.Add(c.R);
                    vehicleData.NeonColorARGB.Add(c.G);
                    vehicleData.NeonColorARGB.Add(c.B);
                }
                var json = JsonConvert.SerializeObject(vehicleData,Formatting.Indented);
                Debug.Write(json);
                TriggerServerEvent("SavePlayerData", clientData.userToken,json,(int)vehicleData.VehicleH);
                Notify(3, "Veículo Salvo Com Sucesso!");
            }
        }

        public static async void LoadVehicleFromJSON(List<string> json,Vector4[] spawnpos)
        {   
            List<VehicleData> vehicles = new List<VehicleData>();
            foreach (var j in json)
            {
                VehicleData vehicleData = JsonConvert.DeserializeObject<VehicleData>(j);
                vehicles.Add(vehicleData);
            }
            for (int i = 0; i < vehicles.Count; i++)
            {
                Model m = new Model(vehicles[i].VehicleH);
                while (!m.IsLoaded)
                {
                    await Delay(0);
                    RequestModel((uint)vehicles[i].VehicleH);
                }
                if (m.IsLoaded)
                {
                    Debug.WriteLine($"Modelo {vehicles[i].VehicleH} Carregado Com Sucesso");
                    Vehicle v = await World.CreateVehicle(m, new Vector3(spawnpos[i].X, spawnpos[i].Y, spawnpos[i].Z), spawnpos[i].W);
                    GarageManager.VehiclesOnSpot.Add(v);
                    v.DirtLevel = 0;
                    //
                    v.Mods.WheelType = vehicles[i].WheelType;
                    //
                    v.Mods.TrimColor = vehicles[i].Colors[0];
                    v.Mods.SecondaryColor = vehicles[i].Colors[1];
                    v.Mods.PrimaryColor = vehicles[i].Colors[2];
                    v.Mods.DashboardColor = vehicles[i].Colors[3];
                    v.Mods.PearlescentColor = vehicles[i].Colors[4];
                    v.Mods.RimColor = vehicles[i].Colors[5];
                    //
                    foreach (var extra in vehicles[i].VehicleExtras)
                    {
                        v.ToggleExtra(extra.Key,extra.Value);
                    }
                    //
                    foreach (var neon in vehicles[i].Neons)
                    {
                        v.Mods.SetNeonLightsOn(neon.Key, neon.Value);
                    }
                    var R = vehicles[i].NeonColorARGB[1];
                    var G = vehicles[i].NeonColorARGB[2];
                    var B = vehicles[i].NeonColorARGB[3];
                    SetVehicleNeonLightsColour(v.Handle,R,G,B);
                    SetVehicleHeadlightsColour(v.Handle, vehicles[i].HeadlightColor);
                    //
                    if (vehicles[i].vehmods.Count > 0)
                    {
                        foreach (var mod in vehicles[i].vehmods)
                        {
                            v.Mods.InstallModKit();
                            v.Mods[mod.Key].Index = mod.Value;
                        }
                    }
                    if (vehicles[i].vehtmods.Count > 0)
                    {
                        foreach (var mod in vehicles[i].vehtmods)
                        {
                            v.Mods.InstallModKit();
                            ToggleVehicleMod(v.Handle, mod.Key, mod.Value);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"Occoreu um Erro ao Carregar o Modelo {vehicles[i].VehicleH}");
                }
            }
        }
    }
}
