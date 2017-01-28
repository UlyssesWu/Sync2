using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Sync.NetEaseUwp
{
    internal class NetEaseUwpContext : DbContext
    {
        public NetEaseUwpContext(string nameOrConnectionString) : base(new SQLiteConnection(nameOrConnectionString), true)
        {
        }

        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<PlayQueue> PlayQueues { get; set; }
    }

    [Table("playhistory")]
    public class PlayHistory
    {
        [Key]
        public string resourceid { get; set; }
        public long resourcetype { get; set; }
        public long userid { get; set; }
        public string resourcedata { get; set; }
        public long updatetime { get; set; }
        public string extrainfo { get; set; }
    }

    [Table("playqueue")]
    public class PlayQueue
    {
        [Key]
        public string id { get; set; }
        public long trackid { get; set; }
        public string trackdata { get; set; }
        public string extradata { get; set; }
        public string privilege { get; set; }
        public string urlinfo { get; set; }
        public long updatetime { get; set; }
        public string extrainfo { get; set; }
    }
}
