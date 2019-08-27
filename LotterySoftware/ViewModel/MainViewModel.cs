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
        private readonly ObservableCollection<Drawer> _resultWinner;

        private bool _isExport;
        private bool _isLottery;
        private bool _isShowSlider;
        private bool _isStartOrPause;
        private bool _isDisplayRecoveryButton;
        private string _currentAwardName;
        private List<Awards> _showAwardsList;
        private ObservableCollection<Drawer> _showDrawerList;

        public RelayCommand ImportCmd => new RelayCommand(Import);
        public RelayCommand ExportCmd => new RelayCommand(Export);
        public RelayCommand MinWindowCmd => new RelayCommand(MinWindow);
        public RelayCommand MaxWindowCmd => new RelayCommand(MaxWindow);
        public RelayCommand MoveWindowCmd => new RelayCommand(MoveWindow);
        public RelayCommand CloseWindowCmd => new RelayCommand(CloseWindow);
        public RelayCommand RecoveryWindowCmd => new RelayCommand(RecoveryWindow);
        public RelayCommand LotteryButtonCmd => new RelayCommand(LotteryFunction);

        public bool IsLottery
        {
            get => _isLottery;
            set
            {
                _isLottery = value;
                RaisePropertyChanged(()=>IsLottery);
            }
        }

        public bool IsShowSlider
        {
            get => _isShowSlider;
            set
            {
                _isShowSlider = value;
                RaisePropertyChanged(()=>IsShowSlider);
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

        public bool IsDisplayRecoveryButton
        {
            get => _isDisplayRecoveryButton;
            set
            {
                _isDisplayRecoveryButton = value;
                RaisePropertyChanged(()=>IsDisplayRecoveryButton);
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

        public MainViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnRefreshClockTimeUp;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 90);
            _drawersImport = new List<Drawer>();
            _resultWinner = new ObservableCollection<Drawer>();
            ShowDrawerList = new ObservableCollection<Drawer>();

            InitXml();
        }

        private static void MoveWindow()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.DragMove();
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

        private void RecoveryWindow()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            IsDisplayRecoveryButton = false;
        }

        private void MaxWindow()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            IsDisplayRecoveryButton = true;
        }

        private void LotteryFunction()
        {
            if (IsLottery != true) return;
            if (!IsStartOrPause)
            {
                _timer.Start();
                ShowAwardsList[_currentAwardIndex].IsBulgeDisplay = true;
                IsStartOrPause = true;
                CurrentAwardName = System.Text.RegularExpressions.Regex.Replace(ShowAwardsList[_currentAwardIndex].AwardsName, " ··· ", "");
            }
            else
            {
                _timer.Stop();
                WinningResult();
                IsExport = true;
                _currentAwardIndex += 1;
                if (_currentAwardIndex == ShowAwardsList.Count)
                {
                    IsLottery = false;
                }
                IsStartOrPause = false;
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
                IsLottery = false;
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
            IsShowSlider = ShowDrawerList.Count > 10;
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
            IsShowSlider = ShowDrawerList.Count > 10;
        }

        private void Import()
        {
            if (_resultWinner.Count !=0)
            {
                if (MessageBox.Show("是否清空中奖名单并导入文件？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    IsExport = false;
                    IsLottery = false;
                    ShowDrawerList.Clear();
                    _resultWinner.Clear();
                    _currentAwardIndex = 0;
                    CurrentAwardName = null;
                    for (var i = 0; i < _showAwardsList.Count; i++)
                    {
                        ShowAwardsList[i].IsBulgeDisplay = false;
                    }
                }
                else
                {
                    return;
                }
            }
            _drawersImport = ExcelHandle.GetDrawers();
            if (_drawersImport.Count == 0) return;
            {
                ShowDrawerList.Clear();
                IsExport = false;
                _currentAwardIndex = 0;
                CurrentAwardName = null;
                for (var i = 0; i < _showAwardsList.Count; i++)
                {
                    ShowAwardsList[i].IsBulgeDisplay = false;
                }
                foreach (var t in _drawersImport)
                {
                    ShowDrawerList.Add(t);
                }
                if (ShowDrawerList.Count == 0) return;
                IsLottery = true;
            }
            IsShowSlider = ShowDrawerList.Count >= 10;
        }

        private void Export()
        {
            ExcelHandle.ExportExcelList(_resultWinner);
        }

        private void InitXml()
        {
            _currentAwardIndex = 0;
            ShowAwardsList = XmlHandle.InitializationXml();
        }

        public string GetOpenDialogFileName()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        public string GetSaveDialogFileName()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }
    }
}