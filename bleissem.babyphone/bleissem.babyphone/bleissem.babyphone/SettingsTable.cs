using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    [Table("Settings")]
    public class SettingsTable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int NoiseLevel { get; set; }
        public string NumberToDial { get; set; }
    }
}
