using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
//EU. 02/18/2012 copy of EditCompetitorPresenter: replaced Competitor with Event (local i_event) 

namespace RacingEventsTrackSystem.Presenters
{
    public class EditRacePresenter: PresenterBase<RaceView>
    {
        private readonly ApplicationPresenter _applicationPresenter;
        private readonly Session _session;

        public EditRacePresenter(
            ApplicationPresenter applicationPresenter,
            RaceView view,
            Session session)
            : base(view, session.StartTime + ", " + session.EventClassID)
        {
            _applicationPresenter = applicationPresenter;
            _session = session;
        }

        public Session Session
        {
            get { return _session; }
        }

        /*EU
        public void Save()
        {
            _applicationPresenter.SaveRace(Session, this);
        }

        public void Delete()
        {
            _applicationPresenter.CloseTab(this);
            _applicationPresenter.DeleteRace(Session);
        }
        */
        public void Close()
        {
            //_applicationPresenter.CloseTab(this);
        }

        public override bool Equals(object obj)
        {
            EditRacePresenter presenter = obj as EditRacePresenter;
            return presenter != null && presenter.Session.Equals(Session);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
