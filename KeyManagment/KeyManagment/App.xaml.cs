using System;
using Xamarin.Forms;
using KeyManagment.Services;
using KeyManagment.Models;
using KeyManagment.Views;
using System.Collections.Generic;

namespace KeyManagment
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            //DataStoreContainer = new DataStoreContainer(new FirebaseAuthService());
            //Item firstone = new Item { NameofApplication = "tmp", PW = "pasword", Date = DateTime.Now.ToString() };
            
            
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
