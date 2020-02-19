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


        public static FirebaseDataStore<Item> RealTimeDatabase { get; set; }
        public static List<Item> ApplicationDatabasse { get; set; }
        public static PageElement MainPageElement { get; set; }


        public MainPage()
        {
            InitializeComponent();
            RealTimeDatabase = new FirebaseDataStore<Item>("Notes");
            ItemList = new ListView
            {
                Margin = 20,
                ItemTemplate = new DataTemplate(() =>
                {
                    var textcell = new TextCell();
                    textcell.SetBinding(TextCell.TextProperty, new Binding("NameofApplication"));
                    textcell.SetBinding(TextCell.DetailProperty, new Binding("PW"));
                    // textcell.BindingContext = new { Text = "", Detail = "PW" };
                    return textcell;
                })

            };
            ItemList.ItemSelected += OnListViewItemSelected;
            NoteViewStackLayout.Children.Insert(0, ItemList);
            ApplicationDatabasse = RealTimeDatabase.GetItemsAsync(true).Result.ToList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ItemList.ItemsSource = ApplicationDatabasse;
        }

        void OnGoBackClick(object sender, EventArgs e)
        {

        }

        public async void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            Debug.WriteLine("called onsave click");

            LeftButton.Clicked -= ButtonEvent;

            if ((((Entry4PW1.Text).Equals(Entry4PW2.Text)) && Entry4PW1.Text != null) &&
                   Entry4Application.Text != null /*&& ToBeChangedApplication != null*/ &&
                   (!((Entry4Application.Text).Equals("ThisApplication"))))
            {
                Item tobechangeditem = new Item();
                tobechangeditem.NameofApplication = Entry4Application.Text;
                tobechangeditem.PW = Entry4PW1.Text;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                tobechangeditem.Date = DateTime.UtcNow.ToString();
                _ = await RealTimeDatabase.UpdateItemAsync(selecteditem, tobechangeditem);
            }
            else
            {
                InfoLabel.TextColor = Color.Red;
                InfoLabel.Text = "It seems something is wrong \n" +
                                  "\n" +
                                 "1. Is the application name ThisApplication? \n" +
                                 "\n" +
                                 "2. pease be sure the both passwards are identical \n";
            }
        }

        public void OnEditClick(object sender, EventArgs e, Item selecteditme)
        {

            Label labelapplication = FindByName("Label4Application") as Label;

            LeftButton.Clicked -= ButtonEvent;
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

        public void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Debug.WriteLine("called onseelected click");
            if (args.SelectedItem != null)
            {
                ItemList.ItemSelected -= OnListViewItemSelected;
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
                RightButton.Clicked += RetrunListView;

                NoteViewStackLayout.Children.Remove(ItemList);
                NoteViewStackLayout.Children.Insert(0, Label4Application);
                NoteViewStackLayout.Children.Insert(1, Label4PW1);
                NoteViewStackLayout.Children.Insert(2, LeftButton);
                NoteViewStackLayout.Children.Insert(3, RightButton);

                InfoLabel.Text = "Now you can modify the name and password of the related application";
            }
        }

        public void RetrunListView(object sender, EventArgs e)
        {
            NoteViewStackLayout.Children.Clear();
            ClearnUIElement();
            ItemList = new ListView
            {
                Margin = 20,
                ItemTemplate = new DataTemplate(() =>
                {
                    var textcell = new TextCell();
                    textcell.SetBinding(TextCell.TextProperty, new Binding("NameofApplication"));
                    textcell.SetBinding(TextCell.DetailProperty, new Binding("PW"));
                    // textcell.BindingContext = new { Text = "", Detail = "PW" };
                    return textcell;
                })

            };
            ItemList.ItemsSource = ApplicationDatabasse;
            ItemList.ItemSelected += OnListViewItemSelected;           
            NoteViewStackLayout.Children.Insert(0, ItemList);            
        }

        private void ClearnUIElement()
        {
            Entry4Application = null;
            Label4Application = null; 
            Entry4PW1 = null;
            Label4PW1 = null;
            Entry4PW2 = null;
            LeftButton = null;
            RightButton = null;
            ButtonEvent = null;
            ItemList = null;
        }
    }

    public class PageElement
    {
        private static Entry Entry4Application;
        private static Label Label4Application;
        private static Entry Entry4PW1;
        private static Label Label4PW1;
        private static Entry Entry4PW2;
        private static Button LeftButton;
        private static Button RightButton;
        private static EventHandler ButtonEvent;
        private static ListView ItemList;

    }
}