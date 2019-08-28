using GalaSoft.MvvmLight;

namespace LotterySoftware.Model
{
    public class Awards : ObservableObject
    {
        private int _awardsNumber;
        private string _awardsName;
        private string _awardsPrize;
        private bool _isAlreadyLottery;
        private bool _isLastAward;

        public string AwardsName
        {
            get => _awardsName;
            set
            {
                _awardsName = value;
                RaisePropertyChanged(() => AwardsName);
            }
        }

        public string AwardsPrize
        {
            get => _awardsPrize;
            set
            {
                _awardsPrize = value;
                RaisePropertyChanged(() => AwardsPrize);
            }
        }

        public int AwardsNumber
        {
            get => _awardsNumber;
            set
            {
                _awardsNumber = value;
                RaisePropertyChanged(() => AwardsNumber);
            }
        }

        public bool IsAlreadyLottery
        {
            get => _isAlreadyLottery;
            set
            {
                _isAlreadyLottery = value;
                RaisePropertyChanged(()=> IsAlreadyLottery);
            }
        }

        public bool IsLastAward
        {
            get => _isLastAward;
            set
            {
                _isLastAward = value;
                RaisePropertyChanged(()=>IsLastAward);
            }
        }
    }
}