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

        public MainPage()
        {
            InitializeComponent();
            _ = new ViewCreator();
            _ = new DataOperation();
            //DataOperation.Init_DataOperation();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewCreator.LoginPage();
            Padding = 10;
            Title = "KeyManagement";
            this.ToolbarItems.Add(ViewCreator.AddItemToolBar("+"));
            this.ToolbarItems.Add(ViewCreator.AddItemToolBar("logo"));
            Content = ViewCreator.PageContain;
        }
    }

    internal class ViewCreator
    {
        public static StackLayout PageContain { get ; private set ; }

        public ViewCreator()
        {
            PageContain = new StackLayout();
        }

        public static ToolbarItem AddItemToolBar(string element)
        {
            ToolbarItem toolbaritem ;
            switch (element)
            {
                case "logo":
                    toolbaritem = new ToolbarItem
                    {
                        Text = "Icon Item",
                        Order = ToolbarItemOrder.Primary,
                        Priority = 1,
                        IconImageSource = ImageSource.FromFile("icon.png")
                    };
                    break;
                default:
                    toolbaritem = new ToolbarItem
                    {
                        Text = element,
                        Order = ToolbarItemOrder.Primary,
                        Priority = 0
                    };
                    toolbaritem.Clicked += OnNoteAddedClicked;
                    break;
            }
            return toolbaritem;
        }

        public static void OnNoteAddedClicked(object sender, EventArgs e)
        {
            if (DataOperation.LogedIN)
            {
                PageContain.Children.Clear();
                Entry entry4name = new Entry { Placeholder = "pls given the name of application", PlaceholderColor = Color.Olive, HorizontalOptions = LayoutOptions.CenterAndExpand };
                // DataOperation.PWChanging = (DataOperation.PWChanging != null ?  null : DataOperation.PWChanging);

                entry4name.BindingContext = DataOperation.PWChanging = new EntryPW();
                entry4name.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedName",
                                     mode: BindingMode.OneWayToSource);

                Entry entry4pw1 = new Entry();
                entry4pw1.Text = "";
                entry4pw1.BindingContext = DataOperation.PWChanging;
                entry4pw1.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW1",
                                     mode: BindingMode.OneWayToSource);

                Entry entry4pw2 = new Entry();
                entry4pw2.Text = "";
                entry4pw2.BindingContext = DataOperation.PWChanging;
                entry4pw2.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW2",
                                     mode: BindingMode.OneWayToSource);

                Button leftbutton = new Button { Text = "Save" };
                leftbutton.Clicked += OnAddSaveClick;

                PageContain.Children.Add(entry4name);
                PageContain.Children.Add(entry4pw1);
                PageContain.Children.Add(entry4pw2);
                PageContain.Children.Add(leftbutton);
            }

        }

        private async static void OnAddSaveClick(object sender, EventArgs e)
        {
            PageContain.Children.Clear();
            try
            {
                if (DataOperation.PWChanging != null &&
                    (((DataOperation.PWChanging.InputedPW1).Equals(DataOperation.PWChanging.InputedPW2)) &&
                    DataOperation.PWChanging.InputedPW1 != null) &&
                    DataOperation.PWChanging.InputedName != null /*&& 
                ToBeChangedApplication != null*/ &&
                    (!((DataOperation.PWChanging.InputedName).Equals("ThisApplication"))))
                {
                    Item tobechangeditem = new Item();
                    tobechangeditem.NameofApplication = DataOperation.PWChanging.InputedName;
                    tobechangeditem.PW = DataOperation.PWChanging.InputedPW1;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                    tobechangeditem.Date = DateTime.UtcNow.ToString();
                    bool updateresult;
                    updateresult = (await DataOperation.RealTimeDatabase.AddItemAsync(tobechangeditem));
                    if (updateresult)
                    {
                        RetrunListView();
                    }

                }
                else
                {

                }
            }
            catch
            {
            }
        }

        public async static void CreateListItemView()
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
            itemlist.ItemsSource = await DataOperation.GetData();
            Label infolabel = new Label { Text = "schoen dich zu sehen" };
            PageContain.Children.Add(itemlist);
            PageContain.Children.Add(infolabel);
        }

        private static void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                Item selecteditem = args.SelectedItem as Item;
                PageContain.Children.Clear();
                Label label4name = new Label { Text = selecteditem.NameofApplication, HorizontalOptions = LayoutOptions.CenterAndExpand };
                Label label4pw = new Label { Text = AESKEY.DecryptStringFromBytes_Aes(selecteditem.PW) };
                Button leftbutton = new Button { Text = "Edit" };
                leftbutton.Clicked += (sender, args) => { OnEditClick(sender, args, selecteditem); };

                Button rightbutton = new Button { Text = "GoBack" };
                rightbutton.Clicked += (sender, args) => { RetrunListView(); };

                Label infolabe = new Label { Text = "Now you can modify the name and password of the related application" };

                PageContain.Children.Add(label4name);
                PageContain.Children.Add(label4pw);
                PageContain.Children.Add(leftbutton);
                PageContain.Children.Add(rightbutton);
                PageContain.Children.Add(infolabe);
            }
        }

        private static void OnEditClick(object sender, EventArgs e, Item selecteditme)
        {
            PageContain.Children.Clear();
            Entry entry4name = new Entry { Text = selecteditme.NameofApplication, HorizontalOptions = LayoutOptions.CenterAndExpand };
            entry4name.BindingContext = DataOperation.PWChanging = new EntryPW();
            entry4name.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedName",
                                 mode: BindingMode.OneWayToSource);

            Entry entry4pw1 = new Entry();
            entry4pw1.Text = "";
            entry4pw1.BindingContext = DataOperation.PWChanging;
            entry4pw1.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW1",
                                 mode: BindingMode.OneWayToSource);

            Entry entry4pw2 = new Entry();
            entry4pw2.Text = "";
            entry4pw2.BindingContext = DataOperation.PWChanging;
            entry4pw2.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW2",
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

        private static void RetrunListView()
        {
            PageContain.Children.Clear();        
            CreateListItemView();
        }

        private async static void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            PageContain.Children.Clear();
            try
            {
                if (DataOperation.PWChanging != null &&
                    (((DataOperation.PWChanging.InputedPW1).Equals(DataOperation.PWChanging.InputedPW2)) &&
                    DataOperation.PWChanging.InputedPW1 != null) &&
                    DataOperation.PWChanging.InputedName != null /*&& 
                ToBeChangedApplication != null*/ &&
                    (!((DataOperation.PWChanging.InputedName).Equals("ThisApplication"))))
                {
                    Item tobechangeditem = new Item();
                    tobechangeditem.NameofApplication = DataOperation.PWChanging.InputedName;
                    tobechangeditem.PW = DataOperation.PWChanging.InputedPW1;//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                    tobechangeditem.Date = DateTime.UtcNow.ToString();
                    bool updateresult;
                    updateresult = (await DataOperation.RealTimeDatabase.UpdateItemAsync(selecteditem, tobechangeditem));
                    if (updateresult)
                    {
                        RetrunListView();
                    }
                        
                }
                else
                {

                }
            }
            catch
            {
            }
        }

        public static void LoginPage()
        {
            PageContain.Children.Clear();
            if (DataOperation.AppPW != null)
            {
                Label plslogin = new Label { Text = "Pls Login", HorizontalOptions = LayoutOptions.CenterAndExpand };

                Entry entrypw = new Entry { Placeholder = "Pls give your app password" };
                entrypw.BindingContext = DataOperation.PWChanging = new EntryPW();
                entrypw.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW1",
                                   mode: BindingMode.OneWayToSource);
                DataOperation.PWChanging.InputedName = "thisapplication";
                Button leftbutton = new Button { Text = "LogIN" };
                leftbutton.Clicked += LoginEvent;

                Button rightbutton = new Button { Text = "Registration" };
                rightbutton.IsEnabled = false;

                PageContain.Children.Add(entrypw);
                PageContain.Children.Add(leftbutton);
                PageContain.Children.Add(rightbutton);
            }
            else
            {
                Label labelthisapplication = new Label { Text = "ThisApplication", HorizontalOptions = LayoutOptions.CenterAndExpand };
                // DataOperation.PWChanging = (DataOperation.PWChanging != null ?  null : DataOperation.PWChanging);

                DataOperation.PWChanging = new EntryPW();
                DataOperation.PWChanging.InputedName = "ThisApplication";

                Entry entry4pw1 = new Entry();
                entry4pw1.Text = "";
                entry4pw1.BindingContext = DataOperation.PWChanging;
                entry4pw1.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW1",
                                     mode: BindingMode.OneWayToSource);

                Entry entry4pw2 = new Entry();
                entry4pw2.Text = "";
                entry4pw2.BindingContext = DataOperation.PWChanging;
                entry4pw2.SetBinding(Entry.TextProperty, "DataOperation.PWChanging.InputedPW2",
                                     mode: BindingMode.OneWayToSource);

                Button leftbutton = new Button { Text = "Save" };
                leftbutton.Clicked += OnSaveRegisterationClick;

                Button rightbutton = new Button { Text = "Cancel" };
                rightbutton.Clicked += (sender, args) => { LoginPage(); };
                rightbutton.IsEnabled = false;

                PageContain.Children.Add(labelthisapplication);
                PageContain.Children.Add(entry4pw1);
                PageContain.Children.Add(entry4pw2);
                PageContain.Children.Add(leftbutton);
                PageContain.Children.Add(rightbutton);

            }

        }

        private static void LoginEvent(object sender, EventArgs e)
        {           
            string encryloginpw;
            AESKEY.Set_AesKey(DataOperation.PWChanging.InputedPW1);
            encryloginpw = AESKEY.EncryptStringToBytes_Aes(DataOperation.PWChanging.InputedPW1);
            Console.WriteLine("what is wrong? {0}", encryloginpw);

            if (encryloginpw.Equals(DataOperation.AppPW) && DataOperation.AppPW != null)
            {
                CreateListItemView();
            }            
            else
            {
                Label infolabe = new Label { Text = "your passowrd is wrong" };
                PageContain.Children.Add(infolabe);

            }
        }

        private async static void OnSaveRegisterationClick(object sender, EventArgs e)
        {
            PageContain.Children.Clear();
            try
            {
                if (DataOperation.PWChanging != null &&
                    (((DataOperation.PWChanging.InputedPW1).Equals(DataOperation.PWChanging.InputedPW2)) &&
                    DataOperation.PWChanging.InputedPW1 != null) && 
                    (DataOperation.PWChanging.InputedName.Equals("ThisApplication"))) 
                {
                    Console.WriteLine("what is wrong? {0}", DataOperation.PWChanging.InputedName.Equals("ThisApplication").ToString());
                    Item appacount = new Item();
                    AESKEY.Set_AesKey(DataOperation.PWChanging.InputedPW1);
                    appacount.NameofApplication = DataOperation.PWChanging.InputedName;
                    appacount.PW = AESKEY.EncryptStringToBytes_Aes(DataOperation.PWChanging.InputedPW1);//AESKEY.EncryptStringToBytes_Aes(Entry4PW1.Text);
                    appacount.Date = DateTime.UtcNow.ToString();
                    bool updateresult;
                    updateresult = (await DataOperation.RealTimeDatabase.CreatAPPAccount(appacount));
                    CreateListItemView();
                }
                else
                {

                }
            }
            catch
            {
            }
        }
    }

    internal class DataOperation
    {
        public static FirebaseDataStore<Item> RealTimeDatabase { get; private set; }

        public static EntryPW PWChanging { get; set; }

        public static String AppPW { get; set; }

        /*I don't like this solution
         * need the property to enable add itme event
         */
        public static bool LogedIN { get; set; }

        //public async static void Init_DataOperation()
        public DataOperation()
        {
            LogedIN = false;
            RealTimeDatabase = new FirebaseDataStore<Item>("Notes");
            Item thisapplication = RealTimeDatabase.GetAppPW().Result;
            AppPW = thisapplication==null? null: thisapplication.PW;
            Console.WriteLine("what is wrong? {0}", AppPW);
        }
        
        public async static Task<List<Item>> GetData()
        {
            return (await RealTimeDatabase.GetItemsAsync()).ToList();
        }

    }

    internal class EntryPW
    {
        public string InputedName { get; set;}
        public string InputedPW1 { get; set;}
        public string InputedPW2 { get; set;}
    }


}  