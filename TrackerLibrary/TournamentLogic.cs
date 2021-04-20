using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        // Check if list is big enough, if not add in byes - 2^n

        // Create first round of matchups

        // Create every round after - matchups = previousMatchups/2

        public static void CreateRounds(TournamentModel tournament)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournament.EnteredTeams);
            int numberOfRounds = CalculateNumberOfRounds(randomizedTeams.Count);
            int byes = CalculateNumberOfByes(numberOfRounds, randomizedTeams.Count);

            tournament.Rounds.Add(CreateFirstRound(randomizedTeams, byes));
            CreateOtherRounds(tournament, numberOfRounds);
        }

        private static void CreateOtherRounds(TournamentModel tournament, int numberOfRounds)
        {
            int currentRoundNumber = 2;
            List<MatchupModel> previousRound = tournament.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            while(currentRoundNumber <= numberOfRounds)
            {
                foreach(var matchup in previousRound)
                {
                    currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = matchup });

                    if(currentMatchup.Entries.Count > 1)
                    {
                        currentMatchup.MatchupRound = currentRoundNumber;
                        currentRound.Add(currentMatchup);
                        currentMatchup = new MatchupModel();
                    }
                }

                tournament.Rounds.Add(currentRound);
                previousRound = currentRound;

                currentRound = new List<MatchupModel>();
                currentRoundNumber++;
            }
        }

        private static List<MatchupModel> CreateFirstRound(List<TeamModel> teams, int byes)
        {
            List<MatchupModel> rounds = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            foreach(var team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if(byes > 0 || currentMatchup.Entries.Count > 1)
                {
                    currentMatchup.MatchupRound = 1;
                    rounds.Add(currentMatchup);
                    currentMatchup = new MatchupModel();

                    if (byes > 0)
                        byes--;
                }
            }

            return rounds;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private static int CalculateNumberOfRounds(int teamCount)
        {
            int rounds = 1;
            int x = 2;

            while(x < teamCount)
            {
                rounds++;
                x *= 2;
            }

            return rounds;
        }

        private static int CalculateNumberOfByes(int rounds, int teamCount)
        {
            int byes = 0;
            int totalMatchups = 1;

            for (int i = 1; i <= rounds; i++)
                totalMatchups *= 2;

            //totalMatchups = Math.Pow(2, rounds)

            byes = totalMatchups - teamCount;

            return byes;
        }
    }
}
