﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Xunit;
using ProtoBuf;
using Xunit.Abstractions;

namespace Examples.TestNumbers
{
    [ProtoContract]
    internal class NumRig
    {
        [ProtoMember(1, DataFormat=DataFormat.Default)]
        public int Int32Default { get; set; }
        [ProtoMember(2, DataFormat = DataFormat.ZigZag)]
        public int Int32ZigZag { get; set; }
        [ProtoMember(3, DataFormat = DataFormat.TwosComplement)]
        public int Int32TwosComplement { get; set; }
        [ProtoMember(4, DataFormat = DataFormat.FixedSize)]
        public int Int32FixedSize { get; set; }

        [ProtoMember(5, DataFormat = DataFormat.Default)]
        public uint UInt32Default { get; set; }
        [ProtoMember(7, DataFormat = DataFormat.TwosComplement)]
        public uint UInt32TwosComplement { get; set; }
        [ProtoMember(8, DataFormat = DataFormat.FixedSize)]
        public uint UInt32FixedSize { get; set; }

        [ProtoMember(9, DataFormat = DataFormat.Default)]
        public long Int64Default { get; set; }
        [ProtoMember(10, DataFormat = DataFormat.ZigZag)]
        public long Int64ZigZag { get; set; }
        [ProtoMember(11, DataFormat = DataFormat.TwosComplement)]
        public long Int64TwosComplement { get; set; }
        [ProtoMember(12, DataFormat = DataFormat.FixedSize)]
        public long Int64FixedSize { get; set; }

        [ProtoMember(13, DataFormat = DataFormat.Default)]
        public ulong UInt64Default { get; set; }
        [ProtoMember(15, DataFormat = DataFormat.TwosComplement)]
        public ulong UInt64TwosComplement { get; set; }
        [ProtoMember(16, DataFormat = DataFormat.FixedSize)]
        public ulong UInt64FixedSize { get; set; }

        [ProtoMember(17)]
        public string Foo { get; set; }
    }

    [ProtoContract]
    internal class ZigZagInt32
    {
        [ProtoMember(1, DataFormat = DataFormat.ZigZag)]
        [DefaultValue(123456)]
        public int Foo { get; set; }
    }
    [ProtoContract]
    internal class TwosComplementInt32
    {
        [ProtoMember(1, DataFormat = DataFormat.TwosComplement)]
        [DefaultValue(123456)]
        public int Foo { get; set; }
    }

    [ProtoContract]
    internal class TwosComplementUInt32
    {
        [ProtoMember(1, DataFormat = DataFormat.TwosComplement)]
        public uint Foo { get; set; }
    }

    [ProtoContract]
    internal class ZigZagInt64
    {
        [ProtoMember(1, DataFormat = DataFormat.ZigZag)]
        public long Foo { get; set; }
    }

    public class SignTests
    {
        public ITestOutputHelper Output { get; }

        public SignTests(ITestOutputHelper output)
            => Output = output;

        [Fact]
        public void RoundTripBigPosativeZigZagInt64()
        {
            ZigZagInt64 obj = new ZigZagInt64 { Foo = 123456789 },
                clone = Serializer.DeepClone(obj);
            Assert.Equal(obj.Foo, clone.Foo);
        }

        [Fact]
        public void RoundTripBigPosativeZigZagInt64ForDateTime()
        {
            // this test to simulate a typical DateTime value
            ZigZagInt64 obj = new ZigZagInt64 { Foo = 1216669168515 },
                clone = Serializer.DeepClone(obj);
            Assert.Equal(obj.Foo, clone.Foo);
        }

        [Fact]
        public void RoundTripBigNegativeZigZagInt64() {
            ZigZagInt64 obj = new ZigZagInt64 { Foo = -123456789 },
                clone = Serializer.DeepClone(obj);
            Assert.Equal(obj.Foo, clone.Foo);
        }

