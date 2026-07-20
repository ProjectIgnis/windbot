using YGOSharp.OCGWrapper.Enums;
using System.Data;

namespace YGOSharp.OCGWrapper
{
    public class Card
    {
        public struct CardData
        {
            public int Code;
            public int Alias;
            public ulong Setcode;
            public uint Type;
            public int Level;
            public uint Attribute;
            public ulong Race;
            public int Attack;
            public int Defense;
            public int LScale;
            public int RScale;
            public int LinkMarker;
        }

        public int Id { get; private set; }
        public int Ot { get; private set; }
        public int Alias { get; private set; }
        public ulong Setcode { get; private set; }
        public uint Type { get; private set; }

        public int Level { get; private set; }
        public int LScale { get; private set; }
        public int RScale { get; private set; }
        public int LinkMarker { get; private set; }

        public uint Attribute { get; private set; }
        public ulong Race { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        internal CardData Data { get; private set; }

        public static Card Get(int id)
        {
            return CardsManager.GetCard(id);
        }

        public bool HasType(CardType type)
        {
            return ((Type & (uint)type) != 0);
        }

        public bool HasSetcode(int setcode)
        {
            ulong setcodes = Setcode;
            int settype = setcode & 0xfff;
            int setsubtype = setcode & 0xf000;
            while (setcodes > 0)
            {
                long check_setcode = (long)(setcodes & 0xffff);
                setcodes >>= 16;
                if ((check_setcode & 0xfff) == settype && (check_setcode & 0xf000 & setsubtype) == setsubtype) return true;
            }
            return false;
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || (HasType(CardType.Link) && HasType(CardType.Monster)));
        }

        internal Card(IDataRecord reader)
        {
            Id = reader.GetInt32(0);
            Ot = reader.GetInt32(1);
            Alias = reader.GetInt32(2);
            Setcode = (ulong)reader.GetInt64(3);
            Type = (uint)reader.GetInt64(4);

            uint levelInfo = (uint)reader.GetInt64(5);
            Level = (int)(levelInfo & 0xff);
            LScale = (int)((levelInfo >> 24) & 0xff);
            RScale = (int)((levelInfo >> 16) & 0xff);

            Race = (ulong)reader.GetInt64(6);
            Attribute = (uint)reader.GetInt64(7);
            Attack = reader.GetInt32(8);
            Defense = reader.GetInt32(9);

            if (HasType(CardType.Link))
            {
                LinkMarker = Defense;
                Defense = 0;
            }

            Data = new CardData()
            {
                Code = Id,
                Alias = Alias,
                Setcode = Setcode,
                Type = Type,
                Level = Level,
                Attribute = Attribute,
                Race = Race,
                Attack = Attack,
                Defense = Defense,
                LScale = LScale,
                RScale = RScale,
                LinkMarker = LinkMarker
            };
        }
    }
}
