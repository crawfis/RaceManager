using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using System.Text;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Data.Objects;

namespace RacingEventsTrackSystem.Presenters
{
    public class AthletePresenter : PresenterBase<AthleteView>
    {
        private readonly AllAthletesPresenter _allAthletesPresenter;
        private readonly Athlete _athlete;

        public AthletePresenter(AllAthletesPresenter allAthletesPresenter,
                                AthleteView view,
                                Athlete athlete)
            : base(view, athlete.FirstName + ", " + athlete.LastName)
        {
            _allAthletesPresenter = allAthletesPresenter;
            _athlete = athlete;
        }

        public Athlete Athlete
        {
            get { return _athlete; }
        }

        public AllAthletesPresenter AllAthletesPresenter
        {
            get { return _allAthletesPresenter; }
        }

        public override bool Equals(object obj)
        {
            AthletePresenter presenter = obj as AthletePresenter;
            return presenter != null && presenter.Athlete.Equals(Athlete);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
