// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected.Collection.Chain;

namespace GUPS.AntiCheat.Tests
{
    public class Protected_DataChain_Tests
    {

#if UNITY_EDITOR

        [SetUp]
        public void Setup_Global_Settings()
        {
            GUPS.AntiCheat.Settings.GlobalSettings.LoadOrCreateAsset();
        }

#endif

        [Test]
        public void Protected_DataChain_Int32_Test()
        {
            // Arrange.
            DataChain<Int32> dataChain = new DataChain<Int32>();

            dataChain.Append(1);
            dataChain.Append(2);
            dataChain.Append(3);
            dataChain.Append(4);
            dataChain.Append(5);

            // Assert - CheckIntegrity the integrity of the data chain.
            Assert.IsTrue(dataChain.CheckIntegrity());
        }

        [Test]
        public void Protected_DataChain_Int32_CheckIntegrity_Test()
        {
            // Arrange.
            DataChain<Int32> dataChain = new DataChain<Int32>();

            dataChain.Append(1);
            dataChain.Append(2);
            dataChain.Append(3);
            dataChain.Append(4);
            dataChain.Append(5);

            // Assert - CheckIntegrity the integrity of the data chain.
            Assert.IsTrue(dataChain.CheckIntegrity());

            // Act - Modify the data chain - With a not allowed operation.
            dataChain.Chain.RemoveLast();

            // Assert - CheckIntegrity the integrity of the data chain.
            Assert.IsFalse(dataChain.CheckIntegrity());

            // Reset the data chain.
            dataChain = new DataChain<Int32>();

            dataChain.Append(1);
            dataChain.Append(2);
            dataChain.Append(3);
            dataChain.Append(4);
            dataChain.Append(5);

            // Act - Modify the data chain - With an allowed operation.
            dataChain.RemoveLast();

            // Assert - CheckIntegrity the integrity of the data chain.
            Assert.IsTrue(dataChain.CheckIntegrity());
        }
    }
}
