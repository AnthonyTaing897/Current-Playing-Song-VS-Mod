
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

#nullable disable

namespace NowPlayingSong.src
{
    internal class ShowPlayingSong : EntityBehavior
    {
        private ICoreAPI api;
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;
        public IMusicTrack currentSong;

        public ShowPlayingSong(Entity entity) : base(entity)
        {
            this.api = entity.Api;
            this.capi = api as ICoreClientAPI;
            this.sapi = api as ICoreServerAPI;
        }

        public override void OnGameTick(float deltaTime)
        {
            base.OnGameTick(deltaTime);
            if (capi != null)
            {
                IMusicTrack song = capi.CurrentMusicTrack;

                if (song != null && currentSong != song)
                {
                    currentSong = song;
                    capi.SendChatMessage("Current song: " + song.Name.Split('/').Last().TrimEnd(['(',')',' ']));
                }
            }
        }

        public override string PropertyName()
        {
            return "showplayingsong";
        }
    }
}
