using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using static Client.Utils.Utils;
using static CitizenFX.Core.Native.API;
using Client.Menus.Garage.SubMenus;

namespace Client.Managers
{
    class CameraManager : BaseScript
    {
        
        private static List<Camera> cameras = new List<Camera>();
        private static int easeTime = 2000;
        private static int fadetime = 500;
        private static int sIndex = 0;
        private static int indexc = 0;
        private static bool isOnFirstSpot = false;
        public static bool isOnFirstSlot = false;
        //Cameras for Garage Slots
        private static readonly Vector3[] SlotCameras =
        {   
            new Vector3(983.00f,-3000.00f,-39.00f),
            //^^Posição PADRÃO!^^
            new Vector3(959.00f,-3020.50f,-39.00f),
            new Vector3(959.00f,-3025.50f,-39.00f),
            new Vector3(959.00f,-3030.50f,-39.00f),
            new Vector3(957.00f,-3031.00f,-39.00f),
            new Vector3(961.00f,-3031.00f,-39.00f),
            new Vector3(966.00f,-3031.00f,-39.00f),
            new Vector3(970.00f,-3031.00f,-39.00f),
            new Vector3(974.00f,-3031.00f,-39.00f),
            new Vector3(978.00f,-3031.00f,-39.00f),
        };
        //
        string[] left = {"1", "~INPUT_CELLPHONE_LEFT~", "Ver Jogador Anterior"};
        string[] right = {"0", "~INPUT_CELLPHONE_RIGHT~", "Ver Próximo Jogador"};
        static List<string[]> Instructions = new List<string[]>();
        //
        public CameraManager()
        {
            RegisterCommand("cmd3", new Action(ToggleCamerasRenderOFF), false);
            RegisterCommand("cmd4", new Action(() => { StopSpectating(); }), false);
            //DEV^^
            //Camera Keys
            KeyManager.RegisterKeyMap("Resetar Câmera (Garagem)", "r", new Action(() => { if (isOnFirstSlot == true) { FadeCameraIntoDefaultPosition(); }}));
            //RACECAM
            KeyManager.RegisterKeyMap("Assistir Jogador Anterior (Corrida)", "LEFT", new Action(() => { if (NetworkIsInSpectatorMode()) { CycleBetweenLobbyPlayers(true); } }));
            KeyManager.RegisterKeyMap("Assistir Próximo Jogador (Corrida)", "RIGHT", new Action(() => { if (NetworkIsInSpectatorMode()) { CycleBetweenLobbyPlayers(false); } }));
            Instructions.Add(left);
            Instructions.Add(right);
            //
            ;
            Update();
        }
        private async void Update()
        {
            while (true)
            { await Delay(0);
                if (cameras.Count > 0)
                {
                    MoveHorizontal();
                    MoveVertical();
                    ChangeFov();
                    if (Game.IsControlPressed(0, Control.VehicleCinCam) && isOnFirstSlot == true)
                    {
                        FadeCameraIntoDefaultPosition();
                    }
                }
            }
        }
        //
        private void MoveVertical()
        {
            if (Game.IsControlPressed(0, Control.MoveUpOnly))
            {
                if(Math.Abs(cameras[0].Position.Z) < 37) { return; }
                cameras[0].Position -=  cameras[0].ForwardVector * 0.15f;
                
            }
            if (Game.IsControlPressed(0, Control.MoveDownOnly))
            {
                if(Math.Abs(cameras[0].Position.Z) > 40) { return; }
                cameras[0].Position += cameras[0].ForwardVector * 0.15f;
            }
        }

        private void ChangeFov()
        {
            if (Game.IsControlPressed(0, Control.VehicleHorn))
            {
                if (Vector3.Distance(cameras[0].Position, GarageManager.VehiclesOnSpot[0].Position) < 3) { return; }
                cameras[0].Position += cameras[0].UpVector * 0.15f;
            }
            if (Game.IsControlPressed(0, Control.VehicleRadioWheel))
            {
                if (Vector3.Distance(cameras[0].Position, GarageManager.VehiclesOnSpot[0].Position) > 5) { return; }
                cameras[0].Position -= cameras[0].UpVector * 0.15f;
            }
        }

        private void MoveHorizontal()
        {
            if (Game.IsControlPressed(0, Control.MoveRightOnly))
            {
                cameras[0].Position += 0.15f * cameras[0].RightVector;
            }
            if (Game.IsControlPressed(0, Control.MoveLeftOnly))
            {
                cameras[0].Position -= 0.15f * cameras[0].RightVector;
            }
        }
        //
        public static void CycleBetweenLobbyPlayers(bool left)
        {
            var players = PositionManager.GetLobbyPlayers();
            if (left) { if (sIndex > 0) { sIndex--; if((players.ElementAt(sIndex).Key != Game.Player))SpectatePlayer(players.ElementAt(sIndex).Key);}}
            else{ if (sIndex < players.Count - 1) {sIndex++; if((players.ElementAt(sIndex).Key != Game.Player))SpectatePlayer(players.ElementAt(sIndex).Key);}}
        }
        //
        public static void StopSpectating()
        {
            NetworkSetInSpectatorMode(false, Game.PlayerPed.Handle);
            RemoveHelpInstructions();
        }
        public static void SpectatePlayer(Player player)
        {
            NetworkSetInSpectatorMode(true,player.Character.Handle);
        }
        public static void SpectateARandomPlayerFromLobby()
        {
            var players = PositionManager.GetLobbyPlayers();
            DrawHelpInstructions(Instructions);
            for (int i = 0; i < players.Count; i++)
            {
                if (players.ElementAt(i).Key != Game.Player) { SpectatePlayer(players.ElementAt(i).Key); sIndex = i; break;}
            }
        }
        public static int GetSlotIndex() { return indexc; }
        public static void ResetCameraIndex() { indexc = 0; }
        public static void CreateCams()
        {
            DestroyAllCams();
            for (int i = 0; i < GarageManager.VehiclesOnSpot.Count; i++)
            {
                CreateCameraAtPosition(SlotCameras[i],GarageManager.VehiclesOnSpot[i],50);
            }
        }

