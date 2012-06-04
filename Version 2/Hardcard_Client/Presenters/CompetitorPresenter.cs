using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using System.Collections.Generic;
using System;

namespace RacingEventsTrackSystem.Presenters
{
    public class CompetitorPresenter : PresenterBase<CompetitorView>
    {
        private readonly AllCompetitorsPresenter _allCompetitorsPresenter;
        private readonly Competitor _competitor;

        // Exception if there is no CurrentPresenter (same for Event, and Class)
        public CompetitorPresenter( AllCompetitorsPresenter allCompetitorsPresenter,
                                    CompetitorView view,
                                    Competitor competitor)
                                    : base(view, competitor.Athlete.FirstName + ", " + competitor.Athlete.LastName)
        {
            _allCompetitorsPresenter = allCompetitorsPresenter;
            _competitor = competitor;
        }

        public Competitor Competitor
        {
            get { return _competitor; }
        }

        public AllCompetitorsPresenter AllCompetitorsPresenter
        {
            get { return _allCompetitorsPresenter; }
        }

        public override bool Equals(object obj)
        {
            CompetitorPresenter presenter = obj as CompetitorPresenter;
            return presenter != null && presenter.Competitor.Equals(Competitor);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
   }
}
