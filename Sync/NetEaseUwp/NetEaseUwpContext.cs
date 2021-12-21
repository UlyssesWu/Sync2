using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;

// ReSharper disable InconsistentNaming

namespace Sync.NetEaseUwp
{
    internal class NetEaseUwpContext : DataConnection
    {
        public NetEaseUwpContext(string nameOrConnectionString) : base(new SQLiteDataProvider(ProviderName.SQLite), nameOrConnectionString)
        {
        }

        public ITable<PlayHistory> PlayHistories => GetTable<PlayHistory>();
        public ITable<PlayQueue> PlayQueues => GetTable<PlayQueue>();
    }

    [Table("playhistory")]
    public class PlayHistory
    {
        [PrimaryKey, Identity]
        public string resourceid { get; set; }
        [Column]
        public long resourcetype { get; set; }
        [Column]
        public long userid { get; set; }
        [Column]
        public string resourcedata { get; set; }
        [Column]
        public long updatetime { get; set; }
        [Column]
        public string extrainfo { get; set; }
    }

    [Table("playqueue")]
    public class PlayQueue
    {
        [PrimaryKey, Identity]
        public string id { get; set; }
        [Column]
        public long trackid { get; set; }
        [Column]
        public string trackdata { get; set; }
        [Column]
        public string extradata { get; set; }
        [Column]
        public string privilege { get; set; }
        [Column]
        public string urlinfo { get; set; }
        [Column]
        public long updatetime { get; set; }
        [Column]
        public string extrainfo { get; set; }
    }
}
