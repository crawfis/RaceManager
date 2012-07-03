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
            var hc = _applicationPresenter.HardcardContext; 
            Athlete newAthlete = new Athlete();
            newAthlete.Id = 0;
            long max_id = 0;
            if ((from c in hc.Athletes select c).Count() > 0) // not empty table
            {
                max_id = (from e in hc.Athletes select e.Id).Max();
                newAthlete.Id = ++max_id;
            }

            newAthlete.FirstName = "FirstName" + newAthlete.Id.ToString();
            newAthlete.LastName = "LastName" + newAthlete.Id.ToString();
            newAthlete.Gender = "F";
            newAthlete.DOB = DateTime.Now;
            newAthlete.AddressLine1 = "1111 High St.";
            newAthlete.AddressLine2 = "2222 High St.";
            newAthlete.AddressLine3 = "3333 High St.";
            newAthlete.City = "Dublin";
            newAthlete.State = "Oh";
            newAthlete.PostalCode = "43017";
            newAthlete.Country = "USA";
            newAthlete.Phone = "(614)614-6145";
            hc.Athletes.AddObject(newAthlete);
            hc.SaveChanges();
            AllAthletes = InitAllAthletes();
            CurrentAthlete = newAthlete;
        }


        // 
        // Create Athletes collection and set default CurrentAthlete 
        //
        public ObservableCollection<Athlete> InitAllAthletes()
        {
            ObservableCollection<Athlete> Tmp = new ObservableCollection<Athlete>();
            CurrentAthlete = null;

            var hc = _applicationPresenter.HardcardContext;
            List<Athlete> query = (from c in hc.Athletes select c).ToList();
            if (query.Count() > 0)
            {
                Tmp = new ObservableCollection<Athlete>(query);
                CurrentAthlete = Tmp.First();
            }

            return Tmp;
        }
        //
        // Update existing Athlete or add new entry if Athlete is not in DataContext.Athletes
        //
        public void SaveAthlete(Athlete athlete)
        {
            if (athlete == null) return;
            if (!ValidateAthlete(athlete)) return;
            _applicationPresenter.HardcardContext.SaveChanges();
            StatusText = string.Format("Athlete '{0} {1}' was saved.", athlete.FirstName, athlete.LastName);
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

        // Copy Value from athlete to DbAthlete
        public void UpdateAthlete(Athlete athlete, Athlete DbAthlete)
        {
            if (athlete == null || DbAthlete == null) return;
            DbAthlete.FirstName   =  athlete.FirstName; 
            DbAthlete.LastName   =  athlete.LastName;
            DbAthlete.DOB = athlete.DOB;
            DbAthlete.Gender = athlete.Gender;
            DbAthlete.AddressLine1 = athlete.AddressLine1;
            DbAthlete.AddressLine2 = athlete.AddressLine2;
            DbAthlete.AddressLine3 = athlete.AddressLine3;
            DbAthlete.City = athlete.City;
            DbAthlete.State = athlete.State;
            DbAthlete.PostalCode = athlete.PostalCode;
            DbAthlete.Country = athlete.Country;
            DbAthlete.Phone = athlete.Phone;
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
