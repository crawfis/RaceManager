using System;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;
using System.Windows;

namespace RacingEventsTrackSystem.Presenters
{
    public partial class ApplicationPresenter : PresenterBase<Shell>
    {
        private readonly ComboCompetitorRepository _comboCompetitorRepository;  // Keeps data from DB(not 'deleted')
        private ObservableCollection<ComboCompetitor> _currentComboCompetitors; // Keeps data, what we see on GUI
        private ComboCompetitor _currentComboCompetitor;                        // Competitor, selected by GUI
        //private ComboCompetitor _comboCurrentCompetitor;

        public ObservableCollection<ComboCompetitor> CurrentComboCompetitors
        {
            get { return _currentComboCompetitors; }
            set
            {
                _currentComboCompetitors = value;
                OnPropertyChanged("CurrentComboCompetitors");
            }
        }

        public ComboCompetitor CurrentComboCompetitor
        {
            get { return _currentComboCompetitor; }
            set
            {
                _currentComboCompetitor = value;
                OnPropertyChanged("CurrentComboCompetitor");
            }
        }
        
        public void NewCompetitor()
        {
            OpenCompetitor(new ComboCompetitor());
        }

        // It might be DisplayCurrentCompetitor(){OpenCompetitor(Competitor competitor)}
        public void DisplayCurrentComboCompetitor()
        {
            OpenCompetitor(_currentComboCompetitor);
        }

        public void OpenCompetitor(ComboCompetitor comboCompetitor)
        {
            if (comboCompetitor == null) return;
            View.AddTab(new EditCompetitorPresenter(this,
                                                      new EditCompetitorView(),
                                                      comboCompetitor), View.tabs);
        }

         /*public void DisplayAllCompetitors()
             {
                 throw new NotImplementedException();
             }*/

    }
}