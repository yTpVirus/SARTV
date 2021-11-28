using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Client.Menus.RC.Managers;
using Client.Managers;
using static CitizenFX.Core.Native.API;
using static Client.vData.RAM;

namespace Client.Utils
{
    public class Utils : BaseScript
    {
        private static string Racename;
        private static List<Vector3> Checks = new List<Vector3>();
        private static List<Vector3> Spawns = new List<Vector3>();
        private static bool saved = false;
        private static int formHandle = 0;
        private static bool EnableForm = false;
        private static bool RenderHelp = false;
        public static bool EnableWait = false;
        private static int countTime = 5;
        public Utils()
        {
            EventHandlers["TriggerNotify"] += new Action<int,string,string>(Notify);
            //Draw WaterMar
            Drawwatermark();
        }

        private async void Drawwatermark()
        {
            while (true)
            {
                await Delay(0);
                if (GarageManager.IsOnGarage == true)
                {
                    SetTextFont(1);
                    SetTextProportional(true);
                    SetTextScale(0.0f, 0.8f);
                    SetTextColour(0, 60, 255, 255);
                    SetTextEdge(255, 255, 255, 255, 255);
                    SetTextOutline();
                    SetTextEntry("STRING");
                    AddTextComponentString("South America Racing Team");
                    DrawText(0.40f, 0.01f);
                }
                else
                {
                    SetTextFont(1);
                    SetTextProportional(true);
                    SetTextScale(0.0f, 0.8f);
                    SetTextColour(0, 60, 255, 255);
                    SetTextEdge(255, 255, 255, 255, 255);
                    SetTextOutline();
                    SetTextEntry("STRING");
                    AddTextComponentString("South America Racing Team");
                    DrawText(0.40f, 0.947f);
                }
            }
        }
        /// <summary>
        /// Recebe uma Lista de Array das Instruções, EX: {"0","~INPUT_SPRINT~","FAZ ALGUMA COISA"} OU {"0","t_E","FAZ AGUMA COISA"};
        /// </summary>
        /// <param name="Instructions">Lista de Instruções a Serem Iteradas</param>
        public static async void DrawHelpInstructions(List<string[]> Instructions)
        {
            Scaleform scale = new Scaleform("INSTRUCTIONAL_BUTTONS");
            while (!scale.IsLoaded) {await Delay(0);}
            scale.Render2D();
            scale.CallFunction("CLEAR_ALL");
            scale.CallFunction("SET_CLEAR_SPACE",200);
            for (int i = Instructions.Count; i -->0;)
            {
                Debug.WriteLine($"INDEX: {i}, COUNT: {Instructions.Count}");
                scale.CallFunction("SET_DATA_SLOT", int.Parse(Instructions[i][0]), Instructions[i][1], Instructions[i][2]);
            }
            scale.CallFunction("SET_BACKGROUND_COLOUR",0,0,0,80);
            scale.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS");
            Debug.WriteLine($"{scale.IsValid}");
            RenderHelp = true;
            while (RenderHelp == true)
            {
                await Delay(0);
                scale.Render2D();
            }
        }

        public static void RemoveHelpInstructions()
        {
            RenderHelp = false;
        }

