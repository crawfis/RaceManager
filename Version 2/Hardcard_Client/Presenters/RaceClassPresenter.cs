using System;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;

namespace RacingEventsTrackSystem.Presenters
{
    public class RaceClassPresenter : PresenterBase<RaceClassView>
    {
        private readonly AllRaceClassesPresenter _allRaceClassesPresenter;
        private readonly RaceClass _raceClass;

        public RaceClassPresenter( AllRaceClassesPresenter allRaceClassesPresenter,
                                   RaceClassView view,
                                   RaceClass raceClass)
            : base(view, raceClass.ClassName + ",")
        {
            _allRaceClassesPresenter = allRaceClassesPresenter;
            _raceClass = raceClass;
        }

        public AllRaceClassesPresenter AllRaceClassesPresenter
        {
            get { return _allRaceClassesPresenter; }
        }
      
        public RaceClass RaceClass
        {
            get { return _raceClass; }
        }

        public void Delete()
        {
         //   _allRaceClassesPresenter.DeleteRaceClass(RaceClass);
        }

        public override bool Equals(object obj)
        {
            RaceClassPresenter presenter = obj as RaceClassPresenter;
            return presenter != null && presenter.RaceClass.Equals(RaceClass);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
