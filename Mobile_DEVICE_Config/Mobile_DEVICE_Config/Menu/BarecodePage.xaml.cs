using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarecodePage : ContentPage
    {
        public BarecodePage()
        {
            InitializeComponent();
        }

        private void ScanResult(ZXing.Result result)
        {
           

            Device.BeginInvokeOnMainThread(async() =>
            {
                string scanResult = result.Text;

                MainPage.SetSerialNumberInformation(scanResult);
                this.Navigation.PopAsync();
                await DisplayAlert("Scanned result", scanResult, "OK");

               
            }
            );
        }
    }
}