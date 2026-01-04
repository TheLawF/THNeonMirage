using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fictology.Data.Serialization;
using Unity.VisualScripting;

namespace Fictology.Data.Record
{
    public abstract record DataRecord: ISynchronizable
    {
        public sealed record Empty : DataRecord
        {
            protected override int GetRecordType() => 0;
            protected override int GetByteLength() => 0;
        }

        public sealed record Int(int Value) : DataRecord
        {
            protected override int GetRecordType() => 1;
            protected override int GetByteLength() => 32;
        }
        public sealed record Float(float Value) : DataRecord
        {
            protected override int GetRecordType() => 2;
            protected override int GetByteLength() => 32;
        }

        public sealed record Double(double Value) : DataRecord
        {
            protected override int GetRecordType() => 3;
            protected override int GetByteLength() => 64;
        }
        public sealed record Long(long Value) : DataRecord
        {
            protected override int GetRecordType() => 4;
            protected override int GetByteLength() => 64;
        }

        public sealed record String(string Value, int ByteLength = 65535) : DataRecord
        {
            protected override int GetRecordType() => 5;
            protected override int GetByteLength() => ByteLength;
        }

        public sealed record List(List<DataRecord> Records) : DataRecord
        {
            protected override int GetRecordType() => 6;

            protected override int GetByteLength()
            {
                var len = Records.Count;
                Records.Select(r => r.GetByteLength()).ToList().ForEach(i => len *= i);
                return len;
            }
        }
        
        public sealed record Object(ConcurrentDictionary<String, DataRecord> Dictionary) : DataRecord
        {
            protected override int GetRecordType() => 7;
            protected override int GetByteLength() => Dictionary.Count;
            public void Add(String key, DataRecord value) => Dictionary.TryAdd(key, value);
        }

        protected abstract int GetRecordType();
        protected abstract int GetByteLength();

        public byte[] Serialize() => this switch
        {
            Empty => Array.Empty<byte>(),
            Int i => Int2Bytes(i),
            Float f => Float2Bytes(f),
            Double d => Double2Bytes(d),
            String s => String2Bytes(s),
            List list => List2Bytes(list),
            Long l => Long2Bytes(l),
            Object o => Object2Bytes(o),
            _ => Array.Empty<byte>()
        };

        public DataRecord Deserialize(byte[] bytes) => GetRecordType() switch
        {
            0 => new Empty(),
            1 => Bytes2Int(bytes),
            2 => Bytes2Float(bytes),
            3 => Bytes2Double(bytes),
            4 => Bytes2Long(bytes),
            5 => Bytes2String(bytes),
            6 => Bytes2List(bytes),
            7 => throw new NotImplementedException(),
            _ => new Empty()
        };

        public static DataRecord ByTypeInt(int typeInt) => typeInt switch
        {
            0 => new Empty(),
            1 => WithInt(),
            2 => WithFloat(),
            3 => WithDouble(),
            4 => WithLong(),
            5 => WithString(),
            6 => NewList(),
            7 => NewObject(),
            _ => new Empty()
        };

        public static Int WithInt() => new (0);
        public static Float WithFloat() => new (0F);
        public static Double WithDouble() => new (0D);
        public static Long WithLong() => new (0L);
        public static String WithString() => new ("");
        public static List NewList() => new (new List<DataRecord>());
        public static Object NewObject() => new (new ConcurrentDictionary<String, DataRecord>());
        
        public byte[] ToBytes() => Serialize();

        public void FromBytes(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }
        
        
        private byte[] Int2Bytes(Int i)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(i.GetRecordType());
            writer.Write(i.Value);
            return stream.ToArray();
        }
        
        private DataRecord Bytes2Int(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return new Int(reader.ReadInt32());
        }
        
        private byte[] Float2Bytes(Float f)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(f.GetRecordType());
            writer.Write(f.Value);
            return stream.ToArray();
        }
        
        private DataRecord Bytes2Float(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return new Float(reader.ReadInt32());
        }
        
        private byte[] Double2Bytes(Double mode)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(mode.GetRecordType());
            writer.Write(mode.Value);
            return stream.ToArray();
        }
        
        private DataRecord Bytes2Double(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return new Double(reader.ReadInt64());
        }
        
        private byte[] Long2Bytes(Long mode)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(mode.GetRecordType());
            writer.Write(mode.Value);
            return stream.ToArray();
        }
        
        private DataRecord Bytes2Long(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return new Long(reader.ReadInt64());
        }
        
        private byte[] String2Bytes(String mode)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(mode.GetRecordType());
            writer.Write(mode.Value);
            return stream.ToArray();
        }
        
        private DataRecord Bytes2String(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            return new String(reader.ReadString());
        }
        
        private byte[] List2Bytes(List list)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            writer.Write(list.GetRecordType());
            writer.Write(list.Records.Count);
            foreach (var mode in list.Records)
            {
                writer.Write(mode.GetRecordType());
                writer.Write(mode.Serialize());
            }

            return stream.ToArray();
        }
        
        private DataRecord Bytes2List(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
                
            var length = reader.ReadInt32();
            var list = new List<DataRecord>();
            for (var i = 0; i < length; i++)
            {
                var type = reader.ReadInt32();
                list.Add(ByTypeInt(type).Deserialize(bytes));
            }

            return new List(list);
        }
        
        private byte[] Object2Bytes(Object obj)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(obj.GetRecordType());
            writer.Write(obj.Dictionary.Count);
            foreach (var mode in obj.Dictionary)
            {
                writer.Write(mode.Key.GetRecordType());
                writer.Write(mode.Key.Serialize());
                
                writer.Write(mode.Value.GetRecordType());
                writer.Write(mode.Value.Serialize());
            }

            return stream.ToArray();
        }
        
        private DataRecord Bytes2Object(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
                
            var length = reader.ReadInt32();
            var dictionary = new ConcurrentDictionary<String, DataRecord>();
            for (var i = 0; i < length; i++)
            {
                var keyType = reader.ReadInt32();
                var key = (String) ByTypeInt(keyType).Deserialize(bytes);

                var valueType = reader.ReadInt32();
                var valueRecord = ByTypeInt(valueType).Deserialize(bytes);

                dictionary.TryAdd(key, valueRecord);
            }

            return new Object(dictionary);
        }
    }
}