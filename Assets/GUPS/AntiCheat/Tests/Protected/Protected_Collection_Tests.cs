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
using GUPS.AntiCheat.Protected.Collection;

namespace GUPS.AntiCheat.Tests
{
    public class Protected_Collection_Tests
    {

#if UNITY_EDITOR

        [SetUp]
        public void Setup_Global_Settings()
        {
            GUPS.AntiCheat.Settings.GlobalSettings.LoadOrCreateAsset();
        }

#endif

        [Test]
        public void Protected_List_Int32_Test()
        {
            // Arange
            ProtectedList<Int32> var_List = new ProtectedList<Int32>();

            // Act
            var_List.Add(1);
            var_List.Add(2);
            var_List.Add(3);

            // Assert
            Assert.AreEqual(1, var_List[0]);
            Assert.AreEqual(2, var_List[1]);
            Assert.AreEqual(3, var_List[2]);
            Assert.IsTrue(var_List.CheckIntegrity());

            // Act
            var_List.RemoveAt(1);

            // Assert
            Assert.AreEqual(1, var_List[0]);
            Assert.AreEqual(3, var_List[1]);
            Assert.IsTrue(var_List.CheckIntegrity());

            // Act
            var_List[1] = 4;

            // Assert
            Assert.AreEqual(1, var_List[0]);
            Assert.AreEqual(4, var_List[1]);
            Assert.IsTrue(var_List.CheckIntegrity());

            // Act
            var_List.Remove(1);

            // Assert
            Assert.AreEqual(4, var_List[0]);
            Assert.IsTrue(var_List.CheckIntegrity());

            // Act
            var_List.Clear();

            // Assert
            Assert.AreEqual(0, var_List.Count);
            Assert.IsTrue(var_List.CheckIntegrity());

            // Act
            int var_HalfMax = Int32.MaxValue / 2;
            var_List.Add(var_HalfMax);
            var_List.Add(var_HalfMax * 2);
            var_List.Add(var_HalfMax * 3);

            // Assert
            Assert.AreEqual(var_HalfMax, var_List[0]);
            Assert.AreEqual(var_HalfMax * 2, var_List[1]);
            Assert.AreEqual(var_HalfMax * 3, var_List[2]);
            Assert.IsTrue(var_List.CheckIntegrity());
        }

        [Test]
        public void Protected_List_Int32_CheckIntegrity_Test()
        {
            // Arange
            ProtectedList<Int32> list = new ProtectedList<Int32>();

            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Assert
            Assert.IsTrue(list.CheckIntegrity());

            // Get the list field via reflection from the protected list.
            var field = list.GetType().GetField("list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Get the list field value.
            var value = (IList)field.GetValue(list);

            // Remove an item from the list.
            value.RemoveAt(1);

            // Assert
            Assert.IsFalse(list.CheckIntegrity());

            // Assert
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Protected_Queue_Int32_Test()
        {
            // Arange
            ProtectedQueue<Int32> queue = new ProtectedQueue<Int32>();

            // Act
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            // Assert
            Assert.IsTrue(queue.CheckIntegrity());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.IsTrue(queue.CheckIntegrity());

            // Act
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            // Assert
            Assert.IsTrue(queue.CheckIntegrity());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.IsTrue(queue.CheckIntegrity());
        }

        [Test]
        public void Protected_Queue_Int32_CheckIntegrity_Test()
        {
            // Arange
            ProtectedQueue<Int32> queue = new ProtectedQueue<Int32>();

            // Act
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            // Assert
            Assert.IsTrue(queue.CheckIntegrity());

            // Get the queue field via reflection from the protected queue.
            var field = queue.GetType().GetField("queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Get the queue field value.
            var value = (Queue<Int32>)field.GetValue(queue);

            // Remove an item from the queue.
            value.Dequeue();

            // Assert
            Assert.IsFalse(queue.CheckIntegrity());

            // Assert
            Assert.AreEqual(2, queue.Count);
        }

        [Test]
        public void Protected_Stack_Int32_Test()
        {
            // Arange
            ProtectedStack<Int32> stack = new ProtectedStack<Int32>();

            // Act
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            // Assert
            Assert.IsTrue(stack.CheckIntegrity());
            Assert.AreEqual(3, stack.Pop());
            Assert.AreEqual(2, stack.Pop());
            Assert.AreEqual(1, stack.Pop());
            Assert.IsTrue(stack.CheckIntegrity());

            // Act
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            // Assert
            Assert.IsTrue(stack.CheckIntegrity());
            Assert.AreEqual(3, stack.Pop());
            Assert.AreEqual(2, stack.Pop());
            Assert.AreEqual(1, stack.Pop());
            Assert.IsTrue(stack.CheckIntegrity());
        }

        [Test]
        public void Protected_Stack_Int32_CheckIntegrity_Test()
        {
            // Arange
            ProtectedStack<Int32> stack = new ProtectedStack<Int32>();

            // Act
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            // Assert
            Assert.IsTrue(stack.CheckIntegrity());

            // Get the stack field via reflection from the protected stack.
            var field = stack.GetType().GetField("stack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Get the stack field value.
            var value = (Stack<Int32>)field.GetValue(stack);

            // Remove an item from the stack.
            value.Pop();

            // Assert
            Assert.IsFalse(stack.CheckIntegrity());

            // Assert
            Assert.AreEqual(2, stack.Count);
        }
    }
}
