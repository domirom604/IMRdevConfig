using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Apulse_xxx6_xxx1 : ContentPage
    {
        string cryptography_key;
        string serial_number;
        //public byte package_sent_preamble = 0xA2;
        //byte package_sent_preambleValue = 162;
        protected byte[] packetBytes = null;
        protected byte[] datagramBytes = null;
        bool allowColorChange = false;
        //protected byte[] datagramLayerBytes = null;
        
       
        
        //public byte package_receive_preamble = 0xA0;
        //byte package_receive_preambleValue = 160;
        public int ivValue;
        public int preambleToEncryptData;
       
        bool connetion;
      
        //byte[] headerAndDataBytesCopy = new byte[1]; // tu coś jest nie tak


        ObservableCollection<OMBname> itemsToChangeAfterGetCommand;
        public static IReadOnlyList<ICharacteristic> ConnectionCharachteristics { get; set; }
        IReadOnlyList<ICharacteristic> characteristics = null;
        public Apulse_xxx6_xxx1(string selectedDriver, string cryptographyKey, string serialNumber,bool COnnection)
        {

            connetion = COnnection;
            selectedOMB = getselectedOMBname();

            accelerometrOMBname = getMainListAccelerometrOMBname();
            accelerometrReadoutOMBname = getMainListAccelerometrReadoutOMBname();

            counterOMBname = getMainListCounterOMBname();


            bleOMBname = getMainListBleOMBname();

            userconfigOMBname = getMainListUserconfigOMBname();

            archiveOMBname = getMainListArchiveOMBname();

            configurationOMBname = getMainListConfigurationOMBname();

            datalinksOMBname = getMainListDatalinksOMBname();

            debugOMBname = getMainListDebugOMBname();

            flashOMBname = getMainListFlashOMBname();

            powerOMBname = getMainListPowerOMBname();

            rWmbusOMBname = getMainListRwmbusOMBname();

            radioOMBname = getMainListRadioOMBname();

            radioWmbusOMBname = getMainListRadioDeviceWmbusOMBname();

            rtcOMBname = getMainListRtcOMBname();

            smopOMBname = getMainListSmopOMBname();

            temperatureOMBname = getMainListTemperatureOMBname();

            wan3OMBname = getMainListWan3OMBname();

            wmbusOMBname = getMainListWmbusOMBname();

            listOfCheckedElement = getMainListOfCheckedElement();
           
            

            itemsToChangeAfterGetCommand = new ObservableCollection<OMBname>();

            InitializeComponent();

            this.BindingContext = this;

            string selected_driver = selectedDriver;
            string information_about_loadeddriver = selected_driver;
            TitleTextLoadedDriver.Text = information_about_loadeddriver;

            cryptography_key = cryptographyKey;
            serial_number = serialNumber;



            ivValue = 0;
            preambleToEncryptData = 0;


            //RefreshData();
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

        private void changed(object sender, EventArgs e)
        {

            
            Entry isEnteredOrNot = (Entry)sender;
            var selectedSelectionEntry = isEnteredOrNot.BindingContext as OMBname;
            if (selectedSelectionEntry != null)
            {
                int selectedSelectionEntryAccess = selectedSelectionEntry.read_write_access;
                var entry = sender as Entry;

            }
            else
            {

            } 
        }

        #region conversion to prepare selected values to fill in
        private void ConversionToFillSelectedValues(byte[] dataSelectedTemplate, byte[] dataSelectedTemplateWithValues, int selectedOMBvalue)
        {
            dataSelectedTemplateWithValues = dataSelectedTemplateWithValues.Skip(1).ToArray();
            //pomijam z uwagi na to że pierwszy element to suma kontrolna czyli nic ciekawego (do pominięci)

            byte arrayTofindZero;
            byte arrayTofindOne;


            int[] indexTable = new int[dataSelectedTemplate.Length+1];
            int[] lengthTable = new int[dataSelectedTemplate.Length/2];

            byte insideSearchArrayZero;
            byte insideSearchArrayOne;


            int i = 0;
            int searchingCount = 0;
            // finding values operation -  to selected element to fill in
            while (i< dataSelectedTemplateWithValues.Length)
            {
                insideSearchArrayZero = dataSelectedTemplateWithValues[i];
                insideSearchArrayOne = dataSelectedTemplateWithValues[i+1];

                arrayTofindZero = dataSelectedTemplate[searchingCount];
                arrayTofindOne = dataSelectedTemplate[searchingCount+1];

                if (insideSearchArrayZero == arrayTofindZero && insideSearchArrayOne == arrayTofindOne)
                {
                    indexTable[searchingCount] = i;
                    indexTable[searchingCount+1] = i+1;
                    searchingCount += 2;
                }
                else
                {
                    if (i< dataSelectedTemplateWithValues.Length)
                    {
                        i += 1;
                    }
                    else
                    {
                        break;
                    }

     
                }
                if (searchingCount == dataSelectedTemplate.Length)
                {
                    break;
                }

            }
            indexTable[indexTable.Length - 1] = dataSelectedTemplateWithValues.Length;

            int element = 1;
            for(int z =0; z< lengthTable.Length;z++)
            {
                lengthTable[z] = indexTable[element + 1] - indexTable[element];
                element += 2;
            }

            int next2 = 1;
            int k  = indexTable[next2]+1;
            int valueTofillinElement = 0;
            byte[] valueTofillin = new byte[lengthTable.Length];
            UInt32 [] intValuesTofillin = new UInt32[selectedOMBvalue];

            for (int j =0;j<selectedOMBvalue ;j++)
            {
                byte[] valueToCOnvertToBigendian = new byte[lengthTable[j]-1];
                int length = 0;
                while (length < lengthTable[j]-1)
                {
                    valueTofillin[valueTofillinElement] = dataSelectedTemplateWithValues[k];
                    valueToCOnvertToBigendian[length] = valueTofillin[valueTofillinElement];
                    k++;
                    length++;
                }

                intValuesTofillin[j] = WAN3.byteTableToInt(valueToCOnvertToBigendian);
                // konwersja big endiana
                next2 += 2 ;
                if(next2< indexTable.Length)
                {
                    k = indexTable[next2] + 1;
                }
                else
                {
                    break;
                }

                
                valueTofillinElement++;

            }

            byte[] arrayToconvert = new byte[2];
            int twobytes = 0;
            int numberValue = 0;
            for (int x = 0; x <= dataSelectedTemplate.Length; x++)
            {
                if (twobytes%2 == 0 && twobytes!=0)
                {
                    string text = twoByteTabletoString(arrayToconvert);
                    itemsToChangeAfterGetCommand.Add(new OMBname { hexOfOMB =  text, valuesToChangeAfterGet =(intValuesTofillin[numberValue]).ToString() } );
                    numberValue++;
                    twobytes = 0;
                }
                if(x == dataSelectedTemplate.Length)
                {
                    break;
                }
                else
                {
                    arrayToconvert[twobytes] = dataSelectedTemplate[x];
                }
               
                twobytes++;

            } 

            }

        #endregion

        #region fill OMB
        void fillGetOMB(ObservableCollection<OMBname> itemToFillin, ObservableCollection<OMBname> selectedOMB)
        {
            
            int elementNumber = itemToFillin.Count();
            for (int i = 0; i < elementNumber; i++)
            {
                int kk = 0;
                if (selectedOMB[i].treeNumber == 0)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in accelerometrOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = accelerometrOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool) )
                    {
                        if(itemToFillin[i].valuesToChangeAfterGet=="0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }
                    if (typeOfOMB == typeof(int))
                    {
                       data  = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }
                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }


                    listItems.omb_values = data;
                    listItems.colors = Color.Green;

                }
                if (selectedOMB[i].treeNumber == 1)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in accelerometrReadoutOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = accelerometrReadoutOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }


                    listItems.omb_values = data;
                    listItems.colors = Color.Green;

                }
                if (selectedOMB[i].treeNumber == 2)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in archiveOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = archiveOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;

                }
                if (selectedOMB[i].treeNumber == 3)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in bleOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = bleOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }
                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(string))
                    {
                        if(selectedOMB[i].stringType =="mac")
                        {
                           // data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                        }
                        
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;


                }

                if (selectedOMB[i].treeNumber == 16)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in userconfigOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = userconfigOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }
                    int value = Convert.ToInt32(data);
                    data = DecimalToHexadecimal(value);
                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }

                if (selectedOMB[i].treeNumber == 5)
                {
                    //counter !!!

                }
                if (selectedOMB[i].treeNumber == 6)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in datalinksOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = datalinksOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 7)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in debugOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = debugOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 8)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in flashOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = flashOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }



                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 9)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in powerOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = powerOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    // brakuje rozszycia double

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 10)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in rWmbusOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = rWmbusOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    
                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 11)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in radioOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = radioOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    
                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(string))
                    {
                        if (selectedOMB[i].stringType == "time")
                        {
                            // data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                        }

                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 12)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in radioWmbusOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = radioWmbusOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 13)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in rtcOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = rtcOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(string))
                    {
                        if (selectedOMB[i].stringType == "time")
                        {
                            // data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                        }

                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 14)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in rtcOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = rtcOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    
                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 15)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in temperatureOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = temperatureOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 4)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in configurationOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = configurationOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;

                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }
                    listItems.omb_values = data;
                    listItems.colors = Color.Green;


                }
                if (selectedOMB[i].treeNumber == 17)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in wan3OMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = wan3OMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                    if (typeOfOMB == typeof(int))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }

                    
                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }
                if (selectedOMB[i].treeNumber == 18)
                {
                    string data = "";
                    OMBname itemToChange = (from itm in wmbusOMBname
                                            where itm.omb_name == itemToFillin[i].omb_name
                                            select itm)
                               .FirstOrDefault<OMBname>();

                    var listItems = wmbusOMBname.FirstOrDefault(z => z.hexOfOMB == itemToFillin[i].hexOfOMB);
                    Type typeOfOMB = selectedOMB[i].type;
                    if (typeOfOMB == typeof(bool))
                    {
                        if (itemToFillin[i].valuesToChangeAfterGet == "0")
                        {
                            data = "False";
                        }
                        else
                        {
                            data = "True";
                        }
                    }

                   
                    if (typeOfOMB == typeof(byte))
                    {
                        data = (itemToFillin[i].valuesToChangeAfterGet).ToString();
                    }


                    listItems.omb_values = data;
                    listItems.colors = Color.Green;
                }

            }
            //for (int i = 0; i < listOfCheckedElement.Count(); i++)
            //{
            //    listOfCheckedElement[i].list_checkedElement.IsChecked = false;

            //}
            //listViewAccelerometrOMB.ItemsSource = null;
            //listViewBleOMB.ItemsSource = null;

            //listViewAccelerometrOMB.ItemsSource = accelerometrOMBname;
            //listViewBleOMB.ItemsSource = bleOMBname;

            //listViewBleOMBentry.Text = null;
            //listViewBleOMBentry.text = bleOMBname;
        }
        #endregion


        #region get command
        private async void GetCommand(object sender, EventArgs e)
        {
            allowColorChange = false;

            bleOMBname[1].colors = Color.FromHex("#909090");
            int count = selectedOMB.Count();

            if (count != 0)
            {
                ivValue++;
                preambleToEncryptData++;
                // wysłać te 2 wartości do WAN 3.0 + SN + Crypto Key + jakoś zaznaczone rzeczy

                bool readyToencryption = false;
                for (int i = 0; i < count; i++)
                {
                    int isGetableOrNo = selectedOMB[i].read_write_access;
                    if (isGetableOrNo == 0 || isGetableOrNo==2)
                    {
                        readyToencryption = true;
                        WAN3.SetSelectedOMBparameter(selectedOMB);
                    }

                    else
                    {
                        DisplayAlert("Error", "You can not get all object because one of object is not getable!", "Ok");
                        readyToencryption = false;
                        return;
                    }
                }
                if (readyToencryption == true)
                {
                    packetBytes = WAN3.WP_PDU_ENCRYPTION(packetBytes, preambleToEncryptData, ivValue, count, false, serial_number, cryptography_key, false);

                    datagramBytes = WAN3.WD_PDU_ENCRYPTION(packetBytes);
                }
                byte[] dataToSend = new byte[datagramBytes.Length + packetBytes.Length];
                datagramBytes.CopyTo(dataToSend, 0);
                packetBytes.CopyTo(dataToSend, datagramBytes.Length);


                IReadOnlyList<ICharacteristic> characteristics = ConnectionCharachteristics;

                if (connetion == true)
                {
                    await characteristics[0].WriteAsync(dataToSend); // sending data to device

                    byte[] receivedData = { };
                    if (characteristics[1].CanRead)
                    {
                        receivedData = await characteristics[1].ReadAsync();
                    }
                    
                    
                    byte[] headerAndDataBytesWithValue = WAN3.Decode_frame(receivedData , serial_number, cryptography_key);
                    if(headerAndDataBytesWithValue!=null)
                    { 
                        ConversionToFillSelectedValues(WAN3.headerAndDataBytesCopy, headerAndDataBytesWithValue, count);
                        fillGetOMB(itemsToChangeAfterGetCommand, selectedOMB);
                        selectedOMB.Clear();
                        itemsToChangeAfterGetCommand.Clear();

                        for (int i = 0; i < listOfCheckedElement.Count(); i++)
                        {
                            //listOfCheckedElement[i].list_checkedElement.BackgroundColor = Color.Red;
                            listOfCheckedElement[i].list_checkedElement.IsChecked = false;

                        }



                    
                        listViewAccelerometrOMB.ItemsSource = null;
                        listViewAccelerometrReadoutOMB.ItemsSource = null;
                        listViewArchiveOMB.ItemsSource = null;
                        listViewBleOMB.ItemsSource = null;
                        listViewConfigurationOMB.ItemsSource = null;
                        listViewDatalinksOMB.ItemsSource = null;

                        listViewDebuglinksOMB.ItemsSource = null;
                        listViewFlashlinksOMB.ItemsSource = null;
                        listViewPowerlinksOMB.ItemsSource = null;
                        listViewRwmbuslinksOMB.ItemsSource = null;
                        listViewRadioOMB.ItemsSource = null;
                        listViewWmbusOMB.ItemsSource = null;
                        listViewRTClinksOMB.ItemsSource = null;
                        listViewSmoplinksOMB.ItemsSource = null;
                        listViewTemperaturelinksOMB.ItemsSource = null;
                        listViewWan3linksOMB.ItemsSource = null;
                        listViewWmbuslinksOMB.ItemsSource = null;

                        listViewUserconfigOMB.ItemsSource = null;



                        listViewAccelerometrOMB.ItemsSource = accelerometrOMBname;
                        listViewAccelerometrReadoutOMB.ItemsSource = accelerometrReadoutOMBname;
                        listViewArchiveOMB.ItemsSource = archiveOMBname;
                        listViewBleOMB.ItemsSource = bleOMBname;
                        listViewConfigurationOMB.ItemsSource = configurationOMBname;
                        listViewDatalinksOMB.ItemsSource = datalinksOMBname;

                        listViewDebuglinksOMB.ItemsSource = debugOMBname;
                        listViewFlashlinksOMB.ItemsSource = flashOMBname;
                        listViewPowerlinksOMB.ItemsSource = powerOMBname;
                        listViewRwmbuslinksOMB.ItemsSource = rWmbusOMBname;
                        listViewRadioOMB.ItemsSource = radioOMBname;
                        listViewWmbusOMB.ItemsSource = radioWmbusOMBname;
                        listViewRTClinksOMB.ItemsSource = rtcOMBname;
                        listViewSmoplinksOMB.ItemsSource = smopOMBname;
                        listViewTemperaturelinksOMB.ItemsSource = temperatureOMBname;
                        listViewWan3linksOMB.ItemsSource = wan3OMBname;
                        listViewWmbuslinksOMB.ItemsSource = wmbusOMBname;

                        listViewUserconfigOMB.ItemsSource = userconfigOMBname;

                        selectedOMB.Clear();
                        allowColorChange = true;
                    }
                    else
                    {
                        await DisplayAlert("Warning", "Wrong MAC!", "Ok");
                    }

                }
                else
                {
                    await DisplayAlert("Warning", "Device send data Error! No connection with device!", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Warning", "Device send data Error! Nothing selected! ", "Ok");
            }

            #region test region
            //bleOMBname.Add(new OMBname { omb_name = "" });

            //OMBname itemToremove = (from itm in bleOMBname
            //                        where itm.omb_name == bleOMBname[1].omb_name
            //                        select itm)
            //                    .FirstOrDefault<OMBname>();

            ////bleOMBname.Remove(itemToremove);
            ////bleOMBname.RemoveAt(itemToremove);

            //bleOMBname.Add(new OMBname { omb_name = bleOMBname[1].omb_name, omb_values = bleOMBname[1].omb_values });
            ////bleOMBname.RemoveAt(1);
            //bleOMBname.Move(bleOMBname.Count - 1, 1);
            //bleOMBname.RemoveAt(bleOMBname.Count - 1);
            //selectedOMB.Clear();

            //bleOMBname.Reverse();
            //this.listViewBleOMB.IsVisible = false;

            //this.listViewBleOMB.IsVisible = true;


            //this.listViewBleOMB.IsVisible = false;
            //this.listViewBleOMB.IsVisible = true;

            //bleOMBname.getAdapter().notifyDataSetChanged()
            //for (int i = 0; i < listOfCheckedElement.Count(); i++)
            //{
            //    listOfCheckedElement[i].list_checkedElement.IsChecked = true;

            //}
            // bleOMBname.RemoveAt(2);
            #endregion

        }

        #endregion

        #region set command
        private async void SetCommand(object sender, EventArgs e)
        {
           

            int count = selectedOMB.Count();

            if (count != 0)
            {
                ivValue++;
                preambleToEncryptData++;
                

                bool readyToencryption = false;

                for (int i = 0; i < count; i++)
                {
                    int isGetableOrNo = selectedOMB[i].read_write_access;
                    if (isGetableOrNo == 1 || isGetableOrNo == 2)
                    {
                        readyToencryption = true;
                        WAN3.SetSelectedOMBparameter(selectedOMB);
                    }

                    else
                    {
                        DisplayAlert("Error", "You can not get all object because one of object is not setable!", "Ok");
                        readyToencryption = false;
                        return;
                    }
                }
                if (readyToencryption == true)
                {
                    packetBytes = WAN3.WP_PDU_ENCRYPTION(packetBytes, preambleToEncryptData, ivValue, count, true, serial_number, cryptography_key, false);

                    datagramBytes = WAN3.WD_PDU_ENCRYPTION(packetBytes);
                }
                byte[] dataToSend = new byte[datagramBytes.Length + packetBytes.Length];
                datagramBytes.CopyTo(dataToSend, 0);
                packetBytes.CopyTo(dataToSend, datagramBytes.Length);


                IReadOnlyList<ICharacteristic> characteristics = ConnectionCharachteristics;

                if (connetion == true)
                {
                    await characteristics[0].WriteAsync(dataToSend); // sending data to device

                }
                else
                {
                    await DisplayAlert("Warning", "Device send data Error! No connection with device!", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Warning", "Device send data Error! Nothing selected! ", "Ok");
            }

        }
        #endregion

        private void OnSelection(object sender, CheckedChangedEventArgs e)
        {
            
            CheckBox isCheckedOrNot = (CheckBox)sender;
           
            var control = (CheckBox)sender;
            //control.IsChecked = false;

            listOfCheckedElement.Add(new OMBname() { list_checkedElement = control } );


            string selectedSelectionName = (isCheckedOrNot.BindingContext as OMBname).omb_name;

            int selectedSelectionAccess = (isCheckedOrNot.BindingContext as OMBname).read_write_access;

            string selectedSelectionHexOfOMB = (isCheckedOrNot.BindingContext as OMBname).hexOfOMB;

            string selectedSelectionValue= (isCheckedOrNot.BindingContext as OMBname).omb_values;
            int selectedSelectionTreeNumber = (isCheckedOrNot.BindingContext as OMBname).treeNumber;

            Color color = (isCheckedOrNot.BindingContext as OMBname).colors;

            if(color == Color.Green && allowColorChange== true)
            {
               // (isCheckedOrNot.BindingContext as OMBname).colors = Color.FromHex("#909090"); 
              
            }
            System.Type selectedSelectionType = (isCheckedOrNot.BindingContext as OMBname).type;
            if (e.Value == true)
            {
                selectedOMB.Add(new OMBname() { omb_name = selectedSelectionName, read_write_access = selectedSelectionAccess, hexOfOMB = selectedSelectionHexOfOMB, omb_values= selectedSelectionValue,type = selectedSelectionType ,treeNumber = selectedSelectionTreeNumber });
                
            }
            else
            {
                OMBname itemToremove = (from itm in selectedOMB
                                        where itm.omb_name == selectedSelectionName
                                        select itm)
                                .FirstOrDefault<OMBname>();

                selectedOMB.Remove(itemToremove);

            }
            this.BindingContext = this;

        }

        private void IsCheckedOrNot_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private  string DecimalToHexadecimal(int dec)
        {
            if (dec < 1) return "0";

            int hex = dec;
            string hexStr = string.Empty;

            while (dec > 0)
            {
                hex = dec % 16;

                if (hex < 10)
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 48).ToString());
                else
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 55).ToString());

                dec /= 16;
            }

            return hexStr;
        }
        public string twoByteTabletoString(byte [] table )
        {
            string text="";
            string hexValue;
            for (int i=0;i<2 ;i++)
            {
                if (table[i]<=15)
                {
                    text += "0";
                    text += Convert.ToString(table[i], 16);
                    text = text.ToUpper();


                }
                else
                {
                    text += Convert.ToString(table[i], 16);
                    text = text.ToUpper();
                }
              
            }
           
           

            return text;
        }
       

        public ObservableCollection<OMBname> selectedOMB { get; set; }
        private ObservableCollection<OMBname> getselectedOMBname()
        {
            return new ObservableCollection<OMBname>
            {
            };
        }


        #region collection MAP of OMB for APULSE_xxx6_xxx1

        public List<OMBname> listOfCheckedElement { get; set; }
        private List<OMBname> getMainListOfCheckedElement()
        {
            return new List<OMBname>
            {
            };
        }
        // structure of omb {NAME  ;  VALUE  ;  READ/WRITE ACCESS  ;  HEX VALUE  ;  SIZE}
        public ObservableCollection<OMBname> accelerometrOMBname { get; set; }
        public ObservableCollection<OMBname> accelerometrReadoutOMBname { get; set; }
        public ObservableCollection<OMBname> counterOMBname { get; set; }

        public ObservableCollection<OMBname> bleOMBname { get; set; }

        public ObservableCollection<OMBname> userconfigOMBname { get; set; }

        public ObservableCollection<OMBname> archiveOMBname { get; set; }

        public ObservableCollection<OMBname> configurationOMBname { get; set; }

        public ObservableCollection<OMBname> datalinksOMBname { get; set; }

        public ObservableCollection<OMBname> debugOMBname { get; set; }

        public ObservableCollection<OMBname> flashOMBname { get; set; }
        public ObservableCollection<OMBname> powerOMBname { get; set; }

        public ObservableCollection<OMBname> rWmbusOMBname { get; set; }

        public ObservableCollection<OMBname> radioOMBname { get; set; }

        public ObservableCollection<OMBname> radioWmbusOMBname { get; set; }

        public ObservableCollection<OMBname> rtcOMBname { get; set; }

        public ObservableCollection<OMBname> smopOMBname { get; set; }

        public ObservableCollection<OMBname> temperatureOMBname { get; set; }

        public ObservableCollection<OMBname> wan3OMBname { get; set; }

        public ObservableCollection<OMBname> wmbusOMBname { get; set; }

        private ObservableCollection<OMBname> getMainListAccelerometrOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                
                new OMBname { omb_name ="ACCELEROMETER_MODULE_ENABLED", omb_values=OMBname.omb_boolen_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite,hexOfOMB ="09DB",type = OMBname.omb_boolen_value.GetType(), treeNumber=(int)ombTree.AccelerometrModule,colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_ACCELEROMETER_DATA_RATE", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09DD",type = OMBname.omb_byte_value.GetType(), treeNumber=(int)ombTree.AccelerometrModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_ACCELEROMETER_TILT_SENSITIVITY", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="098F",type = OMBname.omb_byte_value.GetType(), treeNumber=(int)ombTree.AccelerometrModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_ACCELEROMETER_POSITION", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly, hexOfOMB="09AA",type = OMBname.omb_byte_value.GetType(), treeNumber=(int)ombTree.AccelerometrModule, colors= Color.FromHex("#909090")  },

            };

        }

        private ObservableCollection<OMBname> getMainListAccelerometrReadoutOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name ="ACCELEROMETER_READOUT_X", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly ,hexOfOMB="03A6",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.AccelerometrRedout },
                new OMBname { omb_name ="ACCELEROMETER_READOUT_Y", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly ,hexOfOMB="03A6",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.AccelerometrRedout },
                new OMBname { omb_name ="ACCELEROMETER_READOUT_Z", omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly ,hexOfOMB="03A6",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.AccelerometrRedout },


            };

        }

        private ObservableCollection<OMBname> getMainListCounterOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name ="DEVICE_COUNTER_COEFFICIENT",omb_values=OMBname.omb_double_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0100",type = OMBname.omb_double_value.GetType(),treeNumber =(int)ombTree.CounterModule },
                new OMBname { omb_name ="DEVICE_COUNTERS_REGISTRATION_PERIOD",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite ,hexOfOMB="0101",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule },
                new OMBname { omb_name ="MAX_HOUR_FLOW",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0202",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                new OMBname { omb_name ="CURRENT_FLOW",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly, hexOfOMB="0203",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                new OMBname { omb_name ="HOURLY_USAGE_CURRENT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadOnly, hexOfOMB="0204",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                //new OMBname { omb_name ="TEMPORARY_FLOW_PERIOD_LATCHED", omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="TEMPORARY_FLOW_PERIOD_LATCHED_TIME_COMPRESSED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="HOURLY_FLOW_PERIOD_LATCHED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="TEMPORARY_FLOW_MONTHLY_LATCHED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="TEMPORARY_FLOW_MONTHLY_LATCHED_TIME_COMPRESSED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="HOURLY_FLOW_MONTHLY_LATCHED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="HOURLY_FLOW_MONTHLY_LATCHED_TIME_COMPRESSED",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                new OMBname { omb_name ="REMOVE_COUNTER",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="020D",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                new OMBname { omb_name ="SABOTAGE_COUNTER",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite , hexOfOMB="020F",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule },
                new OMBname { omb_name ="DEVICE_DIFF_COUNT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="02A5",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                new OMBname { omb_name ="METER_USAGE_VALUE",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0405",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.CounterModule  },
                //new OMBname { omb_name ="DEVICE_COMMAND_VOLUME_OFFSET",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="COUNTERS_ARCHIVE_LAST_TIME",omb_counter_values=OMBname.omb_dateTime_value.ToString(), read_write_access=true },
                //new OMBname { omb_name ="COUNTERS_ARCHIVE_LAST_VOLUME",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="COUNTERS_ARCHIVE_LAST_ENERGY",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="REMOVE_COUNTER_TIMESTAMP",omb_counter_values=OMBname.omb_dateTime_value.ToString(), read_write_access=true },
                //new OMBname { omb_name ="REMOVE_COUNTER_DURATION",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="REMOVE_COUNTER_VOLUME",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="REMOVE_COUNTER_ENERGY",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="SABOTAGE_COUNTER_TIMESTAMP",omb_counter_values=OMBname.omb_dateTime_value.ToString(), read_write_access=true },
                //new OMBname { omb_name ="SABOTAGE_COUNTER_DURATION",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="SABOTAGE_COUNTER_VOLUME",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="SABOTAGE_COUNTER_ENERGY",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="LAST_DAILY_LATCH_TIME",omb_counter_values=OMBname.omb_dateTime_value.ToString(), read_write_access=false },
                //new OMBname { omb_name ="LAST_DAILY_LATCH_VOLUME",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=false  },
                //new OMBname { omb_name ="DEVICE_VOLUME_MAX_VALUE",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="DEVICE_COUNTER_SOURCE",omb_counter_values=OMBname.omb_int_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="DEVICE_COUNTER_ARCHIVE_VOLUME",omb_counter_values=OMBname.omb_boolen_value.ToString(), read_write_access=true  },
                //new OMBname { omb_name ="DEVICE_COUNTER_ARCHIVE_ENERGY",omb_counter_values=OMBname.omb_boolen_value.ToString(), read_write_access=true  },
                

            };

        }
        
        private ObservableCollection<OMBname> getMainListBleOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name ="DEVICE_BLE_PIN_ENABLE",omb_values=OMBname.omb_boolen_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09BD",type = OMBname.omb_boolen_value.GetType(),treeNumber =(int)ombTree.BleModule , colors= Color.FromHex("#909090") },
                new OMBname { omb_name ="DEVICE_BLE_POWER_LEVEL",omb_values=OMBname.omb_byte_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09BE",type = OMBname.omb_byte_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_LOGS_CONFIGURATION",omb_values=OMBname.omb_byte_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09C5",type = OMBname.omb_byte_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_RANDOM_MAC_ENABLE",omb_values=OMBname.omb_boolen_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09C6",type = OMBname.omb_boolen_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_ENABLED",omb_values=OMBname.omb_boolen_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="09C8",type = OMBname.omb_boolen_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_FAST_ADV_INTERVAL_MIN",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C51",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_FAST_ADV_INTERVAL_MAX",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C52",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_FAST_ADV_PERIOD",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0xC53" ,type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090") },
                new OMBname { omb_name ="DEVICE_BLE_CON_CURRENT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C60" ,type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090") },
                new OMBname { omb_name ="DEVICE_BLE_PIN",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C61",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_MAX_DURATION_TIMEOUT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C62",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_IDLE_TIMEOUT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C63",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_DISCOVERABLE_TIMEOUT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C64",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_ADV_INTERVAL_MIN",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C65",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_ADV_INTERVAL_MAX",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C66",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                new OMBname { omb_name ="DEVICE_BLE_MAX_DURATION_CONNECTION_TIMEOUT",omb_values=OMBname.omb_int_value.ToString(), read_write_access=(int)getReadWriteAccess.ReadWrite, hexOfOMB="0C67",type = OMBname.omb_int_value.GetType(),treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
                //new OMBname { omb_name ="BLE_MAC_ADDRESS",omb_values=OMBname.mac_value.ToString(), read_write_access=true, hexOfOMB="0D20",type = OMBname.mac_value.GetType(), stringType="mac",treeNumber =(int)ombTree.BleModule, colors= Color.FromHex("#909090")  },
            };

        }

        private ObservableCollection<OMBname> getMainListUserconfigOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_STANDARD_PARAMETER_LIST_VERSION", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0200", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_SERIAL_NBR", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0400", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_HARDWARE_VERSION", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0403", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_FIRMWARE_VERSION", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0404", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BOOTLOADER_VERSION", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04A0", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0500", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "METER_SERIAL_NUMBER", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0501", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
                //new OMBname { omb_name = "DEVICE_TYPE_NAME", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = true, hexOfOMB = "0502", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.UserconfigModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListArchiveOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_ARCHIVE_COUNTERS_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C6C", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_PERIODIC_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C6D", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_MONTHLY_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C6E", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_COUNTERS_SECONDARY_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C6F", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_PERIODIC_SECONDARY_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C70", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_MONTHLY_SECONDARY_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C71", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_PREPAID_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C72", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_PREPAID_LOG_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C73", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_EVENTLOG_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C74", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_DIAGNOSTICS_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C75", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_DISCONTINUITY_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C76", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_GNSS_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C77", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_PACKET_ARCHIVE_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C78", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_ARCHIVE_SACK_RECORD_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C79", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.ArchiveModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListConfigurationOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_RUN_MODE_ACTIVE", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0923", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.ConfigurationModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CONFIG_SLEEP_DENY", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09C3", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.ConfigurationModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListDatalinksOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_EVENT_TRANSMISSION_LINK", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "010C", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_RECEIVED_RADIO_PACKETS_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "021D", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_SENT_RADIO_PACKETS_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "021E", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_EVENT_TRANSMISSION_DEFAULT_PROTOCOL_ID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0917", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_EVENT_TRANSMISSION_LINK_EVENT", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0924", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DATALINKS_STACK_FREE_SPACE", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0C39", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.DatalinksModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListDebugOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_LOG_COMMAND_ENABLE", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01D2", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.DebugModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_LOG_BLOB_LENGTH_LIMIT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02D7", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.DebugModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_LOG_DEBUG_ENABLE", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09D5", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.DebugModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListFlashOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_DIRECT_MEMORY_ALLOW_WRITE", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01D4", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.FlashModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_DIRECT_MEMORY_WRITE_SIZE", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02AB", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.FlashModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_DIRECT_MEMORY_ACCESS_ADDRESS", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04AD", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.FlashModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListPowerOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "BATTERY_PERCENTAGE_USAGE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0107", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_CRITICAL_VOLTAGE_LEVEL", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02B9", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "BATTERY_VOLTAGE_VALUE", omb_values = OMBname.omb_double_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02CE", type = OMBname.omb_double_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "BATTERY_VOLUME_USAGE", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "02E9", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_LOW_VOLTAGE_LEVEL", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02ED", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_CAPACITY", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04A3", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_CONSUMPTION_OFFSET", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04A4", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_STOP_CURRENT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04AB", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_SLEEP_CURRENT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04B6", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_RUN_CURRENT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04C7", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_POWER_CONFIG_FORCE_AC", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0900", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_PVD_MODE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0953", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "PVD_STATE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0965", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_LOW_BATTERY_TRESHOLD", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09D1", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.PowerModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListRwmbusOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "R_WMBUS_MODE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09E2", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RwmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "R_WMBUS_DEVICE_SELF_TYPE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09E3", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RwmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "R_WMBUS_DEVICE_MEDIUM", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09EF", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RwmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "R_WMBUS_TX_POWER", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "09EF", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RwmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "R_WMBUS_TX_POWER", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0A2E", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RwmbusModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListRadioOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_RADIO_CHIP_TYPE", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01A4", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_RESPONSE_DELAY", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "02BA", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_FREQUENCY_CORRECTION", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02C4", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_BATTERY_RADIO_CURRENT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04A7", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "RADIO_LAST_PACKET_RECEIVED_TIME", omb_values = OMBname.omb_dateTime_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04CA", type = OMBname.omb_dateTime_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "RADIO_LAST_PACKET_SEND_TIME", omb_values = OMBname.omb_dateTime_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04CB", type = OMBname.omb_dateTime_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_MODE_TEMPORAL_TIME_REMAINING", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04CD", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "RADIO_INVALID_PACKET_RECEIVED_COUNT", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04CF", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_FREQUENCY_VALUE", omb_values = OMBname.omb_double_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04D1", type = OMBname.omb_double_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_RADIO_DEFAULT_PROTOCOL_ID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0917", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "RADIO_QUALITY", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "091C", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_DEFAULT_MODE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "093A", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RadioModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListRadioDeviceWmbusOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_WMBUS_MODE_VALUE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "01DA", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RadioDeviceWmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_MODE_TEMPORAL_VALUE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "01DA", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RadioDeviceWmbusModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListRtcOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_TIME_ZONE_OFFSET", omb_values = OMBname.omb_double_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0103", type = OMBname.omb_double_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_TIME_ZONE_OFFSET_WINTER", omb_values = OMBname.omb_double_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0104", type = OMBname.omb_double_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_TIME_ZONE_OFFSET_SUMMER", omb_values = OMBname.omb_double_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0105", type = OMBname.omb_double_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MONTHLY_DATA_LATCH_DAY", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0120", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MONTHLY_AND_DAILY_DATA_LATCH_HOUR", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0121", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MONTHLY_AND_DAILY_DATA_LATCH_LOCAL_TIME", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0122", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_TIME_ZONE_AUTO_OFFSET", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0124", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CLOCK_OFFSET", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "020C", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CLOCK", omb_values = OMBname.omb_dateTime_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "040C", type = OMBname.omb_dateTime_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "LIFE_TIMER_DURATION", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "04AC", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_RTC_CALIBRATION_REGISTER", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04D0", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_TIME_MAX_DIFF", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C01", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CLOCK_LOCAL_TIME", omb_values = OMBname.omb_dateTime_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C0A", type = OMBname.omb_dateTime_value.GetType(), treeNumber = (int)ombTree.RTCModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListSmopOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "SMOP_RADIO_MODE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01DA", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "SMOP_DEFAULT_TX_POWER", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01EF", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "SMOP_MTU", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "02D5", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "SMOP_RX_FREQUENCY", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04D1", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "SMOP_VERSION", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0919", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "SMOP_TX_FREQUENCY", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0C43", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.SmopModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListTemperatureOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "TEMPERATURE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "0108", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.TemperatureModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MAX_TEMPERATURE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01A6", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.TemperatureModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MIN_TEMPERATURE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01A7", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.TemperatureModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_HISTERESIS_TEMPERATURE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01AA", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.TemperatureModule, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListWan3OMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "DEVICE_WDP_ARP", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0119", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WDP_AT", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "011A", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WDP_MR", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "011B", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WPP_CRYPTOGRAPHY_OBLIGATORY", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "011C", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WPP_EVENT_SEND_SN", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0123", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "FREQ_REASON", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadOnly, hexOfOMB = "01A1", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_SELF_QUERY_ENCRYPTION_ENABLED", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01AC", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_IV_VERIFICATION_ENABLED", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01C8", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_OVERWRITE_RESPONSE_LINK", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.WriteOnly, hexOfOMB = "01CF", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WDP_IV_IN", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0426", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_EVENT_FREQ_EXECUTION_DELAY", omb_values = OMBname.omb_int_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "04C2", type = OMBname.omb_int_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_CRYPTOGRAPHY_KEY", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.WriteOnly, hexOfOMB = "051B", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_FREQ_EVENT_CONTENT", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0520", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_FREQ_CYCLIC_CONTENT", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "05CB", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.Wan3Module, colors = Color.FromHex("#909090") },
            };
        }

        private ObservableCollection<OMBname> getMainListWmbusOMBname()
        {
            return new ObservableCollection<OMBname>
            {
                new OMBname { omb_name = "WMBUS_SCAN_TOKEN", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01D9", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MULTIHOP_CONFIG_NODES_RELAY_ON", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01F2", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_MULTIHOP_CONFIG_NODES_RELAY_ALL", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01F3", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_CONFIG_DISTRIBUTOR_ID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01F9", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_CONFIG_SYSTEM_ID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01FA", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_CONFIG_AREA_ID", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "01FB", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_SEND_TO_SACK", omb_values = OMBname.omb_boolen_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0901", type = OMBname.omb_boolen_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_DEFAULT_INTERFACE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0903", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
                new OMBname { omb_name = "DEVICE_WMBUS_ALARM_RELAY_MODE", omb_values = OMBname.omb_byte_value.ToString(), read_write_access = (int)getReadWriteAccess.ReadWrite, hexOfOMB = "0922", type = OMBname.omb_byte_value.GetType(), treeNumber = (int)ombTree.WmbusModule, colors = Color.FromHex("#909090") },
            };
        }



        #endregion

        enum ombTree
        {
            AccelerometrModule,
            AccelerometrRedout,
            ArchiveModule,
            BleModule,
            ConfigurationModule,
            CounterModule,
            DatalinksModule,
            DebugModule,
            FlashModule,
            PowerModule,
            RwmbusModule,
            RadioModule,
            RadioDeviceWmbusModule,
            RTCModule,
            SmopModule,
            TemperatureModule,
            UserconfigModule,
            Wan3Module,
            WmbusModule
        }

        enum getReadWriteAccess
        {
            ReadOnly,
            WriteOnly,
            ReadWrite
        }

        #region AccelerometrModule

        int countOfAccelerometrModule = 0;


        void ResizeAccelerometrModule(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                accelerometr_module.HeightRequest = heightRequest;
                accelerometr_module.ForceLayout();
                this.ForceLayout();
            });
        }


        private void AccelerometrModule(object sender, EventArgs e)
        {
            countOfAccelerometrModule++;
            if (countOfAccelerometrModule % 2 == 0)
            {
                _ = accelerometr_module.FadeTo(0);
                ResizeAccelerometrModule(0);
            }

            else
            {
                _ = accelerometr_module.FadeTo(1);
                ResizeAccelerometrModule(300);
            }





        }
        #endregion

        #region AccelerometrRedout
        int countOfAccelerometrRedout = 0;
        void ResizeAccelerometrRedout(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                accelerometr_readout.HeightRequest = heightRequest;
                accelerometr_readout.ForceLayout();
                this.ForceLayout();
            });
        }
        private void AccelerometrRedout(object sender, EventArgs e)
        {
            countOfAccelerometrRedout++;
            if (countOfAccelerometrRedout % 2 == 0)
            {
                _ = accelerometr_readout.FadeTo(0);
                ResizeAccelerometrRedout(0);
            }

            else
            {
                _ = accelerometr_readout.FadeTo(1);
                ResizeAccelerometrRedout(150);
            }

        }
        #endregion

        #region Archive
        int countOfArchive = 0;
        void ResizeArchive(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                archive_module.HeightRequest = heightRequest;
                archive_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void ArchiveModule(object sender, EventArgs e)
        {
            countOfArchive++;
            if (countOfArchive % 2 == 0)
            {
                _ = archive_module.FadeTo(0);
                ResizeArchive(0);
            }

            else
            {
                _ = archive_module.FadeTo(1);
                ResizeArchive(450);
            }

        }
        #endregion

        #region Configuration
        int countOfConfiguration = 0;
        void ResizeConfiguration(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                configuration_module.HeightRequest = heightRequest;
                configuration_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void ConfigurationModule(object sender, EventArgs e)
        {
            countOfConfiguration++;
            if (countOfConfiguration % 2 == 0)
            {
                _ = configuration_module.FadeTo(0);
                ResizeConfiguration(0);
            }

            else
            {
                _ = configuration_module.FadeTo(1);
                ResizeConfiguration(150);
            }

        }
        #endregion

        #region CounterModule
        void ResizeCounterModule(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                counter_module.HeightRequest = heightRequest;
                counter_module.ForceLayout();
                this.ForceLayout();
            });
        }

        int countOfCounterModule = 0;
        private void CounterModule(object sender, EventArgs e)
        {
            countOfCounterModule++;
            if (countOfCounterModule % 2 == 0)
            {
                _ = counter_module.FadeTo(0);
                ResizeCounterModule(0);
            }

            else
            {
                _ = counter_module.FadeTo(1);
                ResizeCounterModule(500);
            }
        }



        #endregion

        # region BleModule

        void ResizeBleModule(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                ble_module.HeightRequest = heightRequest;
                ble_module.ForceLayout();
                this.ForceLayout();
            });
        }

        int countOfBleModule = 0;
        private void BleModule(object sender, EventArgs e)
        {
            countOfBleModule++;
            if (countOfBleModule % 2 == 0)
            {
                _ = ble_module.FadeTo(0);
                ResizeBleModule(0);
            }

            else
            {
                _ = ble_module.FadeTo(1);
                ResizeBleModule(500);
            }
        }
        #endregion

        #region Datalinks

        int countOfDatalonks = 0;
        void ResizeDatalinks(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                datalinks_module.HeightRequest = heightRequest;
                datalinks_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void DatalinksModule(object sender, EventArgs e)
        {
            countOfDatalonks++;
            if (countOfDatalonks % 2 == 0)
            {
                _ = datalinks_module.FadeTo(0);
                ResizeDatalinks(0);
            }

            else
            {
                _ = datalinks_module.FadeTo(1);
                ResizeDatalinks(250);
            }

        }
        #endregion

        #region DebugModule

        int countOfDebug = 0;
        void ResizeDebug(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                debug_module.HeightRequest = heightRequest;
                debug_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void DebugModule(object sender, EventArgs e)
        {
            countOfDebug++;
            if (countOfDebug % 2 == 0)
            {
                _ = debug_module.FadeTo(0);
                ResizeDebug(0);
            }

            else
            {
                _ = debug_module.FadeTo(1);
                ResizeDebug(250);
            }

        }
        #endregion

        #region FlashModule

        int countOfFlash = 0;
        void ResizeFlash(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                flash_module.HeightRequest = heightRequest;
                flash_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void FlashModule(object sender, EventArgs e)
        {
            countOfFlash++;
            if (countOfFlash % 2 == 0)
            {
                _ = flash_module.FadeTo(0);
                ResizeFlash(0);
            }

            else
            {
                _ = flash_module.FadeTo(1);
                ResizeFlash(250);
            }

        }
        #endregion

        #region PowerModule

        int countOfPower = 0;
        void ResizePower(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                power_module.HeightRequest = heightRequest;
                power_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void PowerModule(object sender, EventArgs e)
        {
            countOfPower++;
            if (countOfPower % 2 == 0)
            {
                _ = power_module.FadeTo(0);
                ResizePower(0);
            }

            else
            {
                _ = power_module.FadeTo(1);
                ResizePower(250);
            }

        }
        #endregion

        #region R-WMbus Module

        int countOfRWmbus = 0;
        void ResizeRWmbus(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                rwmbus_module.HeightRequest = heightRequest;
                rwmbus_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void RWmbusModule(object sender, EventArgs e)
        {
            countOfRWmbus++;
            if (countOfRWmbus % 2 == 0)
            {
                _ = rwmbus_module.FadeTo(0);
                ResizeRWmbus(0);
            }

            else
            {
                _ = rwmbus_module.FadeTo(1);
                ResizeRWmbus(250);
            }

        }
        #endregion


        #region Radio Module

        int countOfRadio = 0;
        void ResizeRadio(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                radio_module.HeightRequest = heightRequest;
                radio_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void RadioModule(object sender, EventArgs e)
        {
            countOfRadio++;
            if (countOfRadio % 2 == 0)
            {
                _ = radio_module.FadeTo(0);
                ResizeRadio(0);
            }

            else
            {
                _ = radio_module.FadeTo(1);
                ResizeRadio(250);
            }

        }
        #endregion

        #region Device WmbusModule

        int countOfDeviceWmbus = 0;
        void ResizeDeviceWmbus(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                device_wmbus_module.HeightRequest = heightRequest;
                device_wmbus_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void DeviceWmbusModule(object sender, EventArgs e)
        {
            countOfDeviceWmbus++;
            if (countOfDeviceWmbus % 2 == 0)
            {
                _ = device_wmbus_module.FadeTo(0);
                ResizeDeviceWmbus(0);
            }

            else
            {
                _ = device_wmbus_module.FadeTo(1);
                ResizeDeviceWmbus(250);
            }

        }
        #endregion

        #region RTC module

        int countOfRtc = 0;
        void ResizeRTC(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                rtc_module.HeightRequest = heightRequest;
                rtc_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void RTCModule(object sender, EventArgs e)
        {
            countOfRtc++;
            if (countOfRtc % 2 == 0)
            {
                _ = rtc_module.FadeTo(0);
                ResizeRTC(0);
            }

            else
            {
                _ = rtc_module.FadeTo(1);
                ResizeRTC(250);
            }

        }
        #endregion

        #region SMOP module

        int countOfSmop = 0;
        void ResizeSmop(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                smop_module.HeightRequest = heightRequest;
                smop_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void SmopModule(object sender, EventArgs e)
        {
            countOfSmop++;
            if (countOfSmop % 2 == 0)
            {
                _ = smop_module.FadeTo(0);
                ResizeSmop(0);
            }

            else
            {
                _ = smop_module.FadeTo(1);
                ResizeSmop(250);
            }

        }
        #endregion

        #region Temperature Module

        int countOfTemperature = 0;
        void ResizeTemperature(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                temperature_module.HeightRequest = heightRequest;
                temperature_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void TemperatureModule(object sender, EventArgs e)
        {
            countOfTemperature++;
            if (countOfTemperature % 2 == 0)
            {
                _ = temperature_module.FadeTo(0);
                ResizeTemperature(0);
            }

            else
            {
                _ = temperature_module.FadeTo(1);
                ResizeTemperature(250);
            }

        }
        #endregion


        #region UserconfigModule

        void ResizeUserconfigModule(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                userconfig_module.HeightRequest = heightRequest;
                userconfig_module.ForceLayout();
                this.ForceLayout();
            });
        }

        int countOfUserconfigModule = 0;
        private void UserconfigModule(object sender, EventArgs e)
        {
            countOfUserconfigModule++;
            if (countOfUserconfigModule % 2 == 0)
            {
                _ = userconfig_module.FadeTo(0);
                ResizeUserconfigModule(0);
            }

            else
            {
                _ = userconfig_module.FadeTo(1);
                ResizeUserconfigModule(500);
            }
        }

        #endregion


        #region WAN3 module

        int countOfWAN3 = 0;
        void ResizeWan3(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                wan3_module.HeightRequest = heightRequest;
                wan3_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void WAN3Module(object sender, EventArgs e)
        {
            countOfWAN3++;
            if (countOfWAN3 % 2 == 0)
            {
                _ = wan3_module.FadeTo(0);
                ResizeWan3(0);
            }

            else
            {
                _ = wan3_module.FadeTo(1);
                ResizeWan3(250);
            }

        }
        #endregion

        #region Wmbus module

        int countOfWMBUS = 0;
        void ResizeWmbus(double heightRequest)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                wmbus_module.HeightRequest = heightRequest;
                wmbus_module.ForceLayout();
                this.ForceLayout();
            });
        }
        private void WmbusModule(object sender, EventArgs e)
        {
            countOfWMBUS++;
            if (countOfWMBUS % 2 == 0)
            {
                _ = wmbus_module.FadeTo(0);
                ResizeWmbus(0);
            }

            else
            {
                _ = wmbus_module.FadeTo(1);
                ResizeWmbus(250);
            }

        }
        #endregion



    }
    public class OMBname
    {
        public static int omb_int_value;
        public static bool omb_boolen_value;
        public static double omb_double_value;

        public static string omb_dateTime_value;
        public static string mac_value;

        public static byte omb_byte_value;
        

        static OMBname()
        {
            omb_int_value = 0;
            omb_boolen_value = false;
            omb_double_value = 0.01;
            omb_dateTime_value = "1970-01-01 00:00:00";
            mac_value = "00:00:00:00:00:00";
            omb_byte_value = 0;

        }

        public string omb_name { get; set; }
        public string omb_values { get; set; }

        public int read_write_access { get; set; } // (True = read/write ) (False = only read)
        
        public string hexOfOMB { get; set; }
        public Type type { get; set; }
        public string stringType{ get; set; }
        //public byte byteType { get; set; }
        public Color colors { get; set; }

        public int treeNumber { get; set; }
        
        public  Xamarin.Forms.CheckBox list_checkedElement { get; set; }


        public string valuesToChangeAfterGet { get; set; }


    }
}


