// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.13.1

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {

        private Dictionary<string, string> en = new Dictionary<string, string>()
        {
            {"Echo","Echo:{0}"},
            {"Welcome","Hello and welcome!"}
        };

        private Dictionary<string, string> zh = new Dictionary<string, string>()
        {
            {"Echo","回音:{0}"},
            {"Welcome","欢迎!"}
        };

        private string GetLocalizedText(string key, string locale)
        {
            // 第一种方法，直接把资源定义在代码中
            // if (locale.ToLower().StartsWith("zh"))
            // {
            //     return zh[key];
            // }
            // else
            //     return en[key];

            // 第二种方式，将资源定义在配置文件中
            var lng = locale.ToLower().StartsWith("zh") ? "zh" : "en";
            return Configuration.GetValue<string>($"{lng}:{key}");
        }

        private IConfiguration Configuration;
        public EchoBot(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // 回复普通文本消息
            // var replyText = string.Format(GetLocalizedText("Echo", turnContext.Activity.Locale), turnContext.Activity.Text);
            // await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            // 回复卡片消息
            var lng = turnContext.Activity.Locale.ToLower().StartsWith("zh") ? "zh" : "en";
            var carddef = System.IO.File.ReadAllText($"Resources/cards/samplecard.{lng}.json");
            var cardParseResult = AdaptiveCard.FromJson(carddef);
            var message = MessageFactory.Attachment(new Attachment() { Content = cardParseResult.Card, ContentType = AdaptiveCard.ContentType });
            await turnContext.SendActivityAsync(message);

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = GetLocalizedText("Welcome", turnContext.Activity.GetLocale());
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
