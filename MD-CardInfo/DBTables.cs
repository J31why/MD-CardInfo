using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD_CardInfo
{
    public class DBTables
    {
        public class Cards
        {
            public Cards()
            {
                cn_name = en_name = desc = race = type = attribute = string.Empty;
            }
            [PrimaryKey]
            public int id { get; set; }
            public int password { get; set; }
            public string cn_name { get; set; }
            public string en_name { get; set; }
            public string desc { get; set; }
            public int level { get; set; }
            public int atk { get; set; }
            public int def { get; set; }
            public string race { get; set; }
            public string type { get; set; }
            public string attribute { get; set; }
        }
    }
}
