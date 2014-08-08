/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2014 eaxvac/lastBattle https://github.com/eaxvac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Text;

namespace MSEAHackUtility
{
    public sealed class Packet : IDisposable
    {

        private MemoryStream memoryStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public ushort OpCode { get; private set; }

        public int Length { get { return (int)this.memoryStream.Length; } }
        public int Cursor { get { return (int)this.memoryStream.Position; } }
        public int Remaining { get { return (int)(this.memoryStream.Length - this.memoryStream.Position); } }

        public Packet()
        {
            this.memoryStream = new MemoryStream();
            this.writer = new BinaryWriter(this.memoryStream);
        }

        public Packet(ushort pOpCode)
        {
            this.memoryStream = new MemoryStream();
            this.writer = new BinaryWriter(this.memoryStream);
            this.OpCode = pOpCode;
            WriteUShort(pOpCode);
        }

        public Packet(byte[] pData, bool tosend = false)
        {
            if (!tosend)
            {
                this.memoryStream = new MemoryStream(pData);
                this.reader = new BinaryReader(this.memoryStream);

                ushort opCode;
                this.TryReadUShort(out opCode);
                this.OpCode = opCode;
            }
            else
            {
                this.memoryStream = new MemoryStream();
                this.writer = new BinaryWriter(this.memoryStream);
                WriteBytes(pData);
                this.OpCode = BitConverter.ToUInt16(pData, 0);
            }
        }

        public Packet(SendOps opcode) : this((ushort)opcode) { }

        public void Dispose()
        {
            if (this.writer != null) this.writer.Close();
            if (this.reader != null) this.reader.Close();
            this.memoryStream = null;
            this.writer = null;
            this.reader = null;
        }

        ~Packet()
        {
            Dispose();
        }

        public void Seek(int offset)
        {
            if (offset > this.Length) throw new IndexOutOfRangeException("Cannot go to packet offset.");
            this.memoryStream.Seek(offset, SeekOrigin.Begin);
        }

        #region Write methods

        public void WriteHexAsBytes(string hexString)
        {
            byte[] bytes = ByteUtils.HexToBytes(hexString);
            WriteBytes(bytes);
        }

        public void SetByte(long pOffset, byte pValue)
        {
            long oldoffset = this.memoryStream.Position;
            this.memoryStream.Seek(pOffset, SeekOrigin.Begin);
            this.writer.Write(pValue);
            this.memoryStream.Seek(oldoffset, SeekOrigin.Begin);
        }

        public void Fill(int pLength, byte pValue)
        {
            for (int i = 0; i < pLength; ++i)
            {
                WriteByte(pValue);
            }
        }

