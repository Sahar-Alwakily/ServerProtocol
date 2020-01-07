using Microsoft.Extensions.Options;
using PlanningPoker.Interfaces.Services;
using PlanningPoker.Model;
using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly Pusher _pusher;
        private readonly Model.Configuration.Pusher _pusherOptions;
        private readonly ISanitizerService _sanitizerService;

        public NotificationService(
            IOptions<Model.Configuration.Pusher> pusherOptions,
            ISanitizerService sanitizerService
            )
        {
            this._sanitizerService = sanitizerService;
            _pusherOptions = pusherOptions.Value;
            _pusher = new Pusher(_pusherOptions.AppID, _pusherOptions.Key, _pusherOptions.Secret);
        }

        public async void StartSession(string sessionName)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "BeginSession", "");
        }

        public async void RegisterVote(string sessionName, Vote vote)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "RegisterVote", vote);
        }

        public async void RegisterParticipant(string sessionName, Participant participant)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "RegisterParticipant", participant);
        }

        public async void PrepareRound(string sessionName, Round round)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "PrepareRound", round);
        }

        public async void StartCountdown(string sessionName, Round round)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "StartCountdown", round);
        }

        public async void EndRound(string sessionName, int roundId)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "EndRound", roundId);
        }

        public async void EndSession(string sessionName)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "EndSession", sessionName);
        }

        public async void RemoveParticipant(string sessionName, Guid participantId)
        {
            var result = await _pusher.TriggerAsync(_sanitizerService.LettersAndDigits(sessionName), "RemoveParticipant", participantId);
        }
    }
}
