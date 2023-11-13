using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
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
        public string Name { get; set; }
        public string Class { get; set; }
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
        //public Player() { }

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

    public class Warrior : Player
    {
        public Warrior(string _name, string _class) : base(_name, _class,1,0,5,10,100,100,500,0,0,0, new List<IItem>(), new string[3] { " ", " ", " " })
        {
            
        }
    }
    public class Magicion : Player
    {
        public Magicion(string _name, string _class) : base(_name, _class, 1, 0, 9, 8, 80, 80, 500, 0, 0, 0, new List<IItem>(), new string[3] { " ", " ", " " })
        {
            
        }
    }
    public class Rouge : Player
    {
        public Rouge(string _name, string _class) : base(_name, _class, 1, 0, 8, 8, 100, 100, 500, 0, 0, 0, new List<IItem>(), new string[3] { " ", " ", " " })
        {
            
        }
    }
    public class Priest : Player
    {
        public Priest(string _name, string _class) : base(_name, _class, 1, 0, 3, 10, 120, 120, 500, 0, 0, 0, new List<IItem>(), new string[3] { " ", " ", " " })
        {

        }
    }
}
