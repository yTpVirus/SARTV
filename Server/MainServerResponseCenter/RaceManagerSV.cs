using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Server.SQL.SQLManager;
using static Server.Utils.UtilsSV;
using Newtonsoft.Json;

namespace Server.MainServerResponseCenter
{
    class RaceManagerSV:BaseScript
    {
        public RaceManagerSV()
        {
            //Comando Para Receber Dados no Console
            RegisterCommand("receive", new Action<int, List<object>, string>(SendRaceToClient), false);
            RegisterCommand("Test2", new Action(() => { NotifyALL(0, $"Novo Recorde!\n Jogador: PlayerNoob\n Corrida: {"{racename}"} \n Tempo: {1}m:{11}s:{300}ms"); }),false);
            //
            EventHandlers["OnFinishRace"] += new Action<Player,int,int,string>(OnFinishedRace);
            EventHandlers["OnLobbyPlayerStatusChange"] += new Action<Player,int,bool>(OnLobbyPlayerStatusChange);
            EventHandlers["CheckEveryoneReady"] += new Action<Player,int>(CheckEveryoneReady);
        }

        private void CheckEveryoneReady([FromSource]Player player, int LobbyID)
        {
            var list = LobbyManager.GetLobbys().Find(x => x.ID == LobbyID).Players;
            bool result = list.All(a => a.Value.Ready) || list.All(a => !a.Value.Ready);
            if (result) { list.Keys.ToList().ForEach(p => p.TriggerEvent("OnAllLobbyPlayersReady"));}
        }

        private void OnLobbyPlayerStatusChange([FromSource]Player player, int lobbyID,bool status)
        {
            var lobbys = LobbyManager.GetLobbys();
            var lobby = lobbys.Find(x => x.ID == lobbyID);
            foreach (var p in lobby.Players.Keys)
            {
                if(p == player) { lobby.Players[p] = new PlyrStatus(status,false); return; }
            }
        }
        private void OnFinishedRace([FromSource]Player player, int LobbyID,int Place,string json)
        {
            var lobbys = LobbyManager.GetLobbys();
            Debug.WriteLine($"{player.Name} TERMINOU! ID:{LobbyID}");
            CheckTopTime(player,json);
            var lobby = lobbys.Find(x => x.ID == LobbyID);
            foreach (var p in lobby.Players.Keys)
            {
                if (p != player)
                {
                    NotifyPlayer(p, 3, $" {player.Name} Terminou!\n Pos: {Place}° Lugar!");
                }
            }
            lobby.Players[player].Finished = true;
            bool result = lobby.Players.All(a => a.Value.Finished == true);
            Debug.WriteLine($"RESULT: {result}");
            if (result == true)
            {
                var data = GetDataFromTable("MAIN", "SELECT * FROM Races");
                var TJson = (string)data.Rows[1].ItemArray[3];
                lobby.Players.Keys.ToList().ForEach(x => x.TriggerEvent("UpdateTops",TJson));
                lobby.Players.Keys.ToList().ForEach(x => x.TriggerEvent("OnAllLobbyPlayersFinish"));
            }
        }
        private void CheckTopTime(Player player,string json)
        {
            var top = JsonConvert.DeserializeObject<TopTime>(json);
            var RaceName = top.RaceName;
            var data = GetDataFromTable("MAIN", $"SELECT TopTime FROM Races WHERE RaceName = '{RaceName}'");
            var datad = data.Rows[0].ItemArray[0].ToString();
            var str = JsonConvert.DeserializeObject<List<TopTime>>(datad);
            if (str.Count > 1) { var strl = str.Find(x => x.Tempo == 0); str.Remove(strl); }
            var svrTop = str.OrderBy(x => x.Tempo).ElementAt(0);
            if (top.Tempo < svrTop.Tempo || svrTop.Tempo == default)
            {
                var f = TimeSpan.FromMilliseconds(top.Tempo);
                if (f.Milliseconds < 10) { NotifyALL(0, $"Novo Recorde!\n Jogador: {player.Name}\n Corrida: {RaceName}\n Tempo: {f.Minutes}m:{f.Seconds}s:00{f.Milliseconds}ms"); }
                else if (f.Milliseconds < 100) { NotifyALL(0, $"Novo Recorde!\n Jogador: {player.Name}\n Corrida: {RaceName}\n Tempo: {f.Minutes}m:{f.Seconds}s:0{f.Milliseconds}ms"); }
                else { NotifyALL(0, $"Novo Recorde!\n Jogador: {player.Name}\n Corrida: {RaceName}\n Tempo: {f.Minutes}m:{f.Seconds}s:{f.Milliseconds}ms"); }
                str.Add(top);
                ExecuteRawSQLCommand("MAIN", $"UPDATE Races SET TopTime='{JsonConvert.SerializeObject(str)}' WHERE RaceName='{RaceName}';"); return;
            }
            str.Add(top);
            ExecuteRawSQLCommand("MAIN", $"UPDATE Races SET TopTime='{JsonConvert.SerializeObject(str)}' WHERE RaceName='{RaceName}';");
        }
        ///
        //PlaceHolder Para Um Futuro Manager para Enviar a Corrida Específica Para todos os Clientes do Lobby
        ///
        private async void SendRaceToClient(int source, List<object> args, string RawCommand)
        {
            //Send race
            var data = GetDataFromTable("MAIN", "SELECT * FROM Races");
            var lobbys = LobbyManager.GetLobbys();
            var JSON = (string)data.Rows[1].ItemArray[1];
            var TJson = (string)data.Rows[1].ItemArray[3];
            var u = JsonConvert.DeserializeObject<List<TopTime>>(TJson);
            if (u.Count > 1)
            {
                var ul = u.Find(x => x.Tempo == 0);
                u.Remove(ul);
            }
            var r = u.OrderBy(x => x.Tempo).ElementAt(0);
            foreach (var lobby in lobbys)
            {
                if (lobby.Owner == Players[source])
                {
                    foreach (var player in lobby.Players.Keys)
                    {
                         int dim = lobby.ID;
                        player.TriggerEvent("ReceiveRaceFromServer", JSON, lobby.Players.Keys.ToList().IndexOf(player), dim, r.Tempo, r.PlayerName);
                    }
                    await Delay(2000);
                    Debug.WriteLine($"EXECUTANDO ITERAÇÂO PARA ID DE LOBBY");
                    foreach (var player in lobby.Players.Keys)
                    {
                        var idList = new List<int[]>();
                        lobby.Players.Keys.ToList().ForEach((a) => {
                            int[] b = {int.Parse(a.Handle),lobby.ID};
                            idList.Add(b);
                        });
                        var str = JsonConvert.SerializeObject(idList);
                        player.TriggerEvent("SendLobbyPlayerList", str);
                    }
                    return;
                };
            }
            Debug.WriteLine("Você Não é Dono do Lobby ou Não Está em Nenhum");
        }

    }
}