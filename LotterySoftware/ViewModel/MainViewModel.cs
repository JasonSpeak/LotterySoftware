using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LotterySoftware.Model;

namespace LotterySoftware.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int _currentAwardIndex;
        private readonly DispatcherTimer _timer;
        private readonly List<Drawer> _drawersImport;
        private readonly ObservableCollection<Drawer> _resultWinner;

        private bool _isImport;
        private bool _isExport;
        private bool _isLottery;
        private bool _isStartOrPause;
        private string _lotteryName;
        private string _currentAwardName;
        private string _showMaxButton;
        private string _showNormalButton;
        private ObservableCollection<Drawer> _showDrawerList;
        private ObservableCollection<Awards> _showAwardsList;
        
        private RelayCommand _closeWindowCmd;
        private RelayCommand _minWindowCmd;
        private RelayCommand _maxWindowCmd;
        private RelayCommand _recoveryWindowCmd;
        private RelayCommand _moveWindowCmd;
        private RelayCommand _importCmd;
        private RelayCommand _exportCmd;
        private RelayCommand _lotteryButtonCmd;

        public bool IsLottery
        {
            get => _isLottery;
            set
            {
                _isLottery = value;
                RaisePropertyChanged(()=>IsLottery);
            }
        }

        public bool IsStartOrPause
        {
            get => _isStartOrPause;
            set
            {
                _isStartOrPause = value;
                RaisePropertyChanged(()=>IsStartOrPause);
            }
        }

        public bool IsImport
        {
            get => _isImport;
            set
            {
                _isImport = value;
                RaisePropertyChanged(()=>IsImport);
            }
        }
        public bool IsExport
        {
            get => _isExport;
            set
            {
                _isExport = value;
                RaisePropertyChanged(()=>IsExport);
            }
        }
        public string ShowMaxButton
        {
            get => _showMaxButton;
            set
            {
                _showMaxButton = value;
                RaisePropertyChanged(() => ShowMaxButton);
            }
        }

        public string LotteryName
        {
            get => _lotteryName;
            set
            {
                _lotteryName = value;
                RaisePropertyChanged(()=>LotteryName);
            }
        }

        public string CurrentAwardName
        {
            get => _currentAwardName;
            set
            {
                _currentAwardName = value;
                RaisePropertyChanged(()=>CurrentAwardName);
            }
        }

        public string ShowNormalButton
        {
            get => _showNormalButton;
            set
            {
                _showNormalButton = value;
                RaisePropertyChanged(() => ShowNormalButton);
            }
        }

        public ObservableCollection<Drawer> ShowDrawerList
        {
            get => _showDrawerList;
            set
            {
                _showDrawerList = value;
                RaisePropertyChanged(()=>ShowDrawerList);
            }
        }

        public ObservableCollection<Awards> ShowAwardsList
        {
            get => _showAwardsList;
            set
            {
                _showAwardsList = value;
                RaisePropertyChanged(()=>ShowAwardsList);
            }
        }

        public RelayCommand ExportCmd => _exportCmd ?? (_exportCmd = new RelayCommand(Export));
        public RelayCommand CloseWindowCmd => _closeWindowCmd ?? (_closeWindowCmd = new RelayCommand(CloseWindow));
        public RelayCommand MinWindowCmd => _minWindowCmd ?? (_minWindowCmd = new RelayCommand(MinWindow));
        public RelayCommand MaxWindowCmd => _maxWindowCmd ?? (_maxWindowCmd = new RelayCommand(MaxWindow));
        public RelayCommand RecoveryWindowCmd => _recoveryWindowCmd ?? (_recoveryWindowCmd = new RelayCommand(RecoveryWindow));
        public RelayCommand MoveWindowCmd => _moveWindowCmd ?? (_moveWindowCmd = new RelayCommand(MoveWindow));
        public RelayCommand ImportCmd => _importCmd ?? (_importCmd = new RelayCommand(Import));
        public RelayCommand LotteryButtonCmd => _lotteryButtonCmd ?? (_lotteryButtonCmd = new RelayCommand(LotteryButton));

        public MainViewModel()
        {
            InitializationXml();
            _timer = new DispatcherTimer();
            _timer.Tick += OnRefreshClockTimeUp;
            _timer.Interval = new TimeSpan(0,0,0,0,90);
            IsImport = true;
            IsExport = false;
            IsLottery = false;
            ShowMaxButton = "Visible";
            ShowNormalButton = "Collapsed";
            LotteryName = "³é½±";
            CurrentAwardName = ShowAwardsList[_currentAwardIndex].AwardsName;
            _resultWinner = new ObservableCollection<Drawer>();
            IsStartOrPause = true;
            _drawersImport = new List<Drawer>();
            ShowDrawerList = new ObservableCollection<Drawer>();
        }

        private void OnRefreshClockTimeUp(object sender, EventArgs e)
        {
            ShowDrawerList.Clear();
            
            if (ShowAwardsList[_currentAwardIndex].AwardsNumber > _drawersImport.Count)
            {
                foreach (var t in _drawersImport)
                {
                    var drawer = new Drawer(t.DrawCode,t.DrawName);
                    ShowDrawerList.Add(drawer);
                    IsLottery = false;
                    IsExport = true;
                }
                return;
            }

            var indexList = new List<int>();
            var random = new Random();
            for (var i = 0; i < ShowAwardsList[_currentAwardIndex].AwardsNumber; i++)
            {
                var index = random.Next(_drawersImport.Count);
                for (var j = 0; j < i; j++)
                {
                    if (indexList[j] != index) continue;
                    index = random.Next(_drawersImport.Count);
                    j = -1;
                }
                indexList.Add(index);
                var drawer = new Drawer(_drawersImport[index].DrawCode, _drawersImport[index].DrawName);
                ShowDrawerList.Add(drawer);
            }
        }

        private void WinningResult()
        {
            if (ShowAwardsList[_currentAwardIndex].AwardsNumber > _drawersImport.Count)
            {
                ShowAwardsList[_currentAwardIndex].AwardsNumber = _drawersImport.Count;
            }
            for (var i = 0; i < ShowAwardsList[_currentAwardIndex].AwardsNumber; i++)
            {
                var winners = new Drawer(ShowDrawerList[i].DrawCode, ShowDrawerList[i].DrawName)
                {
                    AwardName = ShowAwardsList[_currentAwardIndex].AwardsName,
                    AwardPrize = ShowAwardsList[_currentAwardIndex].AwardsPrize
                };
                _resultWinner.Add(winners);
                for (var j = 0; j < _drawersImport.Count; j++)
                {
                    if (_drawersImport[j].DrawCode == ShowDrawerList[i].DrawCode)
                    {
                        _drawersImport.Remove(_drawersImport[j]);
                    }
                }
                if (_drawersImport.Count != 0) continue;
                IsLottery = false;
                IsExport = true;
            }
        }

        private void LotteryButton()
        {
            if (IsStartOrPause)
            {
                _timer.Start();
                ShowAwardsList[_currentAwardIndex].AwardsColor = "#000000";
                LotteryName = "½áÊø";
                IsStartOrPause = false;
                CurrentAwardName = ShowAwardsList[_currentAwardIndex].AwardsName;
            }
            else
            {
                _timer.Stop();
                WinningResult();
                _currentAwardIndex += 1 ;
                if (_currentAwardIndex == ShowAwardsList.Count)
                {
                    IsLottery = false;
                    IsExport = true;
                }
                LotteryName = "³é½±";
                IsStartOrPause = true;
            }
        }

        private void Import()
        {
            ExcelAndXmlHandle.GetDrawers(_drawersImport);
            foreach (var t in _drawersImport)
            {
                ShowDrawerList.Add(t);
            }
            if (ShowDrawerList.Count == 0) return;
            IsImport = false;
            IsLottery = true;
        }

        private void Export()
        {
            ExcelAndXmlHandle.ExportExcelList(_resultWinner);
        }

        private static void MoveWindow()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.DragMove();
        }

        private void RecoveryWindow()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            ShowMaxButton = "Visible";
            ShowNormalButton = "Collapsed";
        }

        private void MaxWindow()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            ShowMaxButton = "Collapsed";
            ShowNormalButton = "Visible";
        }

        private static void MinWindow()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private static void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        private void InitializationXml()
        {
            _currentAwardIndex = 0;
            var xmlDoc = new XmlDocument();
            ShowAwardsList = new ObservableCollection<Awards>();
            xmlDoc.Load("config.xml");
            var nameList = xmlDoc.GetElementsByTagName("Name");
            var priceList = xmlDoc.GetElementsByTagName("Prize");
            var numberList = xmlDoc.GetElementsByTagName("Number");

            for (var i = 0; i < nameList.Count; i++)
            {
                if (nameList[i].InnerText == "") continue;

                var award = new Awards
                {
                    AwardsName = nameList[i].InnerText,
                    AwardsPrize = priceList[i].InnerText,
                    AwardsNumber = int.Parse(numberList[i].InnerText),
                    AwardsColor = "#b5b5b5"
                };
                ShowAwardsList.Add(award);
            }
        }

    }
}