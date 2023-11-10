using System;
using System.Collections.Generic;

namespace Hammer.Iiko.ReservePlugin.Dto
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PhoneDto> Phones { get; set; }
        public string CardNumber { get; set; }
    }
}
