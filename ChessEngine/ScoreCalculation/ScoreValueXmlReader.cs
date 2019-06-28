using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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

                var score       = doc.Elements("ScoreSet").ToList();
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

                            if (squareTable is Array array)
                            {
                                GetSquareTable(scoreVal.Value).CopyTo(array, 0);
                            }
                            else
                            {
                                log.Error($"Error writing square table: {name}");
                            }

                            //scoreCalc.GetType().GetProperty(name).SetValue(scoreCalc, squareTable, null);
                        }
                        else
                        {
                            var val = Convert.ToInt32(scoreVal.Value);

                            scoreCalc.GetType().GetProperty(name)?.SetValue(scoreCalc, val, null);
                        }
                    }
                    catch (Exception exc)
                    {
                        log.Error($"Error writing score value for {name}", exc);

                        throw;
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                log.Error($"File {xmlFileName} not found", fnfe);

                throw;
            }
            catch (Exception exc)
            {
                log.Error($"Error reading xml file:{xmlFileName}", exc);

                throw;
            }
        }

        private static int GetScore(IEnumerable<XElement> scoreValues, string scoreName)
        {
            var element = scoreValues.Descendants(scoreName).SingleOrDefault();

            return (int)element;
        }

        private static int[] GetSquareTable(string valueString)
        {
            //string match = scoreValues.FirstOrDefault(stringToCheck => stringToCheck.Contains(valueString));

            var values = new int[64];
                       
            var scoreParts = valueString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (scoreParts.Length == 64)
            {
                for (var i = 0; i < 64; i++)
                {
                    values[i] = int.Parse(scoreParts[i]);
                }
            }
            else
            {
                log.Error($"Error reading square table. Wrong number of values: {valueString}");
            }
            
            return values;
        }
    }
}
