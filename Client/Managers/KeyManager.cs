using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Client.Managers
{
    class KeyManager:BaseScript
    {
        public KeyManager()
        {}
        public static void RegisterKeyMap(string ActionName,string Key,Action action)
        {
            RegisterCommand(action.Method.Name,new Action(action),true);
            RegisterKeyMapping(action.Method.Name,ActionName,"keyboard",Key);
        }
    }
}
