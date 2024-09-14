using System;
using System.Collections.Generic;
using CardTypes;
using Animation;

namespace ResponseTypes
{
    [Serializable]
    public class CardEffectResponse
    {
        public AnimationStructure AnimationStructure { get; set; }
        public Dictionary<int, byte[]> AlliedUnits { get; set; } // Key is board index, value is card data including hero
        public Dictionary<int, byte[]> EnemyUnits { get; set; }

        public byte[] Serialize(bool isCaster)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(AnimationStructure.Serialize());
            if (!isCaster)
            {
                bytes.Add((byte)EnemyUnits.Count);
                bytes.Add((byte)AlliedUnits.Count);
                foreach (var unit in EnemyUnits)
                {
                    bytes.Add((byte)unit.Key);
                    bytes.Add((byte)unit.Value.Length);
                    bytes.AddRange(unit.Value);
                }
                foreach (var unit in AlliedUnits)
                {
                    bytes.Add((byte)unit.Key);
                    bytes.Add((byte)unit.Value.Length);
                    bytes.AddRange(unit.Value);
                }
            }
            else
            {
                bytes.Add((byte)AlliedUnits.Count);
                bytes.Add((byte)EnemyUnits.Count);
                foreach (var unit in AlliedUnits)
                {
                    bytes.Add((byte)unit.Key);
                    bytes.Add((byte)unit.Value.Length);
                    bytes.AddRange(unit.Value);
                }
                foreach (var unit in EnemyUnits)
                {
                    bytes.Add((byte)unit.Key);
                    bytes.Add((byte)unit.Value.Length);
                    bytes.AddRange(unit.Value);
                }
            }
            return bytes.ToArray();
        }

        public static CardEffectResponse Deserialize(byte[] data)
        {
            CardEffectResponse response = new CardEffectResponse();
            var animationSegment = new ArraySegment<byte>(data, 0, AnimationStructure.GetNumBytes());
            response.AnimationStructure = AnimationStructure.Deserialize(animationSegment.ToArray());

            response.AlliedUnits = new Dictionary<int, byte[]>();
            response.EnemyUnits = new Dictionary<int, byte[]>();
            int numAlliedUnits = data[AnimationStructure.GetNumBytes()];
            int numEnemyUnits = data[AnimationStructure.GetNumBytes() + 1];

            int currentIndex = AnimationStructure.GetNumBytes() + 2;
            for (int i = 0; i < numAlliedUnits; i++)
            {
                int boardIndex = data[currentIndex++];
                int cardDataLength = data[currentIndex++];
                var cardDataSegment = new ArraySegment<byte>(data, currentIndex, cardDataLength);
                response.AlliedUnits.Add(boardIndex, cardDataSegment.ToArray());
                currentIndex += cardDataLength;
            }

            for (int i = 0; i < numEnemyUnits; i++)
            {
                int boardIndex = data[currentIndex++];
                int cardDataLength = data[currentIndex++];
                var cardDataSegment = new ArraySegment<byte>(data, currentIndex, cardDataLength);
                response.EnemyUnits.Add(boardIndex, cardDataSegment.ToArray());
                currentIndex += cardDataLength;
            }

            return response;
        }
    }

    [Serializable]
    public class AnimationStructure
    {
        public AnimationId AnimationId { get; set; }
        public int OriginBoardIndex { get; set; }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>
            {
                (byte)AnimationId,
                (byte)OriginBoardIndex
            };
            return bytes.ToArray();
        }

        public static AnimationStructure Deserialize(byte[] data)
        {
            AnimationStructure animationStructure = new AnimationStructure
            {
                AnimationId = (AnimationId)data[0],
                OriginBoardIndex = data[1]
            };
            return animationStructure;
        }

        public static int GetNumBytes()
        {
            return 2;
        }
    }
}