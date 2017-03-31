﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");


        /// <summary>
        /// Tests to make sure a valid nickname returns a Created Response on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
            
        }

        /// <summary>
        /// Tests to make sure a null nickname and an empty nickname return 403 forbidden on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            
            dynamic user = new ExpandoObject();
            user.Nickname = "";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Forbidden, r.Status);
           
        }

        /// <summary>
        /// Tests to make sure a valid nickname returns a UserToken on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            
            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;

            string token = r.Data.ToString();
            Assert.IsTrue(token.Length > 0);
            
            

        }

        /// <summary>
        /// Tests to make sure if UserToken is invalid, TimeLimit less than 5, or TimeLimit greater than 120, 
        /// responds with status 403 (Forbidden) on join game.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            
            dynamic game = new ExpandoObject();
            game.UserToken = "9eb536af-50a1-476f-856e-ffff8f1b25d2";
            game.TimeLimit = 10;
            Response r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, r.Status);

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            string token = r.Data.ToString();

            string word = r.Data.ToString();
            dynamic game2 = new ExpandoObject();
            game2.UserToken = token;
            game2.TimeLimit = 150;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, r.Status);
            
            


        }

        /// <summary>
        /// Tests to make sure if UserToken is already a player in the pending game, responds with status 409 (Conflict) on join game.
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            
            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            dynamic user2 = new ExpandoObject();
            user2.Nickname = "bob";
            r = client.DoPostAsync("users", user2).Result;
            string token2 = r.Data.UserToken;

            dynamic game2 = new ExpandoObject();
            game2.UserToken = token2;
            game2.TimeLimit = 10;
            r = client.DoPostAsync("games", game2).Result;

            

        }

        /// <summary>
        /// Otherwise, if there is already one player in the pending game, adds UserToken as the second player.
        /// The pending game becomes active and a new pending game with no players is created. 
        /// The active game's time limit is the integer average of the time limits requested 
        /// by the two players. Returns the new active game's GameID 
        /// (which should be the same as the old pending game's GameID). 
        /// Responds with status 201 (Created).
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            dynamic user2 = new ExpandoObject();
            user2.Nickname = "fang";
            Response r2 = client.DoPostAsync("users", user2).Result;
            string token2 = r2.Data.UserToken;

            dynamic game2 = new ExpandoObject();
            game2.UserToken = token2;
            game2.TimeLimit = 20;

            r2 = client.DoPostAsync("games", game2).Result;

            Assert.AreEqual(Created, r2.Status);
            Assert.AreEqual(r.Data.GameID, r2.Data.GameID);
        }

        /// <summary>
        /// Tests cancel join request.
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            dynamic gameCancel = new ExpandoObject();
            gameCancel.UserToken = token;
            r = client.DoPutAsync(gameCancel, "games").Result;
            Assert.AreEqual(OK, r.Status);
        }
        /// <summary>
        /// Tests cancel join request.
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            dynamic gameCancel = new ExpandoObject();
            gameCancel.UserToken = "hello";
            r = client.DoPutAsync(gameCancel, "games").Result;
            Assert.AreEqual(Forbidden, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;

            game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;


        }

        /// <summary>
        /// Tests cancel join request.
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            string gameID = r.Data.GameID;
            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("pending", r.Data.GameState.ToString());

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
        }

        /// <summary>
        /// Tests to make sure if UserToken is already a player in the pending game, responds with status 409 (Conflict) on join game.
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;

            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Conflict, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
        }
        [TestMethod]
        public void TestMethod11()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Created, r.Status);

            string gameID = r.Data.GameID;
            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("active", r.Data.GameState.ToString());
            Assert.AreNotEqual(r.Data.Board, null);
            Assert.AreEqual((int)r.Data.TimeLimit, 10);
            Assert.AreNotEqual(r.Data.TimeLeft, 0);
            Assert.AreNotEqual(r.Data.Player1, null);
            Assert.AreNotEqual(r.Data.Player2, null);
        }
        [TestMethod]
        public void TestMethod12()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Created, r.Status);

            DateTime newTime = DateTime.Now.AddSeconds(7);
            while (DateTime.Now < newTime)
            {
            }

            string gameID = r.Data.GameID;
            r = client.DoGetAsync("games/" + gameID).Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("active", r.Data.GameState.ToString());
            Assert.AreNotEqual(r.Data.Board, null);
            Assert.AreEqual((int)r.Data.TimeLimit, 10);
            Assert.AreNotEqual(r.Data.TimeLeft, 0);
            Assert.AreNotEqual(r.Data.Player1, null);
            Assert.AreNotEqual(r.Data.Player2, null);


        }

        [TestMethod]
        public void TestMethod13()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Created, r.Status);

            string gameID = r.Data.GameID;
            r = client.DoGetAsync("games/" + gameID, "Brief=yes").Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("active", r.Data.GameState.ToString());
            Assert.AreNotEqual(r.Data.Player1, null);
            Assert.AreNotEqual(r.Data.Player2, null);
        }

        [TestMethod]
        public void TestMethod14()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = r.Data.UserToken;

            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 5;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, r.Status);

            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            token = r.Data.UserToken;
            game.UserToken = token;
            game.TimeLimit = 5;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Created, r.Status);

            DateTime newTime = DateTime.Now.AddSeconds(7);
            while (DateTime.Now < newTime)
            {
            }
            
            string gameID = r.Data.GameID;
            r = client.DoGetAsync("games/" + gameID, "Brief=yes").Result;
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("completed", r.Data.GameState.ToString());
            Assert.AreNotEqual(r.Data.Player1, null);
            Assert.AreNotEqual(r.Data.Player2, null);


        }
    }
}
