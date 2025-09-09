// System
using System;
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
    public class Protected_BlockChain_Tests
    {

#if UNITY_EDITOR

        [SetUp]
        public void Setup_Global_Settings()
        {
            GUPS.AntiCheat.Settings.GlobalSettings.LoadOrCreateAsset();
        }

#endif

        [Test]
        public void Protected_BlockChain_Local_Int32_Test()
        {
            // Arrange
            BlockChain<Int32> blockChain = new BlockChain<Int32>(10);

            // Act
            blockChain.Append(1);
            blockChain.Append(2);
            blockChain.Append(3);

            // Assert - Count
            Assert.AreEqual(1, blockChain.Chain.Count);

            // Assert - Content
            Assert.AreEqual(1, blockChain.Chain.First.Value.Items[0].Content);
            Assert.AreEqual(2, blockChain.Chain.First.Value.Items[1].Content);
            Assert.AreEqual(3, blockChain.Chain.First.Value.Items[2].Content);

            // Assert - Integrity
            Assert.IsTrue(blockChain.CheckIntegrityOfLastBlock());
            Assert.IsTrue(blockChain.CheckIntegrity());
        }

        [Test]
        public void Protected_BlockChain_Local_Int32_Multiple_Blocks_Test()
        {
            // Arrange
            BlockChain<Int32> blockChain = new BlockChain<Int32>(2);

            // Act
            blockChain.Append(1);
            blockChain.Append(2);
            blockChain.Append(3);
            blockChain.Append(4);
            blockChain.Append(5);

            // Assert - Count
            Assert.AreEqual(3, blockChain.Chain.Count);

            // Assert - Content
            Assert.AreEqual(1, blockChain.Chain.First.Value.Items[0].Content);
            Assert.AreEqual(2, blockChain.Chain.First.Value.Items[1].Content);
            Assert.AreEqual(3, blockChain.Chain.First.Next.Value.Items[0].Content);
            Assert.AreEqual(4, blockChain.Chain.First.Next.Value.Items[1].Content);

            // Assert - Integrity
            Assert.IsTrue(blockChain.CheckIntegrityOfLastBlock());
            Assert.IsTrue(blockChain.CheckIntegrity());
        }

        [Test]
        public void Protected_BlockChain_Local_Int32_Multiple_Blocks_Check_Integrity_Test()
        {
            // Arrange
            BlockChain<Int32> blockChain = new BlockChain<Int32>(2);

            // Act - Append
            blockChain.Append(1);
            blockChain.Append(2);
            blockChain.Append(3);
            blockChain.Append(4);
            blockChain.Append(5);

            // Act - Modify
            blockChain.Chain.First.Value.Items[0] = new Transaction<Int32>(42);

            // Assert - Integrity
            Assert.IsTrue(blockChain.CheckIntegrityOfLastBlock());
            Assert.IsFalse(blockChain.CheckIntegrity());

            // Act - Modify
            blockChain.Chain.Last.Value.Items[0] = new Transaction<Int32>(42);

            // Assert - Integrity
            Assert.IsFalse(blockChain.CheckIntegrityOfLastBlock());
            Assert.IsFalse(blockChain.CheckIntegrity());

            // Assert - Append
            Assert.IsFalse(blockChain.Append(6));
        }

        [Test]
#if UNITY_2023_1_OR_NEWER
        public async Task Protected_BlockChain_File_Int32_Test()
#else
        public async void Protected_BlockChain_File_Int32_Test()
#endif
        {
            // Arrange - Create a temporary file
            string filePath = System.IO.Path.GetTempFileName();

            // Arrange - Create a file synchronizer
            FileSynchronizer<Int32> fileSynchronizer = new FileSynchronizer<Int32>(filePath);

            // Arrange - Create a block chain
            BlockChain<Int32> blockChain = new BlockChain<Int32>(10, fileSynchronizer);

            // Act - Append
            await blockChain.AppendAsync(1);
            await blockChain.AppendAsync(2);
            await blockChain.AppendAsync(3);
            await blockChain.AppendAsync(4);

            // Assert - Count
            Assert.AreEqual(4, blockChain.Chain.First.Value.Count);

            // Assert - Content
            Assert.AreEqual(1, blockChain.Chain.First.Value.Items[0].Content);
            Assert.AreEqual(2, blockChain.Chain.First.Value.Items[1].Content);
            Assert.AreEqual(3, blockChain.Chain.First.Value.Items[2].Content);
            Assert.AreEqual(4, blockChain.Chain.First.Value.Items[3].Content);

            // Arrange - Create a compare block chain
            BlockChain<Int32> compareBlockChain = new BlockChain<Int32>(10, fileSynchronizer);

            // Act - Append & Synchronize
            await compareBlockChain.AppendAsync(5);

            // Assert - Count
            Assert.AreEqual(5, compareBlockChain.Chain.First.Value.Count);

            // Assert - Content
            Assert.AreEqual(1, compareBlockChain.Chain.First.Value.Items[0].Content);
            Assert.AreEqual(2, compareBlockChain.Chain.First.Value.Items[1].Content);
            Assert.AreEqual(3, compareBlockChain.Chain.First.Value.Items[2].Content);
            Assert.AreEqual(4, compareBlockChain.Chain.First.Value.Items[3].Content);
            Assert.AreEqual(5, compareBlockChain.Chain.First.Value.Items[4].Content);

            // Assert - Integrity
            Assert.IsTrue(compareBlockChain.CheckIntegrityOfLastBlock());
            Assert.IsTrue(compareBlockChain.CheckIntegrity());
        }
    }
}