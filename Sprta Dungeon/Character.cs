using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprta_Dungeon
{
    public interface ICharacter
    {
        string Name { get; }
        int Level { get; set; }
        int Atk { get; set; }
        int Def { get; set; }
        public int HP { get; set; }
        int Gold { get; set; }
        bool isDead { get; }
        void TakeDamage(int damage);
    }

    public class Player : ICharacter
    {
        public string Name { get; }
        public string Class { get; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Gold { get; set; }
        public int ExtraAtk { get; set; }
        public int ExtraDef { get; set; }
        public int ExtraHP { get; set; }
        public bool isDead => HP <= 0;
        public List<IItem> Items { get; set; }
        public string[] BodyPart { get; set; }
        public Player(string _name, string _class, int atk, int def, int hP, int maxHP)
        {
            Name = _name;
            Class = _class;
            Level = 1;
            Exp = 0;
            Atk = atk;
            Def = def;
            MaxHP = maxHP;
            HP = hP;
            Gold = 500;
            ExtraAtk = ExtraDef = ExtraHP = 0;
            Items = new List<IItem>();
            BodyPart = new string[3] { " ", " ", " " };
        }
        public Player(string _name, string _class)
        {
            Name = _name;
            Class = _class;
            Level = 1;
            Exp = 0;
            Atk = 10;
            Def = 5;
            MaxHP = 100;
            HP = 100;
            Gold = 500;
            ExtraAtk = ExtraDef = ExtraHP = 0;
            Items = new List<IItem>();
            BodyPart = new string[3] { " ", " ", " " };
        }
        //불러오기를 위한 생성자
        public Player(string name, string @class, int level, int exp, int atk, int def, int hP, int maxHP, int gold, int extraAtk, int extraDef, int extraHP, List<IItem> items, string[] bodypart)
        {
            Name = name;
            Class = @class;
            Level = level;
            Exp = exp;
            Atk = atk;
            Def = def;
            HP = hP;
            MaxHP = maxHP;
            Gold = gold;
            ExtraAtk = extraAtk;
            ExtraDef = extraDef;
            ExtraHP = extraHP;
            Items = items;
            BodyPart = bodypart;
        }

        public void TakeDamage(int damage)
        {
            int realDamage = damage - Def >= 0 ? damage - Def : 0;
            HP -= realDamage;
        }
    }
}
