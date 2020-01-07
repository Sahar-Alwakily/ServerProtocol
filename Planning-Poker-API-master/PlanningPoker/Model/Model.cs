using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Model
{
    public class Participant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ParticipantRole Role { get; set; }

        public Participant(string name, ParticipantRole role)
        {
            Name = name;
            Role = role;
        }
    }

    public enum ParticipantRole
    {
        Observer = 0,
        Voter = 1,
        Master = 2
    }

    public class ParticipantApplication
    {
        public string Name { get; set; }
        public ParticipantRole Role { get; set; }
    }

    public class Session
    {
        private readonly List<Participant> _participants = new List<Participant>();
        private readonly Stack<Round> _rounds = new Stack<Round>();
        private readonly List<Card> _cards = new List<Card> {
            new Card{ Display= "1", Value= 1 },
            new Card{ Display= "2", Value= 2 },
            new Card{ Display= "3", Value= 3 },
            new Card{ Display= "5", Value= 5 },
            new Card{ Display= "8", Value= 8 },
            new Card{ Display= "13", Value= 13 },
            new Card{ Display= "☕", Value= 20 },
            new Card{ Display= "😨", Value= 30 },
        };

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Participant Master { get; set; }


        public IEnumerable<Participant> Participants => _participants.AsEnumerable();

        public Round CurrentRound => _rounds.Any() ? _rounds.Peek() : null;
        public IEnumerable<Card> Cards => _cards;
        public IEnumerable<Round> Rounds => _rounds;

        public Session(string name, string masterName)
        {
            Name = name;
            var master = new Participant(masterName, ParticipantRole.Master);
            this.Master = master;
            _participants.Add(master);
            //_rounds.Push(new Round()
            //{
            //    Id = 0,
            //    State = RoundState.Null
            //});
        }

        public void AddParticipant(Participant particpant)
        {
            _participants.Add(particpant);
        }

        public void AddRound(Round round)
        {
            _rounds.Push(round);
        }

        public void RemoveParticipant(Guid participantId)
        {
            var participant = _participants.Where(p => p.Id == participantId).SingleOrDefault();
            if (participant != null)
            {
                _participants.Remove(participant);
            }
        }
    }

    public class SessionId
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    public class SessionApplication
    {
        public string SessionName { get; set; }
        public string MasterName { get; set; }
    }

    public class Round
    {
        private readonly IList<Vote> _votes = new List<Vote>();
        public int Id { get; set; }
        public RoundState State { get; set; }

        public IEnumerable<Vote> Votes => _votes;

        public DateTime End { get; set; }

        public void AddVote(Vote vote)
        {
            var existingVote = _votes.SingleOrDefault(v => v.Participant.Id == vote.Participant.Id);
            if (existingVote != null)
            {
                _votes.Remove(existingVote);
            }
            _votes.Add(vote);
        }
    }

    public enum RoundState
    {
        Null = 0,
        Pending = 1,
        Started = 2,
        Complete = 3
    }

    public class Vote
    {
        public Participant Participant { get; set; }
        public int Value { get; set; }
    }

    public class VoteBallot
    {
        public string ParticipantName { get; set; }
        public int Value { get; set; }
    }

    public class Card
    {
        public String Display { get; set; }
        public int Value { get; set; }
    }
}
