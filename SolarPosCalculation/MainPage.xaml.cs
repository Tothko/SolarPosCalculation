using SolarPosCalculation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SolarPosCalculation
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public bool isDaylightSavingTime;
        SolarPositionService solService;
        public MainPage()
        {
            this.InitializeComponent();
            solService = new SolarPositionService();
        }

        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            double latitude = 51.477928;
            double longitude = -0.001545;
            Double.TryParse(latitudeInput.Text, out latitude);
            Double.TryParse(longitudeInput.Text, out longitude);

            solService.CalculateInformation(latitude, longitude, isDaylightSavingTime);
            ResultTxtBlock.Text = solService.MoonPHase;
            SunRiseResultTxtBlock.Text = solService.SunRise.ToString();
            SunSetResultTxtBlock.Text = solService.SunSet.ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isDaylightSavingTime = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isDaylightSavingTime = false;
        }
    }
}
