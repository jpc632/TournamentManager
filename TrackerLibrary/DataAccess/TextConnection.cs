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

        /// <summary>
        /// Saves a new prize to the text file.
        /// </summary>
        /// <param name="model">The prize model.</param>
        /// <returns>The prize model.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
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

            model.Id = newId;

            // Add prize with newId
            prizes.Add(model);

            // Overwrite text file with the List<String>
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

    }
}
