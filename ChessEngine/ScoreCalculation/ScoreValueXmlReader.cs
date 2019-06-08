using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ChessEngine.ScoreCalculation
{
    public static class ScoreValueXmlReader
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            
        public static void ReadScores(ScoreCalculator scoreCalc, string xmlFileName)
        {
           
            XDocument doc;

            try
            {
                log.Info(string.Format("Loading score set {0}: ", xmlFileName));

                doc = XDocument.Load(xmlFileName);
                           
                var score = doc.Elements("ScoreSet").ToList();
                var scoreValues = score[0].Descendants().ToList();

                foreach (var scoreVal in scoreValues)
	            {
                    var name = string.Empty;
                    try
                    {
                        name = scoreVal.Name.ToString();

                        if (name.EndsWith("SquareTable"))
                        {
                            var squareTable = scoreCalc.GetType().GetProperty(name).GetValue(scoreCalc, null);

                            if (squareTable is Array)
                            {
                                var arr = (Array)squareTable;

                                GetSquareTable(scoreVal.Value).CopyTo((Array)squareTable, 0);
                                //squareTable = squareValues;
                            }
                            else
                            {
                                log.Error(string.Format("Error writing square table: {0}", name));
                            }

                            //scoreCalc.GetType().GetProperty(name).SetValue(scoreCalc, squareTable, null);
                        }
                        else
                        {
                            var val = Convert.ToDecimal(scoreVal.Value);
                            scoreCalc.GetType().GetProperty(name).SetValue(scoreCalc, val, null);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error(string.Format("Error writing score value for {0}", name), exc);
                    }
	            }
            }
            catch (Exception exc)
            {
                log.Error(string.Format("Error reading xml file:{0}", xmlFileName), exc);
            }
        }

        private static decimal GetScore(List<XElement> scoreValues, string scoreName)
        {
            var element = scoreValues.Descendants(scoreName).SingleOrDefault();

            return (decimal)element;
        }

        private static decimal[] GetSquareTable(string valueString)
        {
            //string match = scoreValues.FirstOrDefault(stringToCheck => stringToCheck.Contains(valueString));

            var values = new decimal[64];
                       
            var scoreParts = valueString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (scoreParts.Length == 64)
            {
                for (var i = 0; i < 64; i++)
                {
                    values[i] = Decimal.Parse(scoreParts[i]);
                }
            }
            else
            {
                log.Error(string.Format("Error reading square table. Wrong number of values: {0}", valueString));
            }
            
            return values;
        }
    }
}
