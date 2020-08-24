using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace cc.creativecomputing.util
{
	public class CCCorrelatedEntry
	{
		public int id;
		public bool IsCorrelated = false;
		public Vector2 Position;
		public Vector2 PositionDelta;


		public CCCorrelatedEntry(int theId, bool theIsCorrelated, Vector2 thePosition)
		{
			id = theId;
			IsCorrelated = theIsCorrelated;
			Position = thePosition;
		}
	}


	public class CCCorrelateEntries<T> where T : CCCorrelatedEntry
	{

		/*
		private readonly Dictionary<int, T> _myEntryMap = new Dictionary<int, T>();

		//private readonly Queue<CCCorrelatedEntry> _myAddedEntries = new Queue<CCCorrelatedEntry>();
		//private readonly Queue<CCCorrelatedEntry> _myMovedEntries = new Queue<CCCorrelatedEntry>();
		//private readonly Queue<CCCorrelatedEntry> _myRemovedEntries = new Queue<CCCorrelatedEntry>();

		private int _myIdCounter = 0;
		
		public CCEventHandler<T> OnNewEntry = new CCEventHandler<T>();
		public CCEventHandler<T> OnMoveEntry = new CCEventHandler<T>();
		public CCEventHandler<T> OnLostEntry = new CCEventHandler<T>();

		private Dictionary<float, List<(int lastID, int currentID)>> CreateDistanceMap(IReadOnlyList<T> theEntries, float theDistanceThreshold)
		{
			var myDistanceMap = new Dictionary<float, List<(int currentID, int lastID)>>();

			foreach (var entry in _myEntryMap)
			{
				//computeIntensity(myCursorIt, myRawRaster);
				var correlatedEntry = entry.Value;
				correlatedEntry.IsCorrelated = false;
				for (var i = 0; i < theEntries.Count; i++)
				{
					var myDistance = Vector2.Distance(correlatedEntry.Position, (theEntries[i].Position));
					if (myDistance > theDistanceThreshold) continue;

					if (!myDistanceMap.ContainsKey(myDistance))
					{
						myDistanceMap[myDistance] = new List<(int currentID, int lastID)>();
					}

					myDistanceMap[myDistance].Add((i, entry.Key));
				}
			}

			return myDistanceMap;
		}

		public void Correlate(List<T> theNewEntries, float theMaxDistance)
		{

			// populate a map with all distances between existing cursors and new positions

			const float myDistanceThreshold = 1000;
			var myDistanceMap = CreateDistanceMap(theNewEntries, myDistanceThreshold);

			// will contain the correlated cursor id at index n for position n or -1 if uncorrelated
			var myCorrelatedEntryIds = new List<int>();
			for (var i = 0; i < theNewEntries.Count; i++)
			{
				myCorrelatedEntryIds.Add(-1);
			}

			// iterate through the distance map and correlate cursors in increasing distance order
			foreach (var myEntry in myDistanceMap.Values)
			{
				foreach (var ( currentId,lastId) in myEntry)
				{
					// check if we already have correlated one of our nodes
					Debug.Log(myCorrelatedEntryIds.Count+ " " + currentId);
					if (myCorrelatedEntryIds[currentId] != -1) continue;

					//var myCursorId = myIndices.id1;        
					var myLastPosition = _myEntryMap[lastId];

					if (myLastPosition.IsCorrelated) continue;

					// correlate
					myCorrelatedEntryIds[currentId] = lastId;
					myLastPosition.IsCorrelated = true;
					myLastPosition.Position = theNewEntries[currentId].Position;

					// post a move event
					OnMoveEntry.Invoke(myLastPosition);
				}
			}
			
			// Now let us iterate through all new positions and create 
			//"cursor add" events for every uncorrelated position

			for (var i = 0; i < theNewEntries.Count; ++i)
			{
				if (myCorrelatedEntryIds[i] != -1) continue;

				// new cursor
				var myNewId = _myIdCounter++;

				myCorrelatedEntryIds[i] = myNewId;

				theNewEntries[i].id = myNewId;
				theNewEntries[i].IsCorrelated = true;
				
				_myEntryMap[myNewId] = theNewEntries[i];
				OnNewEntry.Invoke(theNewEntries[i]);
			}


			var myIdsToRemove = new List<int>();

			// Now let us iterate through all cursors and create 
			//"cursor remove" events for every uncorrelated cursors
			foreach (var myEntry in _myEntryMap.Where(myEntry => !myEntry.Value.IsCorrelated))
			{
				// cursor removed
				OnLostEntry.Invoke(myEntry.Value);
				myIdsToRemove.Add(myEntry.Key);
			}

			foreach (var myId in myIdsToRemove)
			{
				_myEntryMap.Remove(myId);
			}
		}

		public List<T> Entries()
		{
			return new List<T>(_myEntryMap.Values);
		}*/
		private int _myIdCounter;
		public readonly CCEventHandler<T> OnNewEntry = new CCEventHandler<T>();
		public readonly CCEventHandler<T> OnMoveEntry = new CCEventHandler<T>();
		public readonly CCEventHandler<T> OnLostEntry = new CCEventHandler<T>();
		private readonly Dictionary<int, T> _myEntryMap = new Dictionary<int, T>();
		
		private Dictionary<float, List<(int x, int y)>> CreateDistanceMap(IReadOnlyList<T> theNewEntries, float theDistanceThreshold){
			var myDistanceMap = new Dictionary<float, List<(int x, int y)>>();
		
			foreach (var myCursorId in _myEntryMap.Keys) {
				//computeIntensity(myCursorIt, myRawRaster);
				var myÊntry = _myEntryMap[myCursorId];
				myÊntry.IsCorrelated = false;
				for (var i = 0; i < theNewEntries.Count();i++) {
					var myDistance = Vector2.Distance(myÊntry.Position,theNewEntries[i].Position);
					if (myDistance > theDistanceThreshold) continue;
				
					if(!myDistanceMap.ContainsKey(myDistance)) {
						myDistanceMap[myDistance] = new List<(int x, int y)>();
					}
					myDistanceMap[myDistance].Add((i, myCursorId));
				}
			}
		
			return myDistanceMap;
		}
		
		public void Update(List<T> theNewEntries , float myDistanceThreshold){

			// populate a map with all distances between existing cursors and new positions
			var myDistanceMap = CreateDistanceMap(theNewEntries, myDistanceThreshold);        
			
			// will contain the correlated cursor id at index n for position n or -1 if uncorrelated
			var myCorrelatedPositions = new List<int>();
			for(var i = 0; i < theNewEntries.Count();i++) {
				myCorrelatedPositions.Add(-1);
			}

			// iterate through the distance map and correlate cursors in increasing distance order
			foreach (var myEntry in myDistanceMap.Values){
				foreach(var (currentId, lastId) in myEntry) {
					// check if we already have correlated one of our nodes

					if (myCorrelatedPositions[currentId] != -1)  continue;

					var myLastEntry = _myEntryMap[lastId];
					var myNewEntry = theNewEntries[currentId];
		    	    
					if (myLastEntry.IsCorrelated) continue;
					
					// correlate
					myCorrelatedPositions[currentId] = lastId;
					
					myLastEntry.IsCorrelated = true;
					myLastEntry.PositionDelta = myNewEntry.Position - myLastEntry.Position;
					myLastEntry.Position = myNewEntry.Position;
					// post a move event
					
					OnMoveEntry.Invoke(myLastEntry);
				}
			}
		        

			// Now let us iterate through all new positions and create 
			//"cursor add" events for every uncorrelated position

			for (var i = 0; i < theNewEntries.Count(); ++i) {
				if (myCorrelatedPositions[i] != -1) continue;
				// new cursor
				var myNewId = _myIdCounter++;
					
				myCorrelatedPositions[i] =  myNewId;

				var myEntry = theNewEntries[i];
							
				myEntry.id = myNewId;
				myEntry.IsCorrelated = true;

				_myEntryMap[myNewId] = myEntry;
				OnNewEntry.Invoke(myEntry);
			}
		        
		
			var myIdsToRemove = new List<int>();
		        
			// Now let us iterate through all cursors and create 
			//"cursor remove" events for every uncorrelated cursors
		
			foreach (var myEntry in _myEntryMap.Where(myEntry => !myEntry.Value.IsCorrelated))
			{
				// cursor removed
				OnLostEntry.Invoke(myEntry.Value);
				myIdsToRemove.Add(myEntry.Key);
			}
		
			foreach(var myId in myIdsToRemove) {
				_myEntryMap.Remove(myId);
			}
		}

		public IEnumerable<T> Entries()
		{
			return _myEntryMap.Values;
		}
	}
}