        public static async void FadeCameraIntoDefaultPosition()
        {
            DoScreenFadeOut(fadetime);
            await Delay(fadetime);
            SetCameraToDefaultPosition();
            await Delay(fadetime);
            DoScreenFadeIn(fadetime);
            ChangeVeh.back.Enabled = false;
            ChangeVeh.next.Enabled = true;
            isOnFirstSlot = true;
        }

        public static void DestroyAllCams()
        {
            cameras.ForEach((c)=> { c.Delete(); });
            cameras.Clear();
            isOnFirstSlot = false;
        }

        public static async void SetCameraToNextSlot()
        {
            //DoScreenFadeOut(fadeouttime);
            isOnFirstSlot = false;
            if (indexc == GarageManager.VehiclesOnSpot.Count - 2) { ChangeVeh.next.Enabled = false;}
            if (indexc == 0) { indexc++; isOnFirstSpot = true; ChangeVeh.back.Enabled = false; DoScreenFadeOut(fadetime);  await Delay(fadetime*2); DoScreenFadeIn(fadetime); ToggleCameraRenderOn(indexc, true, false, indexc - 1); return; }
            if (indexc < cameras.Count - 1) {indexc++; isOnFirstSpot = false; ChangeVeh.back.Enabled = true; ToggleCameraRenderOn(indexc, true, false, indexc - 1);}
            if (indexc == cameras.Count) { ToggleCameraRenderOn(indexc, true, false, indexc); return; }
            Debug.WriteLine($"Número de Cameras {cameras.Count} NUMERO DO INDEX: {indexc} Mirando em {GarageManager.VehiclesOnSpot[indexc].DisplayName}");
        }

        public static void SetCameraToPreviousSlot()
        {
            //DoScreenFadeIn(fadeintime);
            isOnFirstSlot = false;
            if (indexc > 1) { indexc--; }
            ChangeVeh.next.Enabled = true;
            if (indexc == 1)
            {
                if(isOnFirstSpot == false) {isOnFirstSpot = true; ChangeVeh.back.Enabled = false; ToggleCameraRenderOn(2, true, true, 1); }
                if(isOnFirstSpot == true) { Debug.WriteLine($"Número de Cameras {cameras.Count} NUMERO DO INDEX: {indexc} Mirando em {GarageManager.VehiclesOnSpot[indexc].DisplayName}"); return; }
                Debug.WriteLine($"Parametro1");
                Debug.WriteLine($"Número de Cameras {cameras.Count} NUMERO DO INDEX: {indexc} Mirando em {GarageManager.VehiclesOnSpot[indexc].DisplayName}");
                return;
            }
            if(indexc == 2)
            {
                isOnFirstSpot = false;
                ToggleCameraRenderOn(indexc + 1, true, true,indexc);
                Debug.WriteLine($"Parametro2");
                Debug.WriteLine($"Número de Cameras {cameras.Count} NUMERO DO INDEX: {indexc} Mirando em {GarageManager.VehiclesOnSpot[indexc].DisplayName}");
                return;
            }
            isOnFirstSpot = false;
            ToggleCameraRenderOn(indexc + 1, true, true, indexc);
            Debug.WriteLine($"Número de Cameras {cameras.Count} NUMERO DO INDEX: {indexc} Mirando em {GarageManager.VehiclesOnSpot[indexc].DisplayName}");
            
        }

        private static void SetCameraToDefaultPosition()
        {
            cameras[0].Position = SlotCameras[0];
            cameras[0].FieldOfView = 50;
            ToggleCameraRenderOn(0,false,false);
        }

        public static void ToggleCamerasRenderOFF()
        {
            cameras.ForEach((cam) => { cam.IsActive = false; });
            RenderScriptCams(false, false, 0, false, false);
            Game.Player.CanControlCharacter = true;
        }

        public static void ToggleCameraRenderOn(int toActivate,bool ease,bool reverse,int Previous = default)
        {
            if (Previous != 0)
            {
                
                if (reverse == false) {cameras[toActivate].IsActive = true; cameras[Previous].InterpTo(cameras[toActivate], easeTime, true, false); }
                if (reverse == true) { cameras[Previous].IsActive = true; cameras[toActivate].InterpTo(cameras[Previous], easeTime, true, false); }
                RenderScriptCams(true, ease, easeTime, true, false);
                Game.Player.CanControlCharacter = false;
            }
            else
            {
                cameras[toActivate].IsActive = true;
                RenderScriptCams(true, ease, easeTime, true, false);
                Game.Player.CanControlCharacter = false;
            }
            
        }
        //FAZER AS FUNÇÕES DE CAMERA!
        public static void CreateCameraAtPosition(Vector3 pos, Entity entity, int fov)
        {
            Camera camera = new Camera(CreateCam("DEFAULT_SCRIPTED_CAMERA", false));
            camera.Position = pos;
            camera.PointAt(entity.Position);
            camera.FieldOfView = fov;
            camera.IsActive = true;
            cameras.Add(camera);
        }
    }
}
