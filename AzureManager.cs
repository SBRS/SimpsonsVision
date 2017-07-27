using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using SimpsonsVision.DataModels;

namespace SimpsonsVision
{
    public class AzureManager
    {
        private static AzureManager instance;
		private MobileServiceClient client;
        private IMobileServiceTable<SimpsonsVisionTable> resultTable;

		private AzureManager()
		{
			this.client = new MobileServiceClient("https://simpsonsvision.azurewebsites.net");
            this.resultTable = this.client.GetTable<SimpsonsVisionTable>();
		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}

		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}

        public async Task<List<SimpsonsVisionTable>> GetSimpsonsVisionTableData()
		{
			return await this.resultTable.ToListAsync();
		}

        public async Task PostSimpsonsVisionTableData(SimpsonsVisionTable simpsons_vision_table)
		{
			await this.resultTable.InsertAsync(simpsons_vision_table);
		}
    }
}
