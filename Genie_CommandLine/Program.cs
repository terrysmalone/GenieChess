// See https://aka.ms/new-console-template for more information
using Genie_CommandLine;
using Logging;

var logger = new SerilogLog(Path.Combine(Environment.CurrentDirectory, "Debug.log"));
var genie = new Genie(logger);
genie.Run();