        //public void Drawlabel(int font, bool proportional, Vector2 Scale, int[] rgba, int[] edge,string text, Vector2 pos)
        /// <summary>
        /// Mostra um Texto na Tela do Player *Need Render Every Frame*
        /// </summary>
        /// <param name="Position">Position of the Text</param>
        /// <param name="Scale">Scale of The Text</param>
        /// <param name="Proportional">true if text need to be proportional to screen</param>
        /// <param name="rgba">RGBA Colors to Text</param>
        /// <param name="edge">Edge Thickness and Color of text edges (t,r,g,b,a)</param>
        /// <param name="text">Text to be displayed</param>
        public static void DrawLabel(Vector2 Position,float[] Scale,int font, bool Proportional, int[] rgba, int[] edge, string text)
        {
            SetTextFont(font);
            SetTextProportional(Proportional);
            SetTextScale(Scale[0],Scale[1]);
            SetTextColour(rgba[0],rgba[1],rgba[2],rgba[3]);
            SetTextDropshadow(0, 0, 0, 0, 255);
            SetTextEdge(edge[0], edge[1], edge[2], edge[3], edge[4]);
            SetTextDropShadow();
            SetTextOutline();
            SetTextEntry("STRING");
            AddTextComponentString(text);
            DrawText(Position.X, Position.Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"> (0 = SART) (1 = Alerta) (2 = ERRO) (3 = Info) (4 = Custom) </param>
        /// <param name="msg"> Texto Que Será Apresentado </param>
        /// <param name="Title"> Título Customizado Usado Somente no Modo CUSTOM </param>
        public static void Notify(int type, string msg,string Title = "")
        { 
            SetNotificationTextEntry("STRING");
            if (type == 0) { AddTextComponentString($"~b~[SART]:~w~ {msg}"); }
            if (type == 1) { AddTextComponentString($"~y~ALERTA:~w~ {msg}"); }
            if (type == 2) { AddTextComponentString($"~r~ERRO:~w~ {msg}"); }
            if (type == 3) { AddTextComponentString($"~o~Info:~w~ {msg}"); }
            if (type == 4) { AddTextComponentString($"~g~{Title}:~w~ {msg}"); }
            DrawNotification(true, false);
        }
        /// <summary>
        /// Teleports Player to Speciefic Dimension(Server Sided) {Will Only Teleport IF Player is on a vehicle}
        /// </summary>
        /// <param name="Dim">Dimension ID, Can be any</param>
        public static void SetPlayerDimension(int Dim)
        {
            if (Game.PlayerPed.IsInVehicle()) { TriggerServerEvent("SetPlayerDimension", Dim); }
        }
        public static void SendPlayerToGarageDimension()
        {
            TriggerServerEvent("SetPlayerGarageDimension");
        }
        /// <summary>
        /// Segura Uma Corrida Na Memória Interna, ("Não Fica Salvo Pra Sempre, Sempre Será Substituida Pela Última Criada!")
        /// </summary>
        /// <param name="name">Nome da Corrida</param>
        /// <param name="cps">Lista dos Checkpoints</param>
        /// <param name="sps">Lista dos Spawns</param>
        public static void HoldRace(string name, List<Vector3> cps, List<Vector3> sps)
        {
            Racename = name;
            Checks.Clear();
            Spawns.Clear();
            //Cuida Dos CPS
            foreach (var v in cps) { Checks.Add(v); }
            //Cuida Dos SPS
            foreach (var v in sps) { Spawns.Add(v); }
            //
            saved = true;
        }
        /// <summary>
        /// Função Somente Pra o Editor!!
        /// </summary>
        public static async void ReloadLastSavedRace()
        {
            await Delay(2000);
            //Cuida dos Spawns
            foreach (Vector3 v in Checks)
            {
                CPManager.ReceiveCheckPoint(v);
            }
            Debug.WriteLine($"RETORNADO {Checks.Count} CHECKPOINTS");
            await Delay(100);
            Notify(3,"Você Finalizou o Teste, Checkpoints Spawnados nos Devidos Lugares <3");
            await Delay(100);
            Notify(1,"Spawns Resetados Para Evitar Bugs (Crie Todos Somente Quando for !FINALIZAR!)");
            saved = false;
            
        }
        public static bool hasLastSave()
        {
            return saved;
        }
        /// <summary>
        /// For Countdown only!!
        /// </summary>
        private static async void DrawTextOnScreen(string BigText, string smlText)
        {
            formHandle = RequestScaleformMovie("mp_big_message_freemode");
            while (!HasScaleformMovieLoaded(formHandle))
            {
                await Delay(0);
            }
            BeginScaleformMovieMethod(formHandle, "SHOW_SHARD_WASTED_MP_MESSAGE");
            PushScaleformMovieMethodParameterString(BigText);
            PushScaleformMovieMethodParameterString(smlText);
            PushScaleformMovieMethodParameterInt(0);
            EndScaleformMovieMethod();
            EnableForm = true;
            while (EnableForm == true)
            {
                await Delay(0);
                DrawScaleformMovieFullscreen(formHandle, 255, 255, 255, 255, 1);
            }
        }
        /// <summary>
        ///Escreve Uma Mensagem na Tela do Jogador com os parametros Fornecidos 
        /// </summary>
        /// <param name="BigText">Primiero texto a ser Apresentado</param>
        /// <param name="smlText">Texto Menor (Geralmente usado para dar mais detalhes)</param>
        /// <param name="delay">Tempo de demora em ms para a mensagem sumir</param>
        public static async void DrawTimedTextOnScreen(string BigText,string smlText,int delay)
        {
            DrawTextOnScreen(BigText,smlText);
            await Delay(delay);
            EnableForm = false;
        }

        public static async void WaitForOthers(string BigText, string smlText)
        {
            var form = RequestScaleformMovie("mp_big_message_freemode");
            while (!HasScaleformMovieLoaded(form))
            {
                await Delay(0);
            }
            BeginScaleformMovieMethod(form, "SHOW_SHARD_WASTED_MP_MESSAGE");
            PushScaleformMovieMethodParameterString(BigText);
            PushScaleformMovieMethodParameterString(smlText);
            PushScaleformMovieMethodParameterInt(0);
            EndScaleformMovieMethod();
            EnableWait = true;
            while (EnableWait == true)
            {
                await Delay(0);
                DrawScaleformMovieFullscreen(form, 255, 255, 255, 255, 1);
            }
            
        }
        public static async void RaceCountDown()
        {
            var veh = Game.PlayerPed.CurrentVehicle;
            veh.IsPositionFrozen = false;
            veh.IsHandbrakeForcedOn = true;
            veh.ApplyForce(veh.UpVector,default,ForceType.MinForce);
            DoScreenFadeIn(1000);
            await Delay(7000);
            EnableWait = false;
            //
            for (int i = 0; i < countTime; i++)
            { 
                EnableForm = true;
                DrawTextOnScreen($"{countTime - i}","");
                await Delay(1000);
                EnableForm = false;
                //Hide for 50
                await Delay(0);
                if(i + 1 == countTime)
                {
                    Game.PlayerPed.CurrentVehicle.IsHandbrakeForcedOn = false;
                    DrawTextOnScreen("Vai Vai Vai!", "");
                    RaceTimeManager.StartRaceTimer();
                    await Delay(1000);
                    EnableForm = false;
                    await Delay(0);
                }
            }
        }
    }
}
