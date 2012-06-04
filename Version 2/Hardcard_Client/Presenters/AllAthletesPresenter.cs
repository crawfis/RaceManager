using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using System.Windows;
using System.Data.Objects;

namespace RacingEventsTrackSystem.Presenters
{
    public class AllAthletesPresenter : PresenterBase<Shell> 
    {
        private readonly ApplicationPresenter _applicationPresenter;
        private ObservableCollection<Athlete> _allAthletes;
        private Athlete _currentAthlete;
        private ObservableCollection<EventClass> _eventClassesForAthlete;//keeps EventClasses for current Athlete
        private string _statusText;
        public AllAthletesPresenter( ApplicationPresenter applicationPresenter,
                                     Shell view
                                   ) : base(view)
        {
            try
            {
                _applicationPresenter = applicationPresenter;
                _allAthletes = new ObservableCollection<Athlete>(_applicationPresenter.HardcardContext.Athletes);

                // to populate athleteView with some data while openinig
                if (_allAthletes.Count() > 0) { _currentAthlete = _allAthletes.First(); }
                StatusText = "AllAthletesPresenter constructor no error: ";
            }
            catch (Exception ex)
            {
                StatusText = "AllAthletesPresenter constructor failed with error: " + ex.Message;
                MessageBox.Show(StatusText); // stop executable
            }

        }
  
        public ObservableCollection<Athlete> AllAthletes
        {
            get { return _allAthletes; }
            set
            {
                _allAthletes = value;
                OnPropertyChanged("AllAthletes");
            }
        }

        public ObservableCollection<EventClass> EventClassesForAthlete
        {
            get { return _eventClassesForAthlete; }
            set
            {
                _eventClassesForAthlete = value;
                OnPropertyChanged("EventClassesForAthlete");
            }
        }

        public Athlete CurrentAthlete
        {
            get { return _currentAthlete; }
            set
            {
                _currentAthlete = value;
                // Set EventClasses for CurrentAthlete
                // Copy data from Database into  EventClassesForAthlete.
                EventClassesForAthlete = InitEventClassesForAthlete(_currentAthlete);
                OnPropertyChanged("CurrentAthlete");
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        public void Search(string criteria)
        {
            if (!string.IsNullOrEmpty(criteria) && criteria.Length > 0)
            {
                AllAthletes = new ObservableCollection<Athlete>(FindByLookup(criteria));
                StatusText = string.Format("{0} Athletes found.", AllAthletes.Count);
            }
            else
            {
                AllAthletes = new ObservableCollection<Athlete>(_applicationPresenter.HardcardContext.Athletes);
                StatusText = "Displaying all Athletes.";
            }
        }

        //
        // Insensitive search for substring in First or Last name
        //
        public List<Athlete> FindByLookup(string name)
        {
            IEnumerable<Athlete> found =
                from c in _applicationPresenter.HardcardContext.Athletes
                where (c.LastName.ToLower().Contains(name.ToLower())
                    || c.FirstName.ToLower().Contains(name.ToLower()))
                select c;
            return found.ToList();
        }

        //
        // Create new Athlete. It adds the Athlete to Collection and DataContext.Athlets. 
        // 
        public void CreateNewAthlete()
        {
            Athlete newAthlete = new Athlete();
            newAthlete.FirstName = "Unknown";
            newAthlete.LastName = "Unknown";
            newAthlete.DOB = DateTime.Now;
            CurrentAthlete = newAthlete;
            SaveAthlete(newAthlete);
            OpenAthlete(newAthlete);
        }

        //
        // Update existing Athlete or add new entry if Athlete is not in DataContext.Athletes
        //
        public void SaveAthlete(Athlete athlete)
        {
            if (athlete == null) return;
            if (!ValidateAthlete(athlete)) return;

            var hc = _applicationPresenter.HardcardContext;
            // If athlete is in Athlete DataContext then just update it
            if (IsInAthlete(athlete))
            {
                //update Athlete in DataContext.Athletes
                Athlete dbAthlete = hc.Athletes.Single(p => p.Id == athlete.Id);
                hc.ApplyCurrentValues(dbAthlete.EntityKey.EntitySetName, athlete);
                StatusText = string.Format("Athlete '{0}' was updated.", athlete.ToString());
            }
            else
            {
                //Create new Athlete in DataContext.Athletes
                //Use if (contact.Id == Guid.Empty) contact.Id = Guid.NewGuid(); to get Id.
                long max_id = 0;
                //var q = eventClasses.DefaultIfEmpty();// not empty table
                if ((from c in hc.Athletes select c).Count() > 0) // not empty table
                {
                    max_id = (from c in hc.Athletes select c.Id).Max();
                }
                athlete.Id = ++max_id;
                hc.Athletes.AddObject(athlete);
                StatusText = string.Format("Athlete '{0}' was added to Athlete table.", athlete.ToString());
            }

            // Update AllAthletes Collection            
            int i = AllAthletes.IndexOf(athlete);
            if (i >= 0)
            {
                // If athlete is in AllAthletes Collection then just update it
                AllAthletes.RemoveAt(i); 
                AllAthletes.Insert(i, athlete);
                CurrentAthlete = athlete; // Current event was just deleted before
            }
            else
            {
                AllAthletes.Add(athlete);
            }

            hc.SaveChanges();
            OpenAthlete(athlete);
            StatusText = string.Format("Athlete '{0}' was saved.", athlete.LastName);
        }

        //
        // Create Competitor in Cmpeititors based on existing Athlete and some CompetitorInfo
        //
        public void SaveAtleteAsCompetitor(Athlete athlete, CompetitorInfo compInfo)
        {
            if (athlete == null) return;
            if (!ValidateAthlete(athlete)) return;

            var hc = _applicationPresenter.HardcardContext;
        }
 

        //
        // Returns false if some input data for athlete are not match the DataBase fields
        // (!!!Replace with interective WPF validation during editing)
        public bool ValidateAthlete(Athlete athlete)
        {
            if (athlete.FirstName == null && athlete.LastName == null)
            {
                MessageBox.Show("FirstName and LastName fields can not be null"); 
                return false; 
            }
            if (athlete.Gender != null && athlete.Gender.Count() > 1) 
            {   
                MessageBox.Show("Gender can not be more than 1 charchter"); 
                return false; 
            }
            return true;
        }

        // Returns true if Athlete.Id exists in the Athlete table
        private bool IsInAthlete(Athlete athlete)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Athletes.Count(a => a.Id == athlete.Id) == 0) ? false : true;
        }

