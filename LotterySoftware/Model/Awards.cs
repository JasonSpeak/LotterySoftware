using GalaSoft.MvvmLight;

namespace LotterySoftware.Model
{
    public class Awards : ObservableObject
    {
        private string _awardsName;
        private string _awardsPrize;
        private int _awardsNumber;
        private string _awardsColor;
        private string _visibilityAwards;

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

        public string AwardsColor
        {
            get => _awardsColor;
            set
            {
                _awardsColor = value;
                RaisePropertyChanged(()=>AwardsColor);
            }
        }

        public string VisibilityAwards
        {
            get => _visibilityAwards;
            set
            {
                _visibilityAwards = value;
                RaisePropertyChanged(() => VisibilityAwards);
            }
        }
    }
}