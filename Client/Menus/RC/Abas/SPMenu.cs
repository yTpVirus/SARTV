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
    class SPMenu : BaseScript
    {
        public static Menu spMenu = new Menu("SpawnPoint Options");
        //Items
        MenuItem newsp = new MenuItem("Novo SpawnPoint", "Cria um SpawnPoint Para a Corrida, (Você Não Conseguirá Salvar sem ao menos dois Spawns)");
        MenuItem undo = new MenuItem("Deletar Último", "Deleta o último Spawn Adicionado");
        MenuItem delall = new MenuItem("Deletar Todos","Deleta Todos os Spawns Adicionados");
        public SPMenu()
        {
            //
            spMenu.AddMenuItem(newsp);
            spMenu.AddMenuItem(undo);
            spMenu.AddMenuItem(delall);
            //
            spMenu.OnItemSelect += Selected;
        }

        private void Selected(Menu menu, MenuItem item, int itemIndex)
        {
            if(item == newsp) { SPManager.NewSpawnPoint(); }
            if(item == undo) { SPManager.ClearLastCheckPoint(); }
            if(item == delall) { SPManager.ClearAllSpawnPoints(); }
        }
    }
}
