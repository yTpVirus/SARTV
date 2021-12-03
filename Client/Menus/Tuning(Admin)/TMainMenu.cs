using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static Client.Managers.AdminTManager;
using static CitizenFX.Core.Native.API;
using static Client.vData.RAM;
using Client.Managers;
using static Client.Utils.Utils;
using System.Reflection;

namespace Client.Menus.Tuning_Admin_
{
    class TMainMenu : BaseScript
    {
        public static Menu tMain = new Menu($"SART Tuning", "TUDO DE GRAÇA PRA SART");
        public static Dictionary<int, int> modindexer = new Dictionary<int, int>();
        public static List<VehicleColor> colorIndexer = new List<VehicleColor>();
        public static List<int> wheelIndexer = new List<int>();
        public static List<int> engineIndexer = new List<int>();
        //DESC
        static string[] desc =
        {
            "~y~!ATENÇÂO!~w~ - O Valor Será Aplicado Novamente ao Sair do Menu Caso Volte Ao Original.",
        };

        static int[] wheelTypes = {0,1,2,3,4,5,7,8,9,10,11,12,6};
        
        public TMainMenu()
        { 
            tMain.OnMenuClose += Close; 
            tMain.OnCheckboxChange += CheckMode;
        }

        private void CheckMode(Menu menu, MenuCheckboxItem menuItem, int itemIndex, bool newCheckedState)
        {
            var car = GarageManager.VehiclesOnSpot[0];
            ToggleVehicleMod(car.Handle,(int)menuItem.ItemData,newCheckedState);
        }
        private void Close(Menu menu)
        {
            if (tMain == menu) { Garage.GarageMenu.OpenGMenu(); VehicleManager.SaveVehicleIntoJSON(GarageManager.VehiclesOnSpot[0]);}
        }

        public static void RegisterAllPossibleMods()
        {
            var car = GarageManager.VehiclesOnSpot[0];
            modindexer.Clear();
            colorIndexer.Clear();
            tMain.ClearMenuItems();
            wheelIndexer.Clear();
            var vehcolors = Enum.GetValues(typeof(VehicleColor)).Cast<VehicleColor>().ToList();
            var vehwheels = wheelTypes.ToList();
            //Cuida das Cores
            vehcolors.ForEach((c) => {colorIndexer.Add(c);});
            vehwheels.ForEach((c) => { wheelIndexer.Add(c);});
            foreach (var mod in car.Mods.GetAllMods())
            {
                RegisterDynamicModItem(mod.LocalizedModTypeName, $"{mod.Index}", "", $"{mod.LocalizedModName} {mod.Index + 1}/{mod.ModCount}");
            }
            //WheelType
            RegisterDynamicWheelType();
            //Colors
            RegisterDynamicPrimaryColorItems("Cor Primária", vehcolors,car);
            RegisterDynamicSecondaryColorItems("Cor Secundária", vehcolors,car);
            RegisterDynamicPerolColorItems("Perolado", vehcolors,car);
            RegisterDynamicRimColorItems("Cor da Roda", vehcolors,car);
            RegisterDynamicDashColorItems("Cor do Painel", vehcolors,car);
            RegisterDynamicTrimColorItems("Cor dos Detalhes", vehcolors,car);
            //Toggles
            RegisterToggleItem("Turbo",IsToggleModOn(car.Handle,18),VehicleToggleModType.Turbo);
            RegisterToggleItem("Farol Xenon",IsToggleModOn(car.Handle,22),VehicleToggleModType.XenonHeadlights);
            
        }
        
        //ITEMS DE MENU
        private static void RegisterDynamicWheelType()
        {
            var v = GarageManager.VehiclesOnSpot[0];
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(WheelTypeCallback);
            MenuDynamicListItem list = new MenuDynamicListItem("Classe da Roda",v.Mods.WheelType.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicModItem(string name, string initialV,string desc,string valueToShow)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(DynamicCallBack);
            MenuDynamicListItem list = new MenuDynamicListItem(name,valueToShow,call,desc);
            tMain.AddMenuItem(list);
            modindexer.Add(list.Index,int.Parse(initialV));
        }
        private static void RegisterToggleItem(string name,bool check,VehicleToggleModType type)
        {
            MenuCheckboxItem item = new MenuCheckboxItem(name,check);
            tMain.AddMenuItem(item);
            item.ItemData = type;
        }
        private static void RegisterDynamicPrimaryColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackP);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.PrimaryColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicSecondaryColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackS);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.SecondaryColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicPerolColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackPR);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.PearlescentColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicRimColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackRC);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.RimColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicDashColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackDS);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.DashboardColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        private static void RegisterDynamicTrimColorItems(string name,List<VehicleColor> colors,Vehicle v)
        {
            MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ColorCallBackTR);
            MenuDynamicListItem list = new MenuDynamicListItem(name,v.Mods.TrimColor.ToString(),call);
            tMain.AddMenuItem(list);
        }
        //Métodos Referencia
        //MODTYPE
        public static string DynamicCallBack(MenuDynamicListItem item, bool left)
        {
            if (left){if (modindexer.ContainsKey(item.Index)) { CycleToPreviusModIndex(item.Index, out string i);return i; }; return "Erro 774";}
            else {if (modindexer.ContainsKey(item.Index)) { CycleToNextModIndex(item.Index, out string i); return i; } return "Erro 774";}
        }
        //WheelType
        private static string WheelTypeCallback(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) { return CycleToPreviusWheelType(wheelIndexer.IndexOf(GetVehicleWheelType(v.Handle)));}
            else { return CycleToNextWheelType(wheelIndexer.IndexOf(GetVehicleWheelType(v.Handle)));}
        }
        //WheelType
        //Color
        private static string ColorCallBackP(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CyclePrimaryColorToPrevius(colorIndexer.IndexOf(v.Mods.PrimaryColor),out string i);return i;} 
            else {CyclePrimaryColorToNext(colorIndexer.IndexOf(v.Mods.PrimaryColor),out string i);return i;}
        }
        private static string ColorCallBackS(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CycleSecondaryColorToPrevius(colorIndexer.IndexOf(v.Mods.SecondaryColor),out string i);return i;} 
            else {CycleSecondaryColorToNext(colorIndexer.IndexOf(v.Mods.SecondaryColor),out string i);return i;}
        }
        private static string ColorCallBackPR(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CyclePRColorToPrevius(colorIndexer.IndexOf(v.Mods.PearlescentColor),out string i);return i;} 
            else {CyclePRColorToNext(colorIndexer.IndexOf(v.Mods.PearlescentColor),out string i);return i;}
        }
        private static string ColorCallBackRC(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CycleRCColorToPrevius(colorIndexer.IndexOf(v.Mods.RimColor),out string i);return i;} 
            else {CycleRCColorToNext(colorIndexer.IndexOf(v.Mods.RimColor),out string i);return i;}
        }
        private static string ColorCallBackDS(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CycleDSColorToPrevius(colorIndexer.IndexOf(v.Mods.DashboardColor),out string i);return i;} 
            else {CycleDSColorToNext(colorIndexer.IndexOf(v.Mods.DashboardColor),out string i);return i;}
        }
        private static string ColorCallBackTR(MenuDynamicListItem item, bool left)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            if (left) {CycleTRColorToPrevius(colorIndexer.IndexOf(v.Mods.TrimColor),out string i);return i;}
            else {CycleTRColorToNext(colorIndexer.IndexOf(v.Mods.TrimColor),out string i);return i;}
        }
    }
}
