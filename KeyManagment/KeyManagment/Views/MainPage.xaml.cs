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
        private static Entry Entry4Application;
        private static Label Label4Application;
        private static Entry Entry4PW1;
        private static Label Label4PW1;
        private static Entry Entry4PW2;
        private static Button LeftButton;
        private static Button RightButton;
        private static EventHandler ButtonEvent;

        public static FirebaseDataStore<Item> RealTimeDatabase { get; set; }
        public static List<Item> ApplicationDatabasse { get; set; }


        public MainPage()
        {
            InitializeComponent();
            RealTimeDatabase = new FirebaseDataStore<Item>("Notes");
            ApplicationDatabasse = new List<Item>(RealTimeDatabase.GetItemsAsync(true).Result.ToList());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListView.ItemsSource = ApplicationDatabasse; 
        }
        
        void OnGoBackClick(object sender, EventArgs e)
        {
           
        }
       
        async void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            Debug.WriteLine("called onsave click");
            
            LeftButton.Clicked -= ButtonEvent;

            if (await RealTimeDatabase.AddItemAsync(selecteditem))
            {
                await DisplayAlert("Alert", "The Key update is done", "OK");
            }
            else
            { await DisplayAlert("Alert", "The Key update is not", "OK"); }


            //if ((((Entry4PW1.Text).Equals(Entry4PW2.Text)) && Entry4PW1.Text != null) &&
            //       Entry4Application.Text != null && selecteditem != null &&
            //       (!((Entry4Application.Text).Equals("ThisApplication"))))
            //{
            //    Item itementry = new Item();
            //    itementry.NameofApplication = Entry4Application.Text;
            //    itementry.PW = Entry4PW1.Text;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
            //    itementry.Date = DateTime.Now.ToString();
            //    if ((await RealTimeDatabase.UpdateItemAsync(selecteditem.Date, itementry)))
            //    {
            //        await DisplayAlert("Alert", "The Key update is done", "OK");
            //    }
            //    else
            //    {
            //        await DisplayAlert("Alert", "The Key update is failed", "OK");
            //    }
            //}
            //else
            //{
            //    await DisplayAlert("Alert", "The Key update is failed", "OK");
            //    InfoLabel.TextColor = Color.Red;
            //    InfoLabel.Text = "It seems something is wrong \n" +
            //                      "\n" +
            //                     "1. Is the application name ThisApplication? \n" +
            //                     "\n" +
            //                     "2. pease be sure the both passwards are identical \n";
            //}
        }
        
        void OnEditClick(object sender, EventArgs e, Item selecteditme)
        {
            Debug.WriteLine("called onedit click");
            Label labelapplication = FindByName("Label4Application") as Label;
            
            LeftButton.Clicked -= ButtonEvent;
            //FindClicked.
            Entry4Application = new Entry
            {
                Text = Label4Application.Text,
                HorizontalOptions = LayoutOptions.CenterAndExpand,

            };

            Entry4PW1 = new Entry
            {

            };

            Entry4PW2 = new Entry
            {

            };
            LeftButton.Text = "Save";
            ButtonEvent = (sender, eventArgs) => { OnSaveClick(sender, eventArgs, selecteditme); };
            LeftButton.Clicked += ButtonEvent;

            NoteViewStackLayout.Children.Remove(Label4Application);
            NoteViewStackLayout.Children.Remove(Label4PW1);
            NoteViewStackLayout.Children.Insert(0, Entry4Application);
            NoteViewStackLayout.Children.Insert(1, Entry4PW1);
            NoteViewStackLayout.Children.Insert(2, Entry4PW2);

        }

        void OnNoteAddedClicked(object sender, EventArgs e)
        {
            

        }

        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
           if (args.SelectedItem != null && false)
            {
                Item selecteditme = args.SelectedItem as Item;

                Label4Application = new Label
                {
                    Text = selecteditme.NameofApplication,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };

                Label4PW1 = new Label { Text = selecteditme.PW, };

                LeftButton = new Button { Text = "Edit", };

                RightButton = new Button { Text = "GoBack", };

                ButtonEvent = (sender, args) => { OnEditClick(sender, args, selecteditme); };

                LeftButton.Clicked += ButtonEvent;

                NoteViewStackLayout.Children.Remove(FindByName("ListView") as ListView);
                NoteViewStackLayout.Children.Insert(0, Label4Application);
                NoteViewStackLayout.Children.Insert(1, Label4PW1);
                NoteViewStackLayout.Children.Insert(2, LeftButton);
                NoteViewStackLayout.Children.Insert(3, RightButton);

                InfoLabel.Text = "Now you can modify the name and password of the related application";
            }

            Console.WriteLine("ich habe keine ahnung balbbaa");
        }

        /* try to event handler and event raise as discript in microsofot c# 
         * unsuccessful. 
         * */
        //protected virtual void OnItemInTreat(ItemInTreatEventArgs e)
        //{
        //    EventHandler<ItemInTreatEventArgs> handler = ItemInTreat;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        //public static event EventHandler<ItemInTreatEventArgs> ItemInTreat;
        
    }
    
    //public class ItemInTreatEventArgs : EventArgs
    //{
    //    public Item ItemArgs { get; set; }
    //    public ItemInTreatEventArgs(Item item)
    //    {
    //        this.ItemArgs = item;
    //    }

    //}
}
