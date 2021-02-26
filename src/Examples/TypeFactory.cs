﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Examples
{
    
    public class TypeFactory
    {
        [Fact]
        public void TestInternal()
        {
            var model = RuntimeTypeModel.Create();
            model.AutoCompile = false;
            model.Add(typeof (CanHazFactory), true).SetFactory("MagicMaker");

            Check(model, null, 42, "Runtime");
            model.CompileInPlace();
            Check(model, null, 42, "CompileInPlace");
            Check(model.Compile(), null, 42, "Compile");
        }
        [Fact]
        public void TestExternal()
        {
            var model = RuntimeTypeModel.Create();
            model.AutoCompile = false;
            model.Add(typeof(CanHazFactory), true).SetFactory(typeof(TypeFactory).GetMethod("ExternalFactory"));
            var ctx = new SerializationContext {Context = 12345};
            Check(model, ctx, 12345, "Runtime");
            model.CompileInPlace();
            Check(model, ctx, 12345, "CompileInPlace");
            Check(model.Compile(), ctx, 12345, "Compile");
        }
        private void Check(TypeModel model, SerializationContext ctx, int magicNumber, string caption)
        {
            try
            {
                CanHazFactory orig = new CanHazFactory {Foo = 123, Bar = 456}, clone;
                using(var ms = new MemoryStream())
                {
                    model.Serialize(ms, orig, context: ctx);
                    ms.Position = 0;
#pragma warning disable CS0618
                    clone = (CanHazFactory) model.Deserialize(ms, null, typeof(CanHazFactory), ctx);
#pragma warning restore CS0618
                }

                Assert.NotSame(orig, clone);

                Assert.Equal(123, orig.Foo); //, caption);
                Assert.Equal(456, orig.Bar); //, caption);
                Assert.Equal(0, orig.MagicNumber); //, caption);

                Assert.Equal(123, clone.Foo); //, caption);
                Assert.Equal(456, clone.Bar); //, caption);
                Assert.Equal(magicNumber, clone.MagicNumber); //, caption);

            } catch
            {
                Debug.WriteLine(caption);
                throw;
            }

        }

        public static CanHazFactory ExternalFactory(SerializationContext ctx)
        {
            return new CanHazFactory { MagicNumber = (int)ctx.Context };
        }
        [ProtoContract]
        public class CanHazFactory
        {
            public static CanHazFactory MagicMaker()
            {
                return new CanHazFactory {MagicNumber = 42};
            }

            public int MagicNumber { get; set; }

            [ProtoMember(1)]
            public int Foo { get; set; }

            [ProtoMember(2)]
            public int Bar { get; set; }
        }
    }
    
}
