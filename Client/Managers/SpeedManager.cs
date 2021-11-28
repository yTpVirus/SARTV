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
    class SpeedManager : BaseScript
    {
        private static bool toggle = true;
        public SpeedManager()
        {
            Request();
        }

        public static void ToggleSpeedo(bool t)
        {
            toggle = t;
        }

        private async void Request()
        {   
            SetStreamedTextureDictAsNoLongerNeeded("speedo");
            await Delay(200);
            RequestStreamedTextureDict("speedo", true);
            await Delay(200);
            DrawGear();
            DrawKMP();
            DrawRPM();
        }

        private async void DrawGear()
        {
            await Delay(1000);
            while (true) 
            {
                await Delay(0);
                if (Game.PlayerPed.IsInVehicle() && GetFollowPedCamViewMode() != 4 && toggle == true) 
                {
                    int[] rgba = { 255, 255, 255, 255 };
                    int[] edge = { 2, 0, 0, 0, 255 };
                    float[] Scale = { 0.7f, 0.6f };
                    //
                    var car = Game.PlayerPed.CurrentVehicle;
                    var gear = car.CurrentGear;
                    //
                    if (gear >= 0 && GetEntitySpeed(car.Handle) == 0)
                    {
                        DrawLabel(new Vector2(0.8115f, 0.751f), Scale, 7, true, rgba, edge, $"N");
                    }
                    else if (gear > 0)
                    {
                        DrawLabel(new Vector2(0.8115f, 0.751f), Scale, 7, true, rgba, edge, $"{gear}");
                    }
                    else if (gear == 0 && GetEntitySpeed(car.Handle) >= 0)
                    {
                        DrawLabel(new Vector2(0.8115f, 0.751f), Scale, 7, true, rgba, edge, $"R");
                    }
                    
                }
            }
        }

        private async void DrawKMP()
        {
            await Delay(1000);
            while (true)
            {
                await Delay(0);
                if (Game.PlayerPed.IsInVehicle() && GetFollowPedCamViewMode() != 4 && toggle == true)
                {
                    int[] rgba = { 255, 255, 255, 255 };
                    int[] edge = { 2, 0, 0, 0, 255 };
                    float[] Scale = { 0.7f, 0.7f };
                    DrawSprite("speedo", "sart_rpm_g", 0.86f, 0.86f, 0.25f, 0.45f, 0, 255, 255, 255, 255); // Vel
                    var car = Game.PlayerPed.CurrentVehicle;
                    var l = GetEntitySpeed(car.Handle) * 3.6f;
                    //SetVehicleForwardSpeed(car.Handle,l);
                    float tRot = -210 + l;
                    float result = MathUtil.Clamp(tRot, -210, 30);
                    DrawSprite("speedo", "sart_vl_g", 0.894f, 0.90f, 0.2f, 0.35f, result, 255, 255, 255, 255); // Pointer
                    if (l <= 10)
                    {
                        DrawLabel(new Vector2(0.886f, 0.94f), Scale, 7, true, rgba, edge, $"{(int)l}");
                    }
                    else if (l <= 100)
                    {
                        DrawLabel(new Vector2(0.88f, 0.94f), Scale, 7, true, rgba, edge, $"{(int)l}");
                    }
                    else if (l <= 1000)
                    {
                        DrawLabel(new Vector2(0.874f, 0.94f), Scale, 7, true, rgba, edge, $"{(int)l}");
                    }
                }
            }
        }
        private async void DrawRPM()
        {
            await Delay(500);
            while (true)
            {
                await Delay(0);
                if (Game.PlayerPed.IsInVehicle() && GetFollowPedCamViewMode() != 4 && toggle == true)
                {
                    var car = Game.PlayerPed.CurrentVehicle;
                    var l = car.CurrentRPM;
                    //Scale and Shit
                    if (car.IsEngineRunning == false) { l = 0; }
                    var r = l * 10000;
                    var j = Math.Abs(11000 - r);
                    var rpm = Math.Abs(j + -8500);
                    var rand = new Random();
                    var rpm2 = 0f;
                    if (rpm > 7400) { rpm2 = MathUtil.RevolutionsToDegrees(rpm / 14900) + rand.Next(-5, 5); } else { rpm2 = MathUtil.RevolutionsToDegrees(rpm / 14900) + rand.Next(-1, 2); }
                    if (car.IsEngineRunning == false) { rpm2 = 0; }
                    var k = -280 + rpm2;
                    DrawSprite("speedo", "sart_rpmp", 0.790f, 0.884f, 0.20f, 0.35f, k, 255, 255, 255, 255); // Pointer
                }
            }
        }
    }
}
