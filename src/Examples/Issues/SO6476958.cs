﻿#if FEAT_DYNAMIC_REF

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using ProtoBuf;
using ProtoBuf.Meta;
namespace Examples.Issues
{
    
    public class SO6476958
    {
        [ProtoContract]
        public class A
        {
            [ProtoMember(1, AsReference = true)]
            public string Id { get; set; }

            public override bool Equals(object obj) { return Id == ((A)obj).Id; }
            public override int GetHashCode() { return Id.GetHashCode(); }
            public override string ToString() { return Id; }
        }
        [ProtoContract]
        public class B
        {
            [ProtoMember(1)]
            public string Id { get; set; }

            public override bool Equals(object obj) { return Id == ((B)obj).Id; }
            public override int GetHashCode() { return Id.GetHashCode(); }
            public override string ToString() { return Id; }
        }

        [ProtoContract]
        public class BasicDuplicatedString
        {
            [ProtoMember(1, AsReference = true)]
            public string A {get;set;}
            [ProtoMember(2, AsReference = true)]
            public string B { get; set; }

        }
        [Fact]
        public void TestBasicDuplicatedString()
        {
            BasicDuplicatedString foo = new BasicDuplicatedString
            {
                A = new string('a', 40),
                B = new string('a', 40)
            }, clone;
            Assert.NotSame(foo.A, foo.B); // different string refs

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, foo);
                Assert.Equal(50, ms.Length);
                ms.Position = 0;
                clone = Serializer.Deserialize<BasicDuplicatedString>(ms);
            }
            Assert.Equal(foo.A, clone.A);
            Assert.Equal(foo.B, clone.B);
            Assert.Same(clone.A, clone.B);
        }

        [Fact]
        public void Execute()
        {
            var m = RuntimeTypeModel.Create();
            m.AutoCompile = false;
            m.Add(typeof(object), false).AddSubType(1, typeof(A)).AddSubType(2, typeof(B));

            Test(m);
            m.CompileInPlace();
            Test(m);
            Test(m.Compile());
        }

        private static void Test(TypeModel m)
        {
            var list = new List<object> { new A { Id = "Abracadabra" }, new B { Id = "Focuspocus" }, new A { Id = "Abracadabra" }, };
            using var ms = new MemoryStream();
            m.Serialize(ms, list);
            ms.Position = 0;
#pragma warning disable CS0618
            var list2 = (List<object>)m.Deserialize(ms, null, typeof(List<object>));
#pragma warning restore CS0618
            Debug.Assert(list.SequenceEqual(list2));
            File.WriteAllBytes(@"output.dump", ms.ToArray());
        }
    }
}


#endif