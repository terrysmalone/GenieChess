// See https://aka.ms/new-console-template for more information
using Genie_UCI;
using Logging;

var logger = new SerilogLog(Path.Combine(Environment.CurrentDirectory, "Debug.log"));

logger.Info("==============================================================");
logger.Info("");
logger.Info("Running Genie - UCI version");
logger.Info("");

var uci = new Uci(logger);
uci.UciCommunication();
