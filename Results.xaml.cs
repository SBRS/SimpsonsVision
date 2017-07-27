using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using SimpsonsVision.DataModels;
using Xamarin.Forms;

namespace SimpsonsVision
{
    public partial class Results : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public Results()
        {
            InitializeComponent();
        }

		async void Get_Results(object sender, System.EventArgs e)
		{
            List<SimpsonsVisionTable> SimpsonsVisionTableData = await AzureManager.AzureManagerInstance.GetSimpsonsVisionTableData();

            SimpsonsVisionTableList.ItemsSource = SimpsonsVisionTableData;
		}
    }
}
