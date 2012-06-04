using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.DataAccess;
using System.Windows;
using System.Collections;

namespace RacingEventsTrackSystem.Model
{

    public class ComboCompetitor
    {

        public long   AthleteId    { get; set; }
        public long   CompetitorId { get; set; }
        public long   EventClassId { get; set; }
        public string FirstName    { get; set; }
        public string LastName     { get; set; }
        public DateTime  DOB       { get; set; }
        public string Gender       { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City         { get; set; }
        public string State        { get; set; }
        public string PostalCode   { get; set; }
        public string Country      { get; set; }
        public string Phone        { get; set; }
        public string VehicleType  { get; set; }
        public string VehicleModel { get; set; }
        public int    VehicleCC    { get; set; }

        public Competitor Competitor { get; set; } //03/02

        public ComboCompetitor(long athleteId, long competitorId, long eventClassId, string firstName, string lastName)
        {
            this.AthleteId = athleteId;
            this.CompetitorId = competitorId;
            this.EventClassId = eventClassId;
            this.FirstName = firstName;
            this.LastName = lastName;
       }

        public ComboCompetitor() {} // to make xaml happy


        public global::System.String DataToDisplay
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", 
                       AthleteId, CompetitorId, EventClassId, LastName, FirstName,
                       City, State, Country, VehicleType);
            }
        }
    }

    public class ComboCompetitorRepository
    {
        public event EventHandler<EventArgs> ComboCompetitorAdded;
        protected void OnComboCompetitorAdded()
        {
            if (ComboCompetitorAdded != null)
            {
                ComboCompetitorAdded(this, EventArgs.Empty);
            }
        }
        private HardcardEntities _hardcardContext;
        //private List<Competitor> _competitorStore;
        //private List<EventClass> _eventClassStore;
        //private List<Athlete> _athleteClassStore;
        private List<ComboCompetitor> _comboCompetitorStore = new List<ComboCompetitor>();

        //private List<ComboCompetitor> _comboCompetitors = new List<ComboCompetitor>();

        //Reposotory's list should have property ICollection<>???!!!
        //private List<Widget> _widgets = new List<Widget>();
        //public ICollection<Widget> Widgets{get{return _widgets;}}

        public ComboCompetitorRepository(HardcardEntities hardcardContext)
        {
            try
            {
                this._hardcardContext = hardcardContext;
                IQueryable<ComboCompetitor> competitors =
                    from Competitor c in hardcardContext.Competitors
                    from Athlete a in hardcardContext.Athletes
                    from EventClass e in hardcardContext.EventClasses
                    where c.AthleteId == a.Id
                       && c.EventClassId == e.EventClassID
                       && c.Deleted == false
                    orderby a.FirstName
                    select new ComboCompetitor()
                    {
                       AthleteId    = (long)c.AthleteId,
                       CompetitorId = (long)c.Id,
                       EventClassId = (long)c.EventClassId,
                       FirstName    = a.FirstName,
                       LastName     = a.LastName,
                       DOB          = a.DOB,
                       Gender       = a.Gender,
                       AddressLine1 = a.AddressLine1,
                       AddressLine2 = a.AddressLine2,
                       AddressLine3 = a.AddressLine3,
                       City         = a.City,
                       State        = a.State,
                       PostalCode   = a.PostalCode,
                       Country      = a.Country,
                       Phone        = a.Phone,
                       VehicleType  = c.VehicleType,
                       VehicleModel = c.VehicleModel,
                       VehicleCC    = (int)c.VehicleCC,
                       Competitor = c
                    };              
                //foreach (string na=me in query) Console.WriteLine(name);
                //foreach (var p in competitors)
                //    _comboCompetitorStore.Add();
                _comboCompetitorStore = competitors.ToList();
            }
            catch (Exception ex)
            {
                /*Need better error logging or display*/
                //System.Console.WriteLine(ex.Message);
                MessageBox.Show("Exception in CompetitorRepository():" + ex.Message);
            };
        }

        public void AddComboCompetitor(ComboCompetitor comboCompetitor)
        {
            _comboCompetitorStore.Add(comboCompetitor);
            OnComboCompetitorAdded();
        }
        public List<ComboCompetitor> FindAll()
        {
            return new List<ComboCompetitor>(_comboCompetitorStore);
        }
        /*
         private void CreateDefaultWidgets()
         {
             AddWidget(new Widget(1, "Awesome widget", WidgetType.TypeA));
             AddWidget(new Widget(2, "Okay widget", WidgetType.TypeA));
             AddWidget(new Widget(3, "So-so widget", WidgetType.TypeB));
             AddWidget(new Widget(4, "Horrible widget", WidgetType.TypeB));
         }
         */


    }

}
