using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Client.Managers
{
    class BlipManager:BaseScript
    {
        private static List<Blip> blips = new List<Blip>();
        public BlipManager()
        {
            Veryfier();
        }

        public static void RegisterBlip(Vector3 Pos, float radius, int alpha, string Name, float Scale, BlipSprite Sprite, BlipColor Color)
        {
            Blip b = World.CreateBlip(Pos,radius);
            b.Alpha = alpha;
            b.Name = Name;
            b.Scale = Scale;
            b.Sprite = Sprite;
            b.Color = Color;
            blips.Add(b);
        }
        public static void RegisterBlip(Vector3 Pos, float radius, int alpha, string Name, float Scale, BlipSprite Sprite)
        {
            Blip b = World.CreateBlip(Pos,radius);
            b.Alpha = alpha;
            b.Name = Name;
            b.Scale = Scale;
            b.Sprite = Sprite;
            blips.Add(b);
        }

        private async void Veryfier()
        {
            while (true)
            {
                await Delay(0);
                if (RaceManager.IsOnRace == false && GarageManager.IsOnGarage == false)
                {
                    blips.ForEach((b) => { b.Alpha = 255;});
                }
                else { blips.ForEach((b) => { b.Alpha = 0;});}
            }
        }
    }
}
