﻿using Spectre.Console;
using System.Globalization;

namespace coding_tracker
{
    public class UserInput
    {
        private DatabaseController databaseController = new();

        public void Menu()
        {
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");

                AnsiConsole.Write(new Rows(
            new Text("Type 0 to Close Application.", new Style(Color.Red, Color.Black)),
            new Text("Type 1 to View records"),
            new Text("Type 2 to Add records"),
            new Text("Type 3 to Delete records"),
            new Text("Type 4 to Update records")

        ));

                var command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        closeApp = true;
                        break;

                    case "1":
                        Get("Press any key to back to menu");
                        break;

                    case "2":
                        Add();
                        break;

                    case "3":
                        ProcessDelete();
                        break;

                    case "4":
                        Update();
                        break;

                    default:
                        Console.Clear();
                        AnsiConsole.Markup("[red]Input invalid please choose a number between 1 and 4[/]");
                        break;
                }
            }
        }

        public void Get(string message)
        {
            Console.WriteLine("---------------------------");

            databaseController.Read();

            Console.WriteLine("---------------------------");
            Console.WriteLine(message);
            Console.ReadLine();
            Console.Clear();
        }

        public void Add()
        {
            Console.Clear();
            var coding = new CodingSession();

            coding.Date = DateTime.UtcNow.ToString("dd/MM/yy");

            var startTime = GetStartEndTime("Please insert the start time in format (hh:mm)");

            coding.StartTime = DateTime.ParseExact(startTime, "HH:mm", CultureInfo.InvariantCulture);

            var endTime = GetStartEndTime("Press insert your end time also in format (hh:mm)");

            coding.EndTime = DateTime.ParseExact(endTime, "HH:mm", CultureInfo.InvariantCulture);
            while (coding.EndTime < coding.StartTime)
            {
                AnsiConsole.Markup("[red]End time can not be less than start time!\n");

                endTime = Console.ReadLine();
                coding.EndTime = DateTime.ParseExact(endTime, "HH:mm", CultureInfo.InvariantCulture);
            }

            var duration = GetDuration(startTime, endTime);
            coding.Duration = duration;

            databaseController.Insert(coding);
            Console.WriteLine("Press any key to show the duration of your coding session ");
            Console.ReadLine();
            Console.WriteLine(duration);
        }

        public void ProcessDelete()
        {
            Console.Clear();
            var coding = new CodingSession();
            databaseController.Read();

            Console.Write("Write the number of the id you want to delete: ");
            var id = Console.ReadLine();

            while (!Int32.TryParse(id, out _) || Convert.ToInt32(id) < 0)
            {
                AnsiConsole.Markup("[red]Id invalid please type another id[/]");
                id = Console.ReadLine();
            }
            coding.Id = Convert.ToInt32(id);

            databaseController.Delete(coding);
        }

        public void Update()
        {
            Console.Clear();
            var coding = new CodingSession();

            databaseController.Read();
            Console.WriteLine("Please select the id number you want to update");

            var id = Console.ReadLine();
            while (string.IsNullOrEmpty(id))
            {
                AnsiConsole.Markup("[red]Input can not be empty[/]\n");
                id = Console.ReadLine();
            }

            coding.Id = Convert.ToInt32(id);
            var startTime = GetStartEndTime("Please type the start time you want to update in format (hh:mm)");
            coding.StartTime = DateTime.ParseExact(startTime, "HH:mm", CultureInfo.InvariantCulture);
            var endTime = GetStartEndTime("Please type the end time you want to update in format (hh:mm)");

            coding.EndTime = DateTime.ParseExact(endTime, "HH:mm", CultureInfo.InvariantCulture);
            while (coding.EndTime < coding.StartTime)
            {
                AnsiConsole.Markup("[red]End time can not be less than start time![/]\n");

                endTime = Console.ReadLine();
                coding.EndTime = DateTime.ParseExact(endTime, "HH:mm", CultureInfo.InvariantCulture);
            }

            var duration = GetDuration(startTime, endTime);

            coding.Duration = duration;
            databaseController.Update(coding);
        }

        public string GetStartEndTime(string message)
        {
            Console.WriteLine(message);
            var startTimeInput = Console.ReadLine();

            while (!DateTime.TryParseExact(startTimeInput, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Console.WriteLine("[red]Input not valid please try again[/]");

                startTimeInput = Console.ReadLine();
            }

            return startTimeInput;
        }

        public string GetDuration(string startTime, string endTime)
        {
            var startDateTime = DateTime.ParseExact(startTime, "HH:mm", CultureInfo.InvariantCulture);

            var endDateTime = DateTime.ParseExact(endTime, "HH:mm", CultureInfo.InvariantCulture);

            TimeSpan duration = endDateTime - startDateTime;

            return $"{(int)duration.TotalHours:D2} hours : {duration.Minutes:D2} minutes";
        }
    }
}