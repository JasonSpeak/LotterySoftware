using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LotterySoftware.Model;

namespace LotterySoftware.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int _currentAwardIndex;
        private readonly DispatcherTimer _timer;
        private List<Drawer> _drawersImport;
        private readonly ObservableCollection<Drawer> _winningDrawer;

        private bool _canImport;
        private bool _canExport;
        private bool _canLottery;
        private bool _isDrawerPassOnePage;
        private bool _isStartOrPause;
        private bool _isMaxOrRecoveryButton;
        private string _currentAwardName;
        private List<Awards> _showAwardsList;
        private ObservableCollection<Drawer> _showDrawerList;

        public bool CanImport
        {
            get => _canImport;
            set
            {
                _canImport = value;
                RaisePropertyChanged(()=>CanImport);
            }
        }
        public bool CanExport
        {
            get => _canExport;
            set
            {
                _canExport = value;
                RaisePropertyChanged(() => CanExport);
            }
        }
        public bool CanLottery
        {
            get => _canLottery;
            set
            {
                _canLottery = value;
                RaisePropertyChanged(()=>CanLottery);
            }
        }
        public bool IsDrawerPassOnePage
        {
            get => _isDrawerPassOnePage;
            set
            {
                _isDrawerPassOnePage = value;
                RaisePropertyChanged(()=>IsDrawerPassOnePage);
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
        public bool IsMaxOrRecoveryButton
        {
            get => _isMaxOrRecoveryButton;
            set
            {
                _isMaxOrRecoveryButton = value;
                RaisePropertyChanged(()=>IsMaxOrRecoveryButton);
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
        public List<Awards> ShowAwardsList
        {
            get => _showAwardsList;
            set
            {
                _showAwardsList = value;
                RaisePropertyChanged(() => ShowAwardsList);
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

        public RelayCommand ImportCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand MinWindowCommand { get; }
        public RelayCommand MaxWindowCommand { get; }
        public RelayCommand MoveWindowAction { get; }
        public RelayCommand CloseWindowCommand { get; }
        public RelayCommand RecoveryWindowCommand { get; }
        public RelayCommand LotteryButtonCommand { get; }

        public MainViewModel()
        {
            InitAwards();
            CanImport = true;
            _currentAwardIndex = 0;

            _timer = new DispatcherTimer();
            _timer.Tick += OnRefreshClockTimeUp;
            _timer.Interval = new TimeSpan(90000);

            _drawersImport = new List<Drawer>();
            _winningDrawer = new ObservableCollection<Drawer>();
            ShowDrawerList = new ObservableCollection<Drawer>();

            ImportCommand = new RelayCommand(OnImportCommandExecuted);
            ExportCommand = new RelayCommand(OnExportCommandExecuted);
            MinWindowCommand = new RelayCommand(OnMinWindowCommandExecuted);
            MaxWindowCommand = new RelayCommand(OnMaxWindowCommandExecuted);
            MoveWindowAction = new RelayCommand(OnMoveWindowActionExecuted);
            CloseWindowCommand = new RelayCommand(OnCloseWindowCommandExecuted);
            RecoveryWindowCommand = new RelayCommand(OnRecoveryWindowCommandExecuted);
            LotteryButtonCommand = new RelayCommand(OnLotteryFunctionCommandExecuted);
        }

        private void OnLotteryFunctionCommandExecuted()
        {
            if (CanLottery != true) return;
            if (!IsStartOrPause)
            {
                _timer.Start();
                ShowAwardsList[_currentAwardIndex].IsAlreadyLottery = true;
                IsStartOrPause = true;
                CanImport = false;
                CurrentAwardName = ShowAwardsList[_currentAwardIndex].AwardsName;
            }
            else
            {
                _timer.Stop();
                WinningResult();
                CanExport = true;
                _currentAwardIndex += 1;
                if (_currentAwardIndex == ShowAwardsList.Count)
                {
                    CanLottery = false;
                }
                IsStartOrPause = false;
                CanImport = true;
            }
        }

        private void OnRefreshClockTimeUp(object sender, EventArgs e)
        {
            ShowDrawerList.Clear();
            if (ShowAwardsList[_currentAwardIndex].AwardsNumber > _drawersImport.Count)
            {
                for (var i=0;i<_drawersImport.Count;i++)
                {
                    var drawer = new Drawer(_drawersImport[i].DrawCode, _drawersImport[i].DrawName) {Id = i+1};
                    ShowDrawerList.Add(drawer);
                }
                IsStartOrPause = true;
                OnLotteryFunctionCommandExecuted();
                CanLottery = false;
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
                var drawer = new Drawer(_drawersImport[index].DrawCode, _drawersImport[index].DrawName) {Id = i + 1};
                ShowDrawerList.Add(drawer);
            }
            IsDrawerPassOnePage = ShowDrawerList.Count > 10;
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
                    AwardPrize = ShowAwardsList[_currentAwardIndex].AwardsPrize,
                    Id = i + 1
                };
                _winningDrawer.Add(winners);
                for (var j = 0; j < _drawersImport.Count; j++)
                {
                    if (_drawersImport[j].DrawCode == ShowDrawerList[i].DrawCode)
                    {
                        _drawersImport.Remove(_drawersImport[j]);
                    }
                }
                if (_drawersImport.Count != 0) continue;
                CanLottery = false;
                CanExport = true;
            }
            IsDrawerPassOnePage = ShowDrawerList.Count > 10;
        }

        private void OnImportCommandExecuted()
        {
            if (_winningDrawer.Count !=0)
            {
                if (MessageBox.Show("是否清空中奖名单并导入文件？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ResetDrawers();
                }
                else
                {
                    return;
                }
            }
            var fileName = GetOpenDialogFileName();
            if (fileName != "")
            {
                _drawersImport = ExcelHandle.GetDrawers(fileName);
            }
            else
            {
                return;
            }
            if (_drawersImport == null) return;
            {
                ShowDrawerList.Clear();
                CanExport = false;
                _currentAwardIndex = 0;
                CurrentAwardName = string.Empty;
                for (var i = 0; i < _showAwardsList.Count; i++)
                {
                    ShowAwardsList[i].IsAlreadyLottery = false;
                }
                foreach (var t in _drawersImport)
                {
                    ShowDrawerList.Add(t);
                }
                if (ShowDrawerList.Count == 0) return;
                CanLottery = true;
            }
            IsDrawerPassOnePage = ShowDrawerList.Count >= 10;
        }

        private void OnExportCommandExecuted()
        {
            ExcelHandle.ExportExcelList(_winningDrawer,GetSaveDialogFileName());
        }

        private void InitAwards()
        {
            ShowAwardsList = XmlHandle.GetAwards();
        }

        private void ResetDrawers()
        {
            CanExport = false;
            CanLottery = false;
            ShowDrawerList.Clear();
            _winningDrawer.Clear();
            _currentAwardIndex = 0;
            CurrentAwardName = string.Empty;
            for (var i = 0; i < _showAwardsList.Count; i++)
            {
                ShowAwardsList[i].IsAlreadyLottery = false;
            }
        }

        private static string GetOpenDialogFileName()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        private static string GetSaveDialogFileName()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        private static void OnMoveWindowActionExecuted()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.DragMove();
        }

        private static void OnMinWindowCommandExecuted()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private static void OnCloseWindowCommandExecuted()
        {
            Application.Current.Shutdown();
        }

        private void OnRecoveryWindowCommandExecuted()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            IsMaxOrRecoveryButton = false;
        }

        private void OnMaxWindowCommandExecuted()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            IsMaxOrRecoveryButton = true;
        }
    }
}