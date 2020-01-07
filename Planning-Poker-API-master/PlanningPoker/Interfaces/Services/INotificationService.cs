using PlanningPoker.Model;
using System;

namespace PlanningPoker.Interfaces.Services
{
    public interface INotificationService
    {
        void StartCountdown(string sessionName, Round round);
        void RegisterVote(string sessionName, Vote vote);
        void RegisterParticipant(string sessionName, Participant participant);
        void RemoveParticipant(string sessionName, Guid participantId);
        void StartSession(string sessionName);
        void PrepareRound(string sessionName, Round round);
        void EndRound(string name, int roundId);
        void EndSession(string sessionName);
    }
}