namespace RacingEventsTrackSystem.Presenters
{
    public class PresenterBase<T> : Notifier
    {
        private readonly string _tabHeader;
        private readonly T _view;

        public PresenterBase(T view)
        {
            _view = view;
        }

        public PresenterBase(T view, string tabHeader)
        {
            _view = view;
            if ((tabHeader.Trim()).Equals(",") || tabHeader.Equals(""))
            {
                _tabHeader = "New Entry";
            }
            else
            {
                _tabHeader = tabHeader;
            }
        }

        public T View
        {
            get { return _view; }
        }

        public string TabHeader
        {
            get { return _tabHeader; }
        }
    }
}

