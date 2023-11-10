using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprta_Dungeon
{
    public interface IItem
    {
        string Name { get; set; }
        string Effect { get; }
        string Description { get; }
        int Price { get; }
        bool isEquip { get; set; }
        void Use(Player player);
    }
    public class Weapon : IItem
    {
        public string Name { get; set; }
        public string Effect { get; }
        public string Description { get; }
        public int Price { get; }
        public bool isEquip { get; set; }
        public int StatusUp { get; }
        public void Use(Player player)
        {
            if (isEquip)
            {
                player.Atk -= StatusUp;
                Name = Name.Remove(0, 3);
                player.ExtraAtk -= StatusUp;
                player.BodyPart[(int)BodyParts.Weapon] = " ";
            }
            else
            {
                player.Atk += StatusUp;
                Name = "[E]" + Name;
                player.ExtraAtk += StatusUp;
            }
            isEquip = !isEquip;
        }
        public Weapon(string _name, string _effect, string _description, int _statusUp, int _price)
        {
            Name = _name;
            Effect = _effect;
            Description = _description;
            Price = _price;
            isEquip = false;
            StatusUp = _statusUp;
        }
    }
    public class Armor : IItem
    {
        public string Name { get; set; }
        public string Effect { get; }
        public string Description { get; }
        public int Price { get; }
        public bool isEquip { get; set; }
        public int StatusUp { get; }
        public void Use(Player player)
        {
            if (isEquip)
            {
                player.MaxHP -= StatusUp;
                player.HP -= StatusUp;
                Name = Name.Remove(0, 3);
                player.ExtraHP -= StatusUp;
                player.BodyPart[(int)BodyParts.Armor] = " ";
            }
            else
            {
                player.MaxHP += StatusUp;
                player.HP += StatusUp;
                Name = "[E]" + Name;
                player.ExtraHP += StatusUp;
            }
            isEquip = !isEquip;
        }
        public Armor(string _name, string _effect, string _description, int _statusUp, int _price)
        {
            Name = _name;
            Effect = _effect;
            Description = _description;
            Price = _price;
            isEquip = false;
            StatusUp = _statusUp;
        }
    }
    public class Shield : IItem
    {
        public string Name { get; set; }
        public string Effect { get; }
        public string Description { get; }
        public int Price { get; }
        public bool isEquip { get; set; }
        public int StatusUp { get; }
        public void Use(Player player)
        {
            if (isEquip)
            {
                player.Def -= StatusUp;
                Name = Name.Remove(0, 3);
                player.ExtraDef -= StatusUp;
                player.BodyPart[(int)BodyParts.Shield] = " ";
            }
            else
            {
                player.Def += StatusUp;
                Name = "[E]" + Name;
                player.ExtraDef += StatusUp;
            }
            isEquip = !isEquip;
        }
        public Shield(string _name, string _effect, string _description, int _statusUp, int _price)
        {
            Name = _name;
            Effect = _effect;
            Description = _description;
            Price = _price;
            isEquip = false;
            StatusUp = _statusUp;
        }
    }
}


