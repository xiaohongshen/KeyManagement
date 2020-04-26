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
                    toolbaritem.Clicked += OnAddClicked;
                    break;
            }
            return toolbaritem;
        }

        public static void OnAddClicked(object sender, EventArgs e)
        {
            if (DataOperation.LogedIN)
            {
                PageContain.Children.Clear();
                DataOperation.WorkingItem = new DatainWork();
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
                if (DataOperation.WorkingItem != null &&
                    (((DataOperation.WorkingItem.InputedPW1).Equals(DataOperation.WorkingItem.InputedPW2)) &&
                    DataOperation.WorkingItem.InputedPW1 != null) &&
                    DataOperation.WorkingItem.InputedName != null &&
                    DataOperation.WorkingItem.InputedUserName != null &&
                    (!((DataOperation.WorkingItem.InputedName).Equals("ThisApplication"))))
                {
                    Item newitem = new Item
                    {
                        NameofApplication = DataOperation.WorkingItem.InputedName,
                        UserName = AESKEY.EncryptStringToBytes_Aes(DataOperation.WorkingItem.InputedUserName),
                        PW = AESKEY.EncryptStringToBytes_Aes(DataOperation.WorkingItem.InputedPW1),
                        Date = DateTime.UtcNow.ToString()
                    };
                    if ((await DataOperation.RealTimeDatabase.AddItemAsync(newitem)))
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
                EntryData selecteditem = args.SelectedItem as EntryData;
                DataOperation.WorkingItem.DecryptEntry(args.SelectedItem as EntryData);
                PageContain.Children.Clear();

                EntryView entryview = new EntryView();
                entryview.leftbutton.Clicked += OnEditClick;
                entryview.rightbutton.Clicked += (sender, args) => { RetrunListView(); };
                PageContain.Children.Add(entryview.viewcontain);                
            }
        }

        private static void OnEditClick(object sender, EventArgs e)
        {
            PageContain.Children.Clear();
 
            EditView editview = new EditView();

            editview.leftbutton.Clicked += OnSaveClick;
            editview.rightbutton.Clicked += (sender, args) => { RetrunListView(); };
            editview.midbutton.Clicked += (sender, args) => { KeyGenEvent(false); };
            PageContain.Children.Add(editview.viewcontain);
        }

        private static void RetrunListView()
        {
            PageContain.Children.Clear();
            DataOperation.WorkingItem = new DatainWork();
            CreateListItemView();
        }

        private async static void OnSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (DataOperation.WorkingItem != null &&
                    (((DataOperation.WorkingItem.InputedPW1).Equals(DataOperation.WorkingItem.InputedPW2)) &&
                    DataOperation.WorkingItem.InputedPW1 != null) &&
                    DataOperation.WorkingItem.InputedName != null &&
                    (!((DataOperation.WorkingItem.InputedName).Equals("ThisApplication"))))
                {
                    PageContain.Children.Clear();
                    DataOperation.WorkingItem.EncryptEntry();
                    if ((await DataOperation.RealTimeDatabase.UpdateItemAsync(DataOperation.WorkingItem.EntryinWork)))
                    {  
                        RetrunListView();
                    }
                    else
                    {
                        UpdateFailedView updateFailed = new UpdateFailedView();
                        updateFailed.gobackbutton.Clicked += (sender, args) => { RetrunListView(); };
                        PageContain.Children.Add(updateFailed.viewcontain);
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
            AESKEY.Set_AesKey(DataOperation.WorkingItem.InputedPW1);
            encryloginpw = AESKEY.EncryptStringToBytes_Aes(DataOperation.WorkingItem.InputedPW1);

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
            DataOperation.WorkingItem.InputedPW1 = DataOperation.WorkingItem.InputedPW2 
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
                if (DataOperation.WorkingItem != null &&
                    (((DataOperation.WorkingItem.InputedPW1).Equals(DataOperation.WorkingItem.InputedPW2)) &&
                    DataOperation.WorkingItem.InputedPW1 != null) && 
                    (DataOperation.WorkingItem.InputedName.Equals("ThisApplication"))) 
                {
                    Console.WriteLine("what is wrong? {0}", DataOperation.WorkingItem.InputedName.Equals("ThisApplication").ToString());
                    AESKEY.Set_AesKey(DataOperation.WorkingItem.InputedPW1);
                    Item appacount = new Item 
                    { 
                        NameofApplication = DataOperation.WorkingItem.InputedName,
                        PW = AESKEY.EncryptStringToBytes_Aes(DataOperation.WorkingItem.InputedPW1),
                        Date = DateTime.UtcNow.ToString()
                    };
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
        public static FirebaseDataStore RealTimeDatabase { get; private set; }

        public static DatainWork WorkingItem { get; set; }

        public static String AppPW { get; set; }

        /*I don't like this solution
         * need the property to enable add itme event
         */
        public static bool LogedIN { get; set; }

        //public async static void Init_DataOperation()
        public DataOperation()
        {
            LogedIN = false;
            RealTimeDatabase = new FirebaseDataStore("Notes");
            Item thisapplication = RealTimeDatabase.GetAppPW().Result;
            AppPW = thisapplication == null ? null : thisapplication.PW;
            WorkingItem = new DatainWork();
        }
        
        public async static Task<List<EntryData>> GetData()
        {
            return (await RealTimeDatabase.GetItemsAsync()).ToList();
        }
    }

    internal class DatainWork : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public EntryData EntryinWork { get; set; }

        private string verifypw;

        public DatainWork()
        {
            EntryinWork = new EntryData();
            InputedPW2 = "";
        }

        public void DecryptEntry(EntryData entrydata)
        {
            EntryinWork = entrydata;
            InputedUserName = AESKEY.DecryptStringFromBytes_Aes(InputedUserName);
            InputedPW1 = AESKEY.DecryptStringFromBytes_Aes(InputedPW1);
        }

        public void EncryptEntry()
        {
            InputedUserName = AESKEY.EncryptStringToBytes_Aes(InputedUserName);
            InputedPW1 = AESKEY.EncryptStringToBytes_Aes(InputedPW1);
        }

        /*the following code was implemented during privous development, 
         * to save code change and 
         * save long path in databinding the names of the properties are kept
         */

        public string InputedName
        {
            get { return EntryinWork.EntryItem.NameofApplication; }
            set 
            { 
                EntryinWork.EntryItem.NameofApplication = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedName"));
            }
        }
        public string InputedPW1
        {
            get { return EntryinWork.EntryItem.PW; }
            set 
            { 
                EntryinWork.EntryItem.PW = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedPW1"));
            }
        }
        public string InputedUserName
        {
            get { return EntryinWork.EntryItem.UserName; }
            set 
            { 
                EntryinWork.EntryItem.UserName = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedUserName"));
            }
        }
        public string InputedPW2
        {
            get { return verifypw; }

            set
            {
                verifypw = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputedPW2"));
            }
        }
    }
}  