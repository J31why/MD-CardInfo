using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace MD_CardInfo
{
    public class ViewModel
    {
        public static MainViewModel VM { get; set; } = new();
    }
    public class ObservableObj : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
    public class ObservableCard : ObservableObj
    {
        public ObservableCard(DBTables.Cards card)
        {
            Card = card;
        }
        public DBTables.Cards Card { get; set; }
        public int Cid { get => Card.cid; }
        public int Tid { get => Card.tid; }
        public int Passcode { get => Card.passcode; }
        public string CN_Name { get => Card.cn_name; }
        public string EN_Name { get => Card.en_name; }
        public string JP_Name { get => Card.jp_name; }
        public string Desc
        {
            get
            {
                if (((MD_CardInfo.Type)Card.type).HasFlag(MD_CardInfo.Type.灵摆))
                {
                    if (((MD_CardInfo.Type)Card.type).HasFlag(MD_CardInfo.Type.效果))
                        return Card.desc.Replace("【怪兽效果】", "\n【怪兽效果】");
                    else
                        return Card.desc.Replace("【怪兽描述】", "\n【怪兽描述】");
                }
                else return Card.desc;
            }
        }
        public string Level
        {
            get
            {
                if (((Type)Card.type).HasFlag(MD_CardInfo.Type.连接))
                    return $"[LINK - {(short)Card.level}]";
                else if (((Type)Card.type).HasFlag(MD_CardInfo.Type.超量))
                    return $"[☆ {(short)Card.level}]";
                else return $"[★ {(short)Card.level}]";
            }
        }
        public string Atk
        {
            get => Card.atk == -2 ? "?" : Card.atk.ToString();
        }
        public string Def
        {
            get
            {
                if (((Type)Card.type).HasFlag(MD_CardInfo.Type.连接))
                {
                    string linkMark = "-  ";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.LeftDown))
                        linkMark += "[↙]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.Down))
                        linkMark += "[↓]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.RightDown))
                        linkMark += "[↘]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.Left))
                        linkMark += "[←]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.Right))
                        linkMark += "[→]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.LeftUp))
                        linkMark += "[↖]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.Up))
                        linkMark += "[↑]";
                    if (((LinkMark)Card.def).HasFlag(LinkMark.RightUp))
                        linkMark += "[↗]";
                    return linkMark;
                }
                return Card.def == -2 ? "?" : Card.def.ToString();
            }
        }
        public string Race
        {
            get => ((Race)Card.race).ToString();
        }
        public string Type { get => ((Type)Card.type).ToString().Replace(", ", "|"); }
        public string Attribute { get => ((Attribute)Card.attribute).ToString(); }




        #region DeckInfo
        public int Index { get; set; }
        public string Type_2
        {
            get
            {
                if (((Type)Card.type).HasFlag(MD_CardInfo.Type.魔法))
                    return nameof(MD_CardInfo.Type.魔法);
                else if (((Type)Card.type).HasFlag(MD_CardInfo.Type.陷阱))
                    return nameof(MD_CardInfo.Type.陷阱);
                else if (((Type)Card.type).HasFlag(MD_CardInfo.Type.同调)
                    || ((Type)Card.type).HasFlag(MD_CardInfo.Type.融合)
                    || ((Type)Card.type).HasFlag(MD_CardInfo.Type.连接)
                    || ((Type)Card.type).HasFlag(MD_CardInfo.Type.超量))
                    return "额外";
                else return nameof(MD_CardInfo.Type.怪兽);
            }
        }
        #endregion

        #region CardInfoForUI
        public Visibility IsMonster { get => ((Type)Card.type).HasFlag(MD_CardInfo.Type.怪兽) ? Visibility.Visible : Visibility.Collapsed; }
        #endregion

    }


    public class MainViewModel : ObservableObj
    {
        private static XDocument xDoc { get; set; } = new(new XElement("Config"));
        private static readonly string ConfigPath = "config.xml";
        private static readonly string DBPath = "cards.db";
        private static int CurrentCardID = 0;
        private static SQLite.SQLiteConnection? conn { get; set; }

        public MainViewModel()
        {
            _FontSize = 14;
            _TitleFontSize = 18;
            _TopMost = true;
            _ShowJPName = true;
            _ShowENName = true;
            _Width = 360;
            _Height = 550;
            LoadGameDeck = new(LoadGameDeckCallBack);
            LoadFileDeck = new(LoadFileDeckCallBack);
            SaveDeck = new(SaveDeckCallBack);
            WriteGameDeck = new(WriteGameDeckCallBack);

            if (!File.Exists(DBPath))
            {
                MessageBox.Show("找不到cards.db文件。", "我数据库呢？", MessageBoxButton.OK, MessageBoxImage.Stop);
                Application.Current.Shutdown();
            }
            conn = new("cards.db");
            conn.CreateTable<DBTables.Cards>();
            //load config
            if (File.Exists(ConfigPath))
            {
                xDoc = XDocument.Load(ConfigPath);
                string? value = LoadXElement(nameof(FontSize));
                if(value is not null)
                    FontSize = double.Parse(value);
                value = LoadXElement(nameof(Width));
                if (value is not null)
                    _Width = double.Parse(value);
                value = LoadXElement(nameof(Height));
                if (value is not null)
                    _Height = double.Parse(value);
                value = LoadXElement(nameof(TopMost));
                if (value is not null)
                    _TopMost = bool.Parse(value);
                value = LoadXElement(nameof(ShowJPName));
                if (value is not null)
                    _ShowJPName = bool.Parse(value);
                value = LoadXElement(nameof(ShowENName));
                if (value is not null)
                    _ShowENName = bool.Parse(value);
            }

            BG = new ImageBrush(new BitmapImage(new Uri("bg.png", UriKind.Relative))) { Stretch = Stretch.UniformToFill };
            _Card = new(new()
            {
                cn_name = "中文卡名",
                jp_name = "日文卡名",
                en_name = "英文卡名",
                level = 1,
                atk = 1,
                def = 1,
                attribute = 1,
                desc = "卡片描述",
                race = 1,
                type = 1
            });
            GetCard();
        }
        public void Close()
        {
            Func.CloseMDProcess();
            SaveXElement(Height.ToString(), nameof(Height));
            SaveXElement(Width.ToString(), nameof(Width));
        }
         async void GetCard()
        {
            try
            {
                if (Func.gProcess == null || Func.gProcess.HasExited)
                {
                    Func.GetMDProcess();
                    Func.GetGameAssembly();
                }
                if (Func.gProcess != null)
                {
                    if (Func.pHandle == IntPtr.Zero)
                        Func.OpenMDProcess();
                    if (Func.pHandle != IntPtr.Zero)
                    {
                        var tid = Func.GetCardTID();
                        if (tid != null && conn != null)
                        {
                            if (CurrentCardID != tid.Value)
                            {
                                CurrentCardID = tid.Value;
                                var ret = conn.Table<DBTables.Cards>().Where(v => v.tid == tid).ToList();
                                if (ret.Count > 0)
                                    Card = new ObservableCard(ret.First());
                            }
                        }
                        else if (tid == null) 
                        {
                            Func.gProcess = null;
                            Func.pHandle= IntPtr.Zero;
                        }
                    }
                }
            }
            catch { }
            await Task.Delay(130);
            GetCard();
        }
        public static string? LoadXElement( string XElement)
        {
            var root = xDoc.Root;
            if (root is null) return null;
            var element = root.Element(XElement);
            if (element is null) return null;
            return element.Value;
        }
        public static void SaveXElement(string  value, [CallerMemberName] string? XElement = null)
        {
            if (XElement is null) return;
            var root = xDoc.Root;
            if (root is null) return;
            var element = root.Element(XElement);
            if (element is null)
            {
                element = new(XElement);
                root.Add(element);
            }
            element.Value = value;
            xDoc.Save(ConfigPath);
        }


        public ImageBrush BG { get; set; }
        private double _FontSize { get; set; }
        public double FontSize { get => _FontSize;
            set 
            {
                _FontSize = value <= 6 ? 6 : value >= 60 ? 60 : value;
                TitleFontSize = _FontSize + 4;
                OnPropertyChanged();
                SaveXElement(value.ToString());
            } }
        private double _TitleFontSize { get; set; }
        public double TitleFontSize { get => _TitleFontSize; set { _TitleFontSize = value; OnPropertyChanged(); } }
        private bool _TopMost { get; set; }
        public bool TopMost
        {
            get => _TopMost;
            set { _TopMost = value; OnPropertyChanged(); SaveXElement(value.ToString()); }
        }

        private bool _ShowJPName;
        public bool ShowJPName
        {
            get { return _ShowJPName; }
            set { _ShowJPName = value;OnPropertyChanged(); SaveXElement(value.ToString()); }
        }


        private bool _ShowENName;
        public bool ShowENName
        {
            get { return _ShowENName; }
            set { _ShowENName = value; OnPropertyChanged(); SaveXElement(value.ToString()); }
        }


        private double _Width;
        public double Width
        {
            get { return _Width; }
            set { _Width = value;  }
        }


        private double _Height;
        public double Height
        {
            get { return _Height; }
            set { _Height = value; }
        }


        private ObservableCard _Card;
        public ObservableCard Card
        {
            get { return _Card; }
            set { _Card = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ObservableCard> LoadedDeck { get; set; } = new();
        public RelayCommand LoadGameDeck { get; set; }
        public RelayCommand LoadFileDeck { get; set; }
        public RelayCommand SaveDeck { get; set; }
        public RelayCommand WriteGameDeck { get; set; }
        public void LoadGameDeckCallBack(object? p)
        {
            var deck =Func.GetMainDeck();
            if (deck == null)
            {
                MessageBox.Show($"请打开游戏的卡组编辑界面。",
"读取游戏卡组", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            LoadedDeck.Clear();
            InsertCardtoDeckBycid(deck);
            deck = Func.GetExDeck();
            if (deck == null)
            {
                MessageBox.Show($"请打开游戏的卡组编辑界面。",
"读取游戏卡组", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            InsertCardtoDeckBycid(deck);
        }
        public void LoadFileDeckCallBack(object? p)
        {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "ygopro卡组|*.ydk";
            openFileDialog.ShowDialog();
            List<int> cardList = new();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                string deckText = File.ReadAllText(openFileDialog.FileName);
                foreach (var card in deckText.Split("\n"))
                {
                    int passcode = 0;
                    try
                    {
                        passcode = int.Parse(card);
                    }
                    catch { }
                    if (passcode > 0)
                    {
                        cardList.Add(passcode);
                    }
                }
                if (cardList.Count > 0)
                {
                    LoadedDeck.Clear();
                    InsertCardtoDeckByPasscode(cardList);
                }
            }
        }
        public void SaveDeckCallBack(object? p)
        {
            if (LoadedDeck.Count == 0) return;
            string maindeck = string.Empty;
            string exdeck = string.Empty;
            foreach (var card in LoadedDeck)
            {
                if (card.Type_2 == "额外")
                {
                    exdeck += exdeck == string.Empty ?  $"{card.Passcode}" :  $"\n{card.Passcode}";
                }
                else
                {
                    maindeck += maindeck == string.Empty ?  $"{card.Passcode}" :  $"\n{card.Passcode}";
                }
            }
            string deckText = string.Format("#created by ygopro2\n#main\n{0}\n#extra\n{1}\n!side\n", maindeck, exdeck);
            SaveFileDialog saveFileDialog = new ();
            saveFileDialog.Title = "选择保存路径";
            saveFileDialog.Filter = "ygopro卡组|*.ydk";
            saveFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                File.WriteAllText(saveFileDialog.FileName, deckText);
        }
        public void WriteGameDeckCallBack(object? p)
        {
            if (LoadedDeck.Count == 0) return;
            var Msgret=MessageBox.Show("确定把当前卡组写入游戏？\n写入过程中请勿关闭游戏！\n注意！这将修改游戏内存。", 
                "写入卡组", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if(Msgret == MessageBoxResult.Yes)
            {
                List<int> mainDeck=new();
                List<int> exDeck = new();
                foreach (var card in LoadedDeck)
                {
                    if (card.Type_2 == "额外")
                        exDeck.Add(card.Cid);
                    else
                        mainDeck.Add(card.Cid);
                }
                int ret=Func.WriteDeck(mainDeck, exDeck);
                if (ret == -1)
                {
                    MessageBox.Show($"写入失败，主卡组或额外卡组数量不一。\n主卡组需要预先放入{mainDeck.Count}张卡片\n额外卡组需要预先放入{exDeck.Count}张卡片",
                "写入卡组", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (ret == -2)
                {
                    MessageBox.Show($"写入失败！请打开游戏的卡组编辑界面。",
                   "写入卡组", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (ret == 0)
                {
                    MessageBox.Show($"写入成功！返回游戏保存卡组即可！", "写入卡组", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }
        public void InsertCardtoDeckBycid(List<int> deck)
        {
            if (conn == null) return;
            foreach (var cid in deck)
            {
                var ret = conn.Table<DBTables.Cards>().Where(v => v.cid == cid && v.tid > 0).ToList();
                if (ret.Count() > 0)
                {
                    ObservableCard c = new(ret.First());
                    LoadedDeck.Add(c);
                    c.Index = LoadedDeck.Count;
                }
            }
        }
        public void InsertCardtoDeckByPasscode(List<int> deck)
        {
            if (conn == null) return;
            string notify = string.Empty;
            foreach (var passcode in deck)
            {
                string passcodeStr = $"%({passcode})%";
                if(passcode == 23995348)
                {

                }
                var ret = conn.Query<DBTables.Cards>($"select * from [Cards] where [passcode]='{passcode}' or [ygoPasscode] like '{passcodeStr}';");
                if (ret.Count() > 0)
                {
                    var card = ret.First();
                    if (card.tid > 0) 
                    {
                        ObservableCard c = new(card);
                        LoadedDeck.Add(c);
                        c.Index = LoadedDeck.Count;
                    }
                    else
                    {
                        notify += string.Format("{0}({1})\n", card.cn_name, card.passcode);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("卡密错误：{0}，可能是\n①未输入正确卡密。\n②数据库错误，请告知作者。",passcode)
                        , "读取出错", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (notify != string.Empty)
            {
                MessageBox.Show(string.Format("MD未推出以下卡片：\n{0}", notify)
                    , "读取出错", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
