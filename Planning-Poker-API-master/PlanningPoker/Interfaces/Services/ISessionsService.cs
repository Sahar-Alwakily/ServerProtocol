using System;
using System.Collections.Generic;
using PlanningPoker.Model;

namespace PlanningPoker.Interfaces.Services
{
    public interface ISessionsService
    {
        Round StartCountdown(Guid sessionId, int roundId);

        IEnumerable<Session> Sessions { get; }
        Session GetSession(string sessionName);
        Session CreateSession(string sessionName, string masterName);
        Session JoinSession(string sessionName, string participantName);
        Session GetSession(Guid sessionId);
        Round PrepareRound(Guid sessionId);
        void Vote(string sessionName, string participantName, int round, int vote, bool allowOverwrite = false);
        void EndRound(Guid sessionId, int roundId);
        void EndSession(Guid sessionId);
        void RemoveParticipant(Guid sessionId, Guid participantId);
    }
}