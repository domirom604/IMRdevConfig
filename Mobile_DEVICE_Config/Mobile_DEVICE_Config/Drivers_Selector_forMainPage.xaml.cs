using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Drivers_Selector_forMainPage : ContentPage
    {
        public Drivers_Selector_forMainPage()
        {
            InitializeComponent();
           // DriversItems = GetDrivers();
            this.BindingContext = this;
        }

        public ObservableCollection<Drivers> DriversItems { get; set; }

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
    }
}

public class Drivers
{
    public string Title { get; set; }

}