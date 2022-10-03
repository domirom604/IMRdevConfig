using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mobile_DEVICE_Config
{
    public partial class MainPage : ContentPage

{
        public static bool SetConnection { get; set; }
        public static string SetSerialNumber { get; set; }
        public static IDevice Device { get; set; }

        public static IReadOnlyList<ICharacteristic> ConnectionCharachteristics { get; set; }
        IReadOnlyList<ICharacteristic> characteristics = null;

        static bool set_scanned_serial_number = false;
        bool connection;
        string cryptographyKey = "AAAAAAAABBBBBBBBCCCCCCCCDDDDDDDD";
        string serialNumber = "01000000";
        public MainPage()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDg4OTE2QDMxMzkyZTMyMmUzMFVveGZOa29OSmdvYndIbk1xa3dMaUJhYjRHY1B5OG5tNXMyVlFzOHNTQWM9");
            InitializeComponent();
            MenuItems = GetMenus();
            DriversItems = GetDrivers();
            this.BindingContext = this;
            



        }
        public static void SetConnectionParameters(IReadOnlyList<ICharacteristic> connectionCharachteristics)
        {
            if (connectionCharachteristics != null)
            {
                ConnectionCharachteristics = connectionCharachteristics;
            }
            else
            {
                ConnectionCharachteristics = null;
            }

        }

        public static bool SetConnectionInformation()
        {
            return SetConnection;

        }
        public static string SetSerialNumberInformation(string serial)
        {
            SetSerialNumber = serial;
            set_scanned_serial_number = true;
            return SetSerialNumber;

        }
        public static void SetDeviceConnectionParameters(IDevice device)
        {
            Device = device;

        }


        public ObservableCollection<Menu> MenuItems { get; set; }
        public ObservableCollection<Drivers> DriversItems { get; set; }
        private ObservableCollection<Menu> GetMenus()
        {
            return new ObservableCollection<Menu>
            {
                new Menu { Title = "Connection", Icon = "wsk.png" },
                new Menu { Title = "Settings", Icon = "wsk.png" },
                new Menu { Title = "Firmware", Icon = "wsk.png" },
                new Menu { Title = "Plugins", Icon = "wsk.png" },
                new Menu { Title = "Action List", Icon = "wsk.png" }   
            };
          
        }

        public ObservableCollection<Drivers> GetDrivers()
        {
            return new ObservableCollection<Drivers>
            {
                new Drivers { Title = "RAYPULSE-R0x6" },
                new Drivers { Title = "RAYPAY-R0F6" },
                new Drivers { Title = "APULSExxx6-xxx1" },
                new Drivers { Title = "APULSEWx1x6-xxx1" }

            };

        }


        private async void Show()
        {
          _  = TitleText.FadeTo(0);
           _ = MenuItemsView.FadeTo(1);
           
            
           await MainMenuView.RotateTo(0, 300, Easing.BounceOut);
           await MainMenuViewTwo.RotateTo(0, 300, Easing.BounceOut);
        }

        private async void Hide()
        {
           _ = TitleText.FadeTo(1);
           _ = MenuItemsView.FadeTo(0);
            await MainMenuView.RotateTo(-90, 300, Easing.BounceOut);
            await MainMenuViewTwo.RotateTo(90, 300, Easing.BounceOut);

        }
        int countOfMenuTapped = 0;
        private void ShowMenu(object sender, EventArgs e)
        {
            countOfMenuTapped++;

            if (countOfMenuTapped % 2 == 0)
            {
                countOfMenuTapped = 0;
                Hide();

            }
            else 
            {
                Show();
            }
            
             
        }

        private void MenuTapped(object sender, EventArgs e)
        {
            countOfMenuTapped = 0;
            //TitleText.Text = ((sender as StackLayout).BindingContext as Menu).Title;
            Hide();
        }
        private void ShowBarecode(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BarecodePage());
            //int jestem = 0;
            //SerialNumberText.Text = serialNumber;
           // serialNumber = SetSerialNumberInformation();
        }

            private void ItemTapped(object sender, EventArgs e)
            {
            try
            {
                var item = ((sender as StackLayout).BindingContext as Menu).Title;
                
                switch (item)
                {
                   

                    case "Connection":
                        {
                            if (SetConnectionInformation() == true)
                            {
                                IDevice device = Device;
                                 connection = SetConnectionInformation();
                            }
                            else {  connection = false; }
                            Navigation.PushAsync(new Connection(connection , Device));
                        }
                        break;

                    case "Settings":
                        {
                            Navigation.PushAsync(new Settings());
                        }
                        break;

                    case "Firmware":
                        {
                            string selectedDriver = DriverSelected();
                            if (SetConnectionInformation() == true)
                            {
                                connection = SetConnectionInformation();
                            }
                            else
                            {
                                connection = false;
                            }
                            if (selectedDriver != null)
                            {
                                Navigation.PushAsync(new Firmware(connection));
                            }
                            else
                            {
                                Navigation.PushAsync(new Plugins(selectedDriver));
                            }
                        }
                        break;

                    case "Plugins":
                        {
                            string selectedDriver = DriverSelected();
                            if (SetConnectionInformation() == true)
                            {
                                connection = SetConnectionInformation();
                            }
                            else
                            {
                                connection = false;
                            }
                            if (selectedDriver == "APULSExxx6-xxx1")
                            {
                                if (SetSerialNumber != null)
                                {
                                    //serialNumber = SetSerialNumber;
                                }
                                Navigation.PushAsync(new Apulse_xxx6_xxx1(selectedDriver, cryptographyKey, serialNumber,connection));
                            }
                            else
                            {

                                Navigation.PushAsync(new Plugins(selectedDriver));
                            }
                           
                               
                           
                        }
                        break;

                    case "Action List":
                        {
                            if (SetConnectionInformation() == true)
                            {
                                connection = SetConnectionInformation();
                            }
                            else
                            {
                                connection = false;
                            }
                            Navigation.PushAsync(new ActionList(connection, serialNumber, cryptographyKey));
                        }
                        break;

                }
            }
            catch { }
        }

        
        public string DriverSelected()
        {

            int selectedIndex = comboBoxSelectedItem.SelectedIndex;
           // Object selectedItem = comboBoxSelectedItem.SelectedItem;

           if(selectedIndex == -1)
            {
                DisplayAlert("Error!", "No driver selected!", "ok");
            }

            string selectedDriver = DriversItems[selectedIndex].Title.ToString();
             DisplayAlert("LoadDriver Succes!", selectedDriver, "ok");
            return selectedDriver;
        }

      
        private void cryptographyKeyEntry(object sender, TextChangedEventArgs e)
        {
            cryptographyKey = ((Entry)sender).Text;
           
            
        }

        private async void TryGetClicked(object sender, EventArgs e)
        {
            set_scanned_serial_number = false;
            IReadOnlyList<ICharacteristic> characteristics = ConnectionCharachteristics;

            connection = SetConnectionInformation();
           
                if (connection == true)
                {
                    
                    this.IsBusy = true;
                    byte[] dataToSend = WAN3.Try_Get_Sn();
                
                    await characteristics[0].WriteAsync(dataToSend); // sending data to device
                



                    byte[] receivedData = { };

                    if (characteristics[1].CanRead)
                    {
                        receivedData = await characteristics[1].ReadAsync();

                    }

                    string SerialAfterGet = "";

                    int[] table = new int[4];

                    table[0] = Convert.ToInt32(receivedData[10]);
                    table[1] = Convert.ToInt32(receivedData[9]);
                    table[2] = Convert.ToInt32(receivedData[8]);
                    table[3] = Convert.ToInt32(receivedData[7]);

                    SerialAfterGet = "0" + table[0].ToString("X") + table[1].ToString("X") + table[2].ToString("X") + table[3].ToString("X");
                    SerialNumberText.Text = SerialAfterGet;
                    this.IsBusy = false;
                    await DisplayAlert("Sn:", SerialAfterGet + "\n", "Ok");

                    //DisplayAlert("information:", name_str + "\n" + command_code_str + "\n" + command_code_parOne_str + "\n" + command_code_parTwo_str + "\n" + command_code_parThree_str + "\n", "ok");
                    SetSerialNumber = SerialAfterGet;
                    
                }

                else
                {
                    await DisplayAlert("Warning", "Device send data Error! No connection with device!", "Ok");
                }
            
           

        }

        private void serialNumberEntry(object sender, EventArgs e)
        {
            serialNumber = ((Entry)sender).Text;
            if (SetSerialNumber != null)
            {
                if (serialNumber!=null && set_scanned_serial_number!=true)
                {
                    SerialNumberText.Text = serialNumber;
                }
                if(set_scanned_serial_number==true)
                {
                    SerialNumberText.Text = SetSerialNumber;
                    set_scanned_serial_number = false;
                }
               
            }
            else
            {
                SerialNumberText.Text = serialNumber;
            }
            
        }

        
    }

    
}
public class Menu
{
    public string Title { get; set; }

    public string Icon { get; set; }
}

