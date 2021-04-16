﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        /// <summary>
        /// The team in the matchup.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }
        
        /// <summary>
        /// The score for this particular team.
        /// </summary>
        public double Score { get; set; }
        
        /// <summary>
        /// The matchup that this team came 
        /// from as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
    }
}
