using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace AI.Blackboard.SelfBlackboard.Tests
{
	public class AgentBlackboardTests
	{
		private AIBlackboard blackboard;

		[SetUp]
		public void Setup()
		{
			//Create a new blackboard every time
			blackboard = new AIBlackboard();
			Assert.NotNull(blackboard);
		}

		//Test that adding a a value succedes
		[Test]
		public void AddValue()
		{
			const float val = 1f;
			Assert.True(blackboard.TryAddOrUpdateValue("testValue", val));
		}
		
		//Test that the added value is able to be readback 
		[Test]
		public void AddValueReadback()
		{
			const float val = 1f;
			const string valName = "testValue";

			Assert.True(blackboard.TryAddOrUpdateValue(valName, val));
			
			Assert.True(blackboard.TryGetValue(valName, out float outVal));
			Assert.AreEqual(val, outVal);
		}
		
		//Test that updating a value works
		[Test]
		public void UpdateValue()
		{
			const float originalVal = 1f;
			const float newVal = 2.22f;
			const string valName = "testValue";
			
			Assert.True(blackboard.TryAddOrUpdateValue(valName, originalVal));
			Assert.True(blackboard.TryGetValue(valName, out float outValOrig));
			Assert.AreEqual(originalVal, outValOrig);
			
			//Update Value
			Assert.True(blackboard.TryAddOrUpdateValue(valName, newVal));
			Assert.True(blackboard.TryGetValue(valName, out float outValNew));
			Assert.AreEqual(newVal, outValNew);
		}

		//Test for trying to readback a differnt type from the one in which the value was created
		[Test]
		public void ReadbackIncorrectType()
		{
			const float val = 1f;
			const string valName = "testValue";
			string outString = String.Empty;
			
			//Check that our out type is not of the value type
			Assert.AreNotEqual(outString.GetType(), val.GetType(), "Out and input test types are the same");
			
			//Add value
			Assert.True(blackboard.TryAddOrUpdateValue(valName, val));
			
			//This should return false because the out type is different from the
			//original value type
			Assert.False(blackboard.TryGetValue(valName, out outString));
		}
		
		//Test for trying to update a value to a different type from how it was created
		[Test]
		public void UpdateIncorrectType()
		{
			const float val = 1f;
			const string valName = "testValue";
			const string updateString = "wow, this should fail";
			
			Assert.AreNotEqual(val.GetType(), updateString.GetType(), "Out and input test types are the same");
			
			//Add value
			Assert.True(blackboard.TryAddOrUpdateValue(valName, val));
			
			//Try and update to a different type, this should fail
			Assert.False(blackboard.TryAddOrUpdateValue(valName, updateString));
		}

		//Remove Value from the blackboard
		[Test]
		public void RemoveValue()
		{
			const float val = 1f;
			const string valName = "testValue";
			
			//Add value
			Assert.True(blackboard.TryAddOrUpdateValue(valName, val));
			
			//Remove 
			Assert.True(blackboard.TryRemoveValue(valName));
			Assert.False(blackboard.ContainsValue(valName));
		}
	}
}

