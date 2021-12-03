using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using Client.Managers;


namespace Client.Menus.Tuning
{
    class TuningMenu:BaseScript
    {
        public static Menu main = new Menu("", "");
        public string failed = "Seu Veículo Não Suporta Esta Modificação";
        public string app = "[Aplicado]";
        public TuningMenu() 
        {
            main.OnMenuOpen += OnOpen;
            
        }

        private void OnOpen(Menu menu)
        {
            var veh = GarageManager.VehiclesOnSpot[0];
            menu.MenuTitle = $"{veh.LocalizedName}";
            menu.ClearMenuItems();
            veh.Mods.GetAllMods().ToList().ForEach((mod)=> 
            {
                //Add ModTypeMenu
                MenuItem k = new MenuItem($"{mod.LocalizedModTypeName} [{mod.ModCount}]");
                menu.AddMenuItem(k);
                //AddSubCategories
                Menu itemMenu = new Menu($"{mod.LocalizedModTypeName}");
                MenuController.BindMenuItem(main,itemMenu,k);
                for (int i = 0; i < mod.ModCount; i++)
                {
                   MenuItem mIndex = new MenuItem($"{mod.GetLocalizedModName(i)}"); itemMenu.AddMenuItem(mIndex); mIndex.ItemData = mod.ModType;
                }
                itemMenu.OnIndexChange += OnModSelected;
            });
        }

        private void OnModSelected(Menu menu, MenuItem oldItem, MenuItem newItem, int oldIndex, int newIndex)
        {
            var v = GarageManager.VehiclesOnSpot[0];
            v.Mods.InstallModKit();
            v.Mods[newItem.ItemData].Index = newItem.Index;
        }
    }
}
