// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog1 : LogoutDialog
    {
        protected readonly ILogger Logger;
        protected IConfiguration configuration;

        public MainDialog1(IConfiguration configuration, ILogger<MainDialog1> logger)
            : base(nameof(MainDialog1), configuration["ConnectionName"])
        {
            Logger = logger;
            this.configuration = configuration;

            AddDialog(new OAuthPrompt(
                nameof(OAuthPrompt),
                new OAuthPromptSettings
                {
                    ConnectionName = ConnectionName,
                    Text = "Please Sign In",
                    Title = "Sign In",
                    Timeout = 60000 // User has 1 min to login (1000 * 60)
                }));

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                PromptStepAsync,
                DisplayTokenPhase1Async,
                DisplayTokenPhase2Async,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var result = stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
            stepContext.Context.SendActivityAsync(MessageFactory.Text("Sign in first, then select 'Yes' to proceed."), cancellationToken);

            result.Wait();

            if (result.Result != null)
            {
                stepContext.Context.SendActivityAsync(MessageFactory.Text($"{result.Result.Result}"), cancellationToken);
                return stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Proceed?") }, cancellationToken);
            }
            else
            {
                stepContext.Context.SendActivityAsync(MessageFactory.Text("Login was not successful please try again."), cancellationToken);
                return stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

        }

        private Task<DialogTurnResult> DisplayTokenPhase1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = (bool)stepContext.Result;
            if (result)
            {
                return stepContext.BeginDialogAsync(nameof(OAuthPrompt), cancellationToken: cancellationToken);
            }

            return stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayTokenPhase2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is your token {tokenResponse.Token}"), cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
