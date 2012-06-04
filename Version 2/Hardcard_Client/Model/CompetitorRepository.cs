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
    public class CompetitorRepository
    {
        private HardcardEntities hardcardContext;
        private List<Competitor> _competitorStore;

        public CompetitorRepository(HardcardEntities i_hardcardContext)
        {
           
            try
            {
                //EU 02/19/2012
                // connect to db
                // too many objects of HardcardEntities
                hardcardContext = i_hardcardContext;
                //hardcardContext = new HardcardEntities();  
                IQueryable<Competitor> competitors = 
                    from p in hardcardContext.Competitors 
                    where p.Deleted == false
                    //orderby p.FirstName
                    orderby p.Id
                    select p;
                _competitorStore = competitors.ToList();
            }
            catch (Exception ex) {
                /*Need better error logging or display*/
                //System.Console.WriteLine(ex.Message);
                MessageBox.Show("Exception in CompetitorRepository():" + ex.Message);
            }
        
       }

        public void Update()
        {
            try
            {
                IQueryable<Competitor> competitors =
                    from p in hardcardContext.Competitors
                    where p.Deleted == false
                    //orderby p.FirstName
                    orderby p.Id
                    select p;
                _competitorStore = competitors.ToList();
            }
            catch (Exception ex)
            {
                /*Need better error logging or display*/
                System.Console.WriteLine(ex.Message);
            }
        }

        public void Save(Competitor competitor)
        {
            if (!_competitorStore.Contains(competitor))
            {
                /*auto-increment field for competitorID did not seem to 
                 work with LinQ for Data Entity Model*/
                long newid = 1;
                if ((from p in hardcardContext.Competitors
                     select p.Id).Count() > 0)
                {
                    newid = (from p in hardcardContext.Competitors
                             select p.Id).Max() + 1; //EU CompetitorID->Id 
                }
                competitor.Id = newid;
                hardcardContext.AddToCompetitors(competitor);
                hardcardContext.SaveChanges();
                _competitorStore.Add(competitor);
            }
            else
            {
                hardcardContext.SaveChanges();
            }
        }

        public void Delete(Competitor competitor)
        {
            competitor.Deleted = true;
            hardcardContext.SaveChanges();
            _competitorStore.Remove(competitor);
        }

        /// <summary>
        /// EU Insensitive search for substring in First or Last name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
 
        public List<Competitor> FindByLookup(string name)
        {
            IEnumerable<Competitor> found =
                from c in _competitorStore
                     //where (c.FirstName.ToLower().Contains(name.ToLower()) //tmp
                         //|| c.LastName.ToLower().Contains(name.ToLower()))    //tmp
                     where (c.VehicleType.ToLower().Contains(name.ToLower())
                         || c.VehicleModel.ToLower().Contains(name.ToLower()))
                     select c;
 
            return found.ToList();
           
        }

        /* 
        //Select for first chars in First or Last name
        public List<Competitor> FindByLookup(string name)
        {
            IEnumerable<Competitor> found =
                from c in _competitorStore
                where (c.FirstName.StartsWith( name,  StringComparison.OrdinalIgnoreCase) 
                   ||  c.LastName.StartsWith( name, StringComparison.OrdinalIgnoreCase))
                select c;
            return found.ToList();
        }
         * 
         * fi => fi.DESCRIPTION.toLower().Contains(description.ToLower()) 
        */

        public List<Competitor> FindAll()
        {
            return new List<Competitor>(_competitorStore);
        }
    }
}

