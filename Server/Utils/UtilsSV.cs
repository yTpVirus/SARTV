using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;

namespace Server.Utils
{
    class UtilsSV:BaseScript
    {
        public UtilsSV()
        {

        }
        /// <summary>
        /// Envia Um Toast Para um Player Specífico
        /// </summary>
        /// <param name="player">Player Alvo</param>
        /// <param name="type">(0 = SART) (1 = Alerta) (2 = ERRO) (3 = Info) (4 = Custom)</param>
        /// <param name="Text">Texto a ser Apresentado</param>
        /// <param name="Title">Título (Usável somente no tipo 4)</param>
        public static void NotifyPlayer(Player player,int type, string Text, string Title = "")
        {
            player.TriggerEvent("TriggerNotify", type, Text, Title);
        }
        /// <summary>
        /// Envia um Toast para todos os Players
        /// </summary>
        /// <param name="type">(0 = SART) (1 = Alerta) (2 = ERRO) (3 = Info) (4 = Custom)</param>
        /// <param name="Text">Texto a ser Apresentado</param>
        /// <param name="Title">Título (Usável somente no tipo 4)</param>
        public static void NotifyALL(int type, string Text, string Title = "")
        {
            TriggerClientEvent("TriggerNotify", type, Text, Title);
        }

        public class TopTimes
        {
            private List<TopTime> tl = new List<TopTime>();
            public void InsertTopTime(TopTime time)
            {
                tl.Add(time);
            }
            public List<TopTime> GetTopTimes()
            {
                return tl;
            }
        }

        public struct TopTime
        {
            public string PlayerName { get; set; }
            public string RaceName { get; set; }
            public string Class { get; set; }
            public string Carro { get; set; }
            public float Tempo { get; set; }

            public TopTime(string PlayerName,string RaceName, string Class, string Carro, float Tempo)
            {
                this.PlayerName = PlayerName;
                this.RaceName = RaceName;
                this.Class = Class;
                this.Carro = Carro;
                this.Tempo = Tempo;
            }
        }
    }
}
