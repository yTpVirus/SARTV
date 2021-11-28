using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Client.VespuraFEAPI;
using static Client.vData.RAM;

using Newtonsoft.Json;

namespace Client.Menus.Lobbys
{
    class LobbyMenu : BaseScript
    {

        private static FrontendMenu CurrentLobby = null;

        public LobbyMenu()
        {
            EventHandlers["MakeLobby"] += new Action<string,string>(MakeLobby);
            EventHandlers["AddPlayer"] += new Action<string>(AddPlayer);
            EventHandlers["UpdateTops"] += new Action<string>(UpdateTopTimes);
            SetFrontendActive(false);
        }

        private static void FocusOff()
        {
            StartScreenEffect("SwitchHUDIn", 500, false);
        }

        private static void FocusOn()
        {
            StopScreenEffect("SwitchHUDIn");
        }

        public static void HideLobby()
        {
            if(CurrentLobby != null)
            {
                CurrentLobby.CloseMenu();
                FocusOn();
            }
        }

        public static async void ShowLobby()
        {
            if(CurrentLobby != null)
            {
                await CurrentLobby.OpenMenu();
                FocusOff();
            }
        }

        private void UpdateTopTimes(string toplist)
        {
            if(CurrentLobby != null)
            {
                List<TopTime> tos = JsonConvert.DeserializeObject<List<TopTime>>(toplist);
                UpdateTops(tos);
            }
        }

        private void AddPlayer(string Player)
        {
            if (CurrentLobby != null)
            {
                CurrentLobby.AddPlayer(Players[int.Parse(Player)], 100, "Ready", "CLN", PlayerIcon.MOUSE, HudColor.HUD_COLOUR_GREY, HudColor.HUD_COLOUR_GREEN);
            }
        }

        private async void MakeLobby(string race,string toplist)
        {
            Race rac = JsonConvert.DeserializeObject<Race>(race);
            List<TopTime> tos = JsonConvert.DeserializeObject<List<TopTime>>(toplist);
            CurrentLobby = new FrontendMenu(rac.RaceName,rac.RaceClass,FrontendType.FE_MENU_VERSION_CORONA);
            UpdateTops(tos);
            await Delay(1000);
            await CurrentLobby.OpenMenu();
            FocusOff();
        }

        private async void UpdateTops(List<TopTime> toplist)
        {
            CurrentLobby.ClearOptions();
            await Delay(200);
            var tops = toplist.OrderBy(x => x.Tempo).ToList();
            if (tops.Count > 1)
            {
                var t = tops.Find(x => x.Tempo == 0);
                tops.Remove(t);
            }
            Debug.WriteLine($"{tops.Count}");
            await Delay(500);
            if(tops.Count > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    var top = tops[i];
                    var t = TimeSpan.FromMilliseconds(top.Tempo);
                    var tempo = $"{top.PlayerName} | {t.Minutes}m:{t.Seconds}s:{t.Milliseconds}ms";
                    CurrentLobby.AddOption(tempo);
                }
            }
            else
            {
                for (int i = 0; i < tops.Count; i++)
                {
                    var top = tops[i];
                    var t = TimeSpan.FromMilliseconds(top.Tempo);
                    var tempo = $"{top.PlayerName} | {t.Minutes}m:{t.Seconds}s:{t.Milliseconds}ms";
                    CurrentLobby.AddOption(tempo);
                }
            }
        }
    }
}
