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

        private static MainPageELement MainPageELement { get; set; }
        private static DataOperation DataOperation { get; set; }


        public MainPage()
        {
            InitializeComponent();
            MainPageELement = new MainPageELement();
            DataOperation = new DataOperation();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           // MainPageELement.ListItemView();
            //this.ToolbarItems.Add(MainPageELement.ToolbarAddButton);
            /*ToolbarItem item = new ToolbarItem
            {
                Text = "Example Item",
               // IconImageSource = ImageSource.FromFile("example_icon.png"),
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };*/

            // "this" refers to a Page object
            //this.Content = MainPageELement.PageContain;
            //MainPageToolbar.Add(item);
            //this.ToolbarItems.Add( item);

            //Content = MainPageELement.PageContain;
            //MainPageContent.Children.Add( MainPageELement.PageContain);

/*            Content = new StackLayout
            {
                Children =
                {
                    MainPageELement.InfoLabel,
                }
            };*/
            //ToolbarItems.Add(item);
        }






    }

    internal class MainPageELement
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
        public static Label InfoLabel;
        public static ToolbarItem ToolbarAddButton { get; set; }
        public static StackLayout PageContain;


        public MainPageELement()
        {
            ItemList = new ListView();
            Entry4Application = new Entry();
            Label4Application = new Label();
            Label4PW1 = new Label();
            Entry4PW1 = new Entry();
            Entry4PW2 = new Entry();
            LeftButton = new Button();
            RightButton = new Button();
            InfoLabel = new Label();
            PageContain = new StackLayout();
            ToolbarAddButton = new ToolbarItem();
            ToolbarAddButton.Text = "+";
            ToolbarAddButton.Clicked += OnNoteAddedClicked;
        }

        private void OnNoteAddedClicked(object sender, EventArgs e)
        {


        }

        public void ListItemView()
        {        
            ItemList.ItemTemplate = new DataTemplate(() =>
            {
                var textcell = new TextCell();
                textcell.SetBinding(TextCell.TextProperty, new Binding("NameofApplication"));
                textcell.SetBinding(TextCell.DetailProperty, new Binding("PW"));
                return textcell;
            });
            ItemList.ItemsSource = DataOperation.ApplicationDatabasse;
            ItemList.ItemSelected += OnListViewItemSelected;
           
            InfoLabel.Text = "schoen dich zu sehen";
            PageContain.Children.Add(ItemList);
            PageContain.Children.Add(InfoLabel);

        }

        public void ItemView(Item item)
        {
            PageContain.Children.Clear();

            Label4Application.Text = item.NameofApplication;
            Label4Application.HorizontalOptions = LayoutOptions.CenterAndExpand;

            Label4PW1.Text = item.PW;
            LeftButton.Text = "Edit";
            LeftButton.Clicked += (sender, args) => { OnEditClick(sender, args, item); };

            RightButton.Text = "GoBack";
            RightButton.Clicked += RetrunListView;

            InfoLabel.Text = "Now you can modify the name and password of the related application";

            PageContain.Children.Add(Label4Application);
            PageContain.Children.Add(Label4PW1);
            PageContain.Children.Add(LeftButton);
            PageContain.Children.Add(RightButton);
            PageContain.Children.Add(InfoLabel);

        }

        public void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Debug.WriteLine("called onseelected click");
            if (args.SelectedItem != null)
            {
                ItemView(args.SelectedItem as Item);
            }
        }

        public void OnEditClick(object sender, EventArgs e, Item selecteditme)
        {
            PageContain.Children.Clear();
            Entry4Application.Text = selecteditme.NameofApplication;
            Entry4PW1.Text = "";
            Entry4PW2.Text = "";

            LeftButton = null;
            LeftButton.Text = "Save";
            LeftButton.Clicked += (sender, args) => { OnSaveClick(sender, args, selecteditme); };

            InfoLabel.Text = "Now you can modify the name and password of the related application";

            PageContain.Children.Add(Entry4Application);
            PageContain.Children.Add(Entry4PW1);
            PageContain.Children.Add(Entry4PW2);
            PageContain.Children.Add(LeftButton);
            PageContain.Children.Add(RightButton);
            PageContain.Children.Add(InfoLabel);
        }

        public void RetrunListView(object sender, EventArgs e)
        {
            PageContain.Children.Clear();
            ClearnUIElement();
            ListItemView();
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

        private async void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            Debug.WriteLine("called onsave click");

            if ((((Entry4PW1.Text).Equals(Entry4PW2.Text)) && Entry4PW1.Text != null) &&
                   Entry4Application.Text != null /*&& ToBeChangedApplication != null*/ &&
                   (!((Entry4Application.Text).Equals("ThisApplication"))))
            {
                Item tobechangeditem = new Item();
                tobechangeditem.NameofApplication = Entry4Application.Text;
                tobechangeditem.PW = Entry4PW1.Text;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                tobechangeditem.Date = DateTime.UtcNow.ToString();
                bool updateresult;
                updateresult = await DataOperation.RealTimeDatabase.UpdateItemAsync(selecteditem, tobechangeditem);
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


    }

    internal class DataOperation
    {
        public static FirebaseDataStore<Item> RealTimeDatabase { get; set; }
        public static List<Item> ApplicationDatabasse { get; set; }

        public DataOperation()
        {
            RealTimeDatabase = new FirebaseDataStore<Item>("Notes");
            ApplicationDatabasse = RealTimeDatabase.GetItemsAsync(true).Result.ToList();
        }
    }
}  