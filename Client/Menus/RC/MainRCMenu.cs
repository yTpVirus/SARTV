using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using Client.Menus.RC.Managers;
using static Client.Utils.Utils;
using static Client.vData.RAM;

namespace Client.Menus.RC.Abas
{
    
    class MainMenu : BaseScript
    {

        Menu Main;
        MenuDynamicListItem list;
        MenuItem CreateCheck;
        MenuItem CreateSpawn;
        MenuItem QuickSave;
        MenuItem Save;
        MenuItem Limpar;
        MenuItem Testar;
        //
        public string[] classes = { "Sprint","Circuito","OffRoad Sprint","OffRoad Circuito","Rally","Drag"};
        //
        public MainMenu()
        {
            //Managers
            Main = new Menu($"Race Creator", "Criado Por [SART]Virus~");
            MenuController.AddMenu(Main);
            //Child
            MenuController.AddSubmenu(Main,CPMenu.cpMenu);
            MenuController.AddSubmenu(Main,SPMenu.spMenu);
            //Menu Configs
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.MenuToggleKey = Control.SelectCharacterMichael;
            //Create Menu Items
            CreateDynamicClassList();
            CreateCheck = new MenuItem("Opções de Checkpoint", "Opções De CheckPoint");
            CreateSpawn = new MenuItem("Opções de Spawn", "Opções de SpawnPoints");
            QuickSave = new MenuItem("QuickSave", "Salva na Memória Interna para Testes (!OBRIGATÓRIO!)");
            Save = new MenuItem("Salvar", "Salva a Corrida Criada e Limpa os CheckPoints e SpawnPoints");
            Limpar = new MenuItem("Limpar Tudo", "USE COM CAUTELA CASO NÃO TENHA SALVO VOCÊ PERDERÁ TODO O PROCESSO!");
            Testar = new MenuItem("Testar Corrida", "Inicia o Teste Solo da Corrida");
            //Menu Items
            Main.AddMenuItem(CreateCheck);
            Main.AddMenuItem(CreateSpawn);
            Main.AddMenuItem(QuickSave);
            Main.AddMenuItem(Save);
            Main.AddMenuItem(Limpar);
            Main.AddMenuItem(Testar);
            //Event
            Main.OnItemSelect += Selected;
            Main.OnMenuOpen += CheckIfIsOnVehicle;
            EventHandlers["ReceiveSaveConfirm"] += new Action<string>(NotifyPlayer);
        }
        private async void NotifyPlayer(string name)
        {
            CPManager.ClearAllCheckPoints();
            SPManager.ClearAllSpawnPoints();
            await Delay(400);
            Notify(3,$"Corrida {name} Salva Com Sucesso!");
        }
        private void CreateDynamicClassList()
        {
                MenuDynamicListItem.ChangeItemCallback call = new MenuDynamicListItem.ChangeItemCallback(ClassCallBack);
                list = new MenuDynamicListItem("Classe", classes[0], call);
                list.ItemData = 0;
                Main.AddMenuItem(list);
        }

        private string ClassCallBack(MenuDynamicListItem item, bool left)
        {
            if (left) 
            {
                if((int)item.ItemData > 0) { item.ItemData -= 1; return classes[(int)item.ItemData];}
            }
            else
            {
                if((int)item.ItemData < classes.Length - 1) { item.ItemData += 1; return classes[(int)item.ItemData];}
            }
            return classes[(int)item.ItemData];
        }
        private void CheckIfIsOnVehicle(Menu menu)
        {
            if (!Game.PlayerPed.IsInVehicle())
            {
                Main.CloseMenu();
                Notify(3,"Você Precisa Estar em Um Veículo Para Poder Usar o Race Creator!");
                return;
            }
        }
        private async void Selected(Menu menu, MenuItem item, int itemIndex)
        {
            if (item == CreateCheck) { Main.CloseMenu(); CPMenu.cpMenu.OpenMenu(); }
            if (item == CreateSpawn) { Main.CloseMenu(); SPMenu.spMenu.OpenMenu(); }
            if (item == Limpar) { CPManager.ClearAllCheckPoints(); SPManager.ClearAllSpawnPoints(); }
            if (item == Save)
            {
                DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP8", "", "", "", "", "", 30);
                while (UpdateOnscreenKeyboard() == 0)
                {
                    await Delay(0);
                }
                if (GetOnscreenKeyboardResult() != null)
                {
                    if (CPManager.checks.Count == 0 || SPManager.vl.Count == 0) { Notify(2,"Não Existe Checkpoints Ou SpawnPoints (Save CANCELADO)"); return; }
                    var result = GetOnscreenKeyboardResult();
                    //Executa parametros Para Começar o Save no Servidor!
                    Race race = new Race();
                    race.RaceName = result;
                    race.RaceClass = classes[(int)list.ItemData];
                    Debug.WriteLine($"Aguardando Resposta do Servidor: Checkpoints {CPManager.checks.Count} SpawnPoints: {SPManager.vl.Count}");
                    //SetChecks to Save
                    CPManager.checks.ForEach((v)=> { race.cl.Add(v); });
                    SPManager.vl.ForEach((v)=> { race.sl.Add(v); });
                    SPManager.Hs.ForEach((v)=> { race.Hs.Add(v); });
                    await Delay(400);
                    //
                    string JsonRace = JsonConvert.SerializeObject(race,Formatting.Indented);
                    Debug.Write($"Arquivo Enviado Para o Servidor");
                    //
                    TriggerServerEvent("SaveRaceData",race.RaceName,JsonRace,race.sl.Count.ToString());
                    await Delay(200);
                    Notify(3, "Processo De Save Iniciado!");
                }
            }
            if (item == Testar) 
            {
                if (CPManager.cl.Count != 0 && SPManager.vghostl.Count != 0)
                {
                    if (hasLastSave())
                    {
                        Notify(1, "Você Entrou No Modo De Teste de Corrida, Sua Dimensão Será Diferente dos Demais!");
                        RCTestManager.SetupRace();
                        SetPlayerDimension(10);
                    }
                    else
                    {
                        await Delay(500);
                        HoldRace("QUICKSAVEAUTO",CPManager.checks,SPManager.vl);
                        Notify(1,"QuickSave Não Encontrado! (QuickSave Automático Executado Por Segurança)");
                        await Delay(1000);
                        Notify(1, "Iniciando Modo de Teste");
                        RCTestManager.SetupRace();
                        SetPlayerDimension(10);
                    }
                }
                else
                {
                    Notify(2,"Não Foi Possível Iniciar o Teste (Verifique Se Adicionou Spawns e Checkpoints)");
                }
            }
            if (item == QuickSave)
            {
                if (CPManager.cl.Count != 0 && SPManager.vghostl.Count != 0)
                {
                    HoldRace("QUICKSAVE", CPManager.checks, SPManager.vl);
                    Notify(3, "QuickSave Executado Com Sucesso!");
                }
                else
                {
                    Notify(2,"Não Foi Possivel Dar QuickSave (Verifique Se Adicionou Spawns e Checkpoints)");
                }
            }
        }
    }
}