        // Returns true if Competitor table has FK for input Athlete
        public bool IsAthleteInCompetitor(Athlete athlete)
        {
            var hc = _applicationPresenter.HardcardContext;
            return (hc.Competitors.Count(ec => ec.AthleteId == athlete.Id) == 0) ? false : true;
        }

        // 
        // Delete Athlete from DataContext and Collection. Doesn't reset Current Race Class.
        //
        public void DeleteAthlete(Athlete athlete)
        {
            if (athlete == null) return;
            var hc = _applicationPresenter.HardcardContext;

            // Delete from DataContext
            // Check if athlete is in Athlete table
            if (IsInAthlete(athlete))
            {
                // check if there is reference FK in Competitor table
                if (IsAthleteInCompetitor(athlete))
                {
                     string str = string.Format("Remove dependent Competitor first for Athlete.Id = {0}", 
                         athlete.Id);
                     MessageBox.Show(str);
                     return;
                }
                else
                { 
                    hc.Athletes.DeleteObject(hc.Athletes.Single(p => p.Id == athlete.Id));
                    StatusText = string.Format("Athlete '{0}' was deleted.", athlete.ToString());
                }
            }

            if (AllAthletes.Contains(athlete))
            {
                AllAthletes.Remove(athlete);
                OpenAthlete(new Athlete());
            }
            StatusText = string.Format("Athlete '{0}' was deleted.", athlete.LastName);
        }

        public void OpenAthlete(Athlete athlete)
        {
            if (athlete == null) return;
            View.ShowAthlete(new AthletePresenter(this, new AthleteView(), athlete),
                                                   View.athleteView);
        }

        public ObservableCollection<EventClass> InitEventClassesForAthlete(Athlete athlete)
        {
            ObservableCollection<EventClass> Tmp;
            AllEventsPresenter allEventsPresenter = _applicationPresenter.AllEventsPresenter;
            Event currentEvent = allEventsPresenter.CurrentEvent;
            if (athlete == null || allEventsPresenter == null || currentEvent == null)
                Tmp = new ObservableCollection<EventClass>();
            else
            {
                var hc = _applicationPresenter.HardcardContext;
                IQueryable<EventClass> eventClasses =
                     from ec in hc.EventClasses
                     from c in hc.Competitors
                     where c.AthleteId == athlete.Id
                        && ec.EventId == currentEvent.Id
                        && ec.Id == c.EventClassId
                     select ec;
                Tmp = new ObservableCollection<EventClass>(eventClasses.ToList());
            }
            return Tmp;
        }


        public void DisplayCurrentAthlete()
        {
            if (_currentAthlete != null) 
                OpenAthlete(_currentAthlete);
        }
 
        public ApplicationPresenter ApplicationPresenter
        {
            get { return _applicationPresenter; }
            set { }
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
