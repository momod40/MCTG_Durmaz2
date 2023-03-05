using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTG_Durmaz.User;
using MCTG_Durmaz.Card;

namespace MCTG_Durmaz.DB
{
    public class Datenbank
    {
        NpgsqlConnection conn;
        string connection = "Host=localhost;Port=5432;Username=postgres;Password=#;Database=swe1messagedb";
        public Datenbank()
        {
            conn = new NpgsqlConnection(connection);
            conn.Open();
        }

        public void createTables()
        {
            string CreateTablesCommand = @"
CREATE TABLE IF NOT EXISTS users (username varchar PRIMARY KEY, name varchar, password varchar, token varchar, coins int, bio varchar, image varchar, win int, lose int, elo int, weeklygift timestamp );
CREATE TABLE IF NOT EXISTS messages (id serial PRIMARY KEY, content varchar, username varchar REFERENCES users ON DELETE CASCADE);
CREATE TABLE IF NOT EXISTS cards (id varchar PRIMARY KEY, name varchar, damage decimal, element varchar, art varchar);
CREATE TABLE IF NOT EXISTS stack (id varchar primary key, name varchar);
CREATE TABLE IF NOT EXISTS deck  (id varchar primary key, username varchar);
CREATE TABLE IF NOT EXISTS trading (id varchar primary key, cardtotrade varchar, type varchar, minimumdamage int, owner varchar);
";
            using (NpgsqlCommand cmd = new NpgsqlCommand(CreateTablesCommand, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool IsUserInDatabase(User.User user)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select username from users where username = @username", conn))
            {
                cmd.Parameters.AddWithValue("username", user.Username);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        public List<Cards.Cards> getFreeCards()
        {
            List<Cards.Cards> cards = new List<Cards.Cards>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from cards where id not in (select id from stack) limit 5", conn))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cards.Cards card = new Cards.Cards();
                        card.Id = reader.GetString(0);
                        card.Name = reader.GetString(1);
                        card.Damage = reader.GetInt32(2);
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }
        public User.User getStatsByUser(string token)
        {
            User.User player = new User.User();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select win, lose, elo from users where username = @token", conn))
            {
                cmd.Parameters.AddWithValue("@token", token);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        player.Win = reader.GetInt32(0);
                        player.Lose = reader.GetInt32(1);
                        player.Elo = reader.GetInt32(2);
                    }
                }
            }
            return player;
        }
        public bool isCardMine(string id, string token)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("select owner from trading where id = @token", conn))
            {
                cmd.Parameters.AddWithValue("@token", id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool deleteCard(string id)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("delete from trading where id = @token", conn))
                {
                    cmd.Parameters.AddWithValue("@token", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (PostgresException)
            {
                return false;
            }
        }

        public List<TradingShop> getItemShop(string token)
        {
            List<TradingShop> items = new List<TradingShop>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from trading", conn))
            {
                //cmd.Parameters.AddWithValue("token", token);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TradingShop item = new TradingShop();
                        item.Id = reader.GetString(0);
                        item.CardToTrade = reader.GetString(1);
                        item.Type = reader.GetString(2);
                        item.MinimumDamage = reader.GetInt32(3);
                        item.Owner = reader.GetString(4);
                        items.Add(item);
                    }
                }
            }
            return items;
        }
        public List<User.User> getScoreBoard()
        {
            List<User.User> players = new List<User.User>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select username, win, lose, elo from users order by win desc;", conn))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User.User player = new User.User();
                        player.Username = reader.GetString(0);
                        player.Win = reader.GetInt32(1);
                        player.Lose = reader.GetInt32(2);
                        player.Elo = reader.GetInt32(3);
                        players.Add(player);
                    }
                }
            }
            return players;
        }

        public bool IsCardFromUser(string id, string name)
        {
            List<Cards.Cards> cards = new List<Cards.Cards>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from stack where id = @id and name = @username", conn))
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("username", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool Trade(string id, string buyer)
        {
            try
            {
                List<Cards.Cards> cards = new List<Cards.Cards>();
                using (NpgsqlCommand cmd = new NpgsqlCommand("update stack set name = @buyer where id = @id; update stack set name = (select owner from trading where id = @id )" +
                    " where id = (select cardtotradefrom trading where id = @id) ; ", conn))
                {
                    cmd.Parameters.AddWithValue("buyer", buyer);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public bool updateWeeklyGift(int random, string token)
        {
            try
            {
                List<Cards.Cards> cards = new List<Cards.Cards>();
                using (NpgsqlCommand cmd = new NpgsqlCommand("update users set win = win + @random, weeklygift = current_timestamp where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("random", random);
                    cmd.Parameters.AddWithValue("username", token);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public List<Cards.Cards> getCardsByUser(string name)
        {
            List<Cards.Cards> cards = new List<Cards.Cards>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from cards where id in (select id from stack where name = @username);", conn))
            {
                cmd.Parameters.AddWithValue("username", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cards.Cards card = new Cards.Cards();
                        card.Id = reader.GetString(0);
                        card.Name = reader.GetString(1);
                        card.Damage = reader.GetInt32(2);
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }
        public List<string> getDeckByUser(string name)
        {
            List<string> cards = new List<string>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from deck where username = @username", conn))
            {
                cmd.Parameters.AddWithValue("username", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        cards.Add(reader.GetString(0));
                    }
                }
            }
            return cards;
        }
        public List<Cards.Cards> getBest4Cards(string name)
        {
            List<Cards.Cards> cards = new List<Cards.Cards>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select id, damage from cards where id IN ( select id from stack where name = @username ) order by damage desc limit 4", conn))
            {
                cmd.Parameters.AddWithValue("username", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cards.Cards card = new Cards.Cards();
                        card.Id = reader.GetString(0);
                        card.Damage = reader.GetInt32(1);
                        cards.Add(card);
                    }
                }
            }
            return cards;
        }
        public User.User getUserData(string name)
        {
            User.User player = new User.User();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from users where username = @username", conn))
            {
                cmd.Parameters.AddWithValue("username", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        player.Username = reader.GetString(0);
                        player.Password = reader.GetString(1);
                        player.Coins = reader.GetInt32(4);
                        player.Bio = reader.GetString(5);
                        player.Image = reader.GetString(6);
                    }
                }
            }
            return player;
        }
        public bool changeUserData(User.User user, string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("update users set name = @name, bio = @bio, image = @image where username = @token", conn))
                {
                    cmd.Parameters.AddWithValue("name", user.Name);
                    cmd.Parameters.AddWithValue("bio", user.Bio);
                    cmd.Parameters.AddWithValue("image", user.Image);
                    cmd.Parameters.AddWithValue("token", token);
                    cmd.ExecuteNonQuery();
                }
                return true;

            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public bool setUserPackage(Cards.Cards cards, string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("insert into stack values (@id, @token)", conn))
                {
                    cmd.Parameters.AddWithValue("id", cards.Id);
                    cmd.Parameters.AddWithValue("token", token);
                    cmd.ExecuteNonQuery();
                }
                return true;

            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public bool createNewTrade(TradingShop trading, string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("insert into trading values (@id, @cardtotrade, @type, @minimumdamage, @owner)", conn))
                {
                    cmd.Parameters.AddWithValue("id", trading.Id);
                    cmd.Parameters.AddWithValue("cardtotrade", trading.CardToTrade);
                    cmd.Parameters.AddWithValue("type", trading.Type);
                    cmd.Parameters.AddWithValue("minimumdamage", trading.MinimumDamage);
                    cmd.Parameters.AddWithValue("owner", token);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public bool hasUsedWeeklyGift(string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("select weeklygift FROM users WHERE NOW() - weeklygift < INTERVAL '7 days' and username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", token);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public bool deleteDeckByUser(string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("Delete from deck where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", token);
                    cmd.ExecuteNonQuery();
                }
                return true;

            }
            catch (PostgresException)
            {
                return false;
            }
        }

        public bool InsertIntoDeck(string id, string token)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("insert into deck values (@id, @token)", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("token", token);
                    cmd.ExecuteNonQuery();
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool checkAndRegister(User.User user)
        {
            if (!IsUserInDatabase(user))
            {
                try
                {
                    //user hzfg
                    using (NpgsqlCommand cmd = new NpgsqlCommand("insert into users values (@username, @username, @password, @token, 20, 'bio', 'image', 0 , 0, 0, null);", conn))
                    {
                        cmd.Parameters.AddWithValue("username", user.Username);
                        cmd.Parameters.AddWithValue("password", user.Password);
                        string token = user.Username + "-mctgToken";
                        cmd.Parameters.AddWithValue("token", token);
                        cmd.Parameters.AddWithValue("password", user.Password);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (PostgresException)
                {
                    Console.WriteLine($"failed for {user.Username}");
                }

            }
            return false;
        }
        public bool logging(User.User user)
        {
            if (!IsUserInDatabase(user))
            {
                return false;
            }
            string pw = "";

            using (NpgsqlCommand cmd = new NpgsqlCommand("select password from users where username = @username;", conn))
            {
                cmd.Parameters.AddWithValue("username", user.Username);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pw = reader.GetString(0).ToString();
                    }
                }
            }

            if (pw == user.Password)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool InsertPackage(string id, string name, double dmg, string element, string art)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("insert into cards values (@id, @name, @damage, @element, @art)", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("damage", dmg);
                    cmd.Parameters.AddWithValue("element", element);
                    cmd.Parameters.AddWithValue("art", art);
                    cmd.ExecuteNonQuery();

                }
                return true;
            }
            catch (PostgresException)
            {
                return false;
            }
        }
        public int userCoins(string user)
        {
            int count = 0;
            //using (NpgsqlCommand cmd = new NpgsqlCommand("select coins from users where username = '"+ user +"'", conn))
            using (NpgsqlCommand cmd = new NpgsqlCommand("select coins from users where username = @username", conn))
            {
                cmd.Parameters.AddWithValue("@username", user);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
            }
            return count;

        }
        public bool setCoins(string username, int minus)
        {
            bool test = false;
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("update users set coins = coins -5 where username = @username", conn))
                {

                    cmd.Parameters.AddWithValue("username", username);
                    cmd.ExecuteNonQuery();
                    test = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("hia");
            }
            return test;
        }
    }
}
