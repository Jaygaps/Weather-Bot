using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Weather_Bot.DataModels;

namespace Weather_Bot
{

    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://bankapp123.azurewebsites.net");
            this.timelineTable = this.client.GetTable<banktest>();
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
        private IMobileServiceTable<banktest> timelineTable;

        public async Task<List<banktest>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }


        public async Task AddTimeline(banktest timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }
    }
}