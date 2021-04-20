using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectionProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        public static List<string> LoadFile(this string filePath)
        {
            if(!File.Exists(filePath))
            {
                return new List<string>();
            }

            return File.ReadAllLines(filePath).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach(string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel model = new PrizeModel();
                model.Id = int.Parse(cols[0]);
                model.PlaceNumber = int.Parse(cols[1]);
                model.PlaceName = cols[2];
                model.PrizeAmount = decimal.Parse(cols[3]);
                model.PrizePercentage = double.Parse(cols[4]);
                output.Add(model);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach(var model in models)
            {
                lines.Add($"{ model.Id },{ model.PlaceNumber },{ model.PlaceName },{ model.PrizeAmount },{ model.PrizePercentage }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel model = new PersonModel();
                model.Id = int.Parse(cols[0]);
                model.FirstName = cols[1];
                model.LastName = cols[2];
                model.EmailAddress = cols[3];
                model.CellphoneNumber = cols[4];
                output.Add(model);
            }

            return output;
        }

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (var model in models)
            {
                lines.Add($"{ model.Id },{ model.FirstName },{ model.LastName },{ model.EmailAddress },{ model.CellphoneNumber }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                
                TeamModel team = new TeamModel();
                team.Id = int.Parse(cols[0]);
                team.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    team.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(team);
            }

            return output;
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
                return "";

            foreach (PersonModel person in people)
            {
                output += $"{ person.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (var model in models)
            {
                lines.Add($"{ model.Id },{ model.TeamName },{ ConvertPeopleListToString(model.TeamMembers) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static List<TournamentModel> ConvertToTournamentModels(
            this List<string> lines, 
            string teamFileName, 
            string peopleFileName, 
            string prizesFileName
        )
        {
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName
                .FullFilePath()
                .LoadFile()
                .ConvertToTeamModels(peopleFileName);

            List<PrizeModel> prizes = prizesFileName
                .FullFilePath()
                .LoadFile()
                .ConvertToPrizeModels();

            foreach (var line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tournament = new TournamentModel();
                tournament.Id = int.Parse(cols[0]);
                tournament.TournamentName = cols[1];
                tournament.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach(var teamId in teamIds)
                {
                    tournament.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(teamId)).First());
                }

                string[] prizeIds = cols[4].Split('|');

                foreach(var prizeId in prizeIds)
                {
                    tournament.Prizes.Add(prizes.Where(x => x.Id == int.Parse(prizeId)).First());
                }
                // TODO - Capture round information

                output.Add(tournament);
            }

            return output;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
                return "";

            foreach (var team in teams)
            {
                output += $"{ team.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
                return "";

            foreach (var prize in prizes)
            {
                output += $"{ prize.Id }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
                return "";

            foreach (var round in rounds)
            {
                output += $"{ ConvertMatchupListToString(round) }|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
                return "";

            foreach (var matchup in matchups)
            {
                output += $"{ matchup.Id }^";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (var model in models)
            {
                lines.Add($@"{ model.Id },{ model.TournamentName },{ model.EntryFee },
                    { ConvertTeamListToString(model.EnteredTeams) },
                    { ConvertPrizeListToString(model.Prizes) },
                    { ConvertRoundListToString(model.Rounds) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
    }
}
