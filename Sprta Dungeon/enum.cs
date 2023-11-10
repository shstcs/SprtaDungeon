using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprta_Dungeon
{
    public enum PlayerClass
    {
        Warrior = 1,
        Magician,
        Rogue,
        Priest
    }

    public enum PlayerStat
    {
        Atk = 1,
        Def,
        HP
    }

    public enum BodyParts
    {
        Weapon,
        Armor,
        Shield
    }

    public enum GameScenes
    {
        GameIntro,
        MyInfo,
        Inventory,
        SetItem,
        SortItem,
        Shop,
        BuyItem,
        SellItem,
        Dungeon,
        DungeonEncounter1,
        DungeonEncounter2,
        DungeonEncounter3,
        DungeonEncounter4,
        DungeonClear,
        Rest
    }

    public enum EncounterNum
    {
        GetItem,
        FightMob,
        MeetSomeone,
        DrinkPotion
    }
}
