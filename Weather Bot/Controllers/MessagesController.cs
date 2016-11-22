using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Weather_Bot.Models;
using System.Collections.Generic;
using System.Web.Services.Description;
using Weather_Bot.DataModels;
using Google.Maps.Geocoding;

namespace Weather_Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private string FinalResult;
        private string userMessage;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            if (activity.Type == ActivityTypes.Message)
            {


                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);


                // calculate something for us to return
                currency.RootObject Root12;
                HttpClient client = new HttpClient();
                string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=NZD"));

                Root12 = JsonConvert.DeserializeObject<currency.RootObject>(x);

                string endOutput = "Hello";
                if (userData.GetProperty<bool>("SentGreeting"))
                {
                    endOutput = "Hello again";
                }
                else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }
                bool isWeatherRequest = true;
                var userMessage = activity.Text;
                
                if (userMessage.ToLower().Contains("maps"))
                {
                    var request = new GeocodingRequest();
                    request.Address = "1600 Amphitheatre Parkway";
                    request.Sensor = false;
                    var response = new GeocodingService().GetResponse(request);
                }
                if (userMessage.ToLower().Contains("converter"))
                {
                    string[] value = userMessage.Split(' ');
                    double AUD = Root12.rates.AUD;
                    double NZD = Root12.rates.NZD;
                    if (value[1].ToLower() == "aud")
                    {
                        FinalResult = value[1] + " " + AUD;
                    }
                    Activity replyToConversation = activity.CreateReply("Convertion Information");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://msa.ms",
                        Type = "openUrl",
                        Title = "MSA Website"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "Visit Fixer.io",
                        Subtitle = "TFor more convertions" + FinalResult,
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                    Activity nzd = activity.CreateReply(FinalResult);
                    await connector.Conversations.ReplyToActivityAsync(nzd);
                }


                if (userMessage.ToLower().Contains("clear"))
                {
                    endOutput = "User data cleared";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    isWeatherRequest = false;
                }
                if (userMessage.ToLower().Equals("new timeline"))
                {
                    banktest timeline = new banktest()
                    {
                        CurrentAccount = 22,
                        SeriousSaver = 223,
                        Updatedat = DateTime.Now
                    };

                    await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                    isWeatherRequest = false;

                    endOutput = "New timeline added";
                }

                if (userMessage.ToLower().Equals("lmao"))
                {
                    List<banktest> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    FinalResult = " ";
                    foreach (banktest t in timelines)
                    {
                        FinalResult += "[" + t.FirstName + "] Firstname " + t.CurrentAccount + ", Cheque " + t.SeriousSaver + "\n\n";
                    }
                    isWeatherRequest = false;

                }

                if (!isWeatherRequest)
                {
                    // return our reply to the user
                    Activity infoReply = activity.CreateReply(endOutput);

                    await connector.Conversations.ReplyToActivityAsync(infoReply);

                }

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        //if(stockLUIS.intents.Count()>0)  
        //{  
        //    switch(stockLUIS.intents[0].intent)  
        //    {  
        //        case "StockPrice":  
        //            StockRateString = await GetStock(StLUIS.entities[0].entity);  
        //            break;  
        //private static async Task<stockLUIS> GetEntityFromLUIS(string Query)
        //{
        //    Query = Uri.EscapeDataString(Query);
        //    stockLUIS Data = new stockLUIS();
        //    using (HttpClient client = new HttpClient())
        //    {
        //        string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=7f626790-38d6-4143-9d46-fe85c56a9016&subscription-key=09f80de609fa4698ab4fe5249321d165&q=" + Query;
        //        HttpResponseMessage msg = await client.GetAsync(RequestURI);

        //        if (msg.IsSuccessStatusCode)
        //        {
        //            var JsonDataResponse = await msg.Content.ReadAsStringAsync();
        //            Data = JsonConvert.DeserializeObject<stockLUIS>(JsonDataResponse);
        //        }
        //    }
        //    return Data;
        //}
        //public async Task<Message> Post([FromBody]Message message)
        //{
        //    if (message.Type == "Message")
        //    {
        //        string StockRateString;
        //        stockLUIS StLUIS = await GetEntityFromLUIS(message.Text);
        //        if (StLUIS.intents.Count() > 0)
        //        {
        //            switch (StLUIS.intents[0].intent)
        //            {
        //                case "StockPrice":
        //                    StockRateString = await GetStock(StLUIS.entities[0].entity);
        //                    break;
        //                case "StockPrice2":
        //                    StockRateString = await GetStock(StLUIS.entities[0].entity);
        //                    break;
        //                default:
        //                    StockRateString = "Sorry, I am not getting you...";
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            StockRateString = "Sorry, I am not getting you...";
        //        }

        //        // return our reply to the user  
        //        return message.CreateReplyMessage(StockRateString);
        //    }
        //    else
        //    {
        //        return HandleSystemMessage(message);
        //    }
        //}
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}