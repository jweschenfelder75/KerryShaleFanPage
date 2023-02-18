using System.Collections.Generic;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Shared.Configuration
{
    public class DbLoggerOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public IList<LogEntryDto> LogFields { get; set; } = new List<LogEntryDto>();

        public string LogTable { get; set; } = string.Empty;
    }
}
