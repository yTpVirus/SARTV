using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Client.VespuraFEAPI
{
    public enum FrontendType
    {
        FE_MENU_VERSION_SP_PAUSE,
        FE_MENU_VERSION_MP_PAUSE,
        FE_MENU_VERSION_CREATOR_PAUSE,
        FE_MENU_VERSION_CUTSCENE_PAUSE,
        FE_MENU_VERSION_SAVEGAME,
        FE_MENU_VERSION_PRE_LOBBY,
        FE_MENU_VERSION_LOBBY,
        FE_MENU_VERSION_MP_CHARACTER_SELECT,
        FE_MENU_VERSION_MP_CHARACTER_CREATION,
        FE_MENU_VERSION_EMPTY,
        FE_MENU_VERSION_EMPTY_NO_BACKGROUND,
        FE_MENU_VERSION_TEXT_SELECTION,
        FE_MENU_VERSION_CORONA,
        FE_MENU_VERSION_CORONA_LOBBY,
        FE_MENU_VERSION_CORONA_JOINED_PLAYERS,
        FE_MENU_VERSION_CORONA_INVITE_PLAYERS,
        FE_MENU_VERSION_CORONA_INVITE_FRIENDS,
        FE_MENU_VERSION_CORONA_INVITE_CREWS,
        FE_MENU_VERSION_CORONA_INVITE_MATCHED_PLAYERS,
        FE_MENU_VERSION_CORONA_INVITE_LAST_JOB_PLAYERS,
        FE_MENU_VERSION_CORONA_RACE,
        FE_MENU_VERSION_CORONA_BETTING,
        FE_MENU_VERSION_JOINING_SCREEN,
        FE_MENU_VERSION_LANDING_MENU,
        FE_MENU_VERSION_LANDING_KEYMAPPING_MENU,
    };

    public enum PlayerIcon
    {
        NONE = 0,
        VOICE_ACTIVE = 47,
        VOICE_IDLE = 48,
        VOICE_MUTED = 49,
        GTA_V_LOGO = 54,
        GLOBE = 63,
        KICK_BOOT = 64,
        FREEMODE_RANK = 65,
        SPECTATOR_EYE = 66,
        GAME_PAD = 119,
        MOUSE = 120
    };

    public enum RowColor
    {
        RED
    }

    public enum StatusColor
    {
        RED
    }

    public struct PlayerRow
    {
        public int RowIndex { get; set; }
        public int PlayerRank { get; set; }
        public Player Player { get; set; }
        public string Status { get; set; }
        public string CrewTag { get; set; }
        public PlayerIcon Icon { get; set; }
        public HudColor RowColor { get; set; }
        public HudColor StatusColor { get; set; }
        public PlayerRow(int index, int rank, Player player, string status, string crewTag, PlayerIcon icon, HudColor rowColor, HudColor statusColor)
        {
            this.RowIndex = index;
            this.PlayerRank = rank;
            this.Player = player;
            this.Status = status;
            this.CrewTag = crewTag;
            this.Icon = icon;
            this.RowColor = rowColor;
            this.StatusColor = statusColor;
        }
    }

    public struct Option
    {
        public int RowIndex { get; set; }
        public string Name { get; set; }
        public string ItemData{ get; set; }
        
        public Option(int index,string name)
        {
            this.RowIndex = index;
            this.Name = name;
            this.ItemData = default;
        }
    }


    public class FrontendMenu
    {
        #region constructors
        /// <summary>
        /// FrontendMenu class constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="menuType"></param>
        public FrontendMenu(string name, FrontendType menuType) : this(name, "", menuType) { }

        /// <summary>
        /// FrontendMenu class constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subtitle"></param>
        /// <param name="menuType"></param>
        public FrontendMenu(string name, string subtitle, FrontendType menuType)
        {
            this.name = name;
            this.subtitle = subtitle;
            this.menuType = menuType;
        }
        #endregion


        #region private variables
        private string name;
        private string subtitle;
        private readonly int id = 0;
        private readonly FrontendType menuType;
        private List<PlayerRow> playerRows = new List<PlayerRow>();
        private List<Option> TopTimes = new List<Option>();
        #endregion

        #region public variables
        public bool IsVisible { get; private set; }
        #endregion


        #region public getter functions
        /// <summary>
        /// Get the name/title of this frontend menu.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Get the subtitle of this frontend menu.
        /// </summary>
        /// <returns></returns>
        public string GetSubtitle()
        {
            return subtitle;
        }

        /// <summary>
        /// Get the ID of this frontend menu.
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return id;
        }

        public int GetNumPlayerRows()
        {
            return playerRows.Count;
        }
        #endregion

        #region public setter functions
        /// <summary>
        /// Set the title/name of this frontend menu.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Set the subtitle of this frontend menu.
        /// </summary>
        /// <param name="subtitle"></param>
        public void SetSubtitle(string subtitle)
        {
            this.subtitle = subtitle;
        }
        #endregion


        #region main public functions
        public async void AddPlayer(Player player, int rank, string status, string crewTag, PlayerIcon icon, HudColor rowColor, HudColor statusColor)
        {
            playerRows.Add(new PlayerRow(index: playerRows.Count, rank: rank, player: player, status: status, crewTag: crewTag, icon: icon, rowColor: rowColor, statusColor: statusColor));
            await UpdateList();
        }
        public void AddOption(string name)
        {
            TopTimes.Add(new Option(TopTimes.Count,name));
            UpdateSettings();
        }
        public int GetOpCount()
        {
            return TopTimes.Count;
        }
        public void RemoveOption(int index)
        {
            TopTimes.RemoveAt(index);
            UpdateSettings();
        }
        public void ClearOptions()
        {
            TopTimes.Clear();
            UpdateSettings();
        }
        public async void UpdatePlayer(int index, int rank, string status, string crewTag, PlayerIcon icon, HudColor rowColor, HudColor statusColor)
        {
            PlayerRow p = playerRows[index];
            p.Status = status;
            p.PlayerRank = rank;
            p.CrewTag = crewTag;
            p.Icon = icon;
            p.RowColor = rowColor;
            p.StatusColor = statusColor;
            await UpdateList();
        }

        public async void UpdatePlayer(Player player, int rank, string status, string crewTag, PlayerIcon icon, HudColor rowColor, HudColor statusColor)
        {
            PlayerRow p = playerRows.Find(pr => { return pr.Player.ServerId == player.ServerId; });
            p.Status = status;
            p.PlayerRank = rank;
            p.CrewTag = crewTag;
            p.Icon = icon;
            p.RowColor = rowColor;
            p.StatusColor = statusColor;
            await UpdateList();
        }

        public async void DeletePlayer(int index)
        {
            playerRows.RemoveAt(index);
            PushScaleformMovieFunctionN("SET_DATA_SLOT_EMPTY");
            PushScaleformMovieFunctionParameterInt(3);
            PushScaleformMovieFunctionParameterInt(GetNumPlayerRows() - 1);
            PopScaleformMovieFunctionVoid();
            await UpdateList();
        }

        private async Task UpdateList()
        {
            if (playerRows.Count > 0)
            {
                for (var i = 0; i < playerRows.Count; i++)
                {
                    PlayerRow pr = playerRows[i];
                    if (pr.RowIndex != i)
                    {
                        pr.RowIndex = i;
                        playerRows[i] = pr;
                    }

                    PushScaleformMovieFunctionN("SET_DATA_SLOT");                   // call scaleform function

                    PushScaleformMovieFunctionParameterInt(3);                      // frontend menu column
                    PushScaleformMovieFunctionParameterInt(i);                      // row index

                    PushScaleformMovieFunctionParameterInt(0);                      // menu ID
                    PushScaleformMovieFunctionParameterInt(0);                      // unique ID
                    PushScaleformMovieFunctionParameterInt(2);                      // type (2 = AS_ONLINE_IN_SESSION)

                    PushScaleformMovieFunctionParameterInt(pr.PlayerRank);          // rank value / (initialIndex 1337)
                    PushScaleformMovieFunctionParameterBool(false);                 // isSelectable

                    PushScaleformMovieFunctionParameterString(pr.Player.Name);      // playerName

                    PushScaleformMovieFunctionParameterInt((int)pr.RowColor);       // rowColor

                    PushScaleformMovieFunctionParameterBool(false);                 // reduceColors (if true: removes color from left bar & reduces color opacity on row itself.)

                    PushScaleformMovieFunctionParameterInt(0);                      // unused
                    PushScaleformMovieFunctionParameterInt((int)pr.Icon);           // right player icon.
                    PushScaleformMovieFunctionParameterInt(0);                      // unused

                    PushScaleformMovieFunctionParameterString($"..+{pr.CrewTag}");  // crew label text.

                    PushScaleformMovieFunctionParameterBool(false);                 // should be a thing to toggle blinking of (kick) icon, but doesn't seem to work.

                    PushScaleformMovieFunctionParameterString(pr.Status);           // badge/status tag text
                    PushScaleformMovieFunctionParameterInt((int)pr.StatusColor);    // badge/status tag background color

                    PopScaleformMovieFunctionVoid();                                // done

                }
            }
            else
            {
                await BaseScript.Delay(0);
            }

            PushScaleformMovieFunctionN("DISPLAY_DATA_SLOT");
            PushScaleformMovieFunctionParameterInt(3);
            PopScaleformMovieFunctionVoid();
        }

        public async Task OpenMenu()
        {
            if(IsVisible) { return; }
            IsVisible = true;
            if (IsVisible)
            {
                while (IsPauseMenuActive() || IsPauseMenuRestarting() || IsFrontendFading())
                {
                    SetFrontendActive(false);
                    await BaseScript.Delay(0);
                }
                
                ActivateFrontendMenu((uint)GetHashKey($"{menuType}"), false, -1);

                while (!IsPauseMenuActive() || IsPauseMenuRestarting())
                {
                    await BaseScript.Delay(0);
                }
                N_0xb9449845f73f5e9c("SHIFT_CORONA_DESC");       // start call function - BeginScaleformMovieMethodV
                PushScaleformMovieFunctionParameterBool(true);          // push frontend title menu up.
                PushScaleformMovieFunctionParameterBool(false);         // show extra top border line
                PopScaleformMovieFunction();                        // end call function
                
                N_0xb9449845f73f5e9c("SET_HEADER_TITLE");        // Call set header function

                PushScaleformMovieFunctionParameterString(name);        // Set the title
                //
                PushScaleformMovieFunctionParameterBool(false);         // purpose unknown, is always 0 in decompiled scripts.
                //
                //
                PushScaleformMovieFunctionParameterString(subtitle);    // set the subtitle.
                //
                //
                PushScaleformMovieFunctionParameterBool(true);          // purpose unknown, is always 1 in decompiled scripts.
                PopScaleformMovieFunctionVoid();                        // finish the set header function
                //
                //
                //
                //
                await BaseScript.Delay(100);
                UpdateSettings();
                await BaseScript.Delay(100);
                UpdateDetails();
                await BaseScript.Delay(100);
                PushScaleformMovieFunctionN("DISPLAY_DATA_SLOT");
                PushScaleformMovieFunctionParameterInt(0);
                PopScaleformMovieFunctionVoid();
                
                await BaseScript.Delay(100);
                await UpdateList();
                await BaseScript.Delay(100);
                PushScaleformMovieFunctionN("DISPLAY_DATA_SLOT");
                PushScaleformMovieFunctionParameterInt(3);
                PopScaleformMovieFunctionVoid();
                
                ///// ACTIVATE THE FIRST COLUMN (FOCUS).
                await BaseScript.Delay(100);
                PushScaleformMovieFunctionN("SET_COLUMN_FOCUS");
                PushScaleformMovieFunctionParameterInt(0); // column index // _loc7_
                PushScaleformMovieFunctionParameterInt(1); // highlightIndex // _loc6_
                PushScaleformMovieFunctionParameterInt(1); // scriptSetUniqID // _loc4_
                PushScaleformMovieFunctionParameterInt(0); // scriptSetMenuState // _loc5_
                PopScaleformMovieFunctionVoid();
            }
        }

        public void CloseMenu()
        {
            IsVisible = false;
            SetFrontendActive(false);
        }
        private void UpdateSettings()
        {
            for (int i = 0; i < TopTimes.Count; i++)
            {
                ///// COLUMN 0 (LEFT) - ROW 0
                PushScaleformMovieFunctionN("SET_DATA_SLOT");
                PushScaleformMovieFunctionParameterInt(0); // column
                PushScaleformMovieFunctionParameterInt(i); // index

                // com.rockstargames.gtav.pauseMenu.pauseMenuItems.PauseMenuBaseItem::__set__data
                PushScaleformMovieFunctionParameterInt(0); // menu ID 0
                PushScaleformMovieFunctionParameterInt(i); // unique ID 0
                PushScaleformMovieFunctionParameterInt(-1); // type 0
                dynamic rightThingColor = 0;
                PushScaleformMovieFunctionParameterInt((int)rightThingColor); // initialIndex 0
                PushScaleformMovieFunctionParameterBool(false); // isSelectable true

                PushScaleformMovieFunctionParameterString(TopTimes[i].Name);
                ///// UNSURE HOW THIS WORKS, BUT IF YOU UNCOMMENT THIS, IT'LL ADD AN ICON TO THE ROW.
                ///// MAKING THE STRING "20" AND THE BOOL TRUE SEEMS TO DO SOMETHING WITH A ROCKSTAR LOGO INSTEAD.
                PushScaleformMovieFunctionParameterInt(0);
                PushScaleformMovieFunctionParameterString("20");
                PushScaleformMovieFunctionParameterInt(0);

                PushScaleformMovieFunctionParameterBool(false); // SOMETHING WITH ROCKSTAR/STAR LOGO SWITCHING.

                ///// FINISH.
                PopScaleformMovieFunctionVoid();
            }
        }

        private void UpdateDetails()
        {
            ///// COLUMN 0 (LEFT) - ROW 0
            PushScaleformMovieFunctionN("SET_DATA_SLOT");
            PushScaleformMovieFunctionParameterInt(1); // column
            PushScaleformMovieFunctionParameterInt(1); // index

            // com.rockstargames.gtav.pauseMenu.pauseMenuItems.PauseMenuBaseItem::__set__data
            PushScaleformMovieFunctionParameterInt(0); // menu ID 0
            PushScaleformMovieFunctionParameterInt(0); // unique ID 0
            PushScaleformMovieFunctionParameterInt(-1); // type 0
            dynamic rightThingColor = 0;
            PushScaleformMovieFunctionParameterInt((int)rightThingColor); // initialIndex 0
            PushScaleformMovieFunctionParameterBool(false); // isSelectable true

            PushScaleformMovieFunctionParameterString("aaa");
            PushScaleformMovieFunctionParameterString("aaa");

            ///// UNSURE HOW THIS WORKS, BUT IF YOU UNCOMMENT THIS, IT'LL ADD AN ICON TO THE ROW.
            ///// MAKING THE STRING "20" AND THE BOOL TRUE SEEMS TO DO SOMETHING WITH A ROCKSTAR LOGO INSTEAD.
            PushScaleformMovieFunctionParameterInt(0);
            PushScaleformMovieFunctionParameterString("");
            PushScaleformMovieFunctionParameterInt(0);

            PushScaleformMovieFunctionParameterBool(false); // SOMETHING WITH ROCKSTAR/STAR LOGO SWITCHING.

            ///// FINISH.
            PopScaleformMovieFunctionVoid();
        }
        #endregion
    }
}
