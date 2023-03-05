using NUnit.Framework;
using MCTG_Durmaz.DB;
using MCTG_Durmaz.User;
using MCTG_Durmaz.HTTP;
using System.Net;
using MCTG_Durmaz.Card;
using Newtonsoft.Json;
using MCTG_Durmaz.Cards;

namespace durmaz_nunit_testing
{
    public class Tests
    {
        public Datenbank db;

        [SetUp]
        public void Setup()
        {
            db = new Datenbank();
        }

        [Test, Order(1)]
        public void CheckIfUserisInDatabase()
        {
            User user = new User() { Username = "kienboec" };
            Assert.IsTrue(db.IsUserInDatabase(user));
        }

        [Test, Order(2)]
        public void checkWithWrongCredentials()
        {
            User user = new User() { Username = "momo" };
            Assert.IsFalse(db.IsUserInDatabase(user));
        }
        [Test, Order(3)]
        public void TryToInsertPackage()
        {
            bool insert = db.InsertPackage("somenewpackageid", "somecard", 15, "fire", "spell");
            Assert.IsTrue(insert);
        }
        [Test, Order(4)]
        public void HasUserUsedWeeklyGift()
        {
            string user = "kienboec";
            Assert.IsTrue(db.hasUsedWeeklyGift(user));
        }
        [Test, Order(4)]
        public void TestRegistry()
        {
            User a = new User() { Username = "usermomo", Password = "momopassword" };
            Assert.IsTrue(db.checkAndRegister(a));
        }
        [Test, Order(5)]
        public void TestRegistryShouldFail()
        {
            User a = new User() { Username = "kienboec", Password = "momopassword" };
            Assert.IsFalse(db.checkAndRegister(a));
        }
        [Test, Order(6)]
        public void TestGetItemShopShouldBeTrue()
        {
            var x = db.getItemShop("kienboec");
            Assert.IsTrue(x.Count > 0);
        }
        [Test, Order(7)]
        public void DeleteTradeShouldntFail()
        {
            Assert.IsTrue(db.deleteCard("wrongcard2"));
            //because even when delete == 0 it returns true
            //only when connection is refused it returns false
        }
        [Test, Order(8)]
        public void CreateTrade()
        {
            TradingShop item = new TradingShop() { Id = "somecardid", Owner = "momod", CardToTrade ="newcard", MinimumDamage = 15, Type = "spell" };
            Assert.IsTrue(db.createNewTrade(item, "momo"));

        }
        [Test, Order(9)]
        public void getNewlyAddedTrade()
        {
            Assert.IsTrue(db.getItemShop("momo").Count != 0);
        }
        [Test, Order(10)]
        public void deleteNewlyAddedTrade()
        {
            Assert.IsTrue(db.deleteCard("somecardid"));
        }
        [Test, Order(11)]
        public void testUpdateUser()
        {
            string json = "{\"Name\": \"Kienbodaeck\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}";
            var user = JsonConvert.DeserializeObject<User>(json);
            Assert.IsTrue(db.changeUserData(user, "kienboec"));
        }
        [Test, Order(12)]
        public void TestJsonSerializer()
        {
            string json = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            var x = JsonConvert.DeserializeObject<User>(json);
            Assert.IsTrue(x.Username == "kienboec");
        }
        [Test, Order(13)]
        public void UserHasValidToken()
        {
            var expectedToken = "testusr-msgToken";

            var user = new User() { Username = "testusr" };

            Assert.That(user.Token, Is.EqualTo(expectedToken));
        }
        [Test, Order(14)]
        public void TestLoginShouldFail()
        {
            //var expectedToken = "testusr-msgToken";

            var user = new User() { Username = "testusr" };

            db.logging(user);

            Assert.IsFalse(db.logging(user));
        }
        [Test, Order(15)]
        public void TestLogin()
        {
            //var expectedToken = "testusr-msgToken";

            var user = new User() { Username = "kienboec", Password = "daniel" };

            Assert.IsTrue(db.logging(user));
        }
        [Test, Order(16)]
        public void getUserData()
        {
            //var expectedToken = "testusr-msgToken";
            User a = db.getUserData("kienboec");

            Assert.IsNotNull(a);
        }
        [Test, Order(17)]
        public void createPackage()
        {
            //var expectedToken = "testusr-msgToken";
            Cards card = new Cards() { Id = "somecardids" };
            Assert.IsTrue(db.setUserPackage(card, "kienboec"));
        }
        [Test, Order(18)]
        public void createPackageAgain()
        {
            //var expectedToken = "testusr-msgToken";
            Cards card = new Cards() { Id = "somecardids" };
            Assert.IsFalse(db.setUserPackage(card, "kienboec"));
        }
        [Test, Order(19)]
        public void setUserCoins()
        {
            //var expectedToken = "testusr-msgToken";
            Assert.IsTrue(db.setCoins("kienboec", +10));
        }
        [Test, Order(20)]
        public void createUserAndCheckIfCoinsAre20AsDefault()
        {
            User x = new User() { Username = "stillmomo", Password ="momopasswords"};
            db.checkAndRegister(x);
            User neu = db.getUserData("stillmomo");
            Assert.IsTrue(neu.Coins == 20);
        }





    }
}