using System;
using System.Collections.Generic;
using System.Linq;
using PlanningPoker.Model;
using PlanningPoker.Sys.Exceptions;
using PlanningPoker.Interfaces.Services;

namespace PlanningPoker.Services
{
    public class SessionsService : ISessionsService
    {
        private readonly List<Session> _sessions = new List<Session>();
        private readonly INotificationService _notificationService;
        private readonly ISanitizerService _sanitizerService;

        public IEnumerable<Session> Sessions => _sessions.AsEnumerable();

        public SessionsService(
            INotificationService notificationService,
            ISanitizerService sanitizerService
            )
        {
            _notificationService = notificationService;
            _sanitizerService = sanitizerService;
        }

        public Session GetSession(string sessionName)
        {
            var session = _sessions.FirstOrDefault(s => s.Name == sessionName || _sanitizerService.LettersAndDigits(s.Name) == sessionName );
            if (session == null)
            {
                throw new SessionMissingException();
            }
            return session;
        }

        public Session GetSession(Guid sessionId)
        {
            var session = _sessions.SingleOrDefault(s => s.Id == sessionId);
            if (session == null)
            {
                throw new SessionMissingException();
            }
            return session;
        }

        public Session  CreateSession(string sessionName, string masterName)
        {
            
            if (_sessions.Any(s => (s.Name == sessionName || _sanitizerService.LettersAndDigits(s.Name) == sessionName) && s.Master.Name != masterName))
            {
                throw new SessionClashException();
            }

            var result = _sessions.SingleOrDefault(s => s.Name == sessionName && s.Master.Name == masterName);

            if (result == null)
            {
                result = new Session(sessionName, masterName);
                _sessions.Add(result);
            }
            _notificationService.StartSession(sessionName);
            return result;
        }

        public Session JoinSession(string sessionName, string participantName)
        {
            var session = GetSession(sessionName);

            if (session.Participants.Any(p => p.Name == participantName))
            {
             throw new ParticipantClashException();   
            }

            var participant = new Participant(participantName, ParticipantRole.Voter);
            session.AddParticipant(participant);
            _notificationService.RegisterParticipant(sessionName, participant);
            return session;
        }

        public Round PrepareRound(Guid sessionId)
        {
            var session = GetSession(sessionId);
            var round = new Round()
            {
                Id = (session.CurrentRound?.Id ?? 0) + 1,
                State = RoundState.Pending,
                End = DateTime.Now
            };
            session.AddRound(round);
            _notificationService.PrepareRound(session.Name, round);
            return round;
        }

        public Round StartCountdown(Guid sessionId, int roundId)
        {
            var session = GetSession(sessionId);
            var currentRound = session.CurrentRound;

            if (currentRound.Id != roundId)
            {
                throw new IncorrectRoundException("Round does not exist.");
            }

            if (session.CurrentRound.State != RoundState.Pending)
            {
                throw new IncorrectRoundException("Round is not ready.");
            }

            currentRound.State = RoundState.Started;
            currentRound.End = DateTime.Now.AddSeconds(10).ToUniversalTime();

            _notificationService.StartCountdown(session.Name, currentRound);
            return currentRound;
        }

        public void Vote(string sessionName, string participantName, int round, int vote, bool allowOverwrite = true)
        {
            var session = GetSession(sessionName);
            var currentRound = session.CurrentRound;

            if (currentRound.Id != round)
            {
                throw new IncorrectRoundException("Round does not exist.");
            }

            if (session.CurrentRound.State != RoundState.Pending && session.CurrentRound.State != RoundState.Started)
            {
                throw new IncorrectRoundException("Round has not started.");
            }

            var participant = session.Participants.SingleOrDefault(v => v.Name == participantName);
            if (participant == null)
            {
                throw new MissingMemberException();
            }

            if (!allowOverwrite && currentRound.Votes.Any(v => v.Participant.Name == participantName))
            {
                throw new VoteClashException();
            }

            var newVote = new Vote()
            {
                Participant = participant,
                Value = vote
            };

            currentRound.AddVote(newVote);
            _notificationService.RegisterVote(session.Name, newVote);
        }

        public void EndRound(Guid sessionId, int roundId)
        {
            var session = GetSession(sessionId);
            var currentRound = session.CurrentRound;

            if (currentRound.Id != roundId)
            {
                throw new IncorrectRoundException();
            }

            currentRound.State = RoundState.Complete;
            _notificationService.EndRound(session.Name, roundId);
        }

        public void EndSession(Guid sessionId)
        {
            var session = GetSession(sessionId);
            _sessions.Remove(session);
            _notificationService.EndSession(session.Name);
        }

        public void RemoveParticipant(Guid sessionId, Guid participantId)
        {
            var session = GetSession(sessionId);
            session.RemoveParticipant(participantId);
            _notificationService.RemoveParticipant(session.Name, participantId);
        }
    }
}
