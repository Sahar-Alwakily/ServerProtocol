using System;
using System.Collections.Generic;
using System.Linq;

namespace CoincheServer
{
    class PokerManager
    {
        //SETUP

        public static Random Rng { get; } = new Random();
        public List<Card> Deck { get; }
        public List<Card> Board { get; }
        public List<Player> Players { get; }
        public bool GameIsSetup { get; private set; }

        //GAME

        public int LilBlind { get; }
        public int BigBlind { get; }
        public int Turn { get; private set; }
        public int Played { get; private set; }
        public int CurrentlyPlaying { get; private set; }
        public int CoinOnBoard { get; private set; }
        public int MaxBet { get; private set; }

        public PokerManager()
        {
            Player = 0;
            IsGameStarted = false;
            Players = new List<Player>();
            Deck = new List<Card>
            {
                new Card('S', '1', 13),
                new Card('S', '2', 1),
                new Card('S', '3', 2),
                new Card('S', '4', 3),
                new Card('S', '5', 4),
                new Card('S', '6', 5),
                new Card('S', '7', 6),
                new Card('S', '8', 7),
                new Card('S', '9', 8),
                new Card('S', 'X', 9),
                new Card('S', 'J', 10),
                new Card('S', 'Q', 11),
                new Card('S', 'K', 12),

                new Card('H', '1', 13),
                new Card('H', '2', 1),
                new Card('H', '3', 2),
                new Card('H', '4', 3),
                new Card('H', '5', 4),
                new Card('H', '6', 5),
                new Card('H', '7', 6),
                new Card('H', '8', 7),
                new Card('H', '9', 8),
                new Card('H', 'X', 9),
                new Card('H', 'J', 10),
                new Card('H', 'Q', 11),
                new Card('H', 'K', 12),

                new Card('D', '1', 13),
                new Card('D', '2', 1),
                new Card('D', '3', 2),
                new Card('D', '4', 3),
                new Card('D', '5', 4),
                new Card('D', '6', 5),
                new Card('D', '7', 6),
                new Card('D', '8', 7),
                new Card('D', '9', 8),
                new Card('D', 'X', 9),
                new Card('D', 'J', 10),
                new Card('D', 'Q', 11),
                new Card('D', 'K', 12),

                new Card('C', '1', 13),
                new Card('C', '2', 1),
                new Card('C', '3', 2),
                new Card('C', '4', 3),
                new Card('C', '5', 4),
                new Card('C', '6', 5),
                new Card('C', '7', 6),
                new Card('C', '8', 7),
                new Card('C', '9', 8),
                new Card('C', 'X', 9),
                new Card('C', 'J', 10),
                new Card('C', 'Q', 11),
                new Card('C', 'K', 12)
            };

            Board = new List<Card>();
            GameIsSetup = false;

            //SETUP IS OVER

            LilBlind = 5;
            BigBlind = 10;
            Turn = 1;
            Played = 0;
            CurrentlyPlaying = 1;
            CoinOnBoard = 0;
            MaxBet = 10;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void SetupGame()
        {
            var i = 0;
            var j = 1;

            Shuffle(Deck);
            Board.Clear();
            while (i != 5)
            {
                Board.Add(new Card(Deck[i].Type, Deck[i].Number, Deck[i].Power));
                i++;
            }
            while (j <= Player)
            {
                var tmp = i + 2;
                Players[j - 1].ClearHand();
                while (i != tmp)
                {
                    Players[j - 1]
                        .AddCard(new Card(Deck[i].Type, Deck[i].Number, Deck[i].Power));
                    if (Players[j - 1].Coin <= 0)
                        Players[j - 1].Lost = true;
                    i++;
                }
                j++;
            }
            ResetPlayer();
            CoinOnBoard = 0;
            MaxBet = 10;
            CurrentlyPlaying = 1;
            Players[CurrentlyPlaying - 1].Coin -= 5;
            Players[NextPlayer() - 1].Coin -= 10;
            CoinOnBoard += 15;
            GameIsSetup = true;
        }

        public string LaunchPoker(string msg, string channelId)
        {
            if (GameIsSetup == false)
                SetupGame();
            if (string.Equals(msg, "BOARD"))
                return (AffBoard());
            if (string.Equals(msg, "HAND"))
                return AffPlayerHand(channelId);
            if (string.Equals(msg, "COIN"))
                return (AffCoin(channelId));
            if (string.Equals(msg, "MAXBET"))
                return (AffMaxBet());
            if (string.Equals(msg, "COINBOARD"))
                return (AffCoinOnBoard());
            if (CurrentPlayerIsGood(channelId))
            {
                if (string.Equals(msg, "PASS"))
                {
                    var current = CurrentlyPlaying;

                    Players[CurrentlyPlaying - 1].HasPassed = true;
                    RotatePlayer();
                    if (PlayerInGame() == 1)
                    {
                        Played = 0;
                        Turn += 1;
                        var winner = CheckLastPlayer();
                        Turn = 1;
                        SetupGame();
                        return ("ACTION: Player " + current + " passed " + "\nWinner is " + winner +
                                "\nNew round just started" + "\r\n");
                    }
                    if (Played == PlayerInGame())
                    {
                        Played = 0;
                        Turn += 1;
                        if (Turn > 3)
                        {
                            var winner = CheckWinner();
                            Turn = 1;
                            SetupGame();
                            if (PlayerLost() == 1)
                                return ("ACTION: player " + winner + " won the game\r\n");
                            return ("ACTION: Player " + current + " passed " + "\n" + winner +
                                    "\nNew round just started" + "\r\n");
                        }
                        return ("ACTION: Player " + current + " passed. Starting turn " + Turn + "\r\n");
                    }
                    return ("ACTION: Player " + current + " passed\r\n");
                }
                if (msg.Equals("BETALL"))
                    return (BetAll(msg, channelId));
                if (msg.StartsWith("BET") && msg.Length >= 4 &&  msg.Length <= 8)
                    return (CheckBet(msg, channelId));
            }
            else
                return ("INFO: Sorry this is not your turn player " + CurrentlyPlaying +
                        " is currently playing\r\n");
            return ("INFO: The command was not recognized\r\n");
        }

        public string BetAll(string msg, string chanId)
        {
                var current = CurrentlyPlaying;
            var betValue = GetPlayerById(chanId).Coin;

                GetPlayerById(chanId).Coin -= betValue;
                CoinOnBoard += betValue;
                MaxBet = betValue;
                RotatePlayer();
                Played += 1;
                if (Played == PlayerInGame())
                {
                    Turn += 1;
                    Played = 0;
                    if (Turn > 3)
                    {
                        var winner = CheckWinner();
                        Turn = 1;
                        SetupGame();
                        if (PlayerLost() == 1)
                            return ("ACTION: player " + winner + " won the game\r\n");
                        return ("ACTION: Player " + current + " bet value " + betValue + "\n" + winner + "\nNew round just started" + "\r\n");
                    }

                    return ("ACTION: Player " + current + " bet value " + betValue + ". Starting turn " + Turn +
                            "\r\n");
                }
                return ("ACTION: Player " + current + " bet value " + betValue + "\r\n");
        }

        public string CheckBet(string msg, string chanId)
        {
            var i = 4;
            var betValue = "";

            while (i != msg.Length)
            {
                if (char.IsDigit(msg[i]) == false)
                    return ("INFO: Your bet is invalid\r\n");
                betValue += msg[i];
                i++;
            }
            if (int.Parse(betValue) >= MaxBet && GetPlayerById(chanId).Coin >= int.Parse((betValue)))
            {
                var current = CurrentlyPlaying;

                GetPlayerById(chanId).Coin -= int.Parse(betValue);
                CoinOnBoard += int.Parse(betValue);
                MaxBet = int.Parse(betValue);
                RotatePlayer();
                Played += 1;
                if (Played == PlayerInGame())
                {
                    Turn += 1;
                    Played = 0;
                    if (Turn > 3)
                    {
                        var winner = CheckWinner();
                        Turn = 1;
                        SetupGame();
                        return ("ACTION: Player " + current + " bet value " + betValue + "\n" + winner + "\nNew round just started" + "\r\n");
                    }

                    return ("ACTION: Player " + current + " bet value " + betValue + ". Starting turn " + Turn +
                            "\r\n");
                }
                return ("ACTION: Player " + current + " bet value " + betValue + "\r\n");
            }
            return ("INFO: Your bet is invalid\r\n");
        }

        public void PrintDeck()
        {
            foreach (var c in Deck)
            {
                Console.WriteLine(c.Type + " " + c.Number);
            }
        }

        public string AffPlayerHand(string channelId)
        {
            var hand = "";

            foreach (var p in Players)
            {
                if (string.Equals(p.ChannelId, channelId))
                    hand += p.RetHand();
            }
            return ("INFO: " + hand + "\r\n");
        }

        public string AffCoin(string channelId)
        {
            var coin = Players.Where(p => string.Equals(p.ChannelId, channelId)).Aggregate("", (current, p) => current + p.Coin);
            return ("INFO: " + coin + "\r\n");
        }

        public string AffBoard()
        {
            var boardInfo = "";
            var i = 0;

            while (i != (2 + Turn))
            {
                boardInfo += Board[i].Type.ToString() + Board[i].Number.ToString() + " ";
                i++;
            }
            return ("INFO: " + boardInfo + "\r\n");
        }

        public int NextPlayer()
        {
            if (CurrentlyPlaying + 1 == Player)
                return (1);
            return (CurrentlyPlaying + 1);
        }

        public bool CurrentPlayerIsGood(string chanId)
        {
            foreach (var p in Players)
            {
                if (p.PlayerNbr == CurrentlyPlaying && string.Equals(p.ChannelId, chanId))
                    return (true);
            }
            return (false);
        }

        public void RotatePlayer()
        {
            if (CurrentlyPlaying == Players.Count)
                CurrentlyPlaying = 1;
            else
                CurrentlyPlaying++;

            if (Players[CurrentlyPlaying - 1].HasPassed != true  && Players[CurrentlyPlaying - 1].HasPassed) return;
            while (Players[CurrentlyPlaying - 1].HasPassed || Players[CurrentlyPlaying - 1].Lost)
            {
                if (CurrentlyPlaying == Players.Count)
                    CurrentlyPlaying = 1;
                else
                    CurrentlyPlaying++;
            }
        }

        public string CheckWinner()
        {
            var comb = new Combination();
            var currentWinner = 1;
            var maxPower = 0;
            var maxpowerCard = 0;
            Player winner = null;

            foreach (var p in Players)
            {
                if (p.HasPassed == false && p.Lost == false)
                {
                    var power = comb.CheckAllComb(Board, p.Hand);
                    Console.WriteLine("Power for player " + p.PlayerNbr + " is " + power + " and his most powerfull card is " + comb.Power);
            
                    if (power >= 0 && maxPower <= power && maxpowerCard < comb.Power)
                    {
                        maxPower = power;
                        maxpowerCard = comb.Power;
                        currentWinner = p.PlayerNbr;
                        winner = p;
                    }
                }
                    
            }
            if (winner != null) winner.Coin += CoinOnBoard;
            return ("And the winner for " + CoinOnBoard + " is player " + currentWinner + "\r\n");
        }

        public Player GetPlayerById(string chanId)
        {
            foreach (var p in Players)
            {
                if (p.ChannelId.Equals(chanId))
                    return (p);
            }
            return (null);
        }

        public int PlayerInGame()
        {
            int i = 0;

            foreach (var p in Players)
            {
                if (p.HasPassed == false || p.Lost == false)
                    i++;
            }
            return (i);
        }

        public int PlayerLost()
        {
            int i = 0;

            foreach (var p in Players)
            {
                if (p.Lost == false)
                    i++;
            }
            return (i);
        }

        public void ResetPlayer()
        {
            foreach (var p in Players)
            {
                p.HasPassed = false;
            }
        }

        public int CheckLastPlayer()
        {
            foreach (var p in Players)
            {
                if (p.HasPassed == false && p.HasPassed == false)
                    return (p.PlayerNbr);
            }
            return (0);
        }

        public string AffMaxBet()
        {
            return ("INFO: The current maximum bet is " + MaxBet + "\r\n");
        }

        public string AffCoinOnBoard()
        {
            return ("INFO: The number of coin on board is " + CoinOnBoard + "\r\n");
        }

        public bool IsGameStarted { get; set; }

        public int Player { get; set; }

        public void AddPlayer(int pn, string chanId)
        {
            Players.Add(new Player(pn, chanId));
        }

    }
}
