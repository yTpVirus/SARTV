using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Client.Utils.Utils;

namespace Client.Managers
{
    class ConcManager:BaseScript
    {
        private Vector3 blipP = new Vector3(-43f, -1098f, 26f);
        public ConcManager()
        {
            //StartConc();
            BlipManager.RegisterBlip(blipP,255,"Concessionária",1,BlipSprite.PersonalVehicleCar);
        }

        private async void StartConc()
        {
            Vehicle veh = await World.CreateVehicle(new Model(VehicleHash.Adder), new Vector3(-43f, -1098f, 26f), 0);
            while (true)
            {
                await Delay(0);
                veh.Heading += 0.1f;
            }
        }
    }
}
