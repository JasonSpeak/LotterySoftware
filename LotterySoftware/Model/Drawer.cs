using GalaSoft.MvvmLight;

namespace LotterySoftware.Model
{
    public class Drawer : ObservableObject
    {
        private string _awardName;
        private string _awardPrize;
        private int _id;

        public string DrawCode { get; }
        public string DrawName { get; }

        public string AwardName
        {
            get => _awardName;
            set
            {
                _awardName = value;
                RaisePropertyChanged(() => _awardName);
            }
        }

        public string AwardPrize
        {
            get => _awardPrize;
            set
            {
                _awardPrize = value;
                RaisePropertyChanged(() => _awardPrize);
            }
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(()=>Id);
            }
        }

        public Drawer(string drawCode, string drawName)
        {
            DrawCode = drawCode;
            DrawName = drawName;
        }
    }
}