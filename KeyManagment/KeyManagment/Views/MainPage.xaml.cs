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
        internal static ViewCreator MainPageElement { get ; set ; }
        internal static DataOperation DataOperation { get ; set; }


        public MainPage()
        {
            InitializeComponent();            
            MainPageElement = new ViewCreator();
            DataOperation = new DataOperation();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewCreator.CreateListItemView();
            this.ToolbarItems.Add(ViewCreator.AddToolbarButton());
            Content = ViewCreator.PageContain;
        }
    }

    internal class ViewCreator
    {
        public static ToolbarItem ToolbarAddButton { get; set; }
        private static StackLayout _pagecontain;
        public static StackLayout PageContain { get => _pagecontain; private set => _pagecontain = value; }
        public static EntryPW PWChanging { get; set; }

        public ViewCreator()
        {
            _pagecontain = new StackLayout();
        }

        public static ToolbarItem AddToolbarButton()
        {
            return (new ToolbarItem
            {
                Text = "++++++++",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
                IconImageSource = ImageSource.FromFile("icon.png")
            });
        }

        private static void OnNoteAddedClicked(object sender, EventArgs e)
        {
            

        }   

        public static void CreateListItemView()
        {
            PageContain.Children.Clear();
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
            _pagecontain.Children.Add(itemlist);
            _pagecontain.Children.Add(infolabel);
        }

        private static void CreateItemView(Item item)
        {
            PageContain.Children.Clear();
            Label label4name = new Label { Text = item.NameofApplication, HorizontalOptions = LayoutOptions.CenterAndExpand };
            Label label4pw = new Label { Text = item.PW };
            Button leftbutton = new Button { Text = "Edit" };
            leftbutton.Clicked += (sender, args) => { OnEditClick(sender, args, item); };

            Button rightbutton = new Button { Text = "GoBack" };
            rightbutton.Clicked += (sender, args) => { RetrunListView(); };

            Label infolabe = new Label { Text = "Now you can modify the name and password of the related application" };

            _pagecontain.Children.Add(label4name);
            _pagecontain.Children.Add(label4pw);
            _pagecontain.Children.Add(leftbutton);
            _pagecontain.Children.Add(rightbutton);
            _pagecontain.Children.Add(infolabe);
        }

        private static void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                CreateItemView(args.SelectedItem as Item);
            }
        }

        private static void OnEditClick(object sender, EventArgs e, Item selecteditme)
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

            _pagecontain.Children.Add(entry4name);
            _pagecontain.Children.Add(entry4pw1);
            _pagecontain.Children.Add(entry4pw2);
            _pagecontain.Children.Add(leftbutton);
            _pagecontain.Children.Add(rightbutton);
            _pagecontain.Children.Add(infolabe);
        }

        public static void RetrunListView()
        {
            _pagecontain.Children.Clear();
        }

        private async static void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            if (PWChanging != null &&
                (((PWChanging.InputedPW1).Equals(PWChanging.InputedPW2)) && 
                PWChanging.InputedPW1 != null) &&
                PWChanging.InputedName != null /*&& 
                ToBeChangedApplication != null*/ &&
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

        public static void RefreshData ()
        {
            ApplicationDatabasse = RealTimeDatabase.GetItemsAsync(true).Result.ToList();
        }
    }

    internal class EntryPW
    {
        public string InputedName { get; set;}
        public string InputedPW1 { get; set;}
        public string InputedPW2 { get; set;}
    }


}  