        [Fact]
        public void TestSignTwosComplementInt32_0()
        {
            Assert.True(Program.CheckBytes(new TwosComplementInt32 { Foo = 0 }, 0x08, 0x00), "0");
        }
        [Fact]
        public void TestSignTwosComplementInt32_Default()
        {
            Assert.True(Program.CheckBytes(new TwosComplementInt32 { Foo = 123456 }), "123456");
        }
        [Fact]
        public void TestSignTwosComplementInt32_1()
        {
            Assert.True(Program.CheckBytes(new TwosComplementInt32 { Foo = 1 }, 0x08, 0x01), "+1");
        }
        [Fact]
        public void TestSignTwosComplementInt32_2()
        {
            Assert.True(Program.CheckBytes(new TwosComplementInt32 { Foo = 2 }, 0x08, 0x02), "+2");
        }
        [Fact]
        public void TestSignTwosComplementInt32_m1()
        {
            Assert.True(Program.CheckBytes(Output, new TwosComplementInt32 { Foo = -1 }, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01), "-1");
        }
        [Fact]
        public void TestSignTwosComplementInt32_m2()
        {
            Assert.True(Program.CheckBytes(new TwosComplementInt32 { Foo = -2 }, 0x08, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01), "-2");
        }
        [Fact]
        public void TestSignZigZagInt32_0()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = 0 }, 0x08, 0x00), "0");
        }
        [Fact]
        public void TestSignZigZagInt32_Default()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = 123456 }), "123456");
        }
        [Fact]
        public void TestSignZigZagInt32_1()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = 1 }, 0x08, 0x02), "+1");
        }
        [Fact]
        public void TestSignZigZagInt32_2()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = 2 }, 0x08, 0x04), "+2");
        }
        [Fact]
        public void TestSignZigZagInt32_m1()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = -1 }, 0x08, 0x01), "-1");
        }
        [Fact]
        public void TestSignZigZagInt32_m2()
        {
            Assert.True(Program.CheckBytes(new ZigZagInt32 { Foo = -2 }, 0x08, 0x03), "-2");
        }
        [Fact]
        public void TestSignZigZagInt32_2147483647()
        {
            // encoding doc gives numbers in terms of uint equivalent
            ZigZagInt32 zz = new ZigZagInt32 { Foo = 2147483647 }, clone = Serializer.DeepClone(zz);
            Assert.Equal(zz.Foo, clone.Foo); //, "Roundtrip");
            TwosComplementUInt32 tc = Serializer.ChangeType<ZigZagInt32, TwosComplementUInt32>(zz);
            Assert.Equal(4294967294, tc.Foo);
        }
        [Fact]
        public void TestSignZigZagInt32_m2147483648()
        {
            // encoding doc gives numbers in terms of uint equivalent
            ZigZagInt32 zz = new ZigZagInt32 { Foo = -2147483648 }, clone = Serializer.DeepClone(zz);
            Assert.Equal(zz.Foo, clone.Foo); //, "Roundtrip");
            TwosComplementUInt32 tc = Serializer.ChangeType<ZigZagInt32, TwosComplementUInt32>(zz);
            Assert.Equal(4294967295, tc.Foo);
        }

        [Fact]
        public void TestEOF()
        {
            Program.ExpectFailure<EndOfStreamException>(() =>
            {
                Program.Build<ZigZagInt32>(0x08); // but no payload for field 1
            });
        }

        [Fact]
        public void TestOverflow()
        {
            Program.ExpectFailure<OverflowException>(()
                => Program.Build<ZigZagInt32>(0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF));
        }

        [Fact]
        public void SweepBitsInt32()
        {
            NumRig rig = new NumRig();
            const string SUCCESS = "bar";
            rig.Foo = SUCCESS; // to help test stream ending prematurely
            for (int i = 0; i < 32; i++)
            {
                int bigBit = i == 0 ? 0 : (1 << i - 1);
                for (int j = 0; j <= i; j++)
                {
                    int smallBit = 1 << j;
                    int val = bigBit | smallBit;
                    rig.Int32Default
                        = rig.Int32FixedSize
                        = rig.Int32TwosComplement
                        = rig.Int32ZigZag
                        = val;

                    NumRig clone = Serializer.DeepClone(rig);
                    Assert.Equal(val, clone.Int32Default);
                    Assert.Equal(val, clone.Int32FixedSize);
                    Assert.Equal(val, clone.Int32TwosComplement);
                    Assert.Equal(val, clone.Int32ZigZag);
                    Assert.Equal(SUCCESS, clone.Foo);
                }
            }
        }

        [Fact]
        public void SweepBitsInt64KnownTricky()
        {
            const int i = 31, j = 31;
            const long bigBit = i == 0 ? 0 : (1 << i - 1);
            const long smallBit = 1 << j;
            const long val = bigBit | smallBit;
            NumRig rig = new NumRig();
            rig.Int64Default // 9 => 72
                = rig.Int64FixedSize // 12 => 97?
                = rig.Int64TwosComplement // 11 => 88
                = rig.Int64ZigZag // 10 => 80
                = val;
            const string SUCCESS = "bar";
            rig.Foo = SUCCESS; // to help test stream ending prematurely

            MemoryStream ms = new MemoryStream();
            Serializer.Serialize(ms, rig);
            _ = ms.ToArray();
            ms.Position = 0;
            NumRig clone = Serializer.Deserialize<NumRig>(ms);

            Assert.Equal(val, clone.Int64Default); //, "Default");
            Assert.Equal(val, clone.Int64FixedSize); //, "FixedSize");
            Assert.Equal(val, clone.Int64ZigZag); //, "ZigZag");
            Assert.Equal(val, clone.Int64TwosComplement); //, "TwosComplement");
            Assert.Equal(SUCCESS, clone.Foo); //, "EOF check");
        }

        [Fact]
        public void SweepBitsInt64()
        {
            NumRig rig = new NumRig();
            const string SUCCESS = "bar";
            rig.Foo = SUCCESS; // to help test stream ending prematurely
            for (int i = 0; i < 64; i++)
            {
                long bigBit = i == 0 ? 0 : (1 << i - 1);
                for (int j = 0; j <= i; j++)
                {
                    long smallBit = 1 << j;
                    long val = bigBit | smallBit;
                    rig.Int64Default
                        = rig.Int64FixedSize
                        = rig.Int64TwosComplement
                        = rig.Int64ZigZag
                        = val;

                    NumRig clone = Serializer.DeepClone(rig);
                    Assert.Equal(val, clone.Int64Default); //, "Default");
                    Assert.Equal(val, clone.Int64FixedSize); //, "FixedSize");
                    Assert.Equal(val, clone.Int64ZigZag); //, "ZigZag");
                    Assert.Equal(val, clone.Int64TwosComplement); //, "TwosComplement");
                    Assert.Equal(SUCCESS, clone.Foo); //, "EOF check: " + val.ToString());
                }
            }
        }
    }
}
