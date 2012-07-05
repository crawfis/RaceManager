using System;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
//EU. 02/18/2012 copy of EditCompetitorPresenter: replaced Competitor with Event (local i_event) 

namespace RacingEventsTrackSystem.Presenters
{
    public class EventPresenter: PresenterBase<EventView>
    {
        //private readonly EventPresenter _eventPresenter;
        private readonly AllEventsPresenter _allEventsPresenter;
        private readonly Event _event;
    
        public EventPresenter(AllEventsPresenter allEventsPresenter,
                              EventView view,
                              Event myEvent)
            : base(view, myEvent.EventName + ", " + myEvent.EventLocation) // tabheader text Set to constant string instead of property
        {
            _allEventsPresenter = allEventsPresenter;
            _event = myEvent;
        }

        public Event Event
        {
            get { return _event; }
        }

        public AllEventsPresenter AllEventsPresenter
        {
            get { return _allEventsPresenter; }
        }

        /*
        public void Delete()
        {
            //_allEventsPresenter.DeleteEvent(Event);
        }
         */ 

        public override bool Equals(object obj)
        {
            EventPresenter presenter = obj as EventPresenter;
            return presenter != null && presenter.Event.Equals(Event);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
