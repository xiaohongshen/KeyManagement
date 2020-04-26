using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using KeyManagment.Views;
using System.Diagnostics;

namespace KeyManagment.Services
{
    class EntryList
    {
        public ListView listview { get; set; }
        public Frame frame { get; set; }
        public StackLayout viewcontain { get; set; }

        public EntryList()
        {
            listview = new ListView
            {                
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label namefofapplication = new Label();
                    namefofapplication.SetBinding(Label.TextProperty, "EntryItem.NameofApplication");

                    //Label password = new Label();
                    //password.SetBinding(Label.TextProperty, "PW");

                    BoxView boxView = new BoxView();
                    
                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                             {
                                    boxView,
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            namefofapplication,
                                           // password
                                        }
                                     }
                             }
                        }
                    };
                })
            };
            
            frame = new Frame
            {
                Content = new Label { Text = "schoen dich zu sehen" }
            };

            viewcontain = new StackLayout();

            viewcontain.Children.Add(listview);
            viewcontain.Children.Add(frame);
        }     
    }

    class EntryView
    {
        public StackLayout viewcontain { get; set; }

        public Button leftbutton { get; set; }
        public Button rightbutton { get; set; }


        public EntryView()
        {
            leftbutton = new Button { Text = "Edit",  HorizontalOptions = LayoutOptions.Center};
            rightbutton = new Button { Text = "GoBack", HorizontalOptions = LayoutOptions.Center };

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { },
                    new ColumnDefinition { },
                }
            };
            grid.Children.Add(leftbutton, 0, 0);
            grid.Children.Add(rightbutton, 1, 0);

            Label label4name = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold
            };
            label4name.SetBinding(Label.TextProperty, "InputedName", mode: BindingMode.TwoWay);
            label4name.BindingContext = DataOperation.WorkingItem;

            Label label4username = new Label();
            label4username.SetBinding(Label.TextProperty, "InputedUserName", mode: BindingMode.TwoWay);
            label4username.BindingContext = DataOperation.WorkingItem;

            Label label4pw = new Label();
            label4pw.SetBinding(Label.TextProperty, "InputedPW1", mode: BindingMode.TwoWay);
            label4pw.BindingContext = DataOperation.WorkingItem;

            Frame framlabel4name = new Frame
            {
                BorderColor = Color.Orange,
                CornerRadius = 10,
                HasShadow = true,

                Content = new StackLayout
                {
                    Children =
                    {
                        label4name,


                        new BoxView
                        {
                            Color = Color.Gray,
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.Fill
                        },

                        label4username,
                        label4pw,
                    }      
                }
            };
            Label infolabel = new Label
            {
                Text = "Click on Edit, you can modify the entry \n" +
                        "Click on GoBack, you will go back to the entry list"
            };
            viewcontain = new StackLayout();
            viewcontain.Children.Add(framlabel4name);
            viewcontain.Children.Add(grid); 
            viewcontain.Children.Add(infolabel);
        }
    }

    class EditView
    {
        public StackLayout viewcontain { get; set; }

        public Button leftbutton { get; set; }
        public Button midbutton { get; set; }
        public Button rightbutton { get; set; }

        public EditView()
        {
            leftbutton = new Button { Text = "Save", HorizontalOptions = LayoutOptions.Center };
            rightbutton = new Button { Text = "GoBack", HorizontalOptions = LayoutOptions.Center };
            midbutton = new Button { Text = "KeyGen", HorizontalOptions = LayoutOptions.Center };

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { },
                    new ColumnDefinition { },
                    new ColumnDefinition { }
                }
            };

            grid.Children.Add(leftbutton, 0, 0);
            grid.Children.Add(midbutton, 1, 0);
            grid.Children.Add(rightbutton, 2, 0);

            Entry entry4name = new Entry
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold
            };
            entry4name.SetBinding(Entry.TextProperty, "InputedName", mode: BindingMode.TwoWay);
            entry4name.BindingContext = DataOperation.WorkingItem;

            Entry entry4username = new Entry();
            entry4username.SetBinding(Entry.TextProperty, "InputedUserName", mode: BindingMode.TwoWay);
            entry4username.BindingContext = DataOperation.WorkingItem;

            Entry entry4pw1 = new Entry();
            entry4pw1.SetBinding(Entry.TextProperty, "InputedPW1", mode: BindingMode.TwoWay);
            entry4pw1.BindingContext = DataOperation.WorkingItem;

            Entry entry4pw2 = new Entry();
            entry4pw2.SetBinding(Entry.TextProperty, "InputedPW2", mode: BindingMode.TwoWay);
            entry4pw2.BindingContext = DataOperation.WorkingItem;

            Frame framlabel4name = new Frame
            {
                BorderColor = Color.Orange,
                CornerRadius = 10,
                HasShadow = true,

                Content = new StackLayout
                {
                    Children =
                    {
                        entry4name,

                        entry4username,

                        entry4pw1,

                        entry4pw2,

                        grid,

                        new BoxView
                        {
                            Color = Color.Gray,
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.Fill
                        },
                        new Label
                        {
                            Text = "Now you can modify the name and password of the related application"
                        }
                    }
                }
            };

            viewcontain = new StackLayout();
            viewcontain.Children.Add(framlabel4name);
        }
    }

    class LoginView
    {
        public StackLayout viewcontain { get; set; }

        public Button loginbutton { get; set; }

        public LoginView()
        {
            loginbutton = new Button { Text = "IN", HorizontalOptions = LayoutOptions.End,
                CornerRadius = 25,
                HeightRequest = 50,
                WidthRequest = 50,
            };

            Entry entry4key = new Entry
            {
                Placeholder = "pls give your key",
            };
            entry4key.SetBinding(Entry.TextProperty, "InputedPW1", mode: BindingMode.OneWayToSource);
            entry4key.BindingContext = DataOperation.WorkingItem;

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto  },
                    new ColumnDefinition { Width =new GridLength(40, GridUnitType.Star)}
                }
            };
            grid.Children.Add(entry4key, 0, 0);
            grid.Children.Add(loginbutton, 1, 0);

            Frame framlabel4name = new Frame
            {
                BorderColor = Color.Orange,
                CornerRadius = 10,
                HasShadow = true,

                Content = new StackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            Text = "Pls give the key word",
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center
                        },
                        new BoxView
                        {
                            Color = Color.Gray,
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.Fill
                        },

                        grid,
                    }
                }
            };
            Label infolabel = new Label
            {
                Text = "pls log in"
            };
            viewcontain = new StackLayout();
            viewcontain.Children.Add(framlabel4name);
            viewcontain.Children.Add(infolabel);
        }
    }

    class RegView
    {
        public StackLayout viewcontain { get; set; }

        public Button regbutton { get; set; }
        public Button keygenbutton { get; set; }

        public RegView()
        {
            regbutton = new Button { Text = "reg", HorizontalOptions = LayoutOptions.Center };
            //regbutton.Clicked += ViewCreator.LoginEvent;

            keygenbutton = new Button { Text = "key gen", HorizontalOptions = LayoutOptions.Center };
            //keygenbutton.Clicked += ViewCreator.KeyGenEvent;

            DataOperation.WorkingItem.InputedName = "ThisApplication";

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = 10},
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { },
                    new ColumnDefinition { },
                }
            };
            grid.Children.Add(regbutton, 0, 1);
            grid.Children.Add(keygenbutton, 1, 1);

            Entry entry4pw1 = new Entry
            {
                Placeholder = "pls give the key",
            };

            entry4pw1.BindingContext = DataOperation.WorkingItem;
            entry4pw1.SetBinding(Entry.TextProperty, "InputedPW1", mode: BindingMode.TwoWay);

            Entry entry4pw2 = new Entry
            {
                Placeholder = "pls give the key",
                //FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                //FontAttributes = FontAttributes.Bold
            };
            entry4pw2.SetBinding(Entry.TextProperty, "InputedPW2", mode: BindingMode.TwoWay);
            entry4pw2.BindingContext = DataOperation.WorkingItem;

            DataOperation.WorkingItem.InputedName = "ThisApplication";

            Frame framlabel4name = new Frame
            {
                BorderColor = Color.Orange,
                CornerRadius = 10,
                HasShadow = true,

                Content = new StackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            Text = "Key of This Application",
                            TextColor = Color.Black,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center
                        },

                        new BoxView
                        {
                            Color = Color.Gray,
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.Fill
                        },

                        entry4pw1,

                        entry4pw2
                    }
                }
            };

            viewcontain = new StackLayout();
            viewcontain.Children.Add(framlabel4name);
            viewcontain.Children.Add(grid);
            viewcontain.Children.Add(new Label
                               {
                                   Text = "pls reg your general key for this application"
                               });
        }
    }

    class KeyGenWarning
    {
        public StackLayout viewcontain { get; set; }

        public KeyGenWarning()
        {
            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = 10 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 5 },
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { },
                }
            };

            Label warning = new Label
            {
                Text = "REMEBER IT IMPLICITY",
                TextColor = Color.FromHex("#000080"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Label infolabe = new Label
            {
                Text = "This password will be used for de/encrypted your password - remember it implicitly.",
                HorizontalTextAlignment = TextAlignment.Center
            };

            grid.Children.Add(warning, 0, 1);
            grid.Children.Add(infolabe, 0, 3);
            viewcontain = new StackLayout();
            viewcontain.Children.Add(grid);
        }

    }

    class EntryAddView
    {
        public StackLayout viewcontain { get; set; }

        public Button leftbutton { get; set; }
        public Button midbutton { get; set; }
        public Button rightbutton { get; set; }


        public EntryAddView()
        {
            leftbutton = new Button { Text = "Save", HorizontalOptions = LayoutOptions.Center };
            rightbutton = new Button { Text = "GoBack", HorizontalOptions = LayoutOptions.Center };
            midbutton = new Button { Text = "KeyGen", HorizontalOptions = LayoutOptions.Center };

            Entry entry4name = new Entry
            {
                Placeholder = "pls give the name of application",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold
            };
            entry4name.SetBinding(Entry.TextProperty, "InputedName", mode: BindingMode.TwoWay);
            entry4name.BindingContext = DataOperation.WorkingItem;

            Entry entry4username = new Entry
            {
                Placeholder = "pls give the username",
            };
            entry4username.SetBinding(Entry.TextProperty, "InputedUserName", mode: BindingMode.TwoWay);
            entry4username.BindingContext = DataOperation.WorkingItem;

            Entry entry4pw1 = new Entry
            {
                Placeholder = "pls give new password "
            };
            entry4pw1.SetBinding(Entry.TextProperty, "InputedPW1", mode: BindingMode.TwoWay);
            entry4pw1.BindingContext = DataOperation.WorkingItem;

            Entry entry4pw2 = new Entry
            {
                Placeholder = "pls give new password again"
            };
            entry4pw2.SetBinding(Entry.TextProperty, "InputedPW2", mode: BindingMode.TwoWay);
            entry4pw2.BindingContext = DataOperation.WorkingItem;

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { },
                    new ColumnDefinition { },
                    new ColumnDefinition { }
                }
            };

            grid.Children.Add(leftbutton, 0, 0);
            grid.Children.Add(midbutton, 1, 0);
            grid.Children.Add(rightbutton, 2, 0);

            Frame framlabel4name = new Frame
            {
                BorderColor = Color.Orange,
                CornerRadius = 10,
                HasShadow = true,

                Content = new StackLayout
                {
                    Children =
                    {
                        entry4name,

                        new BoxView
                        {
                            Color = Color.Gray,
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.Fill
                        },
                        entry4username,

                        entry4pw1,

                        entry4pw2,

                        grid,
                    }
                }
            };

            Label infolabel = new Label
            {
                Text = "Now you can add a new entry"
            };

            viewcontain = new StackLayout();
            viewcontain.Children.Add(framlabel4name);
            viewcontain.Children.Add(infolabel);
        }
    }

    class UpdateFailedView
    {
        public StackLayout viewcontain { get; set; }
        public Button gobackbutton { get; set; }

        public UpdateFailedView()
        {
            Label warning = new Label
            {
                Text = "Entry Update Failed \n \n Pls try it late again \n \n",
                TextColor = Color.FromHex("#000080"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };

            gobackbutton = new Button 
            { 
                Text = "GoBack", 
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 80,
                CornerRadius = 20
            };
            viewcontain = new StackLayout();
            viewcontain.Children.Add(warning);
            viewcontain.Children.Add(gobackbutton);
        }
       
    }
}
