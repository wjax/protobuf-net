﻿#if !COREFX
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using Xunit;

namespace Examples.Issues
{

    public class Issue124
    {
        // note this is a simplified example that 
        [ProtoContract]
        public struct MyColor
        {
            [ProtoMember(1, DataFormat=DataFormat.FixedSize)]
            public uint ARGB { get; set; }

            public static explicit operator MyColor  (Color color)
            {
                return new MyColor { ARGB = ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | (uint)color.B};
            }
            public static explicit operator Color(MyColor color)
            {
                return new Color { A = (byte)(color.ARGB >> 24), R = (byte)(color.ARGB >> 16), G = (byte)(color.ARGB >> 8), B = (byte)color.ARGB };
            }
        }
        [ProtoContract]
        public class TypeWithColor
        {
            [ProtoMember(1)]
            public Color Color { get; set; }
        }

        [SkippableFact]
        public void TestMediaColorDirect()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            AvoidJit();
            static void AvoidJit()
            {
                var model = RuntimeTypeModel.Create();
                model.Add(typeof(Color), false).Add("A", "R", "G", "B");

                RoundtripTypeWithColor(model);
            }
        }

        [SkippableFact]
        public void TestMediaColorSurrogate()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            AvoidJit();

            static void AvoidJit()
            {
                var model = RuntimeTypeModel.Create();
                model.Add(typeof(Color), false).SetSurrogate(typeof(MyColor));

                RoundtripTypeWithColor(model);
            }
        }

        private static void RoundtripTypeWithColor(RuntimeTypeModel model)
        {
            var orig = new TypeWithColor
            {
                Color = new Color { A = 1, R = 2, G = 3, B = 4 }
            };
            var clone = (TypeWithColor)model.DeepClone(orig);
            Assert.Equal(1, clone.Color.A);
            Assert.Equal(2, clone.Color.R);
            Assert.Equal(3, clone.Color.G);
            Assert.Equal(4, clone.Color.B);
        }
    }
}
#endif