using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;

namespace Client.vData
{
    class DataReplier : BaseScript
    {
        public DataReplier()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(ReplyToServerOnStart);
        }

        private void ReplyToServerOnStart(string obj)
        {
            if (obj == "MAIN")
            {
                TriggerServerEvent("AskServerForData");
                //DoScreenFadeOut(1000);
            }
        }
    }
}
