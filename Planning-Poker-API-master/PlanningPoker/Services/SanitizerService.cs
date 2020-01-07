using PlanningPoker.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Services
{
    public class SanitizerService : ISanitizerService
    {
        public string LettersAndDigits(string input)
        {
            return new string(input.Where(c => Char.IsLetterOrDigit(c)).ToArray());
        }
    }
}