        public void WriteDouble(double pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteBool(bool pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteSkip(uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                this.writer.Write(0);
            }
        }

        public void WriteByte(byte pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteSByte(sbyte pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteBytes(byte[] pBytes)
        {
            this.writer.Write(pBytes);
        }

        public void WriteUShort(ushort pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteShort(short pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteUInt(uint pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteInt(int pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteFloat(float pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteULong(ulong pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteLong(long pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteChar(char pValue)
        {
            this.writer.Write(pValue);
        }

        public void WriteString(string pValue)
        {
            WriteUShort((ushort)pValue.Length);
            WriteBytes(Encoding.ASCII.GetBytes(pValue));
        }

        public void WriteString(string pValue, int pLen)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(pValue);
            if (buffer.Length > pLen)
            {
                throw new ArgumentException("pValue is bigger than pLen", "pLen");
            }
            else
            {
                WriteBytes(buffer);
                for (int i = 0; i < pLen - buffer.Length; i++)
                {
                    WriteByte(0);
                }
            }
        }

        #endregion

        #region Read methods

        public bool ReadSkip(int pLength)
        {
            if (Remaining < pLength) return false;

            this.memoryStream.Seek(pLength, SeekOrigin.Current);
            return true;
        }

        public bool ReadBool()
        {
            return this.reader.ReadBoolean();
        }

        public bool TryReadBool(out bool pValue)
        {
            pValue = false;
            if (Remaining < 1) return false;
            pValue = this.reader.ReadBoolean();
            return true;
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public bool TryReadByte(out byte pValue)
        {
            pValue = 0;
            if (Remaining < 1) return false;
            pValue = this.reader.ReadByte();
            return true;
        }

        public bool TryReadBytes(int pLength, out byte[] pValue)
        {
            pValue = new byte[] { };
            if (Remaining < pLength) return false;
            pValue = this.reader.ReadBytes(pLength);
            return true;
        }

        public sbyte ReadSByte()
        {
            return this.reader.ReadSByte();
        }

        public bool TryReadSByte(out sbyte pValue)
        {
            pValue = 0;
            if (Remaining < 1) return false;
            pValue = this.reader.ReadSByte();
            return true;
        }

        // UInt16 is more conventional
        public ushort TryReadUShort()
        {
            return this.reader.ReadUInt16();
        }

        // UInt16 is more conventional
        public bool TryReadUShort(out ushort pValue)
        {
            pValue = 0;
            if (Remaining < 2) return false;
            pValue = this.reader.ReadUInt16();
            return true;
        }

        // Int16 is more conventional
        public short ReadShort()
        {
            return this.reader.ReadInt16();
        }

        // Int16 is more conventional
        public bool TryReadShort(out short pValue)
        {
            pValue = 0;
            if (Remaining < 2) return false;
            pValue = this.reader.ReadInt16();
            return true;
        }

        public float ReadFloat()
        {
            return this.reader.ReadSingle();
        }

        public bool TryReadFloat(out float pValue)
        {
            pValue = 0;
            if (Remaining < 2) return false;
            pValue = this.reader.ReadSingle();
            return true;
        }

        // UInt32 is better
        public uint ReadUInt()
        {
            return this.reader.ReadUInt32();
        }

        // UInt32 is better
        public bool TryReadUInt(out uint pValue)
        {
            pValue = 0;
            if (Remaining < 4) return false;
            pValue = this.reader.ReadUInt32();
            return true;
        }

        // Int32
        public int ReadInt()
        {
            return this.reader.ReadInt32();
        }

        // Int32
        public bool TryReadInt(out int pValue)
        {
            pValue = 0;
            if (Remaining < 4) return false;
            pValue = this.reader.ReadInt32();
            return true;
        }

        // UInt64
        public ulong ReadULong()
        {
            return this.reader.ReadUInt64();
        }

        // UInt64
        public bool TryReadULong(out ulong pValue)
        {
            pValue = 0;
            if (Remaining < 8) return false;
            pValue = this.reader.ReadUInt64();
            return true;
        }

        // UInt64
        public long ReadLong()
        {
            return this.reader.ReadInt64();
        }

        // UInt64
        public bool TryReadLong(out long pValue)
        {
            pValue = 0;
            if (Remaining < 8) return false;
            pValue = this.reader.ReadInt64();
            return true;
        }

        public String ReadMapleAsciiString()
        {
            ushort len;
            this.TryReadUShort(out len);
            if (this.Remaining < len)
            {
                throw new Exception("String out of range");
            }
            string pValue;

            TryReadString(out pValue, len);

            return pValue;
        }

        public bool TryReadString(out string pValue)
        {
            pValue = "";
            if (this.Remaining < 2) return false;
            ushort len;
            this.TryReadUShort(out len);
            if (this.Remaining < len) return false;
            return TryReadString(out pValue, len);
        }

        public String ReadString(int len)
        {
            string pValue;
            TryReadString(out pValue, len);

            return pValue;
        }

        public bool TryReadString(out string pValue, int pLen)
        {
            pValue = "";
            if (Remaining < pLen) return false;

            byte[] buffer = new byte[pLen];
            TryReadBytes(buffer);
            int length = 0;
            if (buffer[pLen - 1] != 0)
            {
                length = pLen;
            }
            else
            {
                while (buffer[length] != 0x00 && length < pLen)
                {
                    length++;
                }
            }
            if (length > 0)
            {
                pValue = Encoding.ASCII.GetString(buffer, 0, length);
            }

            return true;
        }

        public byte[] ReadBytes(int len)
        {
            byte[] d = new byte[len];
            this.memoryStream.Read(d, 0, len);
            return d;
        }

        public bool TryReadBytes(byte[] pBuffer)
        {
            if (Remaining < pBuffer.Length) return false;
            this.memoryStream.Read(pBuffer, 0, pBuffer.Length);
            return true;
        }

        #endregion

        public byte[] ToArray()
        {
            return this.memoryStream.ToArray();
        }

        public string Dump()
        {
            return ByteUtils.BytesToHex(this.memoryStream.ToArray(), string.Format("Packet (0x{0} - {1}): ", this.OpCode.ToString("X4"), this.Length));
        }

        public string DumpRemaining()
        {
            byte[] buf = new byte[this.Length - this.memoryStream.Position];
            Buffer.BlockCopy(this.memoryStream.ToArray(), (int)this.memoryStream.Position, buf, 0, buf.Length);

            return ByteUtils.BytesToHex(buf, string.Format("Packet (0x{0} - {1}): ", this.OpCode.ToString("X4"), this.Length));
        }

        public override string ToString()
        {
            byte[] buf = new byte[this.Length - 2];
            Buffer.BlockCopy(this.memoryStream.ToArray(), 2, buf, 0, buf.Length);
            return string.Format("Opcode: 0x{0:X4} Length: {1} Data: {2}", this.OpCode, buf.Length, ByteUtils.BytesToHex(buf));
        }

        public string ToPacketsString()
        {
            byte[] buf = new byte[this.Length];
            Buffer.BlockCopy(this.memoryStream.ToArray(), 0, buf, 0, buf.Length);
            return ByteUtils.BytesToHex(buf);
        }
    }
}
