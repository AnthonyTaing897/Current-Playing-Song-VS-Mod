using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

#nullable disable
namespace NowPlayingSong
{

    public class PlayingSongHud : HudElement
    {
        public override string ToggleKeyCombinationCode => "playingSongHudElement";
        private IMusicTrack currSong;
        private string songName = "No song has played";
        public PlayingSongHud(ICoreClientAPI capi) : base(capi)
        {
            SetupDialog();
        }

        private void SetupDialog()
        {
            // Auto-sized dialog at the top left of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.LeftTop);

            ElementBounds textBounds = ElementBounds.Fixed(55, 20, 210, 35);

            ElementBounds imgBounds = ElementBounds.Fixed(-5, 20, 50, 50);

            AssetLocation image = new AssetLocation("nowplayingsong", "textures/hud/test/note.png");

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(textBounds);
            bgBounds.WithChildren(imgBounds);

            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("playingSongHud", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Song Playing", OnTitleBarCloseClicked)
                .AddStaticTextAutoFontSize("Current Song: " + this.songName, CairoFont.WhiteDetailText(), textBounds, "current song")
                .AddImage(imgBounds, image)
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
            GuiElementStaticText textbox = SingleComposer.GetStaticText("current song");
            IMusicTrack song = capi.CurrentMusicTrack;
            if (song != null && (this.songName == null || GetSong(song) != this.songName))
            {
                this.currSong = song;
                this.songName = GetSong(song);

                

                textbox.Text = "Current Song: " + this.songName.Split('.')[0];
                textbox.AutoBoxSize();
                SingleComposer.ReCompose();
            }
        }

        public static string GetSong(IMusicTrack song)
        {
            string[] songPath = song.Name.Split('/');
            string songFile = songPath[songPath.Length - 1];

            if (songFile.Split('(',')').Length > 1)
            { return songFile.Split('(',')')[1]; }

            return songFile;
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

                dialog = new PlayingSongHud(api);

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


