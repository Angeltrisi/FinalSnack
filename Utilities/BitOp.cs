using System;
using System.Runtime.CompilerServices;

namespace FinalSnack.Utilities
{
    public unsafe class BitOp
    {
        public static readonly ulong[] Masks = GenerateMasks();

        // generates all relevant bitmasks for every possible bit count value for harvest. we don't even need this many but uhhhhhhh
        private static ulong[] GenerateMasks()
        {
            var arr = new ulong[65];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = i == 64 ? ulong.MaxValue : (1UL << i) - 1;
            return arr;
        }
        /// <summary>
        /// Gets a certain range of bits from an unmanaged type and puts them into a new unmanaged type of the provided types.
        /// </summary>
        /// <typeparam name="TSoil">The type of value to get bits from.</typeparam>
        /// <typeparam name="TCrop">The type of value to put the harvested bits into.</typeparam>
        /// <param name="source">Where to get bits from.</param>
        /// <param name="startBit">The index of the bit where bits should begin to be harvested.</param>
        /// <param name="bitCount">The amount of bits that should be harvested.</param>
        /// <returns>A new <typeparamref name="TCrop"/> instance which contains the obtained bits.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TCrop GetBits<TSoil, TCrop>(TSoil source, byte startBit, byte bitCount)
            where TCrop : unmanaged
            where TSoil : unmanaged
        {
            int sourceSize = sizeof(TSoil) * 8;
            if (bitCount < 1 || startBit + bitCount > sourceSize)
            {
                string ln = Environment.NewLine;
                throw new ArgumentException($"Attempted to read bits out of range:{ln}" +
                    $"Harvestee is {sourceSize} bits long, and harvester attempted to read {bitCount} bits at position {startBit}");
            }

            ulong result = Harvest(Unsafe.As<TSoil, ulong>(ref source), startBit, bitCount);

            return Unsafe.As<ulong, TCrop>(ref result);
        }
        /// <summary>
        /// Sets a certain range of bits from an unmanaged type and puts them into a pre-existing unmanaged type of the provided types.
        /// </summary>
        /// <typeparam name="TSeed">The type of the value to get bits from.</typeparam>
        /// <typeparam name="TSoil">The type of value to put the harvested bits into.</typeparam>
        /// <param name="target">Where to put bits into.</param>
        /// <param name="value">Where to get bits from.</param>
        /// <param name="startBitTarget">The index of the bit where bits should begin to be planted.</param>
        /// <param name="startBitValue">The index of the bit where bits should begin to be harvested.</param>
        /// <param name="bitCount">The amount of bits that should be harvested from <typeparamref name="TSeed"/> and planted into <typeparamref name="TSoil"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void SetBits<TSeed, TSoil>(ref TSoil target, TSeed value, byte startBitTarget, byte startBitValue, byte bitCount)
            where TSeed : unmanaged
            where TSoil : unmanaged
        {
            int targetSize = sizeof(TSoil) * 8;
            int valueSize = sizeof(TSeed) * 8;

            // ts nasty
            bool b = false;
            bool isBool = false;
            if (value is bool b2)
            {
                isBool = true;
                b = b2;
            }

            if (bitCount < 1 || startBitTarget + bitCount > targetSize || (startBitValue + bitCount > valueSize && !isBool))
            {
                string ln = Environment.NewLine;
                throw new ArgumentException($"Attempted to place bits out of range:{ln}" +
                    $"Target/value is {targetSize}/{valueSize} bits long, and attempted to place {bitCount} bits at position {startBitTarget}/{startBitValue}");
            }

            ulong finalSeed = isBool ? (b ? ~0UL : 0UL) : Unsafe.As<TSeed, ulong>(ref value);
            ulong result = Plant(Unsafe.As<TSoil, ulong>(ref target), finalSeed, startBitTarget, startBitValue, bitCount);
            target = Unsafe.As<ulong, TSoil>(ref result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Harvest(ulong src, byte startBit, byte bitCount) => (src >> startBit) & Masks[bitCount];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Plant(ulong target, ulong value, int startBitTarget, int startBitValue, int bitCount)
        {
            ulong mask = Masks[bitCount];
            ulong pluck = (value >> startBitValue) & mask;
            ulong clear = target & ~(mask << startBitTarget);
            return clear | (pluck << startBitTarget);
        }
    }
}
