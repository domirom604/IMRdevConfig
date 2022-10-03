using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE;
using System.Globalization;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Connection : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        IDevice device;
        bool connetion;
        public ObservableCollection<IDevice> deviceList;
        public List<Device_Scanning> list = new List<Device_Scanning>();


         
        public Connection(bool COnnection, IDevice Devicee)
        {
            InitializeComponent();
            deviceNames = GetDeviceName();
            rssiValue = GetRssiValue();
           
            this.BindingContext = this;

            connetion = COnnection;
            if(Devicee!=null)
            {
                device = Devicee;
            }
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;



            deviceList =  new  ObservableCollection<IDevice>();
            this.IsBusy = false;
            //listViewDevices.ItemsSource = deviceList;
           if(connetion == true)
           {
                DisconnectButton.BackgroundColor = Color.FromHex("#2981e1"); 
                ConnectButton.BackgroundColor = Color.FromHex("#1C61AC");
                ConnectonInformationText.Text = "True";
                ConnectonInformationText.TextColor = Color.Green;
           }
           else 
           {
                DisconnectButton.BackgroundColor = Color.FromHex("#1C61AC"); 
                ConnectButton.BackgroundColor = Color.FromHex("#2981e1");
           }

        }
       

        public List<Device_Scanning> deviceNames { get; set; }
        public ObservableCollection<Device_Scanning> rssiValue { get; set; }

        private List<Device_Scanning> GetDeviceName()
        {
            return new List<Device_Scanning>
            {
                new Device_Scanning { device_name =" Tap scaning button!" },
               
            };
        }

        private ObservableCollection<Device_Scanning> GetRssiValue()
        {
            return new ObservableCollection<Device_Scanning>
            {
                new Device_Scanning { rssi_value = 0 },
              
            };
        }
        

        private async void Scaning_button_tapped(object sender, EventArgs e)
        {
            this.IsBusy = true;
            deviceNames.Clear();
            deviceList.Clear();
            rssiValue.Clear();
            if (ble.State == BluetoothState.Off)
            {
                await DisplayAlert("Alert!", "Bluetooth disconnected.", "OK");
            }
            else
            {
                adapter.DeviceDiscovered += (s, a) =>
                {
                // var k = a.Device;
                if (a.Device != null)
                    {

                        string Device = "";
                        Device = a.Device.ToString();


                        if (Device != null)
                        {
                            deviceList.Add(a.Device);
                        //IDevice device = a.Device as IDevice;
                        string mac_address = a.Device.NativeDevice.ToString().PadLeft(18, ' ');
                            string device_name = a.Device.ToString().PadLeft(27, ' ');
                            string rssi_valuee = a.Device.Rssi.ToString().PadLeft(6, ' ');
                            int rssi_value = Convert.ToInt32(rssi_valuee);

                            string device_id = a.Device.Id.ToString().PadLeft(27, ' ');
                            string all = device_name;


                            if (deviceNames.Any(p => p.device_name == all) == false)
                            {

                                deviceNames.Add(new Device_Scanning() { device_name = all, rssi_value = rssi_value, device_connection_property = a.Device });

                                var sortableList = new List<Device_Scanning>(deviceNames);

                                sortableList.Sort(new DeviceComparer());
                                deviceNames = sortableList;
                                listViewDevices.ItemsSource = null;
                                listViewDevices.ItemsSource = deviceNames;
                            //for (int i = 0; i < sortableList.Count; i++)
                            //{
                            //    deviceNames.Move(deviceNames.IndexOf(sortableList[i]), i);

                            //}

                            }


                        }
                    }

                };
                if (!ble.Adapter.IsScanning)
                {
                    //adapter.ScanMode= ;
                    await adapter.StartScanningForDevicesAsync();

                }
            }
            this.IsBusy = false;
        }




        private async void Clear_button_tapped(object sender, EventArgs e)
        {
            if (!ble.Adapter.IsScanning)
            {
                await adapter.StopScanningForDevicesAsync();
            }
            deviceNames.Clear();
            deviceList.Clear();
            rssiValue.Clear();
        }

       
        IService Service;

        
        IBluetoothLE bluetoothLE;

        private async void Connect_button_clicked(object sender, EventArgs e)
        {
            this.IsBusy = true;
           
            try
            {
                if(device!=null && connetion==false)
                {
                   
                    await adapter.ConnectToDeviceAsync(device);
                    var services = await device.GetServicesAsync();
                    var service = await device.GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                    //var service2 = await device.GetServiceAsync(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"));
                    
                    if (service != null && services.Count==5)
                    {
                        this.IsBusy = false;
                        Button btn = (Button)sender;
                        btn.BackgroundColor = Color.FromHex("#1C61AC");
                        DisconnectButton.BackgroundColor = Color.FromHex("#2981e1");
                        ConnectonInformationText.Text = "True";
                        ConnectonInformationText.TextColor = Color.Green;
                        IReadOnlyList <ICharacteristic> characteristics = await service.GetCharacteristicsAsync();
                        IReadOnlyList<ICharacteristic> characteristicsOne = await services[0].GetCharacteristicsAsync();
                        IReadOnlyList<ICharacteristic> characteristicsTwo = await services[1].GetCharacteristicsAsync();
                        IReadOnlyList<ICharacteristic> characteristicSecondOne = await services[2].GetCharacteristicsAsync();
                        IReadOnlyList<ICharacteristic> characteristicSecondTwo= await services[3].GetCharacteristicsAsync();
                        IReadOnlyList <ICharacteristic> charactersisticToChangeFirmware = await services[4].GetCharacteristicsAsync();


                        DisplayAlert("", "Device connected correctly!", "Ok");
                        connetion = true;
                        MainPage.SetConnection=connetion;
                        MainPage.SetConnectionParameters(characteristics);
                        Apulse_xxx6_xxx1.SetConnectionParameters(characteristics);
                        ActionList.SetConnectionParameters(characteristics);
                        BleCommands.SetConnectionParameters(charactersisticToChangeFirmware);
                        Firmware.SetConnectionParameters(charactersisticToChangeFirmware);
                        MainPage.SetDeviceConnectionParameters(device);


                        //Plugin.BLE.Android.Characteristic characteristic = characteristics[0];

                        //Plugin.BLE.Abstractions.CharacteristicBase characteristicBase = (Plugin.BLE.Abstractions.CharacteristicBase)characteristics;

                        //apulse_xxx6_xxx1.ConnectionCharachteristics = characteristicBase;

                        //var characteristics2 = await service2.GetCharacteristicsAsync();

                        //string hexString = "A20D0023FA447A1F000000D1C2F05C619325";

                        //byte[] senddata = new byte[hexString.Length / 2];
                        //for (int index = 0; index < senddata.Length; index++)
                        //{
                        //    string byteValue = hexString.Substring(index * 2, 2);
                        //    senddata[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        //}

                        ////data sending


                        //int start = 0;
                        //while (start < senddata.Length)
                        //{
                        //    int chunkLength = Math.Min(24, senddata.Length - start);
                        //    byte[] chunk = new byte[chunkLength];
                        //    Array.Copy(senddata, start, chunk, 0, chunkLength);





                        //    await characteristics[0].WriteAsync(chunk);
                        //    start += 31;
                        //}




                        //byte[] localData = { };
                        //if (characteristics[1].CanRead)
                        //{
                        //    localData = await characteristics[1].ReadAsync();
                        //}

                        //start = 2;

                    }
                    

                    //Services = await device.GetServicesAsync();


                    //var service = await device.GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                    //var TxCharacteristic = await service.GetCharacteristicAsync(Guid.Parse("e400002-b5a3-f393-e0a9-e50e24dcca9e"));
                    //var RxCharacteristic = await service.GetCharacteristicAsync(Guid.Parse("e400002-b5a3-f393-e0a9-e50e24dcca9e"));

                    //string bb = TxCharacteristic.ToString();
                    //var characteristics = await service.GetCharacteristicsAsync();
                    //var bytes = await characteristic.ReadAsync();




                }
                else
                {
                    connetion = false;
                    DisplayAlert("Alert!", "No device selected!", "Ok");
                    this.IsBusy = false;

                }
                
            }
           
            catch 
            {
                DisplayAlert("Alert!", "Check ble of the device!", "Ok");
                this.IsBusy = false;
            }
        }

        private async void Disconnect_button_clicked(object sender, EventArgs e)
        {
            if (device!= null && connetion == true)
            {

                await adapter.DisconnectDeviceAsync(device);
                ConnectButton.BackgroundColor = Color.FromHex("#2981e1");
                DisconnectButton.BackgroundColor = Color.FromHex("#1C61AC");
                ConnectonInformationText.Text = "False";
                ConnectonInformationText.TextColor = Color.Red;
                connetion = false;
                MainPage.SetConnection = connetion;
            }

           
        }
        private void listViewDevices_selected(object sender, SelectedItemChangedEventArgs e)
        {
            // tutaj wszystkie kolekcje zostały zamienione na listy !!!!
            List<Device_Scanning> obStrings = new List<Device_Scanning>();
            int index = (listViewDevices.ItemsSource as List<Device_Scanning>).IndexOf(e.SelectedItem as Device_Scanning);
            
            if (listViewDevices.SelectedItem ==null)
            {
                return;
            }
            string a = listViewDevices.SelectedItem.ToString();

            device = deviceNames[index].device_connection_property;
        }

        public async Task<List<Device_Scanning>> GetScaningDevicesByName(string deviceName)
        {
            var searchedList = new List<Device_Scanning>(deviceNames);
          
            
            searchedList  = deviceNames.Where(x => x.device_name.ToLower().Contains(deviceName.ToLower())).ToList();
            searchedList.Sort(new DeviceComparer());
            //int difference = count_of_list - searchedList.Count();
            ////deviceNames.Clear();
            //if (difference != count_of_list)
            //{
            //    for (int i = 0; i < difference-1; i++)
            //    {
            //        deviceNames.RemoveAt(i);
            //    }
            //    for (int i = 0; i < searchedList.Count; i++)
            //    {

            //        deviceNames.Move(deviceNames.IndexOf(searchedList[i]), i);

            //    }
            //}
            

            return searchedList;
        }
        private async void  TextSearchButton_clicked(object sender, EventArgs e)
        {
            string SearchedText = TextSearch.Text;

            if(!String.IsNullOrEmpty(SearchedText))
            {
                //int countOfList = deviceNames.Count();
                var deviceNamess =await GetScaningDevicesByName(SearchedText);
                listViewDevices.ItemsSource = null;
                listViewDevices.ItemsSource = deviceNamess;
            }
            else
            {
                var deviceName = deviceNames;
                listViewDevices.ItemsSource = null;
                listViewDevices.ItemsSource = deviceName;
            }
            //int a;
            //if (sender is TextChangedEventArgs args)
            //{
            //    string filter = args.NewTextValue;
            //    var SearchedCars = deviceNames.Where(x => x.device_name.ToLower().Contains(filter.Trim().ToLower())).ToList();
            //}
        }
    }
}

public class Device_Scanning
{
    public string device_name { get; set; }
    public int rssi_value { get; set; }
    public IDevice  device_connection_property {get; set;}
    public override bool Equals(object obj)
    {
        return obj is Device_Scanning scanning &&
               device_name == scanning.device_name &&
               rssi_value == scanning.rssi_value;
    }

    public override int GetHashCode()
    {
        int hashCode = 328178615;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(device_name);
        hashCode = hashCode * -1521134295 + rssi_value.GetHashCode();
        return hashCode;
    }
}

public class DeviceComparer : IComparer<Device_Scanning>
{
    public int Compare(Device_Scanning x, Device_Scanning y)
    {
        return y.rssi_value.CompareTo(x.rssi_value);
    }
}