using System.Xml.Linq;
using Logging;

namespace ChessEngine.ScoreCalculation;

public sealed class ScoreValueXmlReader
{
    private ILog _log;

    public void ReadScores(ScoreValues scoreValues, string xmlFileName, ILog log)
    {
        _log = log;

        XDocument doc;

        try
        {
            _log.Info($"Loading score set {xmlFileName}: ");

            doc = XDocument.Load(xmlFileName);

            var score = doc.Elements("ScoreSet").ToList();
            var values = score[0].Descendants().ToList();

            foreach (var scoreVal in values)
            {
                var name = string.Empty;
                try
                {
                    name = scoreVal.Name.ToString();

                    if (name.EndsWith("SquareTable"))
                    {
                        var squareTable = scoreValues.GetType().GetProperty(name).GetValue(scoreValues, null);

                        if (squareTable is Array array)
                        {
                            GetSquareTable(scoreVal.Value).CopyTo(array, 0);
                        }
                        else
                        {
                            _log.Error($"Error writing square table: {name}");
                        }
                    }
                    else
                    {
                        var val = Convert.ToInt32(scoreVal.Value);

                        scoreValues.GetType().GetProperty(name)?.SetValue(scoreValues, val, null);
                    }
                }
                catch (Exception exc)
                {
                    _log.Error($"Error writing score value for {name}", exc);

                    throw;
                }
            }
        }
        catch (FileNotFoundException fnfe)
        {
            _log.Error($"File {xmlFileName} not found", fnfe);

            throw;
        }
        catch (Exception exc)
        {
            _log.Error($"Error reading xml file:{xmlFileName}", exc);

            throw;
        }
    }

    private static int GetScore(IEnumerable<XElement> scoreValues, string scoreName)
    {
        var element = scoreValues.Descendants(scoreName).SingleOrDefault();

        return (int)element;
    }

    private int[] GetSquareTable(string valueString)
    {
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
            _log.Error($"Error reading square table. Wrong number of values: {valueString}");
        }

        return values;
    }
}

