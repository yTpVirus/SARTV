using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Client.Managers;

namespace Client.Conc
{
    internal class StarterConc:BaseScript
    {
        public StarterConc()
        {
            //Add Starter Cars
            ConcManager.Starter.Add(1118611807);
            ConcManager.Starter.Add(int.Parse($"{VehicleHash.Blista}"));
            ConcManager.Starter.Add(-1177863319);
        }
    }
}
