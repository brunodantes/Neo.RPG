using System;

namespace NeoRPG.Contract.DTO
{
    public class BattleRequestDTO
    {
        public Guid FirstPlayer { get; set; }
        public Guid SecondPlayer { get; set; }
    }
}
