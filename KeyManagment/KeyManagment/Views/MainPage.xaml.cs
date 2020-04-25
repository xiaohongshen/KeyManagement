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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();            
            Padding = 10;
            Title = "KeyManagement";
            this.ToolbarItems.Add(ViewCreator.AddItemToolBar("+"));
            this.ToolbarItems.Add(ViewCreator.AddItemToolBar("logo"));
            ViewCreator.LoginPage();
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
                    toolbaritem.Clicked += OnAddedClicked;
                    break;
            }
            return toolbaritem;
        }

        public static void OnAddedClicked(object sender, EventArgs e)
        {
            if (DataOperation.LogedIN)
            {
                PageContain.Children.Clear();
                DataOperation.PWChanging = null;
                DataOperation.PWChanging = new EntryPW();
                EntryAddView entryaddview = new EntryAddView();
                entryaddview.leftbutton.Clicked += OnAddSaveClick;
                entryaddview.midbutton.Clicked += (sender, args) => { KeyGenEvent(false); };
                entryaddview.rightbutton.Clicked += (sender, args) => { RetrunListView(); };
                PageContain.Children.Add(entryaddview.viewcontain);
            }

        }

        private async static void OnAddSaveClick(object sender, EventArgs e)
        {            
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
                    tobechangeditem.PW = AESKEY.EncryptStringToBytes_Aes(DataOperation.PWChanging.InputedPW1);
                    tobechangeditem.Date = DateTime.UtcNow.ToString();
                    if ((await DataOperation.RealTimeDatabase.AddItemAsync(tobechangeditem)))
                    {
                        PageContain.Children.Clear();
                        RetrunListView();
                    }
                }
                else
                {
                    Label infolabe = new Label { Text = "It seems something is wrong with your input",
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    PageContain.Children.Add(infolabe);
                }
            }
            catch
            {
            }
        }

        public async static void CreateListItemView()
        {
            PageContain.Children.Clear();
            EntryList itemlist = new EntryList();
            itemlist.listview.ItemSelected += ItemSelected;
            itemlist.listview.ItemsSource = await DataOperation.GetData();
            Label infolabel = new Label { Text = "schoen dich zu sehen" };
            PageContain.Children.Add(itemlist.viewcontain);
        }

        private static void ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                Item selecteditem = args.SelectedItem as Item;
                PageContain.Children.Clear();

                EntryView entryview = new EntryView(selecteditem.NameofApplication, AESKEY.DecryptStringFromBytes_Aes(selecteditem.PW));
                entryview.leftbutton.Clicked +=   (sender, args) => { OnEditClick(sender, args, selecteditem); };
                entryview.rightbutton.Clicked += (sender, args) => { RetrunListView(); };
                PageContain.Children.Add(entryview.viewcontain);
            }
        }

        private static void OnEditClick(object sender, EventArgs e, Item selecteditem)
        {
            PageContain.Children.Clear();

            EditView editview = new EditView(selecteditem.NameofApplication);

            editview.leftbutton.Clicked += (sender, args) => { OnSaveClick(sender, args, selecteditem); };
            editview.rightbutton.Clicked += (sender, args) => { RetrunListView(); };
            editview.midbutton.Clicked += (sender, args) => { KeyGenEvent(false); };

            PageContain.Children.Add(editview.viewcontain);
        }

        private static void RetrunListView()
        {
            PageContain.Children.Clear();
            DataOperation.PWChanging = null;
            DataOperation.PWChanging = new EntryPW();
            CreateListItemView();
        }

        private async static void OnSaveClick(object sender, EventArgs e, Item selecteditem)
        {
            try
            {
                if (DataOperation.PWChanging != null &&
                    (((DataOperation.PWChanging.InputedPW1).Equals(DataOperation.PWChanging.InputedPW2)) &&
                    DataOperation.PWChanging.InputedPW1 != null) &&
                    DataOperation.PWChanging.InputedName != null &&
                    (!((DataOperation.PWChanging.InputedName).Equals("ThisApplication"))))
                {
                    Item tobechangeditem = new Item();
                    tobechangeditem.NameofApplication = DataOperation.PWChanging.InputedName;
                    tobechangeditem.PW = AESKEY.EncryptStringToBytes_Aes(DataOperation.PWChanging.InputedPW1);
                    tobechangeditem.Date = DateTime.UtcNow.ToString();
                    if ((await DataOperation.RealTimeDatabase.UpdateItemAsync(selecteditem, tobechangeditem)))
                    {
                        PageContain.Children.Clear();
                        RetrunListView();
                    }
                        
                }
                else
                {
                    Label infolabe = new Label
                    {
                        Text = "It seems something is wrong with your input",
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    PageContain.Children.Add(infolabe);
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
                LoginView loginview = new LoginView();
                loginview.loginbutton.Clicked += LoginEvent;
                PageContain.Children.Add(loginview.viewcontain);
            }
            else
            {
                RegView regview = new RegView();
                regview.regbutton.Clicked += OnSaveRegisterationClick;
                regview.keygenbutton.Clicked += (sender, args) => { KeyGenEvent(true); };
                PageContain.Children.Add(regview.viewcontain);
            }
        }

        public static void LoginEvent(object sender, EventArgs e)
        {           
            string encryloginpw;
            AESKEY.Set_AesKey(DataOperation.PWChanging.InputedPW1);
            encryloginpw = AESKEY.EncryptStringToBytes_Aes(DataOperation.PWChanging.InputedPW1);

            if (encryloginpw.Equals(DataOperation.AppPW) && DataOperation.AppPW != null)
            {
                DataOperation.LogedIN = true;
                RetrunListView();
            }
            else
            {
                Label infolabe = new Label { Text = "your passowrd is wrong" };
                PageContain.Children.Add(infolabe);
            }
        }

        public static void KeyGenEvent(bool warningneeded)
        {
            DataOperation.PWChanging.InputedPW1 = DataOperation.PWChanging.InputedPW2 
                                            = KeyGenerator.GeneratePassword();
            if(warningneeded)
            {
                PageContain.Children.Add((new KeyGenWarning()).viewcontain);
            }
        }

        private async static void OnSaveRegisterationClick(object sender, EventArgs e)
        {            
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
                    if( (await DataOperation.RealTimeDatabase.CreatAPPAccount(appacount)))
                    {
                        PageContain.Children.Clear();
                        DataOperation.LogedIN = true;
                        CreateListItemView();
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
            try
            {
                LogedIN = false;
                RealTimeDatabase = new FirebaseDataStore<Item>("Notes");
                Item thisapplication = RealTimeDatabase.GetAppPW().Result;
                AppPW = thisapplication == null ? null : thisapplication.PW;
                Console.WriteLine("what is wrong? {0}", AppPW);
                PWChanging = new EntryPW();
            }
            catch
            {
                Debug.WriteLine("dataoperation wrong");
            }
        }
        
        public async static Task<List<Item>> GetData()
        {
            return (await RealTimeDatabase.GetItemsAsync()).ToList();
        }

    }

    internal class EntryPW: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string inputedname;
        private string inputedpw1;
        private string inputedpw2;

        public string InputedName
        {
            get { return inputedname; }

            set
            {
                inputedname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedName"));
            }
        }
        public string InputedPW1
        {
            get { return inputedpw1; }

            set
            {
                inputedpw1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedPW1"));
            }
        }
        public string InputedPW2
        {
            get { return inputedpw2; }

            set
            {
                inputedpw2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedPW2"));
            }
        }
    }
}  