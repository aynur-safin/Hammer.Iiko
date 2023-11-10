using System;
using System.Collections.Generic;

namespace Hammer.Iiko.ReservePlugin.Dto
{
    public class ReserveDto
    {
        public Guid Id { get; set; }
        public ClientDto Client { get; set; }
        public DateTime EstimatedStartTime { get; set; }
        public List<TableDto> Tables { get; set; }
    }
}
