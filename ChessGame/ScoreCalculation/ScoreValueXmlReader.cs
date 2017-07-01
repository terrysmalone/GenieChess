using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessGame.ScoreCalculation
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
                           
                List<XElement> score = doc.Elements("ScoreSet").ToList();
                List<XElement> scoreValues = score[0].Descendants().ToList();

                foreach (XElement scoreVal in scoreValues)
	            {
                    string name = string.Empty;
                    try
                    {
                        name = scoreVal.Name.ToString();

                        if (name.EndsWith("SquareTable"))
                        {
                            object squareTable = scoreCalc.GetType().GetProperty(name).GetValue(scoreCalc, null);

                            if (squareTable is Array)
                            {
                                Array arr = (Array)squareTable;

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
                            decimal val = Convert.ToDecimal(scoreVal.Value);
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
            XElement element = scoreValues.Descendants(scoreName).SingleOrDefault();

            return (decimal)element;
        }

        private static decimal[] GetSquareTable(string valueString)
        {
            //string match = scoreValues.FirstOrDefault(stringToCheck => stringToCheck.Contains(valueString));

            decimal[] values = new decimal[64];
                       
            string[] scoreParts = valueString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (scoreParts.Length == 64)
            {
                for (int i = 0; i < 64; i++)
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
