﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf.unittest.Serializers
{
    public class SubItems
    {
        [Fact]
        public void TestWriteSubItemWithShortBlob() {
            Util.Test((ref ProtoWriter.State st) =>
            {
                st.WriteFieldHeader(5, WireType.String);
#pragma warning disable CS0618
                SubItemToken token = st.StartSubItem(new object());
                st.WriteFieldHeader(6, WireType.String);
                st.WriteBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
                st.EndSubItem(token);
#pragma warning restore CS0618
            }, "2A" // 5 * 8 + 2 = 42
             + "0A" // sub-item length = 10
             + "32" // 6 * 8 + 2 = 50 = 0x32
             + "08" // BLOB length
             + "0001020304050607"); // BLOB
        }
    }
}
