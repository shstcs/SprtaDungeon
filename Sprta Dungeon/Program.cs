using ConsoleTables;
using NAudio;
using NAudio.Wave;
using System.Diagnostics.Tracing;
using static System.Net.Mime.MediaTypeNames;

namespace Sprta_Dungeon
{
    internal class Program
    {
        private static Player player;
        static string className;

        public static List<IItem> Shop = new List<IItem>() { };
        public static Dictionary<int, IItem> ItemArchive;

        static AudioFileReader startBGM = new AudioFileReader("E:\\TeamSparta\\Sprta Dungeon\\StartBGM.wav");
        static LoopStream startLoop = new LoopStream(startBGM);
        static AudioFileReader mainBGM = new AudioFileReader("E:\\TeamSparta\\Sprta Dungeon\\mainBGM.wav");
        static LoopStream mainLoop = new LoopStream(mainBGM);
        static AudioFileReader dungeonBGM = new AudioFileReader("E:\\TeamSparta\\Sprta Dungeon\\DungeonBGM.wav");
        static LoopStream dungeonLoop = new LoopStream(dungeonBGM);

        static WaveOutEvent audioMgr = new WaveOutEvent();

        //한번 본 텍스트 바로 나오도록.
        static bool[] ViewedSceneCheck = new bool[20];

        static List<string[]> Keywords = new List<string[]>();

        static void Main(string[] args)
        {
            DisplayStartGame();
            DisplayTown();
        }

        //-----------------------------------------------------화면 메서드-------------------------------------------------------//

        //아이템 도감 세팅 / 상점에 넣을 아이템 세팅
        static void SetItemArchive()
        {
            ItemArchive = new Dictionary<int, IItem>()
            {
                {0, new Armor("무쇠갑옷", "체력 + 50", "무쇠로 만들어져 튼튼한 갑옷.", 50, 300) },
                {1, new Weapon("낡은 검", "공격력 + 3", "쉽게 볼 수 있는 낡은 검.", 3, 150)},
                {2, new Shield("목제 방패", "방어력 + 5", "위험한 공격을 한 번은 막아줄지도 모른다.", 5, 200)},
                {3, new Armor("수련자 갑옷", "체력 + 30", "수련에 도움을 주는 갑옷.", 30, 200) },
                {4, new Armor("스파르타의 갑옷", "체력 + 150", "스파르타의 전사들이 사용했다는 전설의 갑옷.", 150, 1000)},
                {5, new Weapon("제국식 창", "공격력 + 7", "보급형이지만 충분히 날카롭다.", 7, 400)},
                {6, new Weapon("붉은 채찍", "공격력 + 10", "사실 다른 용도로 만들었지만 생각보다 강했다고 한다.", 10, 750)},
                {7, new Shield("타워 실드", "방어력 + 10", "몸 전체를 가려줄만큼 큰 방패", 10, 600)},
                {8, new Shield("제국식 버클러", "방어력 + 12", "특수 합금으로 만들어져 내구도가 강하다.", 12, 700)},
                {9, new Weapon("엑스칼리버", "공격력 + 100?", "진품인지는 확실하지 않다.", 10, 10000)},
                {10, new Armor("최첨단 나노 슈트", "체력 + 300", "아이언맨에 나온 그것.", 300, 3000)},
                {11, new Shield("진압 방패", "방어력 + 8", "비살상 진압을 위해 사용한다.", 8, 400)},
                {12, new Armor("마법 수련생의 로브", "체력 + 20", "활동성이 좋지만 갑옷의 기능은 거의 없다.", 20, 150)},
                {13, new Armor("붉은 사제복", "체력 + 30", "사실 하얀 옷이었다는 소문이 있다.", 30, 200)},
                {14, new Armor("사슬 갑옷", "체력 + 40", "무게와 내구성을 동시에 잡았다.", 40, 300)},
                {15, new Weapon("마법 스태프", "공격력 + 5", "마법을 쓰지 못하면 의미가 없다..", 5, 300)},
                {16, new Weapon("사냥용 마체테", "공격력 + 6", "날이 잘 갈려있는 마체테.", 6, 350)},
                {17, new Weapon("스파르타의 창", "공격력 + 30", "스파르타의 전사들이 사용했다는 전설의 창.", 30, 2000)},
                {18, new Shield("얼굴이 그려진 방패", "방어력 + 15", "흉측한 얼굴이 그려져 있다.", 15, 800)},
                {19, new Shield("가시 방패", "방어력 + 20", "전면이 가시로 덮여있는 다소 거친 느낌의 방패", 20, 1000)},
                {20, new Shield("스파르타의 방패", "방어력 + 30", "스파르타의 전사들이 사용했다는 전설의 방패.", 30, 1500)},
            };

        }
        static void ShopItemSet()
        {
            Random randomItem = new Random();

            for (int i = 0; i < 4; i++)
            {
                IItem item = ItemArchive[randomItem.Next(0, 20)];
                if (!player.Items.Contains(item))
                {
                    Shop.Add(item);
                }
                else i--;
            }
        }
        static void SetKeywords()
        {
            Keywords.Add(new string[] { "새로", "이전", "뉴", "이어" });                                                      //0.게임시작
            Keywords.Add(new string[] { "전사", "마법사", "도적", "사제" });                                                 //1.뉴게임
            Keywords.Add(new string[] { "상태", "정보", "인벤", "상점", "던전", "휴식", "저장", "세이브", "종료" });           //2.타운
            Keywords.Add(new string[] { "나가", "나간다", "나갈래", "뒤로" });                                                //3.내 정보
            Keywords.Add(new string[] { "장착", "관리", "착용", "해제", "정렬", "순서", "나가기", "뒤로" });                 //4.인벤토리
            Keywords.Add(new string[] { "정렬", "나가기", "뒤로" });                                                           //5.아이템 정렬
            Keywords.Add(new string[] { "구매", "사기", "판매", "팔기", "새로고침", "최신화", "새로운", "나가기", "뒤로" });    //6.상점  
            Keywords.Add(new string[] { "쉬운", "이지", "보통", "노말", "어려운", "하드", "나가기", "뒤로" });                    //7.던전
            Keywords.Add(new string[] { "연다", "열지 않는다", "싸운다", "돕는다", "지나친다", "먹는다", "먹지 않는다" });   //8. 던전 인카운트
            Keywords.Add(new string[] { "나가기", "뒤로" });                                                                     //9. 던전 클리어
            Keywords.Add(new string[] { "휴식", "쉬기", "나가기", "뒤로" });                                                     //10. 휴식하기
            
        }

