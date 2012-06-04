using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using System.Collections.Generic;
using System;


namespace RacingEventsTrackSystem.Presenters
{
    public class SessionPresenter : PresenterBase<SessionView>
    {
        private readonly AllSessionsPresenter _allSessionsPresenter;
        private readonly Session _session;

        public SessionPresenter(AllSessionsPresenter allSessionsPresenter,
                                    SessionView view,
                                    Session session)
            : base(view, session.Id + ", " + session.EventClassId)
        {
            _allSessionsPresenter = allSessionsPresenter;
            _session = session;
        }

        public Session Session
        {
            get { return _session; }
        }

        public AllSessionsPresenter AllSessionsPresenter
        {
            get { return _allSessionsPresenter; }
        }

        public override bool Equals(object obj)
        {
            SessionPresenter presenter = obj as SessionPresenter;
            return presenter != null && presenter.Session.Equals(Session);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
