// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. Objects that are expensive to construct, or have a lifetime
    /// beyond a single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class MyBot : IBot
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="MyBot"/> class.
        /// </summary>                        
        public MyBot()
        {
        }

        /// <summary>
        /// Every conversation turn calls this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types

            using (WebClient client = new WebClient())
            {

                string base_url = "http://ddragon.leagueoflegends.com/cdn/6.24.1/data/en_US/champion";
                
                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Echo back to the user whatever they typed.
                    string champion = turnContext.Activity.Text;
                    
                    string request_url = $"{base_url}/{champion}.json";
                    
                    string response = client.DownloadString(request_url);
                    
                    dynamic champion_data = JsonConvert.DeserializeObject(response);          

                    // var responseMessage = $"You said '{turnContext.Activity.Text}'\n";
                    var responseMessage = champion_data["data"][champion]["lore"];
                
                    Console.WriteLine(responseMessage);

                    await turnContext.SendActivityAsync($"{responseMessage}");
                }
                else
                {
                    await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
                }
            }
        }
    }
}
