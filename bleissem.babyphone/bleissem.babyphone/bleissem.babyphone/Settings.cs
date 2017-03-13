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

            using (SQLite.Net.SQLiteConnection db = new SQLite.Net.SQLiteConnection(m_SQLitePlatform, m_DBPath))
            {

                db.CreateTable<SettingsTable>();
                if (0 == db.Table<SettingsTable>().Count())
                {
                    var newSettings = new SettingsTable();
                    newSettings.NoiseLevel = 0;
                    newSettings.NumberToDial = string.Empty;
                    newSettings.CallType = SettingsTable.CallTypeEnum.Phone;
                    newSettings.IsSkypeVideoEnabled = false;
                    db.Insert(newSettings);
                }

                SettingsTable table = db.Table<SettingsTable>().First();

                m_NoiseLevel = table.NoiseLevel;
                m_NumberToDial = table.NumberToDial;
                m_CallType = table.CallType;
                m_IsSkypeVideoEnabled = table.IsSkypeVideoEnabled;
            }
        }

        private void Save()
        {
            using (SQLite.Net.SQLiteConnection db = new SQLite.Net.SQLiteConnection(m_SQLitePlatform, m_DBPath))
            {
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
                    table.CallType = m_CallType;
                    table.IsSkypeVideoEnabled = m_IsSkypeVideoEnabled;
                    db.Update(table);
                }

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

        private SettingsTable.CallTypeEnum m_CallType;

        public SettingsTable.CallTypeEnum CallType
        {
            get
            {
                Load();
                return m_CallType;
            }
            set
            {
                if (value == m_CallType)
                {
                    return;
                }

                m_CallType = value;
                Save();
            }
        }

        private bool m_IsSkypeVideoEnabled;

        public bool IsSkypeVideoEnabled
        {
            get
            {
                Load();
                return m_IsSkypeVideoEnabled;
            }
            set
            {
                if (value == m_IsSkypeVideoEnabled)
                {
                    return;
                }

                m_IsSkypeVideoEnabled = value;
                Save();
            }
        }
    }

}
