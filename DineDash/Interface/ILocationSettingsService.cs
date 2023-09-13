using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DineDash.Interface
{
    public interface ILocationSettingsService
    {
        bool IsGpsTurnedOn();
        Task OpenLocationSettingsAsync();
    }
}
