using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using KeyManagment;
using KeyManagment.Models;
using KeyManagment.Services;
using Xamarin.Forms.Internals;
using System.Collections;
using System.Diagnostics;

namespace KeyManagment.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public static FirebaseDataStore RealTimeDatabase { get; set; }
        public static List<Item> ApplicationDatabasse { get; set; }
        private static bool callednotyetfinished ;


        public MainPage()
        {
            InitializeComponent();
            RealTimeDatabase = new FirebaseDataStore();
            ApplicationDatabasse = RealTimeDatabase.GetItems().Result;
            callednotyetfinished = true;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListView.ItemsSource = ApplicationDatabasse;
        }

        void OnNoteAddedClicked(object sender, SelectedItemChangedEventArgs args)
        {

        }

        public async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var tmp = await RealTimeDatabase.TestFunc();
            if (tmp != null)
            { Console.WriteLine("tmp {0}", tmp.NameofApplication); }
        }

    }
}
