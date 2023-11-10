using ConsoleTables;
using NAudio;
using NAudio.Wave;
using static System.Net.Mime.MediaTypeNames;

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
        public Player() { }
        public Player(string _name, string _class)
        {
            Name = _name;
            Class = _class;
            switch (_class)
            {

            }
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

    internal class Program
    {
        private static Player player;
        private static List<IItem> inven = new List<IItem>();
        static string className;
        public static List<IItem> Shop = new List<IItem>() { };
        public static Dictionary<int, IItem> ItemArchive;
        static AudioFileReader startBGM = new AudioFileReader("E:\\TeamSparta\\Sprta Dungeon\\StartBGM.wav");
        static AudioFileReader mainBGM = new AudioFileReader("E:\\TeamSparta\\Sprta Dungeon\\mainBGM.wav");
        static WaveOutEvent audioMgr = new WaveOutEvent();
        static bool[] ViewedSceneCheck = new bool[20];

        static void Main(string[] args)
        {
            DisplayStartGame();
            DisplayGameIntro();
        }
        static void DisplayStartGame()
        {
            audioMgr.Init(startBGM);
            audioMgr.Play();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            TypingMessage("던전");
            Console.ResetColor();
            TypingMessageLine("에 오신 걸 환영합니다.");
            Console.WriteLine();
            TypingMessageLine("1. 새로 시작한다.");
            TypingMessageLine("2. 이전 모험을 이어한다.");
            TypingMessageLine("0. 게임 종료하기.");
            Console.WriteLine();
            TypingMessageLine("원하시는 행동을 입력해주세요.");
            Console.Write(">>");
            SetItemArchive();
            for (int i = 0; i < ViewedSceneCheck.Length; i++)
            {
                ViewedSceneCheck[i] = false;
            }

            int input = CheckValidInput(0, 2);
            switch (input)
            {
                case 0:
                    break;

                case 1:
                    NewGameDataSetting();
                    break;

                case 2:
                    if (File.Exists("E:\\TeamSparta\\Sprta Dungeon\\savedata.txt"))
                    {
                        LoadGameDataSetting();
                    }
                    else
                    {
                        Console.WriteLine("데이터를 찾을 수 없습니다.");
                        Thread.Sleep(700);
                        DisplayStartGame();
                    }
                    break;

            }
        }

        static void SetItemArchive()
        {
            ItemArchive = new Dictionary<int, IItem>()
            {
                {0, new Armor("무쇠갑옷", "체력 + 50", "무쇠로 만들어져 튼튼한 갑옷입니다.", 50, 300) },
                {1, new Weapon("낡은 검", "공격력 + 3", "쉽게 볼 수 있는 낡은 검입니다.", 3, 150)},
                {2, new Shield("목제 방패", "방어력 + 5", "위험한 공격을 한 번은 막아줄지도 모릅니다.", 5, 200)},
                {3, new Armor("수련자 갑옷", "체력 + 30", "수련에 도움을 주는 갑옷입니다.", 30, 200) },
                {4, new Armor("스파르타의 갑옷", "체력 + 150", "스파르타의 전사들이 사용했다는 전설의 갑옷.", 150, 1000)},
                {5, new Weapon("제국식 창", "공격력 + 7", "보급형이지만 충분히 날카롭다.", 7, 400)},
                {6, new Weapon("붉은 채찍", "공격력 + 10", "사실 다른 용도로 만들었지만 생각보다 강했다고 한다.", 10, 750)},
                {7, new Shield("타워 실드", "방어력 + 10", "몸 전체를 가려줄만큼 큰 방패", 10, 600)},
                {8, new Shield("제국식 버클러", "방어력 + 12", "특수 합금으로 만들어져 내구도가 강하다.", 12, 700)},
                {9, new Weapon("엑스칼리버", "공격력 + 100?", "진품인지는 확실하지 않다.", 10, 10000)},
                {10, new Armor("최첨단 나노 슈트", "체력 + 300", "아이언맨에 나온 그거다.", 30, 3000)},
                {11, new Shield("진압 방패", "방어력 + 8", "비살상 진압을 위해 사용한다.", 8, 400)},
            };

        }

        static void NewGameDataSetting()
        {
            Console.Clear();
            TypingMessageLine("고대 스파르타가 멸망한 지 500년.");
            Thread.Sleep(700);
            TypingMessageLine("한 위대한 모험가가 유적을 발견하면서 스파르타의 문명은 세상에 드러나게 되었다.");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessage("허나 알지 못하는 이유로 인해 ");
            Console.ForegroundColor = ConsoleColor.Red;
            TypingMessage("던전");
            Console.ResetColor();
            TypingMessageLine("으로 변모한 이곳. ");
            Thread.Sleep(700);
            TypingMessageLine("각종 몬스터와 함정으로 뒤덮여있어 베테랑 모험가들도 생존을 장담할 수 없는 위험한 지역이 되었다.");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessage("그러나 이 ");
            Console.ForegroundColor = ConsoleColor.Red;
            TypingMessage("던전");
            Console.ResetColor();
            TypingMessageLine(" 어딘가에 ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypingMessage("스파르탄의 보물");
            Console.ResetColor();
            TypingMessageLine("이 있다는 비밀스러운 소문이 퍼지자,");
            Thread.Sleep(700);
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypingMessage("보물");
            Console.ResetColor();
            TypingMessageLine("을 노리는 많은 모험가들이 찾아왔고.");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessageLine("어떤 이들은 포기하고 도망갔으며, ");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessage("어떤 이들은 ");
            Console.ForegroundColor = ConsoleColor.Red;
            TypingMessage("던전");
            Console.ResetColor();
            TypingMessageLine("에 잡아먹혔고,");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessageLine("어떤 이는 아직까지도 살아간다.");
            Thread.Sleep(1500);
            Console.Clear();
            TypingMessageLine("그리고 드디어, 이 던전을 끝내기 위한 한 모험가가 찾아오는데…");
            Thread.Sleep(1500);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            TypingMessage("스파르탄 던전");
            Console.ResetColor();
            TypingMessageLine("에 오신 걸 환영합니다.");
            TypingMessageLine("당신의 이름은 무엇입니까?\n ");
            string name = Console.ReadLine();

            Console.Clear();

            TypingMessageLine($"반갑습니다. {name}");
            TypingMessageLine("당신의 직업은 무엇입니까?\n\n1. 전사\n2. 마법사\n3. 도적\n4. 사제\n");
            Console.Write(">>");
            while (true)
            {
                int selectClass = CheckValidInput(1, 4);

                switch (selectClass)
                {
                    case (int)PlayerClass.Warrior:
                        className = "전사";
                        break;
                    case (int)PlayerClass.Magician:
                        className = "마법사";
                        break;
                    case (int)PlayerClass.Rogue:
                        className = "도적";
                        break;
                    case (int)PlayerClass.Priest:
                        className = "사제";
                        break;
                }
                break;

            }
            player = new Player(name, className);

            player.Items.Add(new Armor("무쇠갑옷", "체력 + 50", "무쇠로 만들어져 튼튼한 갑옷입니다.", 50, 300));
            player.Items.Add(new Weapon("낡은 검", "공격력 + 3", "쉽게 볼 수 있는 낡은 검입니다.", 3, 150));
            player.Items.Add(new Shield("목제 방패", "방어력 + 5", "위험한 공격을 한 번은 막아줄지도 모릅니다.", 5, 200));
            // 아이템 정보 세팅

            Random randomItem = new Random();

            Shop.Add(ItemArchive[randomItem.Next(0, 11)]);
            Shop.Add(ItemArchive[randomItem.Next(0, 11)]);
            Shop.Add(ItemArchive[randomItem.Next(0, 11)]);
        }

        static void LoadGameDataSetting()
        {

            if (File.Exists("E:\\TeamSparta\\Sprta Dungeon\\savedata.txt"))
            {
                StreamReader LoadManager = new StreamReader("E:\\TeamSparta\\Sprta Dungeon\\savedata.txt");

                Console.Clear();
                string[] PlayerData = LoadManager.ReadLine().Split(',');
                TypingMessage("전에 사용하던 이름을 입력해주세요.\n>>");
                if (Console.ReadLine() != PlayerData[0])
                {
                    Console.WriteLine("정보가 일치하지 않습니다.");
                    LoadManager.Close();
                    LoadGameDataSetting();
                }
                else
                {
                    string[] ItemData = LoadManager.ReadLine().Split(',');
                    string[] Bodyinput = LoadManager.ReadLine().Split(',');

                    List<IItem> Itemlist = new List<IItem>();
                    foreach (string item in ItemData)
                    {
                        if (int.Parse(item) >= 0)
                        {
                            Itemlist.Add(ItemArchive[int.Parse(item)]);
                        }
                    }
                    string[] BodyData = new string[3];
                    for (int i = 0; i < Bodyinput.Length; i++)
                    {
                        if (Bodyinput[i] == " ")
                        {
                            BodyData[i] = " ";
                        }
                        else
                        {
                            BodyData[i] = ItemArchive[int.Parse(Bodyinput[i])].Name;
                        }
                    }

                    player = new Player(PlayerData[0], PlayerData[1], int.Parse(PlayerData[2]), int.Parse(PlayerData[3]), int.Parse(PlayerData[4]),
                        int.Parse(PlayerData[5]), int.Parse(PlayerData[6]), int.Parse(PlayerData[7]), int.Parse(PlayerData[8]), 0,
                        0, 0, Itemlist, BodyData);

                    foreach (IItem item in Itemlist)
                    {
                        foreach (string equipItemName in player.BodyPart)
                            if (equipItemName == item.Name)
                            {
                                item.Use(player);
                            }
                    }

                    Random randomItem = new Random();

                    Shop.Add(ItemArchive[randomItem.Next(0, 11)]);
                    Shop.Add(ItemArchive[randomItem.Next(0, 11)]);
                    Shop.Add(ItemArchive[randomItem.Next(0, 11)]);

                    for (int i = 0; i < 20; i++)
                    {
                        ViewedSceneCheck[i] = true;
                    }

                    LoadManager.Close();
                }
            }
        }

        static void TypingMessageLine(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(100);
            }
            Console.WriteLine();
        }
        static void TypingMessage(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(100);
            }
        }
        static void DisplayGameIntro()
        {
            if (ViewedSceneCheck[(int)GameScenes.GameIntro] == false)
            {
                ViewedSceneCheck[(int)GameScenes.GameIntro] = true;
                audioMgr.Stop();
                audioMgr.Init(mainBGM);
                audioMgr.Play();
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                TypingMessage("스파르타 마을");
                Console.ResetColor();
                TypingMessageLine("에 오신 여러분 환영합니다.");
                TypingMessageLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
                Console.WriteLine();
                TypingMessageLine("1. 상태보기");
                TypingMessageLine("2. 인벤토리");
                TypingMessageLine("3. 상점");
                TypingMessageLine("4. 던전 입장");
                TypingMessageLine("5. 휴식하기");
                TypingMessageLine("6. 저장하기");
                TypingMessageLine("0. 게임 종료하기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 6);
                switch (input)
                {
                    case 0:
                        Environment.Exit(0);
                        break;

                    case 1:
                        DisplayMyInfo();
                        break;

                    case 2:
                        DisplayInventory();
                        break;
                    case 3:
                        DisplayShop();
                        break;
                    case 4:
                        GoToDungeonEntrance();
                        break;
                    case 5:
                        TakeARest();
                        break;
                    case 6:
                        SaveDungeon(player);
                        break;
                }
            }
            else
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("스파르타 마을");
                Console.ResetColor();
                Console.WriteLine("에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("1. 상태보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전 입장");
                Console.WriteLine("5. 휴식하기");
                Console.WriteLine("6. 저장하기");
                Console.WriteLine("0. 게임 종료하기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 6);
                switch (input)
                {
                    case 0:
                        Environment.Exit(0);
                        break;

                    case 1:
                        DisplayMyInfo();
                        break;

                    case 2:
                        DisplayInventory();
                        break;
                    case 3:
                        DisplayShop();
                        break;
                    case 4:
                        GoToDungeonEntrance();
                        break;
                    case 5:
                        TakeARest();
                        break;
                    case 6:
                        SaveDungeon(player);
                        break;
                }
            }

        }

        static void DisplayMyInfo()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.MyInfo] == false)
            {
                ViewedSceneCheck[(int)GameScenes.MyInfo] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("상태보기");
                Console.ResetColor();
                TypingMessageLine("캐릭터의 정보를 표시합니다.");
                Console.WriteLine();
                TypingMessageLine($"Lv.{player.Level}");
                TypingMessageLine($"{player.Name} ({player.Class})");

                TypingMessage($"공격력 :{player.Atk}");
                if (player.ExtraAtk != 0) ShowExtraStat(PlayerStat.Atk); else Console.WriteLine();

                TypingMessage($"방어력 : {player.Def}");
                if (player.ExtraDef != 0) ShowExtraStat(PlayerStat.Def); else Console.WriteLine();

                TypingMessage($"체력 : {player.MaxHP} / {player.HP}");
                if (player.ExtraHP != 0) ShowExtraStat(PlayerStat.HP); else Console.WriteLine();

                TypingMessageLine($"Gold : {player.Gold} G");
                Console.WriteLine();
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 0);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("상태보기");
                Console.ResetColor();
                Console.WriteLine("캐릭터의 정보를 표시합니다.");
                Console.WriteLine();
                Console.WriteLine($"Lv.{player.Level}");
                Console.WriteLine($"{player.Name} ({player.Class})");

                Console.Write($"공격력 :{player.Atk}");
                if (player.ExtraAtk != 0) ShowExtraStat(PlayerStat.Atk); else Console.WriteLine();

                Console.Write($"방어력 : {player.Def}");
                if (player.ExtraDef != 0) ShowExtraStat(PlayerStat.Def); else Console.WriteLine();

                Console.Write($"체력 : {player.MaxHP} / {player.HP}");
                if (player.ExtraHP != 0) ShowExtraStat(PlayerStat.HP); else Console.WriteLine();

                Console.WriteLine($"Gold : {player.Gold} G");
                Console.WriteLine();
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 0);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                }
            }

        }

        static void DisplayInventory()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.Inventory] == false)
            {
                ViewedSceneCheck[(int)GameScenes.Inventory] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("인벤토리");
                Console.ResetColor();
                TypingMessageLine("보유중인 아이템을 확인할 수 있습니다.");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable();
                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                TypingMessageLine("1. 장착 관리");
                TypingMessageLine("2. 아이템 정렬");
                TypingMessageLine("0. 나가기");

                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 2);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        DisplaySetItems();
                        break;
                    case 2:
                        DisplaySortItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("인벤토리");
                Console.ResetColor();
                Console.WriteLine("보유중인 아이템을 확인할 수 있습니다.");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                Console.WriteLine("1. 장착 관리");
                Console.WriteLine("2. 아이템 정렬");
                Console.WriteLine("0. 나가기");

                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 2);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        DisplaySetItems();
                        break;
                    case 2:
                        DisplaySortItems();
                        break;
                }
            }

        }

        static void DisplaySetItems()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.SetItem] == false)
            {
                ViewedSceneCheck[(int)GameScenes.SetItem] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("인벤토리 - 장착 관리");
                Console.ResetColor();
                TypingMessageLine("보유중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine();

                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                TypingMessageLine("(장착 / 해제) 를 원하시면 아이템 번호를 입력해주세요.");

                Console.WriteLine();
                TypingMessageLine("0. 나가기");

                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);
                switch (input)
                {
                    case 0:
                        DisplayInventory();
                        break;
                    default:
                        if (player.Items[input - 1].GetType() == typeof(Weapon))
                        {
                            EquipCheck(BodyParts.Weapon, input - 1);
                            player.BodyPart[(int)BodyParts.Weapon] = player.Items[input - 1].Name;
                        }
                        else if (player.Items[input - 1].GetType() == typeof(Armor))
                        {
                            EquipCheck(BodyParts.Armor, input - 1);
                            player.BodyPart[(int)BodyParts.Armor] = player.Items[input - 1].Name;
                        }
                        else if (player.Items[input - 1].GetType() == typeof(Shield))
                        {
                            EquipCheck(BodyParts.Shield, input - 1);
                            player.BodyPart[(int)BodyParts.Shield] = player.Items[input - 1].Name;
                        }

                        player.Items[input - 1].Use(player);
                        DisplaySetItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("인벤토리 - 장착 관리");
                Console.ResetColor();
                Console.WriteLine("보유중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine();

                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                Console.WriteLine("(장착 / 해제) 를 원하시면 아이템 번호를 입력해주세요.");

                Console.WriteLine();
                Console.WriteLine("0. 나가기");

                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);
                switch (input)
                {
                    case 0:
                        DisplayInventory();
                        break;
                    default:
                        if (player.Items[input - 1].GetType() == typeof(Weapon))
                        {
                            EquipCheck(BodyParts.Weapon, input - 1);
                            player.BodyPart[(int)BodyParts.Weapon] = player.Items[input - 1].Name;
                        }
                        else if (player.Items[input - 1].GetType() == typeof(Armor))
                        {
                            EquipCheck(BodyParts.Armor, input - 1);
                            player.BodyPart[(int)BodyParts.Armor] = player.Items[input - 1].Name;
                        }
                        else if (player.Items[input - 1].GetType() == typeof(Shield))
                        {
                            EquipCheck(BodyParts.Shield, input - 1);
                            player.BodyPart[(int)BodyParts.Shield] = player.Items[input - 1].Name;
                        }

                        player.Items[input - 1].Use(player);
                        DisplaySetItems();
                        break;
                }
            }
        }

        static public void EquipCheck(BodyParts bodyParts, int itemNum)
        {
            if (player.BodyPart[(int)bodyParts] != " ")
            {

                IItem settedItem = player.Items.Find(i => i.Name == "[E]" + player.BodyPart[(int)bodyParts]);
                settedItem.Use(player);
                if (settedItem.Name == player.Items[itemNum].Name)
                {
                    DisplaySetItems();
                }

            }
        }

        static void DisplaySortItems()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.SortItem] == false)
            {
                ViewedSceneCheck[(int)GameScenes.SortItem] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("인벤토리 - 정렬 순서 변경");
                Console.ResetColor();

                TypingMessageLine("보유중인 아이템의 순서를 정렬할 수 있습니다.");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable();
                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                Console.WriteLine();
                TypingMessageLine("1. 이름순");
                TypingMessageLine("0. 나가기");

                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 1);
                switch (input)
                {
                    case 0:
                        DisplayInventory();
                        break;
                    case 1:
                        player.Items.Sort((a, b) => a.Name.CompareTo(b.Name));
                        DisplaySortItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("인벤토리 - 정렬 순서 변경");
                Console.ResetColor();

                Console.WriteLine("보유중인 아이템의 순서를 정렬할 수 있습니다.");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description);
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine("1. 이름순");
                Console.WriteLine("0. 나가기");

                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 1);
                switch (input)
                {
                    case 0:
                        DisplayInventory();
                        break;
                    case 1:
                        player.Items.Sort((a, b) => a.Name.CompareTo(b.Name));
                        DisplaySortItems();
                        break;
                }
            }
        }
        static void DisplayShop()
        {
            Console.Clear();

            if (ViewedSceneCheck[(int)GameScenes.Shop] == false)
            {
                TypingMessageLine("스파르타 마을 한 구석에 이름 모를 가게가 하나 보인다.");
                Thread.Sleep(700);
                TypingMessageLine("주변 가게들과는 무언가 다른 느낌을 받은 당신은, 궁금함에 이끌려 서서히 다가간다.");
                Thread.Sleep(1500);
                Console.Clear();

                TypingMessageLine("허름한 외관과는 달리, 상당히 가치 있는 물건들이 가판대에 놓여있다.");
                Thread.Sleep(700);
                TypingMessage("운이 좋다면 ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                TypingMessage("전설의 무기");
                Console.ResetColor();
                TypingMessageLine("를 구할 수 있을지도 모른다.");
                Thread.Sleep(1500);

                Console.Clear();
                ViewedSceneCheck[(int)GameScenes.Shop] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("상점");
                Console.ResetColor();
                TypingMessageLine("던전에서 나온 아이템들을 구할 수 있는 상점입니다.");
                Console.WriteLine();
                TypingMessageLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                TypingMessage($"{player.Gold}");
                Console.ResetColor();
                TypingMessageLine(" G");
                Console.WriteLine();
                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "   가격   ");
                int itemNum = 1;
                foreach (IItem item in Shop)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, item.Price + "G");
                    itemNum++;
                }
                table.Write();

                TypingMessageLine("1. 아이템 구매");
                TypingMessageLine("2. 아이템 판매");
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 2);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        DisplayBuyItems();
                        break;
                    case 2:
                        DisplaySellItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("상점");
                Console.ResetColor();
                Console.WriteLine("던전에서 나온 아이템들을 구할 수 있는 상점입니다.");
                Console.WriteLine();
                Console.WriteLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{player.Gold}");
                Console.ResetColor();
                Console.WriteLine(" G");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "   가격   ");
                int itemNum = 1;
                foreach (IItem item in Shop)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, item.Price + "G");
                    itemNum++;
                }
                table.Write();

                Console.WriteLine("1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 2);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        DisplayBuyItems();
                        break;
                    case 2:
                        DisplaySellItems();
                        break;
                }
            }

        }

        static void DisplayBuyItems()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.BuyItem] == false)
            {
                ViewedSceneCheck[(int)GameScenes.BuyItem] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("상점 - 아이템 구매");
                Console.ResetColor();
                TypingMessageLine("필요한 아이템을 얻을 수 있는 상점입니다.");
                Console.WriteLine();
                TypingMessageLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                TypingMessage($"{player.Gold}");
                Console.ResetColor();
                TypingMessageLine(" G");
                Console.WriteLine();
                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "   가격   ");
                int itemNum = 1;
                foreach (IItem item in Shop)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, item.Price + "G");
                    itemNum++;
                }
                table.Write();

                TypingMessageLine("구매하고 싶은 아이템의 번호를 입력해주세요.");
                TypingMessageLine("0. 나가기");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);
                if (input != 0)
                {
                    while (Shop[input - 1].Price > player.Gold)
                    {
                        TypingMessageLine("골드가 모자랍니다.");
                        input = CheckValidInput(0, table.Rows.Count);
                        if (input == 0) break;
                    }
                }

                switch (input)
                {
                    case 0:
                        DisplayShop();
                        break;
                    default:
                        player.Items.Add(Shop[input - 1]);
                        player.Gold -= Shop[input - 1].Price;
                        Shop.RemoveAt(input - 1);

                        DisplayBuyItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("상점 - 아이템 구매");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
                Console.WriteLine();
                Console.WriteLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{player.Gold}");
                Console.ResetColor();
                Console.WriteLine(" G");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "   가격   ");
                int itemNum = 1;
                foreach (IItem item in Shop)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, item.Price + "G");
                    itemNum++;
                }
                table.Write();

                Console.WriteLine("구매하고 싶은 아이템의 번호를 입력해주세요.");
                Console.WriteLine("0. 나가기");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);
                if (input != 0)
                {
                    while (Shop[input - 1].Price > player.Gold)
                    {
                        Console.WriteLine("골드가 모자랍니다.");
                        input = CheckValidInput(0, table.Rows.Count);
                        if (input == 0) break;
                    }
                }

                switch (input)
                {
                    case 0:
                        DisplayShop();
                        break;
                    default:
                        player.Items.Add(Shop[input - 1]);
                        player.Gold -= Shop[input - 1].Price;
                        Shop.RemoveAt(input - 1);

                        DisplayBuyItems();
                        break;
                }
            }

        }

        static void DisplaySellItems()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.BuyItem] == false)
            {
                ViewedSceneCheck[(int)GameScenes.BuyItem] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("상점 - 아이템 판매");
                Console.ResetColor();
                Console.WriteLine();
                TypingMessageLine("상점에서 아이템을 판매할 수 있습니다.");
                Console.WriteLine();
                TypingMessageLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                TypingMessage($"{player.Gold}");
                Console.ResetColor();
                TypingMessageLine(" G");
                Console.WriteLine();
                TypingMessageLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "  판매가  ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, (int)item.Price * 0.85 + "G");
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();
                TypingMessageLine("판매하고 싶은 아이템의 번호를 입력해주세요.");
                TypingMessageLine("0. 나가기");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);

                switch (input)
                {
                    case 0:
                        DisplayShop();
                        break;
                    default:
                        if (player.Items[input - 1].isEquip) player.Items[input - 1].Use(player);
                        player.Gold += (int)(player.Items[input - 1].Price * 0.85);
                        Shop.Add(player.Items[input - 1]);
                        player.Items.RemoveAt(input - 1);

                        DisplaySellItems();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("상점 - 아이템 판매");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("상점에서 아이템을 판매할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("[보유 골드]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{player.Gold}");
                Console.ResetColor();
                Console.WriteLine(" G");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                ConsoleTable table = new ConsoleTable("No.", "  아이템 이름  ", "   효과   ", "    설명    ", "  판매가  ");
                int itemNum = 1;
                foreach (IItem item in player.Items)
                {
                    table.AddRow(itemNum, item.Name, item.Effect, item.Description, (int)item.Price * 0.85 + "G");
                    itemNum++;
                }
                table.Write();

                Console.WriteLine();
                Console.WriteLine("판매하고 싶은 아이템의 번호를 입력해주세요.");
                Console.WriteLine("0. 나가기");
                Console.Write(">>");

                int input = CheckValidInput(0, table.Rows.Count);

                switch (input)
                {
                    case 0:
                        DisplayShop();
                        break;
                    default:
                        if (player.Items[input - 1].isEquip) player.Items[input - 1].Use(player);
                        player.Gold += (int)(player.Items[input - 1].Price * 0.85);
                        Shop.Add(player.Items[input - 1]);
                        player.Items.RemoveAt(input - 1);

                        DisplaySellItems();
                        break;
                }
            }
        }

        static void ShowExtraStat(PlayerStat stat)
        {
            switch (stat)
            {
                case PlayerStat.Atk:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" (+" + player.ExtraAtk + ")");
                    Console.ResetColor();
                    break;
                case PlayerStat.Def:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" (+" + player.ExtraDef + ")");
                    Console.ResetColor();
                    break;
                case PlayerStat.HP:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" (+" + player.ExtraHP + ")");
                    Console.ResetColor();
                    break;
            }
        }

        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("잘못된 입력입니다.");
            }
        }

        static void GoToDungeonEntrance()
        {
            Random rand = new Random();
            int giveDamage = rand.Next(20, 35);
            int RecommendDef;
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.Dungeon] == false)
            {
                TypingMessage("스파르타 마을 한복판에 위치한 ");
                Console.ForegroundColor = ConsoleColor.Red;
                TypingMessage("던전");
                Console.ResetColor();
                TypingMessageLine("은");
                Thread.Sleep(700);
                TypingMessageLine("그 악명을 뽐내듯이 으스스한 기운을 내뿜고 있다.");
                Thread.Sleep(1500);
                Console.Clear();
                TypingMessageLine("얼핏 보면 동굴처럼 보이는 저 입구는,");
                Thread.Sleep(700);
                TypingMessageLine("어둠으로 가득 차 그 안이 보이지 않고,");
                Thread.Sleep(1500);
                Console.Clear();
                TypingMessageLine("이따금씩 안에서 출처 모를 괴성이 들려올 때마다,");
                Thread.Sleep(700);
                TypingMessageLine("가슴이 저미는 듯한 느낌이 들곤 한다.");
                Thread.Sleep(1500);
                Console.Clear();


                ViewedSceneCheck[(int)GameScenes.Dungeon] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("던전 입구");
                Console.ResetColor();
                Console.WriteLine();
                TypingMessageLine($"이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다. (현재 방어력 - {player.Def})");
                Console.WriteLine();
                TypingMessageLine("1. 쉬운 던전( 권장 방어력 5 )");
                TypingMessageLine("2. 보통 던전( 권장 방어력 11 )");
                TypingMessageLine("3. 어려운 던전( 권장 방어력 17 )");
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 3);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;

                    case 1:
                        DungeonClear(giveDamage, 5);
                        break;

                    case 2:
                        DungeonClear(giveDamage, 11);
                        break;

                    case 3:
                        DungeonClear(giveDamage, 17);
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("던전입장");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다. (현재 방어력 - {player.Def})");
                Console.WriteLine();
                Console.WriteLine("1. 쉬운 던전( 권장 방어력 5 )");
                Console.WriteLine("2. 보통 던전( 권장 방어력 11 )");
                Console.WriteLine("3. 어려운 던전( 권장 방어력 17 )");
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 3);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;

                    case 1:
                        DungeonClear(giveDamage, 5);
                        break;

                    case 2:
                        DungeonClear(giveDamage, 11);
                        break;

                    case 3:
                        DungeonClear(giveDamage, 17);
                        break;
                }
            }
        }

        static void DungeonClear(int givedamage, int recommenddef)
        {
            int lastDamage = givedamage + recommenddef - player.Def;
            Random clearGold = new Random();
            int lastGold = 500 + clearGold.Next(recommenddef * 40, recommenddef * 60);
            Random encounter = new Random();
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.DungeonClear] == false)
            {
                DungeonEncounter((EncounterNum)encounter.Next(0, 4));

                Console.Clear();
                ViewedSceneCheck[(int)GameScenes.DungeonClear] = true;
                if (player.HP <= lastDamage)
                {
                    TypingMessageLine("게임 오버....");
                    Environment.Exit(0);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("던전 클리어");
                Console.ResetColor();
                Console.WriteLine();
                TypingMessageLine("축하합니다! 던전을 클리어 하였습니다.");
                ExpUp(player);
                Console.WriteLine();
                TypingMessageLine("[탐험 결과]");
                Console.WriteLine();
                TypingMessageLine($"남은 체력 : {player.MaxHP} / {player.HP - lastDamage}");
                player.HP -= lastDamage;

                TypingMessageLine($"Gold : {player.Gold} -> {player.Gold + lastGold}");
                player.Gold += lastGold;

                Console.WriteLine();
                Console.WriteLine();
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 0);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                }
            }
            else
            {
                DungeonEncounter((EncounterNum)encounter.Next(0, 4));
                Console.Clear();
                if (player.HP <= lastDamage)
                {
                    Console.WriteLine("게임 오버....");
                    Environment.Exit(0);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("던전 클리어");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("축하합니다! 던전을 클리어 하였습니다.");
                ExpUp(player);
                Console.WriteLine();
                Console.WriteLine("[탐험 결과]");
                Console.WriteLine();
                Console.WriteLine($"체력 : {player.MaxHP} / {player.HP - lastDamage}");
                player.HP -= lastDamage;

                Console.WriteLine($"Gold : {player.Gold} -> {player.Gold + lastGold}");
                player.Gold += lastGold;

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 0);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                }
            }
        }

        static void TakeARest()
        {
            Console.Clear();
            if (ViewedSceneCheck[(int)GameScenes.Rest] == false)
            {
                ViewedSceneCheck[(int)GameScenes.Rest] = true;
                Console.ForegroundColor = ConsoleColor.Green;
                TypingMessageLine("휴식하기");
                Console.ResetColor();
                Console.WriteLine();
                TypingMessageLine($"500G를 내면 체력을 회복할 수 있습니다. (보유 Gold : {player.Gold})");
                Console.WriteLine();
                TypingMessageLine($"현재 체력 : {player.MaxHP} / {player.HP}");
                Console.WriteLine();
                TypingMessageLine("1. 휴식하기");
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 1);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        if (player.Gold >= 500)
                        {
                            if (player.MaxHP - player.HP <= 50) player.HP = player.MaxHP;
                            else player.HP += 50;
                            player.Gold -= 500;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            TypingMessageLine("휴식을 완료했습니다.");
                            Console.ResetColor();
                            Thread.Sleep(700);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            TypingMessageLine("Gold가 부족합니다.");
                            Console.ResetColor();
                            Thread.Sleep(700);
                        }
                        TakeARest();
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("휴식하기");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"500G를 내면 체력을 회복할 수 있습니다. (보유 Gold : {player.Gold})");
                Console.WriteLine();
                Console.WriteLine($"현재 체력 : {player.MaxHP} / {player.HP}");
                Console.WriteLine();
                Console.WriteLine("1. 휴식하기");
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                int input = CheckValidInput(0, 1);
                switch (input)
                {
                    case 0:
                        DisplayGameIntro();
                        break;
                    case 1:
                        if (player.Gold >= 500)
                        {
                            if (player.MaxHP - player.HP <= 50) player.HP = player.MaxHP;
                            else player.HP += 50;
                            player.Gold -= 500;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("휴식을 완료했습니다.");
                            Console.ResetColor();
                            Thread.Sleep(700);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Gold가 부족합니다.");
                            Console.ResetColor();
                            Thread.Sleep(700);
                        }
                        TakeARest();
                        break;
                }
            }
        }

        static void ExpUp(Player player)
        {
            player.Exp++;

            switch (player.Level)
            {
                case 1:
                    if (player.Exp >= 1)
                    {
                        LevelUp(player);
                        player.Exp = 0;
                    }
                    break;
                case 2:
                    if (player.Exp >= 2)
                    {
                        LevelUp(player);
                        player.Exp = 0;
                    }
                    break;
                case 3:
                    if (player.Exp >= 3)
                    {
                        LevelUp(player);
                        player.Exp = 0;
                    }
                    break;
                case 4:
                    if (player.Exp >= 4)
                    {
                        LevelUp(player);
                        player.Exp = 0;
                    }
                    break;
            }
        }

        static void LevelUp(Player player)
        {
            player.Level += 1;
            player.Atk += 1;
            player.Def += 2;
            player.MaxHP += 5;
            player.HP += 5;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"레벨이 올랐습니다! 현재 레벨 : {player.Level}");
            Console.ResetColor();
        }

        static void SaveDungeon(Player player)
        {
            File.WriteAllText("E:\\TeamSparta\\Sprta Dungeon\\savedata.txt", string.Empty);

            StreamWriter saveTxt = new StreamWriter("E:\\TeamSparta\\Sprta Dungeon\\savedata.txt");
            saveTxt.WriteLine($"{player.Name},{player.Class},{player.Level},{player.Exp}," +
                $"{player.Atk - player.ExtraAtk},{player.Def - player.ExtraDef},{player.HP - player.ExtraHP},{player.MaxHP - player.ExtraHP},{player.Gold}");

            for (int i = 0; i < player.Items.Count; i++)
            {
                int ItemNum = -1;
                foreach (var pair in ItemArchive)
                {
                    if (player.Items[i].Name.Contains(pair.Value.Name))
                    {
                        ItemNum = pair.Key;
                    }
                }
                saveTxt.Write(ItemNum.ToString());
                if (i != player.Items.Count - 1) saveTxt.Write(',');
            }
            saveTxt.WriteLine();

            for (int i = 0; i < 3; i++)
            {
                int ItemNum = -1;
                foreach (var pair in ItemArchive)
                {
                    if (pair.Value.Name == player.BodyPart[i])
                    {
                        ItemNum = pair.Key;
                        saveTxt.Write(ItemNum.ToString());
                    }
                }
                if (ItemNum == -1) saveTxt.Write(" ");
                if (i != 2) saveTxt.Write(',');
            }

            saveTxt.Close();

            var pos = Console.GetCursorPosition();
            Console.WriteLine("저장중.");
            Thread.Sleep(700);
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.WriteLine("저장중..");
            Thread.Sleep(700);
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.WriteLine("저장중...");
            Thread.Sleep(700);
            DisplayGameIntro();
        }

        static void DungeonEncounter(EncounterNum encounterNum)
        {
            switch(encounterNum)
            {
                case EncounterNum.GetItem:
                    // 보물상자에서 아이템 획득. 아이템 랜덤 획득.
                    TypingMessageLine("시간이 얼만큼 지났을까.");
                    Thread.Sleep(700);
                    TypingMessageLine("칠흑같이 어두운 시야와, 원인 모를 악취에도 서서히 적응해갈 때쯤");
                    Thread.Sleep(700);
                    TypingMessageLine("저 멀리서 빛이 아른하게 보이기 시작한다.");
                    Thread.Sleep(1500);
                    Console.Clear();

                    TypingMessageLine("빛을 따라 어느 공간에 들어선다. ");
                    Thread.Sleep(700);
                    TypingMessageLine("물건들이 여기저기 쌓여 있는 것으로 보아 창고인 것 같다.");
                    Thread.Sleep(700);
                    TypingMessageLine("그 중 한 상자 안에서 희미하게 빛이 피어오른다.");
                    Thread.Sleep(700);
                    Console.WriteLine();
                    TypingMessageLine("상자를 여시겠습니까?");
                    Thread.Sleep(700);
                    Console.WriteLine();
                    TypingMessageLine("0. 연다.");
                    TypingMessageLine("1. 열지 않는다.");
                    TypingMessage(">>");

                    int input = CheckValidInput(0, 1);
                    switch (input)
                    {
                        case 0:
                            Console.Clear();
                            TypingMessageLine("[연다]");
                            Console.WriteLine();
                            TypingMessageLine("당신은 옅은 불안감에도 불구하고 상자를 열었다.");
                            TypingMessageLine("상자 속에는 놀랍게도 고대의 장비가 들어있었다.");
                            Random randomItem = new Random();
                            player.Items.Add(ItemArchive[randomItem.Next(0, 11)]);
                            TypingMessageLine($"(아이템 획득 - {player.Items.Last().Name})");
                            Thread.Sleep(1500);
                            break;
                        case 1:
                            Console.Clear();
                            TypingMessageLine("[열지 않는다]");
                            Console.WriteLine();
                            TypingMessageLine("혹시나 함정은 아닐까");
                            TypingMessageLine("걱정한 당신은 밀려오는 호기심을 참고 앞으로 지나가기로 했다.");
                            Thread.Sleep(1500);
                            break;
                    }
                    break;
                case EncounterNum.FightMob:
                    // 몬스터랑 싸움. 체력 소모
                    TypingMessageLine("던전에 들어와, 뭐가 나타날지 모른다는 불안감과 두려움에");
                    Thread.Sleep(700);
                    TypingMessageLine("잔뜩 들어갔던 힘이 조금씩 느슨해져 갈 때쯤");
                    Thread.Sleep(700);
                    TypingMessageLine("소름끼치는 숨소리가 당신의 앞에서 들려온다.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    TypingMessageLine("다행히 들키지는 않은 것 같다.");
                    Thread.Sleep(700);
                    TypingMessageLine("당신은 크게 심호흡하고, 조심스럽게 다가가보기로 했다.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    TypingMessageLine("잠시 살펴보니 갑옷으로 무장한 병사들이 모여있었다.");
                    Thread.Sleep(700);
                    TypingMessageLine("하지만 움직임이나 목소리로 보아 인간은 아닌 것 같았다.");
                    Thread.Sleep(700);
                    TypingMessageLine("더 나아가려면 싸우는 수 밖에 없으리라.");
                    Thread.Sleep(700);
                    Console.WriteLine();
                    TypingMessageLine("0. 싸운다.");
                    Console.Write(">>");
                    int MobAppear = CheckValidInput(0, 0);
                    switch (MobAppear)
                    {
                        case 0:
                            Console.Clear();
                            TypingMessageLine("[싸운다]");
                            Console.WriteLine();
                            Thread.Sleep(700);
                            TypingMessageLine("당신은 훌륭한 실력으로 병사들을 제압하였다.");
                            Thread.Sleep(700);
                            TypingMessageLine("하지만 그들은 재빨랐고, 고통을 모르는 듯한 움직임으로 당신에게 상처를 남겨주었다.");
                            Thread.Sleep(700);
                            int damage = 50 - player.Atk;
                            player.HP -= damage;
                            TypingMessageLine($"(체력 소모 - {damage})");
                            Thread.Sleep(1500);
                            break;
                    }
                    break;
                case EncounterNum.MeetSomeone:
                    //사람 만남. 레벨 업
                    TypingMessageLine("주위를 경계하며 앞으로 나아가던 중, ");
                    Thread.Sleep(700);
                    TypingMessageLine("근처에서 시끄러운 소리가 들려 온다.");
                    Thread.Sleep(1500);

                    Console.Clear();
                    TypingMessageLine("경험으로 누군가가 몬스터와 전투하는 소리임을 알아챈 당신은 ");
                    Thread.Sleep(700);
                    TypingMessageLine("그를 도우러 갈지, 가던 길을 갈지 고민 끝에 결정을 내린다.");
                    Thread.Sleep(700);
                    Console.WriteLine();
                    TypingMessageLine("0. 돕는다.");
                    Thread.Sleep(700);
                    TypingMessageLine("1. 지나친다.");
                    Thread.Sleep(700);


                    int MeetPerson = CheckValidInput(0, 1);
                    switch (MeetPerson)
                    {
                        case 0:
                            Console.Clear();
                            TypingMessageLine("[돕는다]");
                            Thread.Sleep(700);
                            Console.WriteLine();
                            TypingMessageLine("이전 몬스터와의 싸움에서, ");
                            Thread.Sleep(700);
                            TypingMessageLine("누군가 도와주어 살아남았던 기억을 떠올린 당신은,");
                            Thread.Sleep(700);
                            TypingMessageLine("그를 도우러 가기로 결단한다.");
                            Thread.Sleep(1500);
                            Console.Clear();
                            TypingMessageLine("소리가 들리던 장소에 도착하니, ");
                            Thread.Sleep(700);
                            TypingMessageLine("한 중년 남자가 갑옷을 입은 몬스터와 맞서고 있는 것이 보인다.");
                            Thread.Sleep(700);
                            TypingMessageLine("당신은 몬스터의 뒤로 접근하여 기습적으로 공격했고, ");
                            Thread.Sleep(700);
                            TypingMessageLine("다행히도 큰 피해 없이 쓰러트릴 수 있었다.");
                            Thread.Sleep(1500);
                            Console.Clear();
                            TypingMessageLine("남성은 당신에게 거듭 감사를 표하며 ");
                            Thread.Sleep(700);
                            TypingMessageLine("마을에 돌아가면 이 은혜를 꼭 값겠다고 이야기한다.");
                            Thread.Sleep(700);
                            player.Level += 1;
                            TypingMessageLine($"(레벨 상승 - {player.Level})");
                            Thread.Sleep(1500);

                            break;
                        case 1:
                            Console.Clear();
                            TypingMessageLine("[지나친다]");
                            Console.WriteLine();
                            TypingMessageLine("던전에서는 몬스터 뿐만 아니라 인간도 조심해야 한다.");
                            Thread.Sleep(700);
                            TypingMessageLine("도우러 가는 사람을 노리는 악독한 함정일지 누가 아는가?");
                            Thread.Sleep(1500);
                            Console.Clear();
                            TypingMessageLine("당신은 신경쓰지 않고 계속해서 나아갔으며,");
                            Thread.Sleep(700);
                            TypingMessageLine("어느 순간 그 소리는 들려오지 않게 되었다.");
                            Thread.Sleep(1500);

                            break;
                    }
                    break;
                case EncounterNum.DrinkPotion:
                    // 포션먹음. 능력치 상승
                    TypingMessageLine("어둠 속을 걸어가던 당신은 어느새 커다란 문 앞에 다다랐음을 눈치챘다.");
                    Thread.Sleep(700);
                    TypingMessageLine("알 수 없는 글씨와 문양으로 장식된 문.");
                    Thread.Sleep(700);
                    TypingMessageLine("안쪽에서는 이상한 향기도 나는 듯하다.");
                    Thread.Sleep(700);
                    TypingMessageLine("당신은 신경을 곤두세우고 천천히 문을 열었다.");
                    Thread.Sleep(1500);
                    Console.Clear();

                    TypingMessageLine("문을 열자 쓴 냄새가 퍼지고, 처음보는 여러 약품들이 놓여있다.");
                    Thread.Sleep(700);
                    TypingMessageLine("방 가운데를 보니 책상이 하나 있었고");
                    Thread.Sleep(700);
                    TypingMessageLine("그 위에 누군가 먹으라는 듯 물약이 하나 놓여있다.");
                    Thread.Sleep(700);
                    Console.WriteLine();
                    TypingMessageLine("0.먹는다.");
                    Thread.Sleep(700);
                    TypingMessageLine("1.먹지 않는다.");
                    Thread.Sleep(1500);

                    int DrinkPotion = CheckValidInput(0, 1);
                    switch (DrinkPotion)
                    {
                        case 0:
                            Console.Clear();
                            TypingMessageLine("[먹는다]");
                            Thread.Sleep(700);
                            Console.WriteLine();
                            TypingMessageLine("당신은 이 기회를 놓치지 않기로 했다, 스파르타의 고대 물약이라니!");
                            Thread.Sleep(700);
                            TypingMessageLine("당신은 이 물약을 놓고 간 누군가에게 감사하며 물약을 마셨다.");
                            Thread.Sleep(1500);
                            Console.Clear();

                            TypingMessageLine("물약을 마시자 갑자기 눈 앞이 어지럽기 시작했다. ");
                            Thread.Sleep(700);
                            TypingMessageLine("사방이 흔들리고, 어느새 당신은 땅 바닥에 누워있었다. ");
                            Thread.Sleep(700);
                            TypingMessageLine("이윽고 시야가 점점 흐려지며, 당신의 의식은 저 밑으로 가라앉았다…");
                            Thread.Sleep(1500);
                            Console.Clear();

                            TypingMessageLine("깨어났다. 시간이 얼마나 지난지 모르겠다.");
                            Thread.Sleep(700);
                            TypingMessageLine("당신은 어느새 방 바깥에 있었으며, 왜인지 온몸에 힘이 넘친다.");
                            Thread.Sleep(700);
                            TypingMessageLine("(체력 상승 + 50)");
                            player.MaxHP += 50;
                            player.HP += 50;
                            Thread.Sleep(1500);
                            break;
                        case 1:
                            Console.Clear();
                            TypingMessageLine("[먹지 않는다]");
                            Thread.Sleep(700);
                            Console.WriteLine();
                            TypingMessageLine("당신은 이 몹시 수상한 물약을 먹지 않기로 했다");
                            Thread.Sleep(700);
                            TypingMessageLine("이런 뻔한 함정에 걸려들 만큼 순진한 사람이 세상에 어디 있겠는가?");
                            Thread.Sleep(700);
                            TypingMessageLine("당신은 방에서 나와 문을 닫고 가던 길로 향했다.");
                            Thread.Sleep(1500);
                            break;
                    }
                    break;
            }
        }
    }


    enum PlayerClass
    {
        Warrior = 1,
        Magician,
        Rogue,
        Priest
    }

    enum PlayerStat
    {
        Atk = 1,
        Def,
        HP
    }

    enum BodyParts
    {
        Weapon,
        Armor,
        Shield
    }

    enum GameScenes
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
        DungeonClear,
        Rest
    }

    enum EncounterNum
    {
        GetItem,
        FightMob,
        MeetSomeone,
        DrinkPotion
    }
}