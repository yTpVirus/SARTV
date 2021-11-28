using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;
using static Client.Utils.Utils;
using static Client.Menus.Lobbys.LobbyMenu;

namespace Client.Managers
{
    class RaceCallerManager : BaseScript
    {
        public RaceCallerManager()
        {
            EventHandlers["OnAllLobbyPlayersReady"] += new Action(OnLobbyPlayersReady);
            EventHandlers["OnAllLobbyPlayersFinish"] += new Action(OnLobbyPlayersFinish);
        }

        private async void OnLobbyPlayersFinish()
        {
            await Delay(2000);
            CameraManager.StopSpectating();
            RaceManager.BlockControl = true;
            Game.PlayerPed.CurrentVehicle.IsPositionFrozen = true;
            SpeedManager.ToggleSpeedo(true);
            DisplayRadar(true);
            ShowLobby();
            Debug.WriteLine("RaceCaller Executado!");
        }

        private async void OnLobbyPlayersReady()
        {
            EnableWait = false;
            await Delay(1200);
            RaceCountDown();
        }
    }
}
