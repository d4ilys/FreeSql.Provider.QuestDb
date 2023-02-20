using FreeSql.Provider.QuestDb.Subtable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace Test
{
    [Index("Id_Index", nameof(Id), false)]
    class QuestDb_Model_Test01
    {
        [Column(DbType = "symbol")] public string Id { get; set; }

        [Column(OldName = "Name")] public string NameUpdate { get; set; }

        public string NameInsert { get; set; } = "NameDefault";

        public double? Activos { get; set; }

        [AutoSubtable(SubtableType.Day)] public DateTime? CreateTime { get; set; }

        public bool? IsCompra { get; set; }
    }
}