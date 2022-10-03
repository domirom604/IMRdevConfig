using Plugin.BLE.Abstractions.Contracts;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionList : ContentPage
    {

        string cryptography_key;
        string serial_number;
        string all;
        int item = 0;
        protected byte[] packetBytes = null;
        protected byte[] datagramBytes = null;

        public static ObservableCollection<Data>  ItemList { get; set; }
        public static int commandCount { get; set; }

        ObservableCollection<Data> itemList;
        public static IReadOnlyList<ICharacteristic> ConnectionCharachteristics { get; set; }
        IReadOnlyList<ICharacteristic> characteristics = null;
        bool connetion;
        int new_command_count = 0;
        int previev_command_count = 0;
        public int ivValue;
        public int preambleToEncryptData;
        public  ActionList(bool Connection, string serialNumber, string cryptographyKey)
        {
            actionOMB = getSelectedActionOMB();
            cryptography_key = cryptographyKey;
            serial_number = serialNumber;
            connetion = Connection;
            InitializeComponent();
            MylistTest.ItemsSource = itemList;
            OnAppearing();
            this.BindingContext = this;
            ivValue = 0;
            preambleToEncryptData = 0;

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
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MylistTest.ItemsSource = await App.Database.GetDataAsync();
        }

        public static ObservableCollection<Data> SetActionListParameters(ObservableCollection<Data> item_list)
        {
            ItemList = item_list;
            return ItemList;
            //addNewCOmmandToList();
            //itemList.Add(new Data { Name = ItemList.ToString() });
        }

        public int SetCountInformation()
        {
            return commandCount;

        }

        public async void Send(object sender, EventArgs e) // sending command!!!
        {
            ivValue++;
            preambleToEncryptData++;
            int count = 1; // bo put na action list 15A7 jest tylko 1


            WAN3.SetSelectedOMBparameter(actionOMB);
            packetBytes = WAN3.WP_PDU_ENCRYPTION(packetBytes, preambleToEncryptData, ivValue, count, true, serial_number, cryptography_key, true);
            datagramBytes = WAN3.WD_PDU_ENCRYPTION(packetBytes);
            byte[] dataToSend = new byte[datagramBytes.Length + packetBytes.Length];
            datagramBytes.CopyTo(dataToSend, 0);
            packetBytes.CopyTo(dataToSend, datagramBytes.Length);


            IReadOnlyList<ICharacteristic> characteristics = ConnectionCharachteristics;

            if (connetion == true)
            {
                await characteristics[0].WriteAsync(dataToSend);

                DisplayAlert("Alert!", "Action sent correctly!", "Ok");
            }
            else
            {
                DisplayAlert("Alert!", "No bluetooth connection!", "Ok");
            }

        }

        private async void ListActionTapingRecognizer(object sender, EventArgs e)
        {
            string name = ((sender as Button).BindingContext as Data).Name;
            string command_code = ((sender as Button).BindingContext as Data).command_code;
            string command_code_param_one = ((sender as Button).BindingContext as Data).command_code_param_one;
            string command_code_param_two = ((sender as Button).BindingContext as Data).command_code_param_two;
            string command_code_param_three = ((sender as Button).BindingContext as Data).command_code_param_three;

            String name_str = "Name: " + name;
            String command_code_str = "Command Code: " + command_code;
            String command_code_parOne_str = "Par1: " + command_code_param_one;
            String command_code_parTwo_str = "Par2: " + command_code_param_two;
            String command_code_parThree_str = "Par3: " + command_code_param_three;

            DisplayAlert("information:",name_str + "\n" + command_code_str + "\n" + command_code_parOne_str + "\n"+ command_code_parTwo_str + "\n" + command_code_parThree_str + "\n", "ok");

        }

        private void AddNewCommandEvent(object sender, EventArgs e)
        {
            
              Navigation.PushAsync(new NewCommandWindow(new_command_count));
                   
        }

        private async void ListView_Refreshing(object sender, EventArgs e)
        {
            MylistTest.ItemsSource = await App.Database.GetDataAsync();
            MylistTest.EndRefresh();
        }
        async void DeleteFromDatabase(int  id)
        {
            await App.Database.DeleteByIdAsync(id);
            MylistTest.ItemsSource = await App.Database.GetDataAsync();
        }
        private async void DeleteItemTapped(object sender, EventArgs e)
        {
            //Button selectedItem = (Button)sender;
            //var id = selectedItem.CommandParameter.ToString();
            string item = ((sender as Image).BindingContext as Data).DeleteImage;
            int Id = ((sender as Image).BindingContext as Data).Id;
            DeleteFromDatabase(Id);
            
            //string item = ((sender as Image).BindingContext as Data).DeleteImage;

            //Data listitem = (from itm in itemList
            //                 where itm.DeleteImage == item
            //                 select itm)
            //                .FirstOrDefault<Data>();
            //itemList.Remove(listitem);

            await DisplayAlert("Command", "Item deleted succeed!", "OK");
        }

        private void listViewAction_selected(object sender, SelectedItemChangedEventArgs e)
        {
            actionOMB.Clear();
            var it = MylistTest.SelectedItem;
            item = (it as Data).Id;
            string selectedSelectionName = (it as Data).Name;
            string selectedSelectionHexOfOMB = (it as Data).command_code;
            string selectedSelectionValueOne = (it as Data).command_code_param_one;
            string  selectedSelectionValueTwo = (it as Data).command_code_param_two;
            string selectedSelectionValueThree = (it as Data).command_code_param_three;
            if (MylistTest.SelectedItem == null)
            {
                return;
            }
            else
            {
                if (selectedSelectionHexOfOMB == "")
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = "" });
                }
                else
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = selectedSelectionHexOfOMB });
                }


                if (selectedSelectionValueOne == "")
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = "" });
                }
                else
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = selectedSelectionValueOne });
                }


                if (selectedSelectionValueTwo == "")
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = "" });
                }
                else
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = selectedSelectionValueTwo });
                }

                if (selectedSelectionValueThree == "")
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = "" });
                }
                else
                {
                    actionOMB.Add(new OMBname() { omb_name = selectedSelectionName, hexOfOMB = "05A7", omb_values = selectedSelectionValueThree });
                }

            }
          
        }

        public ObservableCollection<OMBname> actionOMB { get; set; }
        private ObservableCollection<OMBname> getSelectedActionOMB()
        {
            return new ObservableCollection<OMBname>
            {
            };
        }
    }
}

public class Data
{
    //public Data(string name)
    //{
    //    Name = name;
    //}
    [PrimaryKey, AutoIncrement]

    public int Id { get; set; }
    public string Name { get; set; }
    public string DeleteImage { get; set; }

    public string command_code { get; set; }

    public string command_code_param_one { get; set; }
    public string command_code_param_two { get; set; }
    public string command_code_param_three { get; set; }


}