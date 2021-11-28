using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Managers;
using CitizenFX.Core;
using MenuAPI;

namespace Client.Menus.Garage.SubMenus
{
    class ChangeVeh : BaseScript
    {   
        public static Menu changeveh = new Menu("");
        //
        public static MenuItem next = new MenuItem("Próximo","Seleciona o Próximo Veículo");
        public static MenuItem back = new MenuItem("Anterior","Seleciona o Veículo Anterior");
        MenuItem select = new MenuItem("Selecionar","Seleciona o Veículo Como Principal");
        MenuItem voltar = new MenuItem("Voltar","Seleciona o Veículo Anterior");
        //
        private static int index = 1;
        public ChangeVeh()
        {
            //
            MenuController.AddMenu(changeveh);
            //
            changeveh.AddMenuItem(next);
            changeveh.AddMenuItem(back);
            changeveh.AddMenuItem(select);
            changeveh.AddMenuItem(voltar);
            //
            changeveh.OnItemSelect += Selected;
            changeveh.OnMenuOpen += OnOpen;
            changeveh.OnMenuClose += CheckClose;
        }

        private void OnOpen(Menu menu)
        {
            if(menu == changeveh) { changeveh.MenuTitle = GarageManager.VehiclesOnSpot[index].LocalizedName; changeveh.RefreshIndex(0); }
        }

        private void CheckClose(Menu menu)
        {
            if(menu == changeveh) { CameraManager.FadeCameraIntoDefaultPosition(); CameraManager.ResetCameraIndex(); GarageMenu.OpenGMenu(); }
        }

        public static void ResetIndex()
        {
            index = 1;
        }

        private static void CloseVehChange()
        {
            changeveh.CloseMenu();
        }

        private void Selected(Menu menu, MenuItem menuItem, int itemIndex)
        {
            if(menuItem == next) {index++; CameraManager.SetCameraToNextSlot(); changeveh.MenuTitle = GarageManager.VehiclesOnSpot[index].LocalizedName; }
            if(menuItem == back) {index--; CameraManager.SetCameraToPreviousSlot(); changeveh.MenuTitle = GarageManager.VehiclesOnSpot[index].LocalizedName; }
            //
            if(menuItem == voltar) { CameraManager.FadeCameraIntoDefaultPosition(); CloseVehChange(); GarageMenu.OpenGMenu(); CameraManager.ResetCameraIndex(); }
            if(menuItem == select) 
            { 
                CameraManager.FadeCameraIntoDefaultPosition();
                CloseVehChange();
                GarageMenu.OpenGMenu();
                GarageManager.ChangeHostVehicle(index);
                //Troca os Veículos de Lugar na Lista
                //Seta novas posições pra eles
                //Por último Reseta o Index
                CameraManager.ResetCameraIndex();
                index = 1;
            }
        }
    }
}
