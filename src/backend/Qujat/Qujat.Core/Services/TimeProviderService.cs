using System;

namespace Qujat.Core.Services
{
    public class TimeProviderService
    {
        private readonly DateTime _currentMoment;
        public TimeProviderService()
        {
            _currentMoment = DateTime.UtcNow;
        }

        public DateTime Now => _currentMoment;
    }
}
