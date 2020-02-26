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
            MainPageELement.ListItemView();            
            this.ToolbarItems.Add(MainPageELement.ToolbarAddButton);
            Content = MainPageELement.PageContain;
        }






    }

    internal class MainPageELement
    {
        public static ToolbarItem ToolbarAddButton { get; set; }
        public static StackLayout PageContain;
        private static EntryPW PWChanging;

        public static string testpw;


        public MainPageELement()
        {
            PageContain = new StackLayout();
            ToolbarAddButton = new ToolbarItem
            {
                Text = "++++++++",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
                IconImageSource = ImageSource.FromFile("icon.png"),
            };
            ToolbarAddButton.Clicked += OnNoteAddedClicked;
        }

        private void OnNoteAddedClicked(object sender, EventArgs e)
        {
            

        }

        public void ListItemView()
        {
            ListView itemlist = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var textcell = new TextCell();
                    textcell.SetBinding(TextCell.TextProperty, new Binding("NameofApplication"));
                    textcell.SetBinding(TextCell.DetailProperty, new Binding("PW"));
                    return textcell;
                })
            };
            itemlist.ItemSelected += OnListViewItemSelected;
            itemlist.ItemsSource = DataOperation.ApplicationDatabasse;           
            Label infolabel = new Label { Text = "schoen dich zu sehen" };
            PageContain.Children.Add(itemlist);
            PageContain.Children.Add(infolabel);

        }

        public void ItemView(Item item)
        {
            PageContain.Children.Clear();

            Label label4name = new Label { Text = item.NameofApplication, HorizontalOptions = LayoutOptions.CenterAndExpand };
            Label label4pw = new Label { Text = item.PW };
            Button leftbutton = new Button { Text = "Edit" };
            leftbutton.Clicked += (sender, args) => { OnEditClick(sender, args, item); };

            Button rightbutton = new Button { Text = "GoBack" };
            rightbutton.Clicked += (sender, args) => { RetrunListView(); };

            Label infolabe = new Label { Text = "Now you can modify the name and password of the related application" };

            PageContain.Children.Add(label4name);
            PageContain.Children.Add(label4pw);
            PageContain.Children.Add(leftbutton);
            PageContain.Children.Add(rightbutton);
            PageContain.Children.Add(infolabe);

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

            Entry entry4name = new Entry { Text = selecteditme.NameofApplication, HorizontalOptions = LayoutOptions.CenterAndExpand };
            entry4name.BindingContext = PWChanging = new EntryPW();
            entry4name.SetBinding(Entry.TextProperty, "PWChanging.InputedName",
                                 mode: BindingMode.OneWayToSource);

            Entry entry4pw1 = new Entry();
            entry4pw1.Text = "";
            entry4pw1.BindingContext = PWChanging;
            entry4pw1.SetBinding(Entry.TextProperty, "PWChanging.InputedPW1",
                                 mode: BindingMode.OneWayToSource);

            Entry entry4pw2 = new Entry();
            entry4pw2.Text = "";
            entry4pw2.BindingContext = PWChanging;
            entry4pw2.SetBinding(Entry.TextProperty, "PWChanging.InputedPW2",
                                 mode: BindingMode.OneWayToSource); 

            Button leftbutton = new Button { Text = "Save" };
            leftbutton.Clicked += (sender, args) => { OnSaveClick(sender, args, selecteditme); };

            Button rightbutton = new Button { Text = "GoBack" };
            rightbutton.Clicked += (sender, args) => { RetrunListView(); };

            Label infolabe = new Label { Text = "Now you can modify the name and password of the related application" };

            PageContain.Children.Add(entry4name);
            PageContain.Children.Add(entry4pw1);
            PageContain.Children.Add(entry4pw2);
            PageContain.Children.Add(leftbutton);
            PageContain.Children.Add(rightbutton);
            PageContain.Children.Add(infolabe);
        }

        public void RetrunListView()
        {
            PageContain.Children.Clear();
            ClearnUIElement();

            ListItemView();
        }

        private void ClearnUIElement()
        {
        }

        private async void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            Debug.WriteLine("called onsave click");
            

            if (PWChanging != null &&

                (((PWChanging.InputedPW1).Equals(PWChanging.InputedPW2)) && PWChanging.InputedPW1 != null) &&
                   PWChanging.InputedName != null /*&& ToBeChangedApplication != null*/ &&
                   (!((PWChanging.InputedName).Equals("ThisApplication"))))
            {
                Item tobechangeditem = new Item();
                tobechangeditem.NameofApplication = PWChanging.InputedName;
                tobechangeditem.PW = PWChanging.InputedPW1;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                tobechangeditem.Date = DateTime.UtcNow.ToString();
                bool updateresult;
                updateresult = await DataOperation.RealTimeDatabase.UpdateItemAsync(selecteditem, tobechangeditem);
                if (updateresult)
                {
                    RetrunListView();
                }
            }
            else
            {

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

        public void RefreshData ()
        {
            ApplicationDatabasse = RealTimeDatabase.GetItemsAsync(true).Result.ToList();
        }
    }

    internal class EntryPW
    {
        public string InputedName { get; set; }
        public string InputedPW1 { get; set; }
        public string InputedPW2 { get; set; }
    }


}  