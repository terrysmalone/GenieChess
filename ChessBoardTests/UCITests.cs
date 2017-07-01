using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessBoardTests
{
    [TestClass]      
    class UCITests
    {
        private List<string> responses = new List<string>(); 

        [TestMethod]
        public void Test_Input_uci()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);



                UCI uci = new UCI();
                //uci.UCICommunication();

                Thread uciThread = new Thread(new ThreadStart(uci.UCICommunication));
                uciThread.Start();

                //Thread listenerThread = new Thread(ListenForResponse);
                //listenerThread.Start();

                Console.WriteLine("isready");

                Thread.Sleep(10000);                

                //Console.ReadLine();
                //Console.ReadLine();

                //string response = Console.ReadLine();

                string result = sw.ToString();
                Assert.AreEqual("uciok", result);
            }


        }

        private void ListenForResponse()
        {
            while (true)
            {
                string input = Console.ReadLine();

                if(!string.IsNullOrEmpty(input))
                {
                    responses.Add(input);
                }
            }
        }

        [TestMethod]
        public void Test_Input_ucinewgame()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_Input_setoption()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Test_Input_isready()
        {
            UCI uci = new UCI();
            uci.UCICommunication();

            Console.WriteLine("isready");

            string response =  Console.ReadLine();

            Assert.AreEqual("readyok", response);
        }
    }
}
