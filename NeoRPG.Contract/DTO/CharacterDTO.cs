using System;

namespace NeoRPG.Contract.DTO
{
    public class CharacterDTO : CharacterBaseResponse
    {
        public Guid Id { get; set; }
        public bool IsAlive { get; set; }
    }
}
