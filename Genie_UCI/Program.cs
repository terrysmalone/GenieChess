using ChessGame;
using log4net;

namespace Genie_UCI
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Log.Info("==============================================================");
            Log.Info("");
            Log.Info("Running Genie - UCI version");
            Log.Info("");
            
            var uci = new Uci();
            uci.UciCommunication();
        }
    }
}
