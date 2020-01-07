
namespace CoincheServer
{
    using System;
    using System.Threading.Tasks;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Groups;

    public class ServerHandler : SimpleChannelInboundHandler<string>
    {
        static volatile IChannelGroup _group;
        static volatile PokerManager _poker = new PokerManager();


        class EveryOneBut : IChannelMatcher
        {
            readonly IChannelId _id;

            public EveryOneBut(IChannelId id)
            {
                _id = id;
            }

            public bool Matches(IChannel channel) => !Equals(channel.Id, _id);
        }

        public override void ChannelActive(IChannelHandlerContext contex)
        {
            IChannelGroup g = _group;

            if (g == null)
            {
                lock (this)
                {
                    if (_group == null)
                    {
                        g = _group = new DefaultChannelGroup(contex.Executor);
                    }
                }
            }
            g?.Add(contex.Channel);
            _poker.Player += 1;
            _poker.AddPlayer(_poker.Player, contex.Channel.RemoteAddress.ToString());
            //contex.WriteAndFlushAsync(g.Count);
            if (_poker.Player == 4)
            {
                contex.WriteAndFlushAsync("Welcome to the game!\n");
                _group.WriteAndFlushAsync("Welcome to the game\n", new EveryOneBut(contex.Channel.Id));
                _poker.IsGameStarted = true;
            }
            Console.WriteLine(_poker.Player);
        }

        protected override void ChannelRead0(IChannelHandlerContext contex, string msg)
        {
            string response;
            bool close = false;

            if (string.IsNullOrEmpty(msg))
            {
                response = "Please type something.\r\n";
            }
            else if (string.Equals("bye", msg, StringComparison.OrdinalIgnoreCase))
            {
                response = "Have a good day!\r\n";
                close = true;
            }
            else if (_poker.IsGameStarted == false)
            {
                response = "Waiting for player\r\n";
            }
            else
            {
                //group.WriteAndFlushAsync(broadcast, new EveryOneBut(contex.Channel.Id));
                response = _poker.LaunchPoker(msg.Trim().ToUpper(), contex.Channel.RemoteAddress.ToString());
                if (response.StartsWith("ACTION:"))
                    _group.WriteAndFlushAsync(response, new EveryOneBut(contex.Channel.Id));
            }

            Task waitClose = contex.WriteAndFlushAsync(response);
            if (close)
            {
                Task.WaitAll(waitClose);
                contex.CloseAsync();
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
        {
            Console.WriteLine("{0}", e.StackTrace);
            contex.CloseAsync();
        }

        public override bool IsSharable => true;
    }
}