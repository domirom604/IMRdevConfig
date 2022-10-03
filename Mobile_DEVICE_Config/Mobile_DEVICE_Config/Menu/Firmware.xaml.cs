using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Firmware : ContentPage
    {
        bool connection;
        private string file;
        int progressValue;
        string path;
        public ProgrammerBle programmer;
        CancellationToken ct;
        //CancellationTokenSource source = new CancellationTokenSource();
        string textToShow;
        
        public static int SetProgress { get; set; }

        private string myStringProperty;
        public static IReadOnlyList<ICharacteristic> ConnectionCharachteristics { get; set; }


        public Firmware(bool COnnection)
        {
            InitializeComponent();
            
            this.BindingContext = this;
            Showing_Progress_Information();

            this.IsBusy = false;
            connection = COnnection;
           
            //int progressValue = SetProgress;
            //string textToShow = Convert.ToString(progressValue);
            //MyStringProperty = textToShow+"%";
            // this.progressBar.Text = textToShow;
            // this.OnPropertyChanged(textToShow);




            programmer = new ProgrammerBle(ct);

            //Showing_Progress_Information();


            //Showing_Progress_Information();
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

        //public Firmware( bool COnnection)
        //{
        //    InitializeComponent();
        //    connection = COnnection;
        //    //programmer = new ProgrammerBle(ct);
        //    // InitializeComponent();
        //    //this.BindingContext = this;
        //    this.IsBusy = false;

        //}


        public  static void SetProgressInformation(int Number)
        {
            SetProgress = Number;
            
            //Showing_Progress_Information();

        }
        public async void  Showing_Progress_Information()
        {
            //InitializeComponent();
           // Navigation.PushAsync(new Firmware(connection));

            //int progressValue = SetProgress;
            //string textToShow = Convert.ToString(progressValue);
            
            //progressBar.Text = textToShow + "%";
            this.BindingContext = this;
            
        }

        
        private async void Load_file_clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync();
                var stream = await result.OpenReadAsync();
                if (result != null)
                {
                    if (result.FileName.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
                    {
                        textFileName.Text = result.FileName;
                        path = result.FullPath;

                        DisplayAlert("Alertt!", "File Loaded Succesfully!","Ok") ;
                        //fileByte = File.ReadAllBytes(textFileName.Text);
                    }
                    else
                    {
                        DisplayAlert("Alertt!", "Wrong file selected!", "Ok");
                    }
                    
                }

                
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

        }

        private async void Program_button_tapped(object sender, EventArgs e)
        {
            this.IsBusy = true;
            progressBar.Text = "Please wait Programming is in progress!";
            try
            {

                if (String.IsNullOrEmpty(textFileName.Text))
                {
                    DisplayAlert("Error!", "Please specify firmware file first", "Ok");
                    return;
                }
                byte[] fileByte =null;
                fileByte = File.ReadAllBytes(path);
                UInt32 len = Convert.ToUInt32(fileByte.Length);

                if (fileByte == null)
                {
                    DisplayAlert("Error!", "Unable to load file", "Ok");
                    return;
                }
                
                //byte[] content = fileByte.Take(149).ToArray();
                programmer.UpdateFV(fileByte);
               
            }
            
            catch { }
            //this.IsBusy = false;

        }

        private async void Break_button_tapped(object sender, EventArgs e)
        {
            programmer.SetCancellationToken(true);
            progressBar.Text = "Programming has been stopped!";
            this.IsBusy = false;
            //byte[] a = new byte[16];
            //for (int i=0;i<16;i++)
            //{
            //    a[i] = 0;
            //}
            //a[0] = 200;
            //a[3] = 200;
            //a[6] = 85;
            //await ConnectionCharachteristics[1].WriteAsync(a);
        }

        
    }
}
