using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BestMatchDialog;
using SearchBotUpdated.Database;
using System.Collections.Generic;
using Npgsql;
namespace SearchBotUpdated.Dialogs
{

    [Serializable]
    public class CommonResponseDialog : BestMatchDialog<object>   // Originally, the interface inherited is BestMatchDialog<bool> (Initial conversation design)
    {
       // DatabaseHelper db;
        
        [BestMatch(new string[] { "Hi", "Hi There", "Hello There", "Hey", "Hello", "Hey There", "Greetings", "Good morning", "Good afternoon", "Good evening", "Good day" },
            threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: true)]
        public async Task HandleGreeting(IDialogContext context, string message)
        {
            DatabaseHelper db = new DatabaseHelper();
            db.openConnection();
            
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.company ORDER BY id ASC", db.connection);

            //// Prepare the command.
            command.Prepare();

            //// Execute SQL command.
            //NpgsqlDataReader dr = command.ExecuteReader();
            db.closeConnection();
            //  String result = db.get();
            //    await context.PostAsync(result);
            //db.get();
            // This line instantiates a Random class object
            Random numberGenerator = new Random();

                // This variable takes in a randomly generated number from a range of 1 to 3.
                int responseIndex = numberGenerator.Next(1, 4);
            
            {

                // This switch statement returns a response based on which random number is assigned to variable "responseIndex". 
                switch (responseIndex)
                {
                    case 1:
                        await context.PostAsync("gd");
                        break;
                    case 2:
                        await context.PostAsync("hi");
                        break;
                    default:
                        await context.PostAsync("abc");
                        break;
                }
                // This line suspends the Dialog and will be continued from where it left off if a new input message from the user is passed to this Dialog.
                context.Wait(MessageReceived);
            }
           

        }


        [BestMatch(new string[] { "How goes it", "how do", "hows it going", "how are you", "how do you feel", "whats up", "sup", "hows things" },
            threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: true)]
        public async Task HandleStatusRequest(IDialogContext context, string message)
        {
            // This line instantiates a Random class object
            Random numberGenerator = new Random();

            // This variable takes in a randomly generated number from a range of 1 to 3.
            int responseIndex = numberGenerator.Next(1, 4);

            // This switch statement returns a response based on which random number is assigned to variable "responseIndex".
            switch (responseIndex)
            {
                case 1:
                    await context.PostAsync("Feeling great, my man!");
                    break;
                case 2:
                    await context.PostAsync("Awesome, baby!");
                    break;
                default:
                    await context.PostAsync("I am great. Thanks for your concern.");
                    break;
            }

            // This line suspends the Dialog and will be continued from where it left off if a new input message from the user is passed to this Dialog.
            context.Wait(MessageReceived);
        }


        [BestMatch(new string[] { "Bye", "Bye bye", "Got to go", "See you later", "Laters", "Adios" },
            threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: true)]
        public async Task HandleGoodbye(IDialogContext context, string message)
        {
            // This line instantiates a Random class object
            Random numberGenerator = new Random();

            // This variable takes in a randomly generated number from a range of 1 to 3.
            int responseIndex = numberGenerator.Next(1, 4);

            // This switch statement returns a response based on which random number is assigned to variable "responseIndex".
            switch (responseIndex)
            {
                case 1:
                    await context.PostAsync("Sayonara, my friend!");
                    break;
                case 2:
                    await context.PostAsync("See you soon.");
                    break;
                default:
                    await context.PostAsync("Bye, have a great day ahead.");
                    break;
            }

            // This line suspends the Dialog and will be continued from where it left off if a new input message from the user is passed to this Dialog.
            context.Wait(MessageReceived);
        }


        [BestMatch(new string[] { "Thank you", "Thanks", "Much appreciated", "Thanks very much", "Thanking you" },
            threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: true)]
        public async Task HandleThanks(IDialogContext context, string message)
        {
            // This line instantiates a Random class object
            Random numberGenerator = new Random();

            // This variable takes in a randomly generated number from a range of 1 to 3.
            int responseIndex = numberGenerator.Next(1, 4);

            // This switch statement returns a response based on which random number is assigned to variable "responseIndex".
            switch (responseIndex)
            {
                case 1:
                    await context.PostAsync("Glad to be of use to you.");
                    break;
                case 2:
                    await context.PostAsync("You're welcome.");
                    break;
                default:
                    await context.PostAsync("No problemo, buddy.");
                    break;
            }

            // This line suspends the Dialog and will be continued from where it left off if a new input message from the user is passed to this Dialog.
            context.Wait(MessageReceived);
        }

        public override async Task NoMatchHandler(IDialogContext context, string message)
        {
            // This line creates a new Dialog called LuisDialog and passes the activity message to it to execute Tasks that are located in the Dialog.
            await context.Forward(new LuisDialog(), NoMatchCompletionHandler, context.Activity.AsMessageActivity(), CancellationToken.None);
        }

        // In the NoMatchCompletionHandler Task, if you do not want to post a message back to the user,
        // you can call a qna dialog using context.Forward() to check if the query is directed towards the qnamaker.ai
        // For example, use qnamaker.ai to handle FAQs if you believe that the user's input is a FAQ.
        private async Task NoMatchCompletionHandler(IDialogContext context, IAwaitable<bool> result)
        {
            // This variable takes in a Boolean value from the Boolean value that is being parsed into the method as a parameter.
            bool LuisCompletion = await result;

            // This "if" statement will be executed if the Boolean value from the variable, "LuisCompletion" is false.
            if (!LuisCompletion)
            {
                // Returns the response back to the user.
                await context.PostAsync("Sorry, I don't understand. Please rephrase.");
            }

            // This line suspends the Dialog and will be continued from where it left off if a new input message from the user is passed to this Dialog.
            context.Wait(MessageReceived);
        }
    }
}