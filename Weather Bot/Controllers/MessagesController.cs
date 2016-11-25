using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using ContosoBank.Models;
using System.Collections.Generic;
using System.Web.Services.Description;
using ContosoBank.DataModels;
using Google.Maps.Geocoding;

namespace ContosoBank
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private string FinalResult;
        private string userMessage;
        public Boolean helpfound = false;
        public Boolean greeting1 = false;
        public Boolean about = false;
        public Boolean currencylist = false;
        public Boolean thankyou = false;



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
                string endOutput = "Hello";

                // calculate something for us to return

                string StockRateString;
                Rootobject StLUIS = await GetEntityFromLUIS(activity.Text);
                if (StLUIS.intents.Count() > 0)
                {
                    switch (StLUIS.intents[0].intent)
                    {
                        case "helpcommand":
                            StockRateString = GetHelp(StLUIS.entities[0].entity);
                            helpfound = true;
                            break;
                        case "greeting":
                            StockRateString = getGreeting(StLUIS.entities[0].entity);
                            greeting1 = true;
                            break;
                        case "aboutme":
                            StockRateString = getAbout(StLUIS.entities[0].entity);
                            about = true;
                            break;
                        case "currencylist":
                            StockRateString = getcurrencylist(StLUIS.entities[0].entity);
                            currencylist = true;
                            break;
                        case "Thankyou":
                            StockRateString = getAppreciation(StLUIS.entities[0].entity);
                            thankyou = true;
                            break;
                        default:
                            StockRateString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    StockRateString = "Sorry, I am not getting you...";
                    Activity sorry = activity.CreateReply(StockRateString);
                    await connector.Conversations.ReplyToActivityAsync(sorry);
                }
                if (thankyou == true)
                {
                    Activity replyToConversation = activity.CreateReply("You are welcome " + activity.From.Name);
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "help",
                        Type = "imBack",
                        Title = "yes"
                    };
                    cardButtons.Add(plButton);
                    CardAction plButton1 = new CardAction()
                    {
                        Value = "bye",
                        Type = "imBack",
                        Title = "No"
                    };
                    cardButtons.Add(plButton1);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "Do you need help with more commands?",
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (about == true)
                {
                    endOutput = "Hello " + activity.From.Name + " This bot is intended for JazBot";                   
                    Activity shoes1 = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(shoes1);

                    Activity replyToConversation = activity.CreateReply("About me");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://s13.postimg.org/ahasnmoqf/Screen_Shot_2016_11_25_at_3_30_27_AM.png"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://msa.ms",
                        Type = "openUrl",
                        Title = "More information"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Text = "I am here to make your job easier, ask me conversion rates, opening hours, closest ATM's or to retreive your bank balance",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                if(currencylist == true)
                {
                    Activity replyToConversation = activity.CreateReply("Currencies");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    HeroCard plCard = new HeroCard()
                    {
                        Text = "Use 'AUD' for Australian dollar" + "\n\n " + "Use 'JPY' for Japanese dollar" + "\n\n" + "Use 'INR' for Indian Rupees"

                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                if (greeting1 == true)
                {
                    endOutput = "Hello " + activity.From.Name + "\n\n" + "Welcome to the Jaz Bank bot!, If you need help guiding through this bot. Type 'help'";

                    if (userData.GetProperty<bool>("SentGreeting"))
                    {
                        endOutput = "Hello again " + activity.From.Name;
                    }
                    else
                    {
                        userData.SetProperty<bool>("SentGreeting", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                    Activity shoes = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(shoes);
                }
                if (helpfound == true)
                {
                    Activity replyToConversation = activity.CreateReply("");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://s18.postimg.org/vg9pw5kwp/Screen_Shot_2016_11_25_at_3_48_58_AM.png"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    HeroCard plCard = new HeroCard()
                    {
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
               
                bool isWeatherRequest = true;
                var userMessage = activity.Text;

                currency.RootObject Root12;
                HttpClient client = new HttpClient();
                string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=NZD"));
                Root12 = JsonConvert.DeserializeObject<currency.RootObject>(x);

                if (userMessage.ToLower().Contains("conversion rate"))
                {
                    string[] value = userMessage.Split(' ');
                    double AUD = Root12.rates.AUD;
                    double JPY = Root12.rates.JPY;
                    double INR = Root12.rates.INR;
                    double BGN = Root12.rates.BGN;
                    string LOL = value[4];
                    double lolhaha = Convert.ToDouble(LOL);
                    if (value[2].ToLower() == "aud")
                    {
                        FinalResult = "The current " + value[2] + " rate is " + AUD + "\n\n " + "converting " + value[4] + " NZD = " + AUD * lolhaha + " AUD";
                    }
                    if (value[2].ToLower() == "jpy")
                    {
                        FinalResult = "The current " + value[2] + " rate is " + JPY + "\n\n " + "converting " + value[4] + " NZD = " + JPY * lolhaha + " JPY";
                    }
                    if (value[2].ToLower() == "inr")
                    {
                        FinalResult = "The current " + value[2] + " rate is " + INR + "\n\n " + "converting " + value[4] + " NZD = " + INR * lolhaha + " INR";
                    }
                    if (value[2].ToLower() == "bgn")
                    {
                        FinalResult = "The current " + value[2] + " rate is " + BGN + "\n\n " + "converting " + value[4] + " NZD = " + BGN * lolhaha + " BGN";
                    }

                    Activity nzd = activity.CreateReply(FinalResult);
                    await connector.Conversations.ReplyToActivityAsync(nzd);
                }
               
                if (userMessage.ToLower().Contains("clear"))
                {
                    endOutput = "User data cleared";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    isWeatherRequest = false;
                }
                if (userMessage.ToLower().Contains("change my name from"))
                {
                    string[] swag = userMessage.Split(' ');
                    List<BankBot> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    foreach (BankBot t in timelines)
                    {
                        if (swag[4].ToLower() == t.Name)
                        {
                            t.Name = swag[6];
                            endOutput = t.Name;
                            await AzureManager.AzureManagerInstance.UpdateTimeline(t);
                            Activity update = activity.CreateReply("We have updated your name to " + t.Name);
                            await connector.Conversations.ReplyToActivityAsync(update);
                        }
                        
                    }
                    
                }
                if (userMessage.ToLower().Contains("update my balance"))
                {
                    Activity name123 = activity.CreateReply("Please type: '<name and deposit {money}' e.g. <jayraj and deposit 200> ");
                    await connector.Conversations.ReplyToActivityAsync(name123);
                }
                if (userMessage.ToLower().Contains("deposit"))
                {
                    string[] dep = userMessage.Split(' ');
                    Boolean found = false;
                    List<BankBot> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    int currentA = 0;
                    int depositv = 0;
                    foreach (var t in timelines)
                    {
                        currentA = Int32.Parse(t.CurrentAccount);
                        depositv = Int32.Parse(dep[3]);
                        if (dep[0].ToLower().Equals(t.Name.ToLower()))
                        {
                            t.Name = dep[0];
                            currentA += depositv;
                            t.CurrentAccount = "" + currentA;
                            await AzureManager.AzureManagerInstance.UpdateTimeline(t);
                            Activity updatenam11e = activity.CreateReply("Hello " + dep[0] + " we have deposited $" + depositv + " you now have $" + t.CurrentAccount);
                            await connector.Conversations.ReplyToActivityAsync(updatenam11e);
                            return Request.CreateResponse(HttpStatusCode.OK);
                            found = true;
                        }

                    };
                    if (found == false)
                    {
                        Activity updatename1 = activity.CreateReply("error: No customer name found in database");
                        await connector.Conversations.ReplyToActivityAsync(updatename1);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    isWeatherRequest = false;
                }

                if (userMessage.ToLower().Contains("calculate interest rate for"))
                {
                    string[] interest = userMessage.Split(' ');
                    List<BankBot> timeline = await AzureManager.AzureManagerInstance.GetTimelines();
                    endOutput = "";
                    Random rndm = new Random();
                    int distance = rndm.Next(1, 10);
                    foreach (var t in timeline)
                    {
                        if (interest[4].ToLower().Equals(t.Name.ToLower()))
                        {
                            Activity updatehours = activity.CreateReply("The interest rate for " + interest[4] + "'s saving account is " + distance + "%");
                            await connector.Conversations.ReplyToActivityAsync(updatehours);
                        }
                    }
                }

                if (userMessage.ToLower().Contains("change times"))
                {
                    string[] day = userMessage.Split(' ');
                    List<Timings> timelined = await Amanager.AzureManagerInstance2.GetTimelines();
                    endOutput = "";
                    string current = day[2];
                    foreach(Timings timing in timelined)
                    {
                       if (current.ToLower() == timing.Day.ToLower())
                        {
                            timing.Hours = day[3];
                            await Amanager.AzureManagerInstance2.UpdateTimetable1(timing);
                            Activity updatehours = activity.CreateReply("Hours updated for " + day[2] + " " + day[3]);
                            await connector.Conversations.ReplyToActivityAsync(updatehours);
                        }
                    }
                }

                if (userMessage.ToLower().Contains("get customer data"))
                {
                    string[] splitting = userMessage.Split(' ');
                    Boolean found1 = false;
                    List<BankBot> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    endOutput = "";
                    string l = splitting[3];
                    foreach (BankBot t in timelines)
                    {
                        if (l == t.Name)
                        {
                            endOutput += "Customer: " + t.Name + ", current account: $" + t.CurrentAccount + ", Savings: $" + t.Savings + "\n\n";
                            found1 = true;
                        }
                    };
                    if (found1 == false)
                    {
                        Activity updatename1 = activity.CreateReply("error: No customer name found in database");
                        await connector.Conversations.ReplyToActivityAsync(updatename1);
                    }
                    isWeatherRequest = false;
                }

                if (userMessage.ToLower().Contains("delete customer"))
                {
                    string[] delete = userMessage.Split(' ');
                    string namedelete = delete[2];
                    List<BankBot> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    foreach (var i in timelines)
                    {
                        if (namedelete.ToLower() == i.Name.ToLower())
                        {
                            await AzureManager.AzureManagerInstance.DeleteTimeline(i);
                        }
                    }
                    Activity deleteaccount = activity.CreateReply("We have deleted your account :) Thank you for being Jaz Bank's Lovely customer");
                    await connector.Conversations.ReplyToActivityAsync(deleteaccount);
                }

                if (userMessage.ToLower().Contains("hours"))
                {
                    string[] valued = userMessage.Split(' ');
                    List<Timings> timelines1 = await Amanager.AzureManagerInstance2.GetTimelines();
                    endOutput = "";
                    foreach (Timings t in timelines1)
                    {
                        endOutput += "Day: " + t.Day + " ||  Hours: " + t.Hours + "\n\n";
                    }
                    isWeatherRequest = false;

                }

                if (userMessage.ToLower().Contains("bye"))
                {
                    Activity nzd = activity.CreateReply("Goodbye " + activity.From.Name);
                    await connector.Conversations.ReplyToActivityAsync(nzd);
                }

                if (userMessage.ToLower().Contains("new customer"))
                {
                    string[] value = userMessage.Split(' ');
                    BankBot timeline = new BankBot()
                    {
                        Name = value[2],
                        CurrentAccount = value[3],
                        Savings = value[4],
                        Date = DateTime.Now
                    };

                    await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                    isWeatherRequest = false;

                    endOutput = "New JazBank member " + value[2] + " added " + "[" + timeline.Date + "]";
                }

                if (userMessage.ToLower().Contains("closest atm near"))
                {
                    string[] value = userMessage.Split(' ');
                    Random rndm = new Random();
                    int distance = rndm.Next(1, 3);
                    value[3] = distance.ToString();

                    Activity replyToConversation = activity.CreateReply("ATM's near by");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcSRgwTwj2hrZQSeN7BRUikzEaNoitlKtwPOUyvBIBr24Fe1oca8rsc9lDg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://msa.ms",
                        Type = "openUrl",
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Subtitle = "Closest ATM to you is... " + value[2] + "KM",
                        Title = value[3] + " KM",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                    Activity nzd = activity.CreateReply(value[2]);
                    await connector.Conversations.ReplyToActivityAsync(nzd);

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
        private static async Task<Rootobject> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            Rootobject Data = new Rootobject();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v2.0/apps/694bc628-ff01-4852-9578-4213ddce56a7?subscription-key=2c6e21452e0944f7b9157d2ef14cefec&q=" + Query + "&verbose=true";
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<Rootobject>(JsonDataResponse);
                }
            }
            return Data;
        }
        private string GetHelp(string StockSymbol)
        {
            return StockSymbol;
        }
        private string getGreeting(string StockSymbol)
        {
            return StockSymbol;
        }
        private string getAbout(string StockSymbol)
        {
            return StockSymbol;
        }
        private string getcurrencylist(string StockSymbol)
        {
            return StockSymbol;
        }
        private string getAppreciation(string StockSymbol)
        {
            return StockSymbol;
        }
    }
}
