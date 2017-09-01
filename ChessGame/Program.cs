using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessGame.BoardRepresentation.Enums;
using ChessGame.Debugging;
using ChessGame.ResourceLoading;
using log4net;

namespace ChessGame
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            
            UCI uci = new UCI();
            uci.UCICommunication();

        }
    }
}
