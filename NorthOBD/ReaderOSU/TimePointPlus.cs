using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NorthOBD.ReaderOSU
{
    struct TimePoint 
    {
        public double BPM;
        public double offsetSong;
        public bool timingPoint;
    }
    class TimePointPlus
    {
        private uint _numberOfTimePoints;
        private TimePoint[] _timePoints;

        public TimePointPlus(ref BinaryReader readerDB)// : this(ref readerDB, readerDB.BaseStream.Position)
        {
            this.ReadTimePoints(ref readerDB);
        }
        public TimePointPlus(ref BinaryReader readerDB, long position)
        {
            this.ReadTimePoints(ref readerDB, position);
        }

        public TimePoint[] GetTimePoints()
        {
            return _timePoints;
        }

        private void ReadTimePoints(ref BinaryReader readerDB)
        {
            this._numberOfTimePoints = readerDB.ReadUInt32();
            this._timePoints = new TimePoint[this._numberOfTimePoints];
            for (uint index = 0; index < this._numberOfTimePoints; index++)
            {
                _timePoints[index].BPM = readerDB.ReadDouble();
                _timePoints[index].offsetSong = readerDB.ReadDouble();
                _timePoints[index].timingPoint = readerDB.ReadBoolean();

            }
        }

        private void ReadTimePoints(ref BinaryReader readerDB, long position)
        {

            long savePosition = readerDB.BaseStream.Position;
            readerDB.BaseStream.Position = position;
            this._numberOfTimePoints = readerDB.ReadUInt32();
            this._timePoints = new TimePoint[this._numberOfTimePoints];
            for (uint index = 0; index < this._numberOfTimePoints; index++)
            {
                _timePoints[index].BPM = readerDB.ReadDouble();
                _timePoints[index].offsetSong = readerDB.ReadDouble();
                _timePoints[index].timingPoint = readerDB.ReadBoolean();

            }
            readerDB.BaseStream.Position = savePosition;
        }
    }
}
