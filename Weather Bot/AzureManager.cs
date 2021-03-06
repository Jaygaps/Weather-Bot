﻿using Microsoft.WindowsAzure.MobileServices;
using ContosoBank.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoBank
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<BankBot> timelineTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://jayrajbot1.azurewebsites.net");
            this.timelineTable = this.client.GetTable<BankBot>();
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
        public async Task DeleteTimeline(BankBot timeline)
        {
            await this.timelineTable.DeleteAsync(timeline);
        }
        public async Task UpdateTimeline(BankBot timeline)
        {
            await this.timelineTable.UpdateAsync(timeline);
        }
        public async Task AddTimeline(BankBot timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<List<BankBot>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }
    }
}
