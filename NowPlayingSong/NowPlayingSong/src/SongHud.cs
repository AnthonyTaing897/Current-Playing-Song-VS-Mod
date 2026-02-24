using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace NowPlayingSong.src
{
    #nullable disable
    internal class SongHud : HudElement
    {
        private IMusicTrack currentSong;

        private ICoreAPI api;
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        public SongHud(ICoreClientAPI capi) : base(capi)
        {
            this.capi = capi;
        }

        public void OnGameTick(float deltaTime)
        {
            if (capi != null)
            {
                IMusicTrack song = capi.CurrentMusicTrack;

                if (song != null && currentSong != song)
                {
                    currentSong = song;
                    
                }
            }
        }
    }
}
