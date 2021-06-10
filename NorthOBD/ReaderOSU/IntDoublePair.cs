using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NorthOBD.ReaderOSU
{
    struct IntDouble
    {
        public uint intPart;
        public double doublePart;
    }

    class IntDoublePair
    {
        private uint _numberOfPair;
        private IntDouble[] _pairID;
        public IntDoublePair(ref BinaryReader readerDB)// : this(ref readerDB, readerDB.BaseStream.Position)
        {
            this.ReadIntDouble(ref readerDB);
        }
        public IntDoublePair(ref BinaryReader readerDB, long position)
        {
            this.ReadIntDouble(ref readerDB, position);
        }

        public IntDouble[] GetIntDouble()
        {
            return _pairID;
        }

        private void ReadIntDouble(ref BinaryReader readerDB)
        {
            this._numberOfPair = readerDB.ReadUInt32();
            this._pairID = new IntDouble[this._numberOfPair];
            for (uint index = 0; index < this._numberOfPair; index++)
            {
                readerDB.BaseStream.Position += 1;
                _pairID[index].intPart = readerDB.ReadUInt32();
                readerDB.BaseStream.Position += 1;
                _pairID[index].doublePart = readerDB.ReadDouble();

            }
        }

        private void ReadIntDouble(ref BinaryReader readerDB, long position)
        {
            long savePosition = readerDB.BaseStream.Position;
            readerDB.BaseStream.Position = position;
            this._numberOfPair = readerDB.ReadUInt32();
            this._pairID = new IntDouble[this._numberOfPair];
            for (uint index = 0; index < this._numberOfPair; index++)
            {
                readerDB.BaseStream.Position += 1;
                _pairID[index].intPart = readerDB.ReadUInt32();
                readerDB.BaseStream.Position += 1;
                _pairID[index].doublePart = readerDB.ReadDouble();

            }
            readerDB.BaseStream.Position = savePosition;
        }
    }
}
