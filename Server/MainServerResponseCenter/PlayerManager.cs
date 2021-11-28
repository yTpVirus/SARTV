using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using CitizenFX.Core.Native;
using System.Text.RegularExpressions;

namespace Server.MainServerResponseCenter
{
    class PlayerManager : BaseScript
    {

        public PlayerManager()
        {
            EventHandlers["playerJoining"] += new Action<Player, string>(onPlayerJoin);
            EventHandlers["onServerResourceStart"] += new Action<string>(OnMainStart);
            EventHandlers["UserTokenValidationFail"] += new Action<Player>(KickPlayerFromServer);
            EventHandlers["SetPlayerDimension"] += new Action<Player, int, string>(SetDimension);
            Debug.WriteLine("Handler Executado!");
        }

        private void KickPlayerFromServer([FromSource]Player player)
        {
            player.Drop("Falha na Validação do UserToken!");
        }
        private static void KickPlayer(Player player,string reason)
        {
            //player.Drop(reason);
            Debug.WriteLine($"Player: {player.Name} {reason}");
        }
        private void SetDimension([FromSource] Player player, int dim, string dm)
        {
            int id = GetVehiclePedIsIn(player.Character.Handle, false);
            SetPlayerRoutingBucket(player.Handle, dim);
            SetEntityRoutingBucket(id, dim);
            //
            SetPedIntoVehicle(player.Character.Handle,id,-1);
            //
            Debug.WriteLine($"Mudando Dimensão! PlayerHandle {player.Handle} NAME: {player.Name} Dimensão {dim}");
        }
        public static void SetPDimension(Player player, int dim)
        {
            int id = GetVehiclePedIsIn(player.Character.Handle, false);
            SetPlayerRoutingBucket(player.Handle, dim);
            SetEntityRoutingBucket(id, dim);
            //
            SetPedIntoVehicle(player.Character.Handle,id,-1);
            //
            Debug.WriteLine($"Mudando Dimensão! PlayerHandle {player.Handle} NAME: {player.Name} Dimensão {dim}");
        }
        public static string GetPlayerToken(Player player)
        {
            var list = player.Identifiers;
            foreach (var i in list)
            {
                Regex rx = new Regex("fivem:");
                var m = rx.Matches(i);
                if (m.Count > 0)
                {
                    var result = i.Remove(0, 6);
                    return result;
                }
            }
            return null;
        }
        
        private void OnMainStart(string obj)
        {
            if(obj != "MAIN") { return; }
            Debug.WriteLine($"SCRIPT {obj} INICANDO");
            Debug.WriteLine($"ISSO PODE DEMORAR UM POUCO!!");
            if (Players.Count() > 0)
            {
                Debug.WriteLine($"Script Carregado, Aguardando Informação dos Jogadores {Players.Count()} Player(s)!");
            }
            else
            {
                Debug.WriteLine($"Nenhum Player Online!, Pulando Agregação");
                Debug.WriteLine($"Lista de Dados Em Espera..");
            }    
        }

        private void onPlayerJoin([FromSource]Player player, string arg)
        {
            Debug.WriteLine($"O Jogador {player.Name} Acabou de Conectar");
            Debug.WriteLine($"Aguardando Confirmação do Cliente...");
        }
    }
}
