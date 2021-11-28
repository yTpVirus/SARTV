using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Client.vData.RAM;
using Newtonsoft.Json;

namespace Client.Managers
{
    class PDM : BaseScript
    {
        public PDM() 
        {
            EventHandlers["ReceiveReplyFromServer"] += new Action<string,string>(ApplyReplyToRAM);
        }


        private void ApplyReplyToRAM(string data,string token)
        {
            Debug.WriteLine("Descompactando Dados...");
            List<string[]> raceList = JsonConvert.DeserializeObject<List<string[]>>(data);
            if(raceList.Count > 0)
            {
                Debug.WriteLine("Descompactação Executada Com Sucesso!");
                foreach (string[] race in raceList)
                {
                    clientData.RaceList.Add(race);
                }
                if (token.Length > 0)
                {
                    clientData.userToken = token;
                    Debug.WriteLine($"UserToken Confirmado Com Sucesso!");
                    //
                    //ExecuteCommand("int");
                }
                else
                {
                    TriggerServerEvent("UserTokenValidationFail");
                }
                
            }
            else
            {
                Debug.WriteLine("Algo Deu Errado Na Descompactação, Por Favor ENTRE EM PÂNICO!");
            }
        }
    }
}
