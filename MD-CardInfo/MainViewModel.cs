using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MD_CardInfo
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public MainViewModel()
        {
            FontSize = 14;
            topMost = true;
            _BGIMAGE = new ImageBrush(new BitmapImage(new Uri("bg.png", UriKind.Relative))) { Stretch = Stretch.UniformToFill };
        }
        private ImageBrush _BGIMAGE { get; set; }
        public ImageBrush BGIMAGE { get => _BGIMAGE; }
        private double _FontSize { get; set; }
        public double FontSize { get => _FontSize;
            set 
            {
                _FontSize = value <= 10 ? 10 : value >= 30 ? 30 : value;
                TitleFontSize = _FontSize + 4;
                OnPropertyChanged();
                if (MainWindow.ConfigXML is not null)
                {
                    var element = MainWindow.ConfigXML.Root?.Element("fontSize");
                    if (element != null)
                        element.Value = value.ToString();
                    MainWindow.ConfigXML.Save(MainWindow.ConfigXMLPath);
                }

            } }
        private double _TitleFontSize { get; set; }
        public double TitleFontSize { get => _TitleFontSize; set { _TitleFontSize = value; OnPropertyChanged(); } }
        private DBTables.Cards _Card { get; set; } = new()
        {
            cn_name = "中文卡名",
            en_name = "英文卡名",
            atk = 0,
            def = 0,
            level = 12,
            race = "种族",
            attribute = "光",
            type = "怪兽|效果|同调|融合|仪式|超量",
            desc = "怪物效果"
        };
        public DBTables.Cards Card { get => _Card;
            set {
                _Card = value;
                OnPropertyChanged("cn_name");
                OnPropertyChanged("en_name");
                OnPropertyChanged("atk");
                OnPropertyChanged("def");
                OnPropertyChanged("level");
                OnPropertyChanged("race");
                OnPropertyChanged("attribute");
                OnPropertyChanged("type");
                OnPropertyChanged("desc");
                OnPropertyChanged("visibility");
            } }
        public string cn_name { get => _Card.cn_name; }
        public string en_name { get => _Card.en_name; }
        public string atk { get {
                if (_Card.atk == -2)
                    return "?";
                else
                    return _Card.atk.ToString();
            } }
        public string def
        {
            get
            {
                if (_Card.def == -2)
                    return "?";
                else
                    return _Card.def.ToString();
            }
        }
        public string level { get {
                if (type.IndexOf("连接") > 0)
                {
                    return "LINK - " + (short)_Card.level;
                }
                else if (type.IndexOf("超量") > 0)
                {
                    return "☆ " + (short)_Card.level;
                }
                else
                {
                    return "★ " + (short)_Card.level;
                }
            } }
        public string race { get => _Card.race; }
        public string attribute { get => _Card.attribute; }
        public string type { get => _Card.type; }
        public string desc { get => _Card.desc; }
        public Visibility visibility { get => type.IndexOf("怪兽") != -1 ? Visibility.Visible : Visibility.Hidden; }
        private bool _topMost { get; set; }
        public bool topMost { get=> _topMost; 
            set 
            { 
                _topMost = value;
                OnPropertyChanged();
                if (MainWindow.ConfigXML is not null)
                {
                    var element = MainWindow.ConfigXML.Root?.Element("topMost");
                    if (element != null)
                        element.Value = value.ToString();
                    MainWindow.ConfigXML.Save(MainWindow.ConfigXMLPath);
                }


            } }
    }
}
