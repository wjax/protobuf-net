﻿using Xunit;
using ProtoBuf;
using ProtoBuf.Meta;
using System.IO;

namespace Examples.Issues
{
    
    public class SO16797650
    {
        [ProtoContract]
        public abstract class MessageBase
        {
            [ProtoMember(1)]
            public string ErrorMessage { get; set; }

            public abstract int Type { get; }
        }

        [ProtoContract]
        public class Echo : MessageBase
        {
            public const int ID = 1;

            public override int Type
            {
                get { return ID; }
            }

            [ProtoMember(1)]
            public string Message { get; set; }
        }
        [ProtoContract]
        public class Foo : MessageBase { public override int Type { get { return 42; } } }
        [ProtoContract]
        public class Bar : MessageBase { public override int Type { get { return 43; } } }
        [Fact]
        public void AddSubtypeAtRuntime()
        {
            var model = RuntimeTypeModel.Create();
            var messageBase = model[typeof(MessageBase)];
            // this could be explicit in code, or via some external config file
            // that you process at startup
            messageBase.AddSubType(10, typeof(Echo)); // would need to **reliably** be 10
            messageBase.AddSubType(11, typeof(Foo));
            messageBase.AddSubType(12, typeof(Bar)); // etc

            // test it...
            Echo echo = new Echo { Message = "Some message", ErrorMessage = "XXXXX" };
            MessageBase echo1;
            using (var ms = new MemoryStream())
            {
#pragma warning disable CS0618
                model.Serialize(ms, echo);
                ms.Position = 0;
                echo1 = model.Deserialize<MessageBase>(ms);
#pragma warning restore CS0618
            }
            Assert.Same(echo.GetType(), echo1.GetType());
            Assert.Equal(echo.ErrorMessage, echo1.ErrorMessage);
            Assert.Equal(echo.Message, ((Echo)echo1).Message);
        }
    }
}
