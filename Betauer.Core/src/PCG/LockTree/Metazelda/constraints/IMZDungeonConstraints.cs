using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.LockTree.Metazelda.constraints;

/**
* Implementing classes may specify constraints to be placed on MZDungeon
* generation.
*
* @see generators.IMZDungeonGenerator
*/
public interface IMZDungeonConstraints {

 /**
    * @return  the maximum number of MZRooms an
    * {@link generators.IMZDungeonGenerator} may
    *          place in an {@link IMZDungeon}
    */
 int GetMaxRooms();
    
 /**
    * @return  the maximum number of keys an
    * {@link generators.IMZDungeonGenerator} may
    *          place in an {@link IMZDungeon}
    */
 int GetMaxKeys();

 /**
    * Gets the number of switches the
    * {@link generators.IMZDungeonGenerator} is allowed to
    * place in an {@link IMZDungeon}.
    * Note only one switch is ever placed due to limitations of the current
    * algorithm.
    *
    * @return  the maximum number of switches an
    * {@link generators.IMZDungeonGenerator} may
    *          place in an {@link IMZDungeon}
    */
 int GetMaxSwitches();
    
 /**
    * Gets the collection of ids from which an
    * {@link generators.IMZDungeonGenerator} is allowed to
    * pick the entrance room.
    *
    * @return the collection of ids
    */
 List<int> InitialRooms();
    
 /**
    * @return a weighted list of ids of rooms that are adjacent to the room
    * with the given id.
    */
 List<KeyValuePair<Double, int>> GetAdjacentRooms(int id, int keyLevel);
    
 /**
    * @return desired probability for an extra edge to be Added between the
    * given rooms during the Graphify phase.
    */
 double EdgeGraphifyProbability(int id, int nextId);
    
 /**
    * @return a set of Coords which the room with the given id occupies.
    */
 List<Vector2I> GetCoords(int id);
    
 /**
    * Runs post-generation checks to determine the suitability of the dungeon.
    *
    * @param dungeon   the {@link IMZDungeon} to check
    * @return  true to keep the dungeon, or false to discard the dungeon and
    *          attempt generation again
    */
 bool IsAcceptable(MZDungeon dungeon);
    
 bool RoomCanFitItem(int id, MZSymbol key);
    
}