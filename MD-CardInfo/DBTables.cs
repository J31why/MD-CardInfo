using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD_CardInfo
{
    [Flags]
    public enum Race
    {
        战士 = 1,
        魔法师 = 2,
        天使 = 4,
        恶魔 = 8,
        不死 = 16,
        机械 = 32,
        水 = 64,
        炎 = 128,
        岩石 = 256,
        鸟兽 = 512,
        植物 = 1024,
        昆虫 = 2048,
        雷 = 4096,
        龙 = 8192,
        兽 = 16384,
        兽战士 = 32768,
        恐龙 = 65536,
        鱼 = 131072,
        海龙 = 262144,
        爬虫类 = 524288,
        念动力 = 1048576,
        幻神兽 = 2097152,
        创造神 = 4194304,
        幻龙 = 8388608,
        电子界 = 16777216
    }
    [Flags]
    public enum Type
    {
        怪兽 = 1,
        魔法 = 2,
        陷阱 = 4,
        通常 = 16,
        效果 = 32,
        融合 = 64,
        仪式 = 128,
        灵魂 = 512,
        同盟 = 1024,
        二重 = 2048,
        调整 = 4096,
        同调 = 8192,
        速攻 = 65536,
        永续 = 131072,
        装备 = 262144,
        场地 = 524288,
        反击 = 1048576,
        反转 = 2097152,
        卡通 = 4194304,
        超量 = 8388608,
        灵摆 = 16777216,
        特殊召唤 = 33554432,
        连接 = 67108864
    }
    [Flags]
    public enum Attribute
    {
        地 = 1,
        水 = 2,
        炎 = 4,
        风 = 8,
        光 = 16,
        暗 = 32,
        神 = 64
    }
    [Flags]
    public enum LinkMark
    {
        LeftDown=1,
        Down =2,
        RightDown=4,
        Left=8,
        Right=32,
        LeftUp=64,
        Up=128,
        RightUp=256
    }
    public class DBTables
    {
        public class Cards
        {
            public Cards()
            {
                cn_name = en_name = jp_name = jp_name2 = desc = string.Empty;
            }
            [PrimaryKey]
            public int cid { get; set; }
            public int tid { get; set; }
            public int passcode { get; set; }
            public string ygoPasscode { get; set; }
            public string cn_name { get; set; }
            public string en_name { get; set; }
            public string jp_name { get; set; }
            public string jp_name2 { get; set; }
            public string desc { get; set; }
            public int level { get; set; }
            public int atk { get; set; }
            public int def { get; set; }
            public int race { get; set; }
            public int type { get; set; }
            public int attribute { get; set; }
        }
    }
}
