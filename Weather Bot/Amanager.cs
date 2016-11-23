using Microsoft.WindowsAzure.MobileServices;
using ContosoBank.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBank
{
    public class Amanager
    {

        private static Amanager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Timings> timelineTable;

        private Amanager()
        {
            this.client = new MobileServiceClient("http://jayrajbot1.azurewebsites.net");
            this.timelineTable = this.client.GetTable<Timings>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static Amanager AzureManagerInstance2
        {
            get
            {
                if (instance == null)
                {
                    instance = new Amanager();
                }

                return instance;
            }
        }

        public async Task AddTimeline(Timings timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<List<Timings>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }
    }
}
