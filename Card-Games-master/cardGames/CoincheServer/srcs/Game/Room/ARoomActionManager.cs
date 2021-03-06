﻿/*
 * Created by Axel Drozdzynski on 11/06/2017
 */
using Common.GameUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoincheServer.Game.Room
{
    public abstract class ARoomActionManager : ARoomEventManager
    {
        public ARoomActionManager(CoincheServer.Network.CServer _server) : base(_server)
        {

        }

        /*
         * Here is action of Room Manager
         */
        public bool DoAddPlayer(Player player)
        {
            if (player == null)
                return (false);
            if (room == null)
                return (false);
            roomPlayers.Add(player);
            Game.Table.TableManager t;
            try
            {
                t = tables.Last();
                if ((player.TableId = tables.Count - 1) == 0)
                    player.TableId = 0;
                t.AddPlayer(player);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                t = new Table.TableManager(server);
                tables.Add(t);
                t.AddPlayer(player);
            }
                return (true);
        }

        public bool DoAddBet(Bet bet)
        {
            Game.Table.TableManager t = FindTable(bet.player);
            t.AddBet(bet);
            return (true);
        }
        public bool DoAddCard(Player player, int cardId)
        {
            Game.Table.TableManager t = FindTable(player);
            t.AddCard(player, cardId);
            return (true);
        }
    }
}
