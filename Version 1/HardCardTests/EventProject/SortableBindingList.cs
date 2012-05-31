using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace EventProject
{
    [Serializable()]
    public class SortableBindingList<T> : BindingList<T>
    {
        //hacky!
        //proper way of implementing would be to pass a sorting function
        //as a param, thus removing any type-specific code from this class
        public void ExplicitlySort(bool isQualify, bool sortByClasses)
        {            
            List<T> crList = base.Items.ToList() as List<T>;

            doingSort = true;
            try
            {
                if (isQualify)
                {
                    crList.Sort(delegate(T cr1T, T cr2T)
                    {
                        CompetitorRace cr1 = cr1T as CompetitorRace;
                        CompetitorRace cr2 = cr2T as CompetitorRace;

                        if (cr1 == null || cr2 == null) return 0;

                        //in qualification, no results (0 in timing) should go to the end of the list
                        if (cr1.bestLap == 0 && cr2.bestLap == 0) return 0;
                        if (cr1.bestLap == 0 && cr2.bestLap != 0) return 1;
                        if (cr1.bestLap != 0 && cr2.bestLap == 0) return -1;

                        //sort by class first                        
                        if (sortByClasses && cr1.className != null && cr2.className != null)
                        {
                            if (cr1.className.CompareTo(cr2.className) != 0)
                                return cr1.className.CompareTo(cr2.className);
                        }

                        if (sortByClasses)
                        {
                            if (cr1.className == null && cr2.className == null) return 0;
                            if (cr1.className != null && cr2.className == null) return -1;
                            if (cr1.className == null && cr2.className != null) return 1;
                        }
                        
                        if (cr1.bestLap < cr2.bestLap)
                            return -1;
                        else if (cr1.bestLap > cr2.bestLap)
                            return 1;
                        else
                            return 1;
                    });
                }
                else
                {
                    crList.Sort(delegate(T cr1T, T cr2T)
                    {
                        CompetitorRace cr1 = cr1T as CompetitorRace;
                        CompetitorRace cr2 = cr2T as CompetitorRace;

                        //use lapsCompleted; 
                        //however, currentPlace calculation is exactly this:
                        //who completed first most laps, and if the number 
                        //the same, check latest time
                        if (cr1 == null || cr2 == null) return 0;

                        //sort by class first
                        if (sortByClasses && cr1.className != null && cr2.className != null)
                        {
                            if (cr1.className.CompareTo(cr2.className) != 0)
                                return cr1.className.CompareTo(cr2.className);
                        }

                        if (sortByClasses)
                        {
                            if (cr1.className == null && cr2.className == null) return 0;
                            if (cr1.className != null && cr2.className == null) return -1;
                            if (cr1.className == null && cr2.className != null) return 1;
                        }

                        if (cr1.Position < cr2.Position)
                            return -1;
                        else if (cr1.Position > cr2.Position)
                            return 1;
                        else
                            return 1;
                    });
                }
                /*
                base.ClearItems();
                for (int i = 0; i < crList.Count; i++)
                {
                    //is there a way to add all items simultaneously? Otherwise,
                    //add/clear events are fired, and some nasty exceptions may occur
                    //base.InsertItem(i, crList[i]);
                    base.Items.Add(crList[i]);
                }*/

                //from msdn, http://msdn.microsoft.com/en-us/library/aa480736.aspx
                for (int i = 0; i < this.Count; i++)
                {
                    int position = IndexOf(crList[i]);
                    if (position != i)
                    {
                        T temp = this[i];
                        this[i] = this[position];
                        this[position] = temp;
                    }
                }
                
                if (isQualify)//reset
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        (this[i] as CompetitorRace).Position = i + 1;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
            }
            doingSort = false;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        bool doingSort = false;

        //don't fire events while sorting is "in progress", 
        //when elements are re-inserted
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (doingSort) return;

            try
            {
                base.OnListChanged(e);
            }
            catch (Exception exc)//hack! should not be catching exceptions here
            {
                Console.WriteLine(exc.StackTrace);
                DataManager.Log(exc.StackTrace);
            }
        }

        /// <summary>
        /// Have to perform this re-wiring, otherwise property changed events 
        /// are not propagated to the corresponding listeners. Solution taken 
        /// from the article 
        /// http://www.codeproject.com/KB/cs/FixingBindingListDeserial.aspx
        /// </summary>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            List<T> items = new List<T>(Items);
            int index = 0;
            foreach (T item in items)
            {
                base.SetItem(index++, item);
            }
        }
    }
}
