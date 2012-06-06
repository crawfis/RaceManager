using System;
using System.Collections.Generic;

namespace Hardcard.Scoring.Simulator
{
    internal class TestTagProcessor : ILineProcessStrategy
    {
        #region ILineProcessStrategy Members
        public TagInfo Process(string line)
        {
            Dictionary<string, string> tokenPairs = ParseLine(line);
            //long id = System.Int64.Parse(tokenPairs["tag"], System.Globalization.NumberStyles.AllowHexSpecifier);
            TagId id = new TagId();
            if (!tokenPairs.ContainsKey("tag"))
                throw new ApplicationException("The keyword tag does not exist in the read packet: " + line);
            id.Value = tokenPairs["tag"];
            if (!tokenPairs.ContainsKey("freq"))
                throw new ApplicationException("The keyword freq does not exist in the read packet: " + line);
            float frequency = System.Single.Parse(tokenPairs["freq"]);
            if (!tokenPairs.ContainsKey("sig"))
                throw new ApplicationException("The keyword sig does not exist in the read packet: " + line);
            float signalStrength = System.Single.Parse(tokenPairs["sig"]);
            if (!tokenPairs.ContainsKey("ant"))
                throw new ApplicationException("The keyword ant does not exist in the read packet: " + line);
            int antenna = System.Int32.Parse(tokenPairs["ant"]);
            if (!tokenPairs.ContainsKey("time"))
                throw new ApplicationException("The keyword time does not exist in the read packet: " + line);
            long time = System.Int64.Parse(tokenPairs["time"]);
            TagInfo tagInfo = new TagInfo(id, frequency, signalStrength, antenna, time);
            return tagInfo;
        }

        private Dictionary<string, string> ParseLine(string line)
        {
            string[] tokens = line.Split(':', ',', ' ', '\t', '\n', '\0');

            Dictionary<string, string> tokenPairs = new Dictionary<string, string>(tokens.Length);
            string lastKey = null;
            foreach (string token in tokens)
            {
                if (token != String.Empty)
                {
                    //System.Console.WriteLine(token);
                    if (lastKey == null)
                        lastKey = token;
                    else
                    {
                        tokenPairs[lastKey] = token;
                        lastKey = null;
                    }
                }
            }

            //foreach (var pair in tokenPairs)
            //{
            //    System.Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            //}

            return tokenPairs;
        }
        #endregion
    }
}
