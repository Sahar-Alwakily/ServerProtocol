using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Sys.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException()
        {
            
        }

        public ServiceException(string message) : base(message)
        {
        }

        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SessionClashException : ServiceException
    {
        public SessionClashException() : base("Session already exists.")
        {
            
        }
    }

    public class SessionMissingException : ServiceException
    {
        public SessionMissingException() : base("Session could not be found.")
        {
            
        }
    }

    public class ParticipantClashException : ServiceException
    {
        public ParticipantClashException() : base("Participant already in session.")
        {
            
        }
    }

    public class RoundClashException : ServiceException
    {
        public RoundClashException() : base("Active round already started.")
        {
        }

        public RoundClashException(string message) : base(message)
        {
        }

        public RoundClashException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class IncorrectRoundException : ServiceException
    {
        public IncorrectRoundException() : base("Could not find round.")
        {
        }

        public IncorrectRoundException(string message) : base(message)
        {
        }

        public IncorrectRoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class VoteClashException : ServiceException
    {
        public VoteClashException() : base("Cannot change vote.")
        {
        }

        public VoteClashException(string message) : base(message)
        {
        }

        public VoteClashException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
