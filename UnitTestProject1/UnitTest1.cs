﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESP8266_1;


namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public string testval1 = "111111111111111111111111111111111111"+
"0000 0000 0000 1111 0000 1111 1111 111 0000 1111"+
"0000 1111 0000 0000 0000 0000 1111 111 0000 1111"+
"0000 1111 0000 0000 1111 0000 1111 1111 0000 111"+
"0000 0000 0000 1111 1111 0000 1111 1111 0000 1111"+
"0000 0000 0000 0000 0000 000 1111 0000 0000 1111 "+
"0000 0000 0000 00000 1111 1111 111 0000 0000 1111"+
"0000 1111 0000 1111 1111 0000 0000 0000 000 1111"+
"0000 0000 1111 0000 1111 0000 0000 0000 000 1111"+
"0000 1111 1111 0000 0000 0000 1111 1111 0000 1111"+
"0000 0000 0000 000 1111 0000 1111 1111 0000 1111"+
"0000 1111 111 0000 1111 0000 1111 1111 00000 1111"+
"0000 1111 1111 0000 0000 1111 1111 111 0000 1111"+
"0000 1111 0000 11111 0000 11111 1111 11 0000 1111"+
"0000 1111 0000 1111 1111 0000 1111 1111 0000 111"+
"0000 0000 0000 0000 0000 00000 1111 0000 0000 1111"+
"0000 0000 0000 0000 000 1111 1111 0000 0000 1111"+
"0000 0000 0000 000 1111 1111 1111 1111 0000 1111"+
"0000 0000 1111 0000 0000 0000 1111 111 0000 1111 "+
"0000 0000 0000 1111 0000 0000 1111 1111 000 1111"+
"0000 1111 0000 1111 1111 0000 0000 00000 000 1111 "+
"0000 000 1111 0000 1111 0000 0000 0000 0000 1111"+
"0000 1111 111 0000 0000 0000 1111 1111 0000 1111"+
"0000 1111 1111 0000 000 1111 1111 1111 0000 1111"+
"0000 1111 0000 1111 0000 1111 1111 111 0000 1111"+
"0000 1111 0000 11111 111 0000 1111 1111 0000 111 "+
"0000 0000 0000 0000 0000 0000 1111 0000 0000 1111"+
"0000 0000 0000 0000 000 1111 1111 0000 0000 1111"+
"0000 0000 0000 000 1111 1111 1111 1111 0000 1111"+
"0000 0000 1111 000 0000 0000 1111 1111 0000 1111 "+
"0000 0000 0000 1111 0000 0000 111 1111 0000 1111 "+
"0000 1111 0000 1111 1111 0000 0000 0000 0000 111"+
"0000 0000 1111 0000 1111 0000 0000 0000 0000 1111"+
"111111111111111111111111111111111111111111111111";
        [TestMethod]
        public void TestMethod1()
        {
            var res = ESP8266_1.UART_Decode.b0InputToBinary(new byte[] {0, 1, 2, 3, 4}, 5, 0);
            Assert.AreEqual(res.Length, 1);
            Assert.AreSame(res[0], 0);
        }

    }
}
