using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnection : IDataConnection
    {
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PeopleModels.csv";
        private const string TeamFile = "TeamModels.csv";

        public PersonModel CreatePerson(PersonModel person)
        {
            // Load the text file
            // Convert the text to List<PrizeModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the max ID
            int newId = 1;

            if (people.Count > 0)
            {
                newId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            person.Id = newId;

            // Add person with newId
            people.Add(person);

            // Overwrite text file with the List<String>
            people.SaveToPeopleFile(PeopleFile);

            return person;
        }

        /// <summary>
        /// Saves a new prize to the text file.
        /// </summary>
        /// <param name="prize">The prize model.</param>
        /// <returns>The prize model.</returns>
        public PrizeModel CreatePrize(PrizeModel prize)
        {
            // Load the text file
            // Convert the text to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the max ID
            int newId = 1;

            if(prizes.Count > 0)
            {
                newId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            prize.Id = newId;

            // Add prize with newId
            prizes.Add(prize);

            // Overwrite text file with the List<String>
            prizes.SaveToPrizeFile(PrizesFile);

            return prize;
        }

        /// <summary>
        /// Saves a new team to the text file.
        /// </summary>
        /// <param name="team">The team model.</param>
        /// <returns>The team model.</returns>
        public TeamModel CreateTeam(TeamModel team)
        {
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            // Find the max ID
            int newId = 1;

            if (teams.Count > 0)
            {
                newId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            team.Id = newId;

            // Add prize with newId
            teams.Add(team);

            // Overwrite text file with the List<String>
            teams.SaveToTeamFile(TeamFile);

            return team;
        }

        public TournamentModel CreateTournament(TournamentModel tournament)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all people in the text file.
        /// </summary
        /// <returns>The list of people.</returns>
        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }
    }
}
