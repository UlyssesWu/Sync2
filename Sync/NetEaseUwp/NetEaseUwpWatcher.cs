using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sync.NetEaseUwp
{
    class NetEaseUwpWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private NetEaseUwpContext _db;
        public string CurrentSong { get; private set; }

        public bool UsePInvoke { get; set; } = true;
        public bool UseForceEncoding { get; set; } = false;
        public string QQ { get; set; }

        public NetEaseUwpWatcher()
        {
            var dbPath = UwpHelper.GetNetEaseDbPath();
            if (!File.Exists(dbPath))
            {
                return;
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(dbPath))
            {
                Filter = Path.GetFileName(dbPath),
                EnableRaisingEvents = false
            };
            _db = new NetEaseUwpContext($"Data Source=\"{dbPath}\"");
            _watcher.Changed += DbChanged;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
            DbChanged(null, null);
        }

        public void Pause()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }
            Helper.Send2QQ(QQ, "", UsePInvoke);
        }
        
        private void DbChanged(object sender, FileSystemEventArgs e)
        {
            var currentSongItem = _db.PlayHistories.OrderByDescending(item => item.updatetime).FirstOrDefault();
            if (currentSongItem == null)
            {
                return;
            }
            dynamic jItem = JsonConvert.DeserializeObject(currentSongItem.resourcedata);
            string songName = jItem.track.name.ToString();
            StringBuilder sb = new StringBuilder();
            foreach (dynamic artist in jItem.track.ar)
            {
                if (artist.name == null)
                {
                    return;
                }
                sb.Append(artist.name.ToString());
                sb.Append(" & ");
            }
            sb.Remove(sb.Length - 3, 3);
            string songArtist = sb.ToString();
            string songFullName = $"{songArtist} - {songName}";
            if (string.IsNullOrWhiteSpace(CurrentSong) || !CurrentSong.Equals(songFullName))
            {
                CurrentSong = songFullName;
                Helper.Send2QQ(QQ, CurrentSong, UsePInvoke, UseForceEncoding);
            }
        }

        public void Dispose()
        {
            Pause();
            _watcher.Dispose();
            _db.Dispose();
        }
    }
}
