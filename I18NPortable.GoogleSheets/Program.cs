using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Build.Evaluation;

namespace I18NPortable.GoogleSheets
{
    // If modifying these scopes, delete your previously saved credentials
    // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
    

    [Command(Description = "My global command line tool.")]
    class Program
    {
        

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "A positional parameter that must be specified.\nThe name of the person to greet.")]
        [Required]
        public string Name { get; }

        [Option(Description = "An optional parameter, with a default value.\nThe number of times to say hello.")]
        [Range(1, 1000)]
        public int Count { get; } = 1;

        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "I18NPortable.GoogleSheets.Tool";

        UserCredential credential;
        private int OnExecute()
        {
            Environment.SetEnvironmentVariable("MSBuildSDKsPath", 
                "C:\\Program Files\\dotnet\\sdk\\2.1.300\\Sdks");

            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", 
                "C:\\Program Files\\dotnet\\sdk\\2.1.300\\MSBuild.dll");

            var projectFile = Path.Combine(Directory.GetCurrentDirectory(), "I18NPortable.GoogleSheets.csproj");
            var project = new Project(projectFile);
            project.AddItem("Hey", "probando");
            Console.WriteLine(Directory.GetCurrentDirectory());

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-i18nportable-tool.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            String range = "Class Data";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine($"{row[0]}, {row[1]}, {row[2]}, {row[3]}");
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            return 0;
        }
    }
}
