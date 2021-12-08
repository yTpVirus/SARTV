using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Server.SQL.SQLManager;
using Newtonsoft.Json;
using static Server.Utils.UtilsSV;

namespace Server.MainServerResponseCenter
{
    class MSRC : BaseScript
    {

        public MSRC()
        {
            //Events
            EventHandlers["SavePlayerData"] += new Action<string, string, int>(SavePlayerData);
            EventHandlers["SetLastUsed"] += new Action<Player,string,int>(SetLastUsedVehicle);
            EventHandlers["SaveMainData"] += new Action<string, string, string>(SaveMainData);
            EventHandlers["SaveRaceData"] += new Action<Player, string, string, string>(SaveRaceData);
            EventHandlers["SaveReplayData"] += new Action<string, string, string>(SaveReplayData);
            EventHandlers["AskServerForData"] += new Action<Player>(ReplyDataToClient);
            EventHandlers["LOAD"] += new Action<Player,string>(SendPlayerToGarage);
            //CRIA TODAS AS DATABASES NECESSÁRIAS
            CreateNewDBFile("MAIN");
            CreateNewDBFile("PlayerVehData");
            CreateNewDBFile("PlayerData");
            CreateNewDBFile("ReplayData");
        }
        private async void SetLastUsedVehicle([FromSource]Player player, string userToken,int hash)
        {   
            ExecuteRawSQLCommand("PlayerVehData", $"UPDATE a{userToken} SET LastUsed='{0}';");
            await Delay(100);
            ExecuteRawSQLCommand("PlayerVehData", $"UPDATE a{userToken} SET LastUsed='{1}' WHERE Hash='{hash}';");
        }

        //REMINDER!
        //Nome > 0 , JSON > 1, SpawnIndex > 2;
        //REMINDER!
        private void ReplyDataToClient([FromSource]Player player)
        {
            //Send RaceListFirst
            var raceData = GetDataFromTable("MAIN","SELECT * FROM Races");
            List<string[]> raceList = new List<string[]>();
            for (int i = 0; i < raceData.Rows.Count; i++)
            {
                string racename = (string)raceData.Rows[i].ItemArray[0];
                string[] race = {racename, i.ToString()};
                raceList.Add(race);
            }
            //Recebe Chamado Para Reply de Racelist
            Debug.WriteLine($"Confirmação do Cliente {player.Name} Recebida! REPLY: {raceList.Count}");
            //Envia para o Cliente os Dados
            Debug.WriteLine($"Compactando Dados");
            string res = JsonConvert.SerializeObject(raceList);
            Debug.WriteLine($"Enviando Dados Para o Cliente {player.Name} {res.Length} BYTES");
            string usertoken = PlayerManager.GetPlayerToken(player);
            player.TriggerEvent("ReceiveReplyFromServer",res,usertoken);
        }
        //ITA > 1 Veh, 2 HASH
        //ROW > Outros Veículos
        private void SendPlayerToGarage([FromSource]Player player,string usertoken)
        {
            var data = GetDataFromTable("PlayerVehData", $"SELECT * FROM a{usertoken} ORDER BY LastUsed DESC");
            List<string> list = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                list.Add((string)data.Rows[i].ItemArray[0]);
            }
            string json = JsonConvert.SerializeObject(list);
            SetGarageDimension(player, json);
            Debug.WriteLine($"{list.Count}");
        }

        //Data Methods
        /// <summary>
        /// Salva o Replay da Corrida de Certo Player
        /// </summary>
        /// <param name="accName">Nome da Conta do Player</param>
        /// <param name="RaceName">Nome da Corrida</param>
        /// <param name="JSON">Arquivo JSON Contendo as Informações do Replay</param>
        private void SaveReplayData(string accName,string RaceName, string JSON)
        {
            //Cria a Tabela Caso Não Exista
            CreateTable("ReplayData",RaceName,"ReplayJson JSON");
            //
            InsertDataOnTable("ReplayData",RaceName,"ReplayJson",JSON);
        }
        /// <summary>
        /// Salva Todas as Corridas do Servidor
        /// </summary>
        /// <param name="RaceName">Nome da Corrida</param>
        /// <param name="JSON">Arquivo JSON</param>
        private void SaveRaceData([FromSource]Player player,string RaceName, string JSON, string spCount)
        {
            string[] v = {RaceName, JSON, spCount};
            var Top = new TopTime("Ninguém",RaceName,"Nenhum","Nenhum",0);
            var s = new List<TopTime>();
            s.Add(Top);
            var d = JsonConvert.SerializeObject(s);
            //
            InsertRaceDataOnTable("MAIN","Races","RaceName, Data, SpawnCount",v);
            ExecuteRawSQLCommand("MAIN", $"UPDATE Races SET TopTime='{d}' WHERE RaceName='{RaceName}';");
            //
            player.TriggerEvent("ReceiveSaveConfirm",RaceName);
        }

        /// <summary>
        /// Salva Parametros Principais No Banco MAIN
        /// </summary>
        /// <param name="name">Nome da Tabela</param>
        /// <param name="coluna">coluna a ser salva</param>
        /// <param name="JSON">Arquivo a Ser Salvo</param>
        private void SaveMainData(string name,string coluna,string JSON)
        {
            
        }
        private void SavePlayerData(string userToken,string JSON,int HashCode)
        {
            CreateTable("PlayerVehData", $"a{userToken}", "Veiculos JSON, Hash INT,FID INT,LastUsed INT");
            ExecuteRawSQLCommand("PlayerVehData", $"INSERT INTO a{userToken} (Veiculos,Hash,FID) SELECT '{JSON}','{HashCode}','{userToken}' WHERE NOT EXISTS (SELECT 1 FROM a{userToken} WHERE Hash='{HashCode}')");
            ExecuteRawSQLCommand("PlayerVehData", $"UPDATE a{userToken} SET Veiculos='{JSON}' WHERE Hash='{HashCode}';");
        }

        //Misc Methods
        private async void SetGarageDimension(Player player,string json)
        {
            SetPlayerRoutingBucket(player.Handle, int.Parse(player.Handle));
            Debug.WriteLine($"Mudando Dimensão! PlayerHandle {player.Handle} NAME: {player.Name} Dimensão {player.Handle}");
            while (GetPlayerRoutingBucket(player.Handle) != int.Parse(player.Handle))
            {
                await Delay(0);
            }
            TriggerClientEvent(player, "teste", json);
        }
    }
}