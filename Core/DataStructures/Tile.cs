using FinalSnack.Common;
using System;

namespace FinalSnack.Core.DataStructures
{
    public struct Tile
    {
        private uint _packedValue;
        // 9 bit int
        private ushort _cutPointB;
        // 9 bit int
        private ushort _cutPointA;
        // 1 bit
        private bool _collideSide;
        // 1 bit
        private bool _phaseThrough;
        // nybble
        private byte _spriteVariation;
        // byte
        private byte _paletteIndex;

        /// <summary>
        /// A property for working with this tile's PackedValue. When setting, all of the backing fields for the other properties will be recalculated.
        /// </summary>
        public uint PackedValue
        { 
            readonly get
            {
                return _packedValue;
            }
            set
            {
                // if we set packedvalue directly, recalculate all of the other backing fields
                _packedValue = value;
                _paletteIndex = BitOp.GetBits<uint, byte>(_packedValue, 0, 8);
                _spriteVariation = BitOp.GetBits<uint, byte>(_packedValue, 8, 4);
                _phaseThrough = BitOp.GetBits<uint, bool>(_packedValue, 12, 1);
                _collideSide = BitOp.GetBits<uint, bool>(_packedValue, 13, 1);

                uint combinedCut = BitOp.GetBits<uint, uint>(_packedValue, 14, 18);
                _cutPointA = (ushort)(combinedCut & 0x1FF);
                _cutPointB = (ushort)((combinedCut >> 9) & 0x1FF);
            }
        }
        /// <summary>
        /// A property for working with this tile's CutPoint.
        /// <para>Note that this represents two 9-bit integers; they cannot go above 511.</para>
        /// </summary>
        public Point16U CutPoint
        {
            readonly get
            {
                return new(_cutPointA, _cutPointB);
            }
            set
            {
                if (value.X > 511 || value.Y > 511)
                    throw new ArgumentOutOfRangeException(nameof(value), "One of the fields of this Point is higher than 511. As 9 bit ints, this isn't allowed.");
                _cutPointA = value.X;
                _cutPointB = value.Y;
                // plants two 9 bit integers directly into _packedValue at index 14 (indices 0 - 13 are taken by the byte, nybble and bools)
                uint combined = (uint)((value.X & 0x1FF) | ((value.Y & 0x1FF) << 9));
                BitOp.SetBits(ref _packedValue, combined, 14, 0, 18);
            }
        }
        /// <summary>
        /// A property for working with this tile's CutPointB. If editing both cut points at once, use <see cref="CutPoint"/> instead for performance.
        /// <para>Note that this represents a 9-bit integer; it cannot go above 511.</para>
        /// </summary>
        public ushort CutPointB
        {
            readonly get
            {
                return _cutPointB;
            }
            set
            {
                if (value > 511)
                    throw new ArgumentOutOfRangeException(nameof(value), "This is a 9-bit int, it cannot be higher than 511.");
                _cutPointB = value;
                // plants a 9 bit integer directly into _packedValue at index 23 (indices 0 - 22 are taken by the byte, nybble, bools and 9 bit int)
                BitOp.SetBits(ref _packedValue, _cutPointB, 23, 0, 9);
            }
        }
        /// <summary>
        /// A property for working with this tile's CutPointA. If editing both cut points at once, use <see cref="CutPoint"/> instead for performance.
        /// <para>Note that this represents a 9-bit integer; it cannot go above 511.</para>
        /// </summary>
        public ushort CutPointA
        {
            readonly get
            {
                return _cutPointA;
            }
            set
            {
                if (value > 511)
                    throw new ArgumentOutOfRangeException(nameof(value), "This is a 9-bit int, it cannot be higher than 511.");
                _cutPointA = value;
                // plants a 9 bit integer directly into _packedValue at index 14 (indices 0 - 13 are taken by the byte, nybble and bools)
                BitOp.SetBits(ref _packedValue, _cutPointA, 14, 0, 9);
            }
        }
        /// <summary>
        /// A property for working with this tile's CollideSide.
        /// <para>Set to true to use the top of the slice for collisions.</para>
        /// </summary>
        public bool CollideSide
        { 
            readonly get
            { 
                return _collideSide; 
            }
            set
            {
                _collideSide = value;
                // plants a bit directly into _packedValue at index 13 (indices 0 - 12 are taken by the byte, nybble and bool)
                BitOp.SetBits(ref _packedValue, _collideSide, 13, 0, 1);
            }
        }
        /// <summary>
        /// A property for working with this tile's PhaseThrough.
        /// <para><c>PhaseThrough = true</c> tiles do not collide with Kirby and are drawn darker than normal.</para>
        /// </summary>
        public bool PhaseThrough
        {
            readonly get 
            { 
                return _phaseThrough;
            }
            set
            {
                _phaseThrough = value;
                // plants a bit directly into _packedValue at index 12 (indices 0 - 11 are taken by the byte and nybble)
                BitOp.SetBits(ref _packedValue, _phaseThrough, 12, 0, 1);
            }
        }
        /// <summary>
        /// A property for working with this tile's SpriteVariation.
        /// <para>SpriteVariation is the index used when drawing each tile, to decide which section of the spritesheet to use. (Variation amounts are declared per TileDefinition singleton)</para>
        /// <para>Note that this is a nybble; it cannot go above 15</para>
        /// </summary>
        public byte SpriteVariation
        {
            readonly get
            {
                return _spriteVariation;
            }
            set
            {
                if (value > 15)
                    throw new ArgumentOutOfRangeException(nameof(value), "This is a nybble, it cannot be higher than 15.");
                _spriteVariation = value;
                // plants the lower nybble directly into _packedValue at index 8 (indices 0 - 7 are taken by the byte)
                BitOp.SetBits(ref _packedValue, _spriteVariation, 8, 0, 4);
            }
        }
        /// <summary>
        /// A property for working with this tile's PaletteIndex. Set to 0 to remove this tile, although underlying data won't be deleted until serialization.
        /// <para>The 'type' of this tile, relative to the per-world provided palette. Used for array look ups to get the TileDefinition that belongs to this tile.</para>
        /// </summary>
        public byte PaletteIndex
        {
            readonly get
            {
                return _paletteIndex;
            }
            set
            {
                _paletteIndex = value;
                // simple. plants this byte directly into _packedValue at index 0
                BitOp.SetBits(ref _packedValue, _paletteIndex, 0, 0, 8);
            }
        }
        public void SetSimpleSlope(SimpleSlope slope)
        {
            Point16U targetSlope = slope switch
            {
                _ => new(0, 255) // Default slope. Represents a full tile
            };
            CutPoint = targetSlope;
        }
    }
}
