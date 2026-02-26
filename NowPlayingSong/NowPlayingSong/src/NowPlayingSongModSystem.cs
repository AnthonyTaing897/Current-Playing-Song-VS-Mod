using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

#nullable disable
namespace NowPlayingSong
{

    public class GuiDialogAnnoyingText : HudElement
    {
        public override string ToggleKeyCombinationCode => "annoyingtextgui";
        private IMusicTrack currSong;
        private string songName = "No song has played";
        public GuiDialogAnnoyingText(ICoreClientAPI capi) : base(capi)
        {
            SetupDialog();
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.LeftTop);

            // Just a simple 300x300 pixel box
            ElementBounds textBounds = ElementBounds.Fixed(0, 20, 210, 35);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(textBounds);

            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("myAwesomeDialog", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Song Playing", OnTitleBarCloseClicked)
                .AddStaticTextAutoFontSize("Current Song: " + this.songName, CairoFont.WhiteDetailText(), textBounds, "current song")
                .Compose()
            ;
        }

        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }
        public override void OnRenderGUI(float deltaTime)
        {
            base.OnRenderGUI(deltaTime);

            IMusicTrack song = capi.CurrentMusicTrack;
            if (song != null && song != this.currSong)
            {
                this.currSong = song;
                this.songName = GetSong(song);

                GuiElementStaticText textbox = SingleComposer.GetStaticText("current song");

                textbox.Text = "Current Song: " + this.songName;
                textbox.AutoBoxSize();
                SingleComposer.ReCompose();

            }
        }

        public static string GetSong(IMusicTrack song)
        {
            string[] songPath = song.Name.Split('/');
            string[] songFile = songPath[songPath.Length - 1].Split('.');
            return songFile[0];
        }
    }
        public class NowPlayingSongModSystem : ModSystem
        {
            private ICoreClientAPI capi;
            GuiDialog dialog;

            // Called on server and client
            // Useful for registering block/entity classes on both sides
            public override void Start(ICoreAPI api)
            {
                Mod.Logger.Notification("Hello from template mod: " + api.Side);
            }

            public override void StartServerSide(ICoreServerAPI api)
            {
                Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("nowplayingsong:hello"));
            }

            public override void StartClientSide(ICoreClientAPI api)
            {
                Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("nowplayingsong:hello"));

                dialog = new GuiDialogAnnoyingText(api);

                capi = api;
                capi.Input.RegisterHotKey("playedsonggui", "Showing what song is being played", GlKeys.F8, HotkeyType.HelpAndOverlays);
                capi.Input.SetHotKeyHandler("playedsonggui", ToggleGui);
            }
            private bool ToggleGui(KeyCombination comb)
            {
                if (dialog.IsOpened()) dialog.TryClose();
                else dialog.TryOpen();

                return true;
            }

        }
}