        //게임 시작시 화면 / 새로하기,이어하기 시 데이터 세팅
        static void DisplayStartGame()
        {
            SetKeywords();
            audioMgr.Init(startLoop);
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


            string key = CheckValidInputText(Keywords[0]);
            if (key == "새로" || key == "뉴")
            {
                NewGameDataSetting();
            }
            else if (key == "이전" || key == "이어")
            {
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
            }

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

            string selectClass = CheckValidInputText(Keywords[1]);
            if (selectClass == "전사")
            {
                player = new Warrior(name, className);
                player.Items.Add(ItemArchive[0]);
                player.Items.Add(ItemArchive[1]);
                player.Items.Add(ItemArchive[2]);
            }
            else if (selectClass == "마법사")
            {
                player = new Magicion(name, className);
                player.Items.Add(ItemArchive[12]);
                player.Items.Add(ItemArchive[15]);
                player.Items.Add(ItemArchive[2]);
            }
            else if (selectClass == "도적")
            {
                player = new Rouge(name, className);
                player.Items.Add(ItemArchive[3]);
                player.Items.Add(ItemArchive[1]);
                player.Items.Add(ItemArchive[2]);
            }
            else if (selectClass == "사제")
            {
                player = new Priest(name, className);
                player.Items.Add(ItemArchive[13]);
                player.Items.Add(ItemArchive[15]);
                player.Items.Add(ItemArchive[2]);
            }

            // 아이템 정보 세팅
            ShopItemSet();

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

                    ShopItemSet();

                    for (int i = 0; i < 20; i++)
                    {
                        ViewedSceneCheck[i] = true;
                    }

                    LoadManager.Close();
                }
            }
        }

        //시작 마을 화면
        static void DisplayTown()
        {
            audioMgr.Stop();
            audioMgr.Init(mainLoop);
            audioMgr.Play();

            if (ViewedSceneCheck[(int)GameScenes.GameIntro] == false)
            {
                ViewedSceneCheck[(int)GameScenes.GameIntro] = true;

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

                string input = CheckValidInputText(Keywords[2]);
                if (input == "상태" || input == "정보")
                {
                    DisplayMyInfo();
                }
                else if (input == "인벤")
                {
                    DisplayInventory();
                }
                else if (input == "상점")
                {
                    DisplayShop();
                }
                else if (input == "던전")
                {
                    GoToDungeonEntrance();
                }
                else if (input == "휴식")
                {
                    TakeARest();
                }
                else if (input == "저장" || input == "세이브")
                {
                    SaveDungeon(player);
                }
                else if (input == "종료")
                {
                    Environment.Exit(0);
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

                string input = CheckValidInputText(Keywords[2]);
                if (input == "상태" || input == "정보")
                {
                    DisplayMyInfo();
                }
                else if (input == "인벤")
                {
                    DisplayInventory();
                }
                else if (input == "상점")
                {
                    DisplayShop();
                }
                else if (input == "던전")
                {
                    GoToDungeonEntrance();
                }
                else if (input == "휴식")
                {
                    TakeARest();
                }
                else if (input == "저장" || input == "세이브")
                {
                    SaveDungeon(player);
                }
                else if (input == "종료")
                {
                    Environment.Exit(0);
                }
            }

        }

        //상태 확인 화면
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

                TypingMessage($"공격력 : {player.Atk}");
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

                string input = CheckValidInputText(Keywords[3]);
                if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[3]);
                if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
                }
            }

        }

        //인벤토리 화면 / 아이템 착용 화면 / 아이템 정렬 화면
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

                string input = CheckValidInputText(Keywords[4]);
                if (input == "장착" || input == "관리" || input == "착용" || input == "해제")
                {
                    DisplaySetItems();
                }
                else if (input == "정렬" || input == "순서")
                {
                    DisplaySortItems();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[4]);
                if (input == "장착" || input == "관리" || input == "착용" || input == "해제")
                {
                    DisplaySetItems();
                }
                else if (input == "정렬" || input == "순서")
                {
                    DisplaySortItems();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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
                TypingMessageLine("1. 정렬하기");
                TypingMessageLine("0. 나가기");

                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                string input = CheckValidInputText(Keywords[5]);
                if (input == "정렬" || input == "순서")
                {
                    player.Items.Sort((a, b) => a.Name.CompareTo(b.Name));
                    DisplaySortItems();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[5]);
                if (input == "정렬" || input == "순서")
                {
                    player.Items.Sort((a, b) => a.Name.CompareTo(b.Name));
                    DisplaySortItems();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
                }
            }
        }

        //상점 화면 / 아이템 구매 화면 / 아이템 판매 화면
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
                TypingMessageLine("3. 아이템 새로고침 (100G 소모)");
                TypingMessageLine("0. 나가기");
                Console.WriteLine();
                TypingMessageLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                string input = CheckValidInputText(Keywords[6]);
                if (input == "구매" || input == "사기")
                {
                    DisplayBuyItems();
                }
                else if (input == "판매" || input == "팔기")
                {
                    DisplaySellItems();
                }
                else if (input == "새로고침" || input == "최신화" || input == "새로운")
                {
                    ItemRefresh();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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
                Console.WriteLine("3. 아이템 새로고침 (100G 소모)");
                Console.WriteLine("0. 나가기");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                string input = CheckValidInputText(Keywords[6]);
                if (input == "구매" || input == "사기")
                {
                    DisplayBuyItems();
                }
                else if (input == "판매" || input == "팔기")
                {
                    DisplaySellItems();
                }
                else if (input == "새로고침" || input == "최신화" || input == "새로운")
                {
                    ItemRefresh();
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

        //던전 입장 / 던전 인카운트 화면 / 던전 클리어 화면
        static void GoToDungeonEntrance()
        {
            audioMgr.Stop();
            audioMgr.Init(dungeonLoop);
            audioMgr.Play();

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

                string input = CheckValidInputText(Keywords[7]);
                if (input == "쉬운" || input == "이지")
                {
                    DungeonClear(giveDamage, 5);
                }
                else if (input == "보통" || input == "노말")
                {
                    DungeonClear(giveDamage, 11);
                }
                else if (input == "어려운" || input == "하드")
                {
                    DungeonClear(giveDamage, 11);
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[7]);
                if (input == "쉬운" || input == "이지")
                {
                    DungeonClear(giveDamage, 5);
                }
                else if (input == "보통" || input == "노말")
                {
                    DungeonClear(giveDamage, 11);
                }
                else if (input == "어려운" || input == "하드")
                {
                    DungeonClear(giveDamage, 11);
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
                }
            }
        }
        static void DungeonEncounter(EncounterNum encounterNum)
        {
            switch (encounterNum)
            {
                case EncounterNum.GetItem:
                    // 보물상자에서 아이템 획득. 아이템 랜덤 획득.
                    if (ViewedSceneCheck[(int)GameScenes.DungeonEncounter1] == false)
                    {
                        ViewedSceneCheck[(int)GameScenes.DungeonEncounter1] = true;
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

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "연다")
                        {
                            Console.Clear();
                            TypingMessageLine("[연다]");
                            Console.WriteLine();
                            TypingMessageLine("당신은 옅은 불안감에도 불구하고 상자를 열었다.");
                            TypingMessageLine("상자 속에는 놀랍게도 고대의 장비가 들어있었다.");
                            Random randomItem = new Random();
                            player.Items.Add(ItemArchive[randomItem.Next(0, 11)]);
                            TypingMessageLine($"(아이템 획득 - {player.Items.Last().Name})");
                            Thread.Sleep(1500);
                        }
                        else if (input == "열지 않는다")
                        {
                            Console.Clear();
                            TypingMessageLine("[열지 않는다]");
                            Console.WriteLine();
                            TypingMessageLine("혹시나 함정은 아닐까");
                            TypingMessageLine("걱정한 당신은 밀려오는 호기심을 참고 앞으로 지나가기로 했다.");
                            Thread.Sleep(1500);
                        }
                    }
                    else
                    {
                        Console.WriteLine("시간이 얼만큼 지났을까.");
                        Console.WriteLine("칠흑같이 어두운 시야와, 원인 모를 악취에도 서서히 적응해갈 때쯤");
                        Console.WriteLine("저 멀리서 빛이 아른하게 보이기 시작한다.");
                        Thread.Sleep(1500);
                        Console.Clear();

                        Console.WriteLine("빛을 따라 어느 공간에 들어선다. ");
                        Console.WriteLine("물건들이 여기저기 쌓여 있는 것으로 보아 창고인 것 같다.");
                        Console.WriteLine("그 중 한 상자 안에서 희미하게 빛이 피어오른다.");
                        Console.WriteLine();
                        Console.WriteLine("상자를 여시겠습니까?");
                        Console.WriteLine();
                        Console.WriteLine("0. 연다.");
                        Console.WriteLine("1. 열지 않는다.");
                        Console.Write(">>");

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "연다")
                        {
                            Console.Clear();
                            Console.WriteLine("[연다]");
                            Console.WriteLine();
                            Console.WriteLine("당신은 옅은 불안감에도 불구하고 상자를 열었다.");
                            Console.WriteLine("상자 속에는 놀랍게도 고대의 장비가 들어있었다.");
                            Random randomItem = new Random();
                            player.Items.Add(ItemArchive[randomItem.Next(0, 11)]);
                            Console.WriteLine($"(아이템 획득 - {player.Items.Last().Name})");
                            Thread.Sleep(1500);
                        }
                        else if (input == "열지 않는다")
                        {
                            Console.Clear();
                            Console.WriteLine("[열지 않는다]");
                            Console.WriteLine();
                            Console.WriteLine("혹시나 함정은 아닐까");
                            Console.WriteLine("걱정한 당신은 밀려오는 호기심을 참고 앞으로 지나가기로 했다.");
                            Thread.Sleep(1500);
                        }
                    }
                    break;
                case EncounterNum.FightMob:
                    if (ViewedSceneCheck[(int)GameScenes.DungeonEncounter2] == false)
                    {
                        ViewedSceneCheck[(int)GameScenes.DungeonEncounter2] = true;
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

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "싸운다")
                        {
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
                        }
                    }
                    else
                    {
                        Console.WriteLine("던전에 들어와, 뭐가 나타날지 모른다는 불안감과 두려움에");
                        Console.WriteLine("잔뜩 들어갔던 힘이 조금씩 느슨해져 갈 때쯤");
                        Console.WriteLine("소름끼치는 숨소리가 당신의 앞에서 들려온다.");
                        Thread.Sleep(1500);
                        Console.Clear();
                        Console.WriteLine("다행히 들키지는 않은 것 같다.");
                        Console.WriteLine("당신은 크게 심호흡하고, 조심스럽게 다가가보기로 했다.");
                        Thread.Sleep(1500);
                        Console.Clear();
                        Console.WriteLine("잠시 살펴보니 갑옷으로 무장한 병사들이 모여있었다.");
                        Console.WriteLine("하지만 움직임이나 목소리로 보아 인간은 아닌 것 같았다.");
                        Console.WriteLine("더 나아가려면 싸우는 수 밖에 없으리라.");
                        Console.WriteLine();
                        Console.WriteLine("0. 싸운다.");
                        Console.Write(">>");

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "싸운다")
                        {
                            Console.Clear();
                            Console.WriteLine("[싸운다]");
                            Console.WriteLine();
                            Console.WriteLine("당신은 훌륭한 실력으로 병사들을 제압하였다.");
                            Console.WriteLine("하지만 그들은 재빨랐고, 고통을 모르는 듯한 움직임으로 당신에게 상처를 남겨주었다.");
                            int damage = 50 - player.Atk;
                            player.HP -= damage;
                            Console.WriteLine($"(체력 소모 - {damage})");
                            Thread.Sleep(1500);
                        }
                    }
                    // 몬스터랑 싸움. 체력 소모
                    break;
                case EncounterNum.MeetSomeone:
                    if (ViewedSceneCheck[(int)GameScenes.DungeonEncounter3] == false)
                    {
                        ViewedSceneCheck[(int)GameScenes.DungeonEncounter3] = true;
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
                        TypingMessage(">>");

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "돕는다")
                        {
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
                            LevelUp(player);
                            TypingMessageLine($"(레벨 상승 - {player.Level})");
                            Thread.Sleep(1500);
                        }
                        else if (input == "지나친다")
                        {
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
                        }
                    }
                    else
                    {
                        Console.WriteLine("주위를 경계하며 앞으로 나아가던 중, ");
                        Console.WriteLine("근처에서 시끄러운 소리가 들려 온다.");
                        Thread.Sleep(1500);

                        Console.Clear();
                        Console.WriteLine("경험으로 누군가가 몬스터와 전투하는 소리임을 알아챈 당신은 ");
                        Console.WriteLine("그를 도우러 갈지, 가던 길을 갈지 고민 끝에 결정을 내린다.");
                        Console.WriteLine();
                        Console.WriteLine("0. 돕는다.");
                        Console.WriteLine("1. 지나친다.");
                        Console.Write(">>");

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "돕는다")
                        {
                            Console.Clear();
                            Console.WriteLine("[돕는다]");
                            Console.WriteLine();
                            Console.WriteLine("이전 몬스터와의 싸움에서, ");
                            Console.WriteLine("누군가 도와주어 살아남았던 기억을 떠올린 당신은,");
                            Console.WriteLine("그를 도우러 가기로 결단한다.");
                            Thread.Sleep(1500);
                            Console.Clear();
                            Console.WriteLine("소리가 들리던 장소에 도착하니, ");
                            Console.WriteLine("한 중년 남자가 갑옷을 입은 몬스터와 맞서고 있는 것이 보인다.");
                            Console.WriteLine("당신은 몬스터의 뒤로 접근하여 기습적으로 공격했고, ");
                            Console.WriteLine("다행히도 큰 피해 없이 쓰러트릴 수 있었다.");
                            Thread.Sleep(1500);
                            Console.Clear();
                            Console.WriteLine("남성은 당신에게 거듭 감사를 표하며 ");
                            Console.WriteLine("마을에 돌아가면 이 은혜를 꼭 값겠다고 이야기한다.");
                            LevelUp(player);
                            Console.WriteLine($"(레벨 상승 - {player.Level})");
                            Thread.Sleep(1500);
                        }
                        else if (input == "지나친다")
                        {
                            Console.Clear();
                            Console.WriteLine("[지나친다]");
                            Console.WriteLine();
                            Console.WriteLine("던전에서는 몬스터 뿐만 아니라 인간도 조심해야 한다.");
                            Console.WriteLine("도우러 가는 사람을 노리는 악독한 함정일지 누가 아는가?");
                            Thread.Sleep(1500);
                            Console.Clear();
                            Console.WriteLine("당신은 신경쓰지 않고 계속해서 나아갔으며,");
                            Console.WriteLine("어느 순간 그 소리는 들려오지 않게 되었다.");
                            Thread.Sleep(1500);
                        }
                    }
                    //사람 만남. 레벨 업
                    break;
                case EncounterNum.DrinkPotion:
                    if (ViewedSceneCheck[(int)GameScenes.DungeonEncounter3] == false)
                    {
                        ViewedSceneCheck[(int)GameScenes.DungeonEncounter4] = true;
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

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "먹는다")
                        {
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
                        }
                        else if (input == "먹지 않는다")
                        {
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
                        }
                    }
                    else
                    {
                        Console.WriteLine("어둠 속을 걸어가던 당신은 어느새 커다란 문 앞에 다다랐음을 눈치챘다.");
                        Console.WriteLine("알 수 없는 글씨와 문양으로 장식된 문.");
                        Console.WriteLine("안쪽에서는 이상한 향기도 나는 듯하다.");
                        Console.WriteLine("당신은 신경을 곤두세우고 천천히 문을 열었다.");
                        Thread.Sleep(1500);
                        Console.Clear();

                        Console.WriteLine("문을 열자 쓴 냄새가 퍼지고, 처음보는 여러 약품들이 놓여있다.");
                        Console.WriteLine("방 가운데를 보니 책상이 하나 있었고");
                        Console.WriteLine("그 위에 누군가 먹으라는 듯 물약이 하나 놓여있다.");
                        Console.WriteLine();
                        Console.WriteLine("0.먹는다.");
                        Console.WriteLine("1.먹지 않는다.");
                        Thread.Sleep(1500);

                        string input = CheckValidInputText(Keywords[8]);
                        if (input == "먹는다")
                        {
                            Console.Clear();
                            Console.WriteLine("[먹는다]");
                            Console.WriteLine();
                            Console.WriteLine("당신은 이 기회를 놓치지 않기로 했다, 스파르타의 고대 물약이라니!");
                            Console.WriteLine("당신은 이 물약을 놓고 간 누군가에게 감사하며 물약을 마셨다.");
                            Thread.Sleep(1500);
                            Console.Clear();

                            Console.WriteLine("물약을 마시자 갑자기 눈 앞이 어지럽기 시작했다. ");
                            Console.WriteLine("사방이 흔들리고, 어느새 당신은 땅 바닥에 누워있었다. ");
                            Console.WriteLine("이윽고 시야가 점점 흐려지며, 당신의 의식은 저 밑으로 가라앉았다…");
                            Thread.Sleep(1500);
                            Console.Clear();

                            Console.WriteLine("깨어났다. 시간이 얼마나 지난지 모르겠다.");
                            Console.WriteLine("당신은 어느새 방 바깥에 있었으며, 왜인지 온몸에 힘이 넘친다.");
                            Console.WriteLine("(체력 상승 + 50)");
                            player.MaxHP += 50;
                            player.HP += 50;
                            Thread.Sleep(1500);
                        }
                        else if (input == "먹지 않는다")
                        {
                            Console.Clear();
                            Console.WriteLine("[먹지 않는다]");
                            Console.WriteLine();
                            Console.WriteLine("당신은 이 몹시 수상한 물약을 먹지 않기로 했다");
                            Console.WriteLine("이런 뻔한 함정에 걸려들 만큼 순진한 사람이 세상에 어디 있겠는가?");
                            Console.WriteLine("당신은 방에서 나와 문을 닫고 가던 길로 향했다.");
                            Thread.Sleep(1500);
                        }
                    }
                    // 포션먹음. 능력치 상승
                    break;
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

                string input = CheckValidInputText(Keywords[9]);
                if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[9]);
                if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
                }
            }
        }



        //-------------------------------------------------------기능 메서드--------------------------------------------------------//

        //휴식하기 화면
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

                string input = CheckValidInputText(Keywords[10]);
                if (input == "휴식" || input == "쉬기")
                {
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
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
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

                string input = CheckValidInputText(Keywords[10]);
                if (input == "휴식" || input == "쉬기")
                {
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
                }
                else if (input == "나가기" || input == "뒤로")
                {
                    DisplayTown();
                }
            }
        }

        //저장하기 기능
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
                    if (pair.Value.Name.Contains(player.BodyPart[i]))
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
            DisplayTown();
        }

        //타이핑 효과
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

        //아이템 장착여부 판별
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

        //상점 새로고침
        static void ItemRefresh()
        {
            player.Gold -= 100;
            Shop.Clear();
            ShopItemSet();
            DisplayShop();
        }

        //아이템 장착시 + 붙여주는 효과
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

        //사용자 입력 알맞은 값만 받도록
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

        //사용자 텍스트 입력 처리
        static string CheckValidInputText(string[] keyword)
        {
            while (true)
            {
                string input = Console.ReadLine();

                foreach (string text in keyword)
                {
                    if (input.Contains(text))
                    {
                        return text;
                    }
                }
                Console.WriteLine("잘못된 입력입니다.");
            }
        }

        //경험치 / 레벨 업 기능
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
    }
}