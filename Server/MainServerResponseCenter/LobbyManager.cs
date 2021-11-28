using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static Server.SQL.SQLManager;
using static Server.Utils.UtilsSV;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;

namespace Server.MainServerResponseCenter
{
    class LobbyManager : BaseScript
    {
        static List<Lobby> Salas = new List<Lobby>();
        public LobbyManager()
        {
            RegisterCommand("CreateLobby", new Action<int, List<object>, string>(CreateLobby), false);
            RegisterCommand("JoinLobby", new Action<int, List<object>, string>(JoinLobby), false);
            RegisterCommand("LeaveLobby", new Action<int, List<object>, string>(LeaveLobby), false);
        }

        private void CreateLobby(int source, List<object> args, string rawcommand)
        {
            if(source > 0) 
            {
                Lobby room = null;
                var player = Players[source];
                try
                {
                    room = Salas.Find(x => x.Players.Keys.ToList().Find(y => y == player) == player);
                }
                catch(ArgumentNullException e)
                {
                    Debug.WriteLine($"{e}"); NotifyPlayer(player, 3, "Um Erro Não Esperado Aconteceu Ao Criar o Lobby");
                }
                finally
                {
                    if(room != null) {CreateLobbyUnderConditions(player, room); } 
                    else { CreateLobbyUnderConditions(player); }
                }
            }
        }
        private void CreateLobbyUnderConditions(Player player,Lobby room = null)
        {
            if (room != null)
            {
                if (room.Owner == player) { NotifyPlayer(player, 3, "Você Já é Dono de um Lobby!"); return;}
                else { NotifyPlayer(player, 3, "Você Já Pertence a um Lobby!"); return;}
            }
            else
            {
                room = new Lobby(8,2);
                room.InsertPlayer(player,new PlyrStatus(false,false));
                room.Owner = player;
                Salas.Add(room);
                room.ID = Salas.IndexOf(room) + 1000;
                Debug.WriteLine($"Sala Criada, ID: {room.ID}");
                NotifyALL(3, $"O Jogador {player.Name} Criou um Lobby! ID: {room.ID}");
                PlayerManager.SetPDimension(player, room.ID);
                //
                MakeVisualLobby(player);
                AddLobbyPlayer(player,player);
            }
        }

        private void MakeVisualLobby(Player player)
        {
            var data = GetDataFromTable("MAIN", $"SELECT * FROM Races");
            var JSON = (string)data.Rows[1].ItemArray[1];
            var TJson = (string)data.Rows[1].ItemArray[3];
            player.TriggerEvent("MakeLobby", JSON, TJson);
        }

        private void AddLobbyPlayer(Player Host,Player player)
        {
            Host.TriggerEvent("AddPlayer", player.Handle);
        }

        private void JoinLobby(int source, List<object> args, string rawcommand) 
        {
            if (source > 0)
            {
                var player = Players[source];
                Lobby room = null;
                room = Salas.Find(x => x.Players.Keys.ToList().Find(y => y == player) == player);
                if (room != null) { NotifyPlayer(player,1,"Você Já Pertence a Um Lobby!"); return; }
                try
                {
                    room = Salas.Find(x => x.ID == int.Parse((string)args[0]));
                }
                catch
                {
                    NotifyPlayer(player, 3, "O Lobby Especificado Não Existe!");
                }
                finally
                {
                    if(room != null)
                    {
                        if (room.InsertPlayer(player,new PlyrStatus(false,false)))
                        {
                            NotifyPlayer(player, 4, $"Você Entrou no Lobby do Jogador {room.Owner.Name} [{room.Players.Count}/{room.MaxPlayers}]", "Lobby");
                            MakeVisualLobby(player);
                            AddLobbyPlayer(player,room.Owner);
                            AddLobbyPlayer(player,player);
                            room.Players.Keys.ToList().ForEach((p) =>
                            {
                                if (p != player)
                                { 
                                    NotifyPlayer(p, 4, $"{player.Name} Entrou no Lobby! [{room.Players.Count}/{room.MaxPlayers}]", "Lobby");
                                    AddLobbyPlayer(p, player);
                                    if(p != room.Owner) { AddLobbyPlayer(player,p); }
                                }
                            });
                            //DEBUG
                            PlayerManager.SetPDimension(player, room.ID);
                        }
                        else
                        {
                            NotifyPlayer(player, 3, "O Lobby Está Cheio!");
                        }
                    }
                    else
                    {
                        NotifyPlayer(player,3,"O Lobby Informado Não Existe!","Lobby");
                    }
                }
                
            }
        }
        private void LeaveLobby(int source, List<object> args, string rawcommand) 
        {
            if (source > 0)
            {
                var player = Players[source];
                Lobby room = null;
                try
                {
                    room = Salas.Find(x => x.ID == int.Parse((string)args[0]));
                }
                catch
                {
                    NotifyPlayer(player, 3, "O Lobby Especificado Não Existe!");
                }
                finally
                {
                    if (room != null)
                    {
                        if (room.RemovePlayer(player))
                        {
                            NotifyPlayer(player, 4, $"Você Deixou o Lobby do Jogador {room.Owner.Name}", "Lobby");
                            if (room.Players.Count > 0)
                            {
                                room.Players.Keys.ToList().ForEach((p) =>
                                {
                                    NotifyPlayer(p, 4, $"{player.Name} Saiu do Lobby! [{room.Players.Count}/{room.MaxPlayers}]", "Lobby");
                                });
                            }
                            else
                            {
                                Salas.Remove(room);
                                NotifyALL(4,$"O Lobby {room.ID} Foi Deletado Por Não Possuir Players","Lobby");
                            }
                            //DEBUG
                            PlayerManager.SetPDimension(player, 0);
                        }
                        else
                        {
                            Debug.WriteLine($"Player Não Encontrado LOBBY?:{room.ID}"); NotifyPlayer(player, 3, "Você Não Está No Lobby Informado!");
                        }
                    }
                    else
                    {
                        NotifyPlayer(player, 3, "O Lobby Informado Não Existe!", "Lobby");
                    }
                }

            }
        }
        public static List<Lobby> GetLobbys()
        {
            return Salas;
        }
    }

    public class Lobby
    {
        public Player Owner = default;
        public Dictionary<Player, PlyrStatus> Players = new Dictionary<Player, PlyrStatus>();
        public int ID = 0;
        public int MaxPlayers = 4;
        public int MinPlayers = 1;
        public Lobby(int maxPlayers = 4, int minPlayers = 1) { MaxPlayers = maxPlayers; MinPlayers = minPlayers; }
        public bool InsertPlayer(Player player,PlyrStatus status)
        {
            if (Players.Count < MaxPlayers) { status.Finished = false; status.Ready = false; Players.Add(player,status); return true; }
            return false;
        }
        public bool RemovePlayer(Player player)
        {
            if (Players.Keys.ToList().Contains(player)) { Players.Remove(player) ; return true; }
            else { return false; }
        }
        public int PlayersCount()
        {
            return Players.Count;
        }
    }
    public class PlyrStatus
    {
        public bool Ready = false;
        public bool Finished = false;
        public PlyrStatus(bool ready, bool finished)
        {
            Ready = ready;
            Finished = finished;
        }
    }
}

