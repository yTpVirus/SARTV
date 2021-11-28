using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Client.Menus.Garage.SubMenus;
using Client.Menus;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;
using static Client.vData.RAM;
using MenuAPI;
using Client.Managers;

namespace Client.Menus.Garage
{
    class GarageMenu : BaseScript
    {
        private static Menu menu = new Menu("Garagem",$"{Game.Player.Name}");
        bool over = false;
        //Itens
        public static MenuItem changeVeh = new MenuItem("Mudar Veículo","Troque Seu Veículo Por Outro que já Possui Na Garagem.");
        public static MenuItem modoLivre = new MenuItem("Modo Livre","Saia Para Explorar o Mundo e Disputar Corridas Valendo Dinheiro ou o Próprio Carro!");
        public static MenuItem custom = new MenuItem("Customizar Veículo","Aqui você Pode Aplicar as Peças Que Comprou nas Lojas!");
        public static MenuItem admincustom = new MenuItem("SART Tuning","Aqui você Pode Tunar o Carro de Graça! (ADMINS APENAS)");
        public static MenuItem Corridas = new MenuItem("Corridas","Aqui Você pode Treinar Em Todos as Corridas do Servidor!");
        public GarageMenu()
        {
            //Manage Menu
            MenuController.AddMenu(menu);
            MenuController.AddSubmenu(menu, Tuning.TuningMenu.main);
            menu.EnableInstructionalButtons = false;
            //
            menu.AddMenuItem(changeVeh);
            menu.AddMenuItem(modoLivre);
            menu.AddMenuItem(custom);
            menu.AddMenuItem(admincustom);
            menu.AddMenuItem(Corridas);
            //Disabe unused
            Corridas.Enabled = false;
            //
            menu.OnItemSelect += Selected;
            menu.OnMenuClose += PreventClose;
            menu.OnMenuOpen += OnOpen;
            RegisterCommand("Load",new Action(()=> { TriggerServerEvent("LOAD", clientData.userToken);}),false);
        }

        private async void OnOpen(Menu menu)
        {
            await Delay(2000);
            DrawHelpInstructions(GarageManager.Instructions);
        }

        private void PreventClose(Menu m)
        {
            if(m == menu && over == false) { menu.OpenMenu(); }
        }

        private void CloseGmenu()
        {
            over = true;
            menu.CloseMenu();
            over = false;
        }

        private void Selected(Menu menu, MenuItem menuItem, int itemIndex)
        {
            if(menuItem == changeVeh) {CameraManager.SetCameraToNextSlot(); CloseGmenu(); ChangeVeh.changeveh.OpenMenu(); ChangeVeh.ResetIndex(); }
            if(menuItem == modoLivre) {CloseGmenu(); GarageManager.SendPlayerToOpenWorld();}
            if(menuItem == custom) { CloseGmenu(); Tuning.TuningMenu.main.OpenMenu();}
            if(menuItem == admincustom) {CloseGmenu(); Tuning_Admin_.TMainMenu.RegisterAllPossibleMods(); Tuning_Admin_.TMainMenu.tMain.OpenMenu(); AdminTManager.SaveValues(GarageManager.VehiclesOnSpot[0].Handle); }
        }

        public static void OpenGMenu()
        {
            menu.OpenMenu();
        }
    }
}
