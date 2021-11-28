using CitizenFX.Core;
using static Client.Menus.Tuning_Admin_.TMainMenu;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;
using System.Threading.Tasks;
using MenuAPI;
using System.Linq;

namespace Client.Managers
{
    class AdminTManager : BaseScript
    {
        private static float WheelWidth = 0;
        public AdminTManager()
        { }
        private static string[] WheelNames = {"Sport","Muscle","Lowrider","SUV","Off-Road","Tuner","HighEnd","Benny's Orig.","Benny's Pers.","Slicks","Street","Street 2","BikeWheels"};
        //Wheel Returner
        public static float ReturnWheel()
        {
            if (WheelWidth != 0) { return WheelWidth;}  
            else { return 0.5f;}   
        }
        public static void SaveValues(int id)
        {
            WheelWidth = GetVehicleWheelWidth(id);
        }
        //
        //MOD
        public static void CycleToPreviusModIndex(int modIndex, out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            var mod = car.Mods.GetAllMods();
            if (mod[modIndex].Index >= 0) {car.Mods.InstallModKit(); mod[modIndex].Index--;}
            if (mod[modIndex].Index < 0) { i = $"{mod[modIndex].LocalizedModName} {mod[modIndex].Index + 1}/{mod[modIndex].ModCount}"; }
            i = $"{mod[modIndex].LocalizedModName} {mod[modIndex].Index + 1}/{mod[modIndex].ModCount}";
            
        }
        public static void CycleToNextModIndex(int modIndex, out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            var mod = car.Mods.GetAllMods();
            if (mod[modIndex].Index < mod[modIndex].ModCount - 1) {car.Mods.InstallModKit(); mod[modIndex].Index++; }
            i = $"{mod[modIndex].LocalizedModName} {mod[modIndex].Index + 1}/{mod[modIndex].ModCount}";
        }
        //Wheels
        public static string CycleToNextWheelType(int wheelIndex)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if (wheelIndex < wheelIndexer.Count - 1) { wheelIndex++; SetVehicleWheelType(car.Handle,wheelIndexer[wheelIndex]);}
            return WheelNames[wheelIndex];
        }
        public static string CycleToPreviusWheelType(int wheelIndex)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if (wheelIndex > 0) { wheelIndex--; SetVehicleWheelType(car.Handle, wheelIndexer[wheelIndex]); }
            return WheelNames[wheelIndex];
        }
        //Colors
        public static void CyclePrimaryColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.PrimaryColor = colorIndexer[colorIndex];}
            i = car.Mods.PrimaryColor.ToString();
        }
        public static void CyclePrimaryColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.PrimaryColor = colorIndexer[colorIndex];}
            i = car.Mods.PrimaryColor.ToString();
        }
        public static void CycleSecondaryColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.SecondaryColor = colorIndexer[colorIndex];}
            i = car.Mods.SecondaryColor.ToString();
        }
        public static void CycleSecondaryColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.SecondaryColor = colorIndexer[colorIndex];}
            i = car.Mods.SecondaryColor.ToString();
        }
        public static void CyclePRColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.PearlescentColor = colorIndexer[colorIndex];}
            i = car.Mods.PearlescentColor.ToString();
        }
        public static void CyclePRColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.PearlescentColor = colorIndexer[colorIndex];}
            i = car.Mods.PearlescentColor.ToString();
        }
        public static void CycleRCColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.RimColor = colorIndexer[colorIndex];}
            i = car.Mods.RimColor.ToString();
        }
        public static void CycleRCColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.RimColor = colorIndexer[colorIndex];}
            i = car.Mods.RimColor.ToString();
        }
        public static void CycleDSColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.DashboardColor = colorIndexer[colorIndex];}
            i = car.Mods.DashboardColor.ToString();
        }
        public static void CycleDSColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.DashboardColor = colorIndexer[colorIndex];}
            i = car.Mods.DashboardColor.ToString();
        }
        public static void CycleTRColorToNext(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex < colorIndexer.Count - 1) { colorIndex++; car.Mods.TrimColor = colorIndexer[colorIndex];}
            i = car.Mods.TrimColor.ToString();
        }
        public static void CycleTRColorToPrevius(int colorIndex,out string i)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            if(colorIndex > 0) { colorIndex--; car.Mods.TrimColor = colorIndexer[colorIndex];}
            i = car.Mods.TrimColor.ToString();
        }
    }
}
