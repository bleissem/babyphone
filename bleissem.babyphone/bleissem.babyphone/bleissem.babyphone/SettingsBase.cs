using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public abstract class DBSettingBase<TTable> where TTable : class, new()
    {
        public DBSettingBase(string dbPath, ISQLitePlatform platform)
        {
            m_DBPath = dbPath;
            m_Platform = platform;
            Load();
        }

        protected string m_DBPath;
        protected ISQLitePlatform m_Platform;

        protected void Load()
        {

            using (var db = new SQLite.Net.SQLiteConnection(m_Platform, m_DBPath))
            {

                db.CreateTable<TTable>();
                if (0 == db.Table<TTable>().Count())
                {
                    var newSettings = new TTable();
                    InitializeTable(newSettings);
                    db.Insert(newSettings);
                }

                TTable table = db.Table<TTable>().First();
                Load(table);
            }
        }

        protected abstract void InitializeTable(TTable table);
        protected abstract void Load(TTable table);

        protected void Save()
        {
            using (SQLite.Net.SQLiteConnection db = new SQLite.Net.SQLiteConnection(m_Platform, m_DBPath))
            {
                db.CreateTable<TTable>();
                if (0 == db.Table<TTable>().Count())
                {
                    Load();
                }

                var table = db.Table<TTable>().First();
                Save(table);
                db.Update(table);

            }
        }

        protected abstract void Save(TTable table);
    }
}
