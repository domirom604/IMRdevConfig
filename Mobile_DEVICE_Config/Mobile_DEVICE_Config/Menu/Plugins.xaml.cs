using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Plugins : ContentPage
    {
       
        public Plugins(string selectedDriver)
        {

            //MainPage win = (MainPage)Window.GetWindow(this);
            Element parent = App.Current.MainPage;
            //MainPage win = new MainPage();

            InitializeComponent();

            string  selected = selectedDriver;
            string information_about_loadeddriver =selected;
            TitleTextLoadedDriver.Text = information_about_loadeddriver;

        }
    }
}