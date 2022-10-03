using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_DEVICE_Config
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewCommandWindow : ContentPage
    {
        int new_Command_count = 0;
        ObservableCollection<Data> itemList;
        string entry_name="";
        string entry_command_code = "";
        string entry_command_per_one= "";
        string entry_command_per_two = "";
        string entry_command_per_three = "";

        public NewCommandWindow(int new_command_count)
        {
            new_Command_count = new_command_count;
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDg4OTE2QDMxMzkyZTMyMmUzMFVveGZOa29OSmdvYndIbk1xa3dMaUJhYjRHY1B5OG5tNXMyVlFzOHNTQWM9");
            InitializeComponent();
            itemList = new ObservableCollection<Data>();
        }
        

        private void NewCommandWindowCancel(object sender, EventArgs e)
        {
            this.Navigation.PopAsync();


        }

        private async void NewCommandWindowOk(object sender, EventArgs e)
        {
            //itemList.Add(new Data { Name = entry_name, DeleteImage = "trash.png", command_code = entry_command_code, command_code_param_one = entry_command_per_one, command_code_param_two = entry_command_per_two, command_code_param_three = entry_command_per_three});
            //ActionList.SetActionListParameters(itemList);
            //new_Command_count += 1;
            //ActionList.commandCount=new_Command_count;
            //this.Navigation.PopAsync();

            if (!string.IsNullOrWhiteSpace(commandCodeEntry.Text) && !string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                await App.Database.SaveDataAsync(new Data
                {
                    Name = entry_name,
                    DeleteImage = "trash.png",
                    command_code = entry_command_code,
                    command_code_param_one = entry_command_per_one,
                    command_code_param_two = entry_command_per_two,
                    command_code_param_three = entry_command_per_three
                });

                nameEntry.Text = commandCodeEntry.Text = string.Empty;
                
                //collectionView.ItemsSource = await App.Database.GetDataAsync();
            }
            this.Navigation.PopAsync();
            DisplayAlert("Information!", "Command added succesfully", "Ok");
            
        }

        private void EntryName(object sender, EventArgs e)
        {
            entry_name = ((Entry)sender).Text;
        }

        private void EntryCommandCode(object sender, EventArgs e)
        {
            entry_command_code = ((Entry)sender).Text;
        }
        private void EntryCommandParOne(object sender, EventArgs e)
        {
            entry_command_per_one = ((Entry)sender).Text;
        }

        private void EntryCommandParTwo(object sender, EventArgs e)
        {
            entry_command_per_two = ((Entry)sender).Text;
        }

        private void EntryCommandParThree(object sender, EventArgs e)
        {
            entry_command_per_three = ((Entry)sender).Text;
        }

    }
}