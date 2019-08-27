using GalaSoft.MvvmLight;

namespace LotterySoftware.Model
{
    public class Awards : ObservableObject
    {
        private int _awardsNumber;
        private string _awardsName;
        private string _awardsPrize;
        private bool _isBulgeDisplay;

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

        public bool IsBulgeDisplay
        {
            get => _isBulgeDisplay;
            set
            {
                _isBulgeDisplay = value;
                RaisePropertyChanged(()=> IsBulgeDisplay);
            }
        }
    }
}