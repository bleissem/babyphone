using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class Settings
    {

        public Settings(string dbPath, ISQLitePlatform platform)
        {
            m_DBPath = dbPath;
            m_SQLitePlatform = platform;
        }

        private void Load()
        {

            var db = new SQLite.Net.SQLiteConnection(m_SQLitePlatform, m_DBPath);

            db.CreateTable<SettingsTable>();
            if (0 == db.Table<SettingsTable>().Count())
            {
                var newSettings = new SettingsTable();
                newSettings.NoiseLevel = 0;
                newSettings.NumberToDial = string.Empty;
                db.Insert(newSettings);
            }

            SettingsTable table = db.Table<SettingsTable>().First();

            m_NoiseLevel = table.NoiseLevel;
            m_NumberToDial = table.NumberToDial;

        }

        private void Save()
        {
            var db = new SQLite.Net.SQLiteConnection(m_SQLitePlatform, m_DBPath);

            db.CreateTable<SettingsTable>();
            if (0 == db.Table<SettingsTable>().Count())
            {
                Load();
            }
            else
            {
                var table = db.Table<SettingsTable>().First();
                table.NoiseLevel = m_NoiseLevel;
                table.NumberToDial = m_NumberToDial;
                db.Update(table);
            }


        }


        private string m_DBPath;
        private ISQLitePlatform m_SQLitePlatform;


        private int m_NoiseLevel;

        public int NoiseLevel
        {
            get
            {
                Load();
                return m_NoiseLevel;
            }
            set
            {
                if (value == m_NoiseLevel)
                {
                    return;
                }

                m_NoiseLevel = value;
                Save();
            }
        }

        private string m_NumberToDial;

        public string NumberToDial
        {
            get
            {
                Load();
                return m_NumberToDial;
            }
            set
            {
                if (value == m_NumberToDial)
                {
                    return;
                }

                m_NumberToDial = value;
                Save();
            }
        }
    }

}
