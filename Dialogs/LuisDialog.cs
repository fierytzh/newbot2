using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;




namespace SearchBotUpdated.Dialogs
{
    [Serializable]
    [LuisModel("164e74e6-d29e-478e-98f8-9b32adef9734", "54aef1eba9cb46daa0f5f567629ac7ab")]
    public class LuisDialog : LuisDialog<bool>    // Originally, the interface inherited is LuisDialog<object> (Initial conversation design)
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            // This line returns the response back to the user.
            await context.PostAsync("Sorry, Luis did not find any  intent from your input message.");

            // This line "pushes" the current Dialog (LuisDialog) out of the IDialogContext class object, and returns a Boolean value back to the Dialog that called LuisDialog,
            // which is CommonResponseDialog.
            context.Done(true);
        }

        [LuisIntent("Document.GetPhysicalFile")]
        public async Task GetPhysicalFile(IDialogContext context, LuisResult result)
        {
            await ShowLuisResult(context, result);

            // This line instantiates the file search method, taking in the IDialogContext class object, context and LuisResult class object, result as parameters.
            // Take note that fileSearch method already contains context.PostAsync(). There is no need to instantiate another context.PostAsync() after this method
            // finishes its execution.
            await this.fileSearch(context, result);

            // This line "pushes" the current Dialog (LuisDialog) out of the IDialogContext class object, and returns a Boolean value back to the Dialog that called LuisDialog,
            // which is CommonResponseDialog.
            context.Done(true);
        }



        // fileSearch method
        private async Task fileSearch(IDialogContext context, LuisResult result)
        {
            // This line instantiates a variable that is determined as an empty string for now.
            string LuisTag = string.Empty;

            // This line instantiates a variable that is used to create a reply message back to the user.
            // The variable is determined as empty for now.
            var replyText = context.MakeMessage();

            // create an exception handler
            try
            {
                var FileFullPath = System.Web.HttpContext.Current.Server.MapPath("~/PSAFileCorpus/PSAfileinformation.csv");

                // This line instantiates a PSAFileInformation class List object.
                                                            // Next, it reads every rows in the csv file that is parsed in.
                List<Models.PSAFileInformation> files = File.ReadAllLines(FileFullPath)
                                                            // Skipping the first row because it contains the header for each column in the csv file.
                                                            .Skip(1)
                                                            // Selects the currentline that the reader is on and parse the entire line as a parameter
                                                            // to the FromCsv method found in PSAFileInformation class.
                                                            .Select(currentline => Models.PSAFileInformation.FromCsv(currentline))
                                                            // Store the instantiated PSAFileInformation class object into the
                                                            // instantiated PSAFileInformation class List object.
                                                            .ToList();



                // This "if" statement will be executed if there is at least one entity extracted from the user's input message by Luis Cognitive Service.
                if (result.Entities.Count > 0)
                {
                    // This "foreach" statement scans through all entities extracted by Luis Cognitive Service and returned to LuisDialog.
                    foreach (EntityRecommendation item in result.Entities)
                    {
                        // This "if" statement will be executed if the current entity being scanned in the "foreach" statement
                        // contains the "tag" label.
                        if (item.Type == "tag")
                        {
                            // Assigns the empty string instantiated at the start of the method with the raw string extracted directly
                            // from the user's input query by Luis Cognitive Service.
                            LuisTag = item.Entity.ToString();
                        }
                        // This "else" statement will be executed if the "if" condition(s) are not met above.
                        else
                        {
                            // The empty string variable instantiated at the start of the method will continue to remain
                            // as an empty string or a null value.
                            LuisTag = null;
                        }
                    }
                }



                // This line instantiates another new PSAFileInformation class List object.
                List<Models.PSAFileInformation> filteredfiles = new List<Models.PSAFileInformation>();

                // This "if" statement will be executed if the variable, LuisTag, is not Null or empty.
                if (!string.IsNullOrWhiteSpace(LuisTag))
                {
                    // This "for" statement scans through each PSAFileInformation class object stored in the PSAFileInformation class List object.
                    for (int i = 0; i < files.Count; i++)
                    {
                        // This "if" statement will be executed if the current PSAFileInformation class object's attribute, fileName, contains
                        // the LuisTag variable.
                        if (files[i].fileName.ToLower().Contains(LuisTag))
                        {
                            // Add the current PSAFileInformation class object into the newly instantiated PSAFileInformation class List object,
                            // filteredfiles.
                            filteredfiles.Add(files[i]);
                        }
                    }
                    // This "if" statement will be executed if the number of PSAFileInformation class objects stored in
                    // PSAFileInformation class List object, filteredfiles, equals 0.
                    if (filteredfiles.Count == 0)
                    {
                        // This "for" statement scans through each PSAFileInformation class object stored in the PSAFileInformation class List object
                        for (int i = 0; i < files.Count; i++)
                        {
                            // This "if" statement will be executed if the current PSAFileInformation class object's attribute, fileTags, contains
                            // the LuisTag variable.
                            if (files[i].fileTags.ToLower().Contains(LuisTag))
                            {
                                // Add the current PSAFileInformation class object into the PSAFileInformation class List object, filteredfiles again.
                                filteredfiles.Add(files[i]);
                            }
                        }
                    }
                }
                // This "else if" statement will be executed if the variable, LuisTag, is Null or empty.
                else if (string.IsNullOrWhiteSpace(LuisTag))
                {
                    // Assigns the following string text to the Text attribute of the replyText variable instantiated at the start of the method.
                    replyText.Text = "Sorry, I could not find the file information that you requested.";
                }



                // This "if" statement will be executed if the number of PSAFileInformation class objects stored in
                // PSAFileInformation class List object, filteredfiles, equals 0.
                if (filteredfiles.Count == 0)
                {
                    // Assigns the following string text to the Text attribute of the replyText variable instantiated at the start of the method.
                    replyText.Text = $"There is no files regarding {LuisTag} that I have detected in your request message. Try searching again with request message.";
                }
                // This "else" statement will be executed if the "if" condition(s) are not met above.
                else
                {
                    // Assigns the following string text to the Text attribute of the replyText variable instantiated at the start of the method.
                    replyText.Text = $"I got {filteredfiles.Count} files on {LuisTag} detected by Luis Cognitive Service.\n\n";

                    // This "for" statement scans through each PSAFileInformation class object stored in the PSAFileInformation class List object.
                    for (int i = 0; i < filteredfiles.Count; i++)
                    {
                        // Add on to the Text attribute of the replyText variable with the following string.
                        replyText.Text += $"FileName: {filteredfiles[i].fileName}\n\nFileLocation: {filteredfiles[i].fileLocation}\n\n";                                             
                    }
                }

                // This line returns the response back to the user.
                await context.PostAsync(replyText);
            }

            catch(Exception ex)
            {
                // appends the error message to the instantiated string variable.
                string errorMsg = ex.Message;
                // This line returns the response back to the user.
                await context.PostAsync(errorMsg);
            }
        }



        // method to find intent and entities extracted and output as a string
        // use this to test your intent if you decide to add a new one. helps to check if your new intent works before implementing custom codes
        // in your new intent
        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            string entities = BotEntityRecognition(result);

            string roundedScore = result.Intents[0].Score != null ? (Math.Round(result.Intents[0].Score.Value, 2).ToString()) : "0";

            await context.PostAsync($"**Query**: {result.Query}, **Intent**: {result.Intents[0].Intent}, **Score**: {roundedScore}. **Entities**: {entities}");
        }
        public string BotEntityRecognition(LuisResult result)
        {
            StringBuilder entityResults = new StringBuilder();

            if(result.Entities.Count > 0)
            {
                foreach(EntityRecommendation item in result.Entities)
                {
                    entityResults.Append(item.Type + "=" + item.Entity + ",");
                }

                // remove last comma
                entityResults.Remove(entityResults.Length - 1, 1);
            }

            return entityResults.ToString();
        }
    }
}