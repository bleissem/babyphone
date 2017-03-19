using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class Settings: DBSettingBase<SettingsTable>
    {

        public Settings(string dbPath, ISQLitePlatform platform):base(dbPath, platform)
        {

        }

        protected override void InitializeTable(SettingsTable table)
        {
            table.NoiseLevel = 0;
            table.NumberToDial = string.Empty;
            table.CallType = SettingsTable.CallTypeEnum.Phone;
            table.IsSkypeVideoEnabled = false;
        }

        protected override void Load(SettingsTable table)
        {
            m_NoiseLevel = table.NoiseLevel;
            m_NumberToDial = table.NumberToDial;
            m_CallType = table.CallType;
            m_IsSkypeVideoEnabled = table.IsSkypeVideoEnabled;
        }

        protected override void Save(SettingsTable table)
        {
            table.NoiseLevel = m_NoiseLevel;
            table.NumberToDial = m_NumberToDial;
            table.CallType = m_CallType;
            table.IsSkypeVideoEnabled = m_IsSkypeVideoEnabled;
        }
      
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
