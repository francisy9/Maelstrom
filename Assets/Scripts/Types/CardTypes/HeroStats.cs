using System;
using System.Collections.Generic;

namespace CardTypes
{
    [Serializable]
    public class HeroStats
    {
        public int NumAttacks { get; set; }
        public int TotalNumAttacks { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }

        public HeroStats() { }

        // Constructor
        public HeroStats(int hp)
        {
            NumAttacks = 0;
            TotalNumAttacks = 0;
            CurrentAttack = 0;
            MaxHP = hp;
            CurrentHP = hp;
        }

        public byte[] Serialize()
        {
            List<byte> serializedData = new List<byte>
            {
                (byte)NumAttacks,
                (byte)TotalNumAttacks,
                (byte)CurrentAttack
            };

            sbyte CurrentHp = (sbyte)CurrentHP;
            serializedData.Add((byte)CurrentHp);

            serializedData.Add((byte)MaxHP);

            return serializedData.ToArray();
        }

        public static HeroStats Deserialize(byte[] serialized)
        {

            HeroStats heroStats = new HeroStats();

            int currentIndex = 0;

            heroStats.NumAttacks = serialized[currentIndex++];
            heroStats.TotalNumAttacks = serialized[currentIndex++];
            heroStats.CurrentAttack = serialized[currentIndex++];

            byte currentHpByte = serialized[currentIndex++];
            sbyte currentHpSbyte = (sbyte)currentHpByte;
            heroStats.CurrentHP = currentHpSbyte;

            heroStats.MaxHP = serialized[currentIndex++];

            return heroStats;
        }
    }
}