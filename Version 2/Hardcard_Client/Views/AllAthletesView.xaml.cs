﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using RacingEventsTrackSystem.Model;
using RacingEventsTrackSystem.Views;
using RacingEventsTrackSystem.DataAccess;
using RacingEventsTrackSystem.Presenters;

namespace RacingEventsTrackSystem.Views
{
    /// <summary>
    /// Interaction logic for AllAthletesView.xaml
    /// </summary>
    public partial class AllAthletesView : UserControl
    {
        public AllAthletesView()
        {
            InitializeComponent();
        }
        public AllAthletesPresenter Presenter
        {
            get { return DataContext as AllAthletesPresenter; }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //Presenter.Close();
        }
        
        //vertial button for each athlete in the row
        /*
        private void OpenContact_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;

            if (button != null)
                Presenter.OpenContact(button.DataContext as Contact);
        }
        */ 
    }
}
