using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Client.Menus.RC.Managers;

namespace Client.Menus.RC.Abas
{
    class CPMenu : BaseScript
    {
        public static Menu cpMenu = new Menu("CheckPoint Options");
        //Items
        MenuItem newcp = new MenuItem("Novo CheckPoint","Cria um Novo Checkpoint, Caso Seja o Primeiro Iniciará o processo de criação de uma nova corrida");
        MenuItem undo = new MenuItem("Deletar Último","Deleta o Último Spawn");
        MenuItem delall = new MenuItem("Deletar Todos","Deleta todos os Spawns Adicionados");

        public CPMenu()
        {
            //
            cpMenu.AddMenuItem(newcp);
            cpMenu.AddMenuItem(undo);
            cpMenu.AddMenuItem(delall);
            //
            cpMenu.OnItemSelect += Selected;
        }

        private void Selected(Menu menu, MenuItem item, int itemIndex)
        {
           if(item == newcp) {CPManager.NewCheckPoint();};
           if(item == undo) { CPManager.ClearLastCheckPoint(); };
           if(item == delall) { CPManager.ClearAllCheckPoints(); };
        }
    }
}
