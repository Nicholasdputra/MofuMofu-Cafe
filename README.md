# 🍵 About
Mofumofu cafe is a fast paced yet cozy, time management-diner dash game, where you play as a barista who has to pick up customer orders and make said orders, then deliver them to the customers' tables starting from 7 A.M. until 8 P.M. Sometimes the customers will know  what they want and give you direct orders and other times, they'll just give you a general description of what kind of drinks they want. Also, you'll sometimes be bothered by the cats that are in the cafe while you're working and carrying your drinks, so make sure that they don't snatch it away from you and make you spill it.

---

# 💻 My Contributions 

For this project I worked mainly on the drinks, menu, trash, audio, dialogue, and lent a hand on a few other scripts. Aside from that, I also refactored the other scripts' code, with a special focus on how scripts interact with each other, making everything cleaner and more efficient. Speaking of which, you can see the scripts down below! Additionally, for this project I also worked as a game designer, for which I also came up with the overall game idea, the QTE, customers, and dialogues. Overall, this project took me about 50-60 hours to do!

---

# 📜 Scripts Breakdown

<table width="100%">
  <thead>
    <tr>
      <th width="33%">
        <h4>
          <a>Script Name</a>
        </h4>
      </th>
      <th width="67%">
        <h4>
          <a>Script Description</a>
        </h4>
      </th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        🎵 AudioManager.cs
      </td>
      <td>
        Responsible for everything sound related in the game
      </td>
    </tr>
    <tr>
      <td>
        👨‍🦱 CafeNPC.cs
      </td>
      <td>
        Handles NPC behavior and states as well as pathfinding
      </td>
    </tr>
    <tr>
      <td>
        💸 CashierManager.cs
      </td>
      <td>
        Handles queueing and ordering for customers when they enter and go to the cashier to order
      </td>
    </tr>
    <tr>
      <td>
        🐱 CatNPC.cs
      </td>
      <td>
        Handles cat states, basic movement, behaviour, and Cat Pathfinding (with the help of the Cat Pathfinder Class) 
      </td>
    </tr>
    <tr>
      <td>
        🛣️ CatPathfinder.cs
      </td>
      <td>
        A helper script to help with pathfinding 
      </td>
    </tr>
    <tr>
      <td>
        🗯️ CharacterDialogueSO.cs
      </td>
      <td>
        A scriptable object for holding character dialogue
      </td>
    </tr>
    <tr>
      <td>
        💬 DialogueManager.cs
      </td>
      <td>
        Handles what dialogue to use for NPCs
      </td>
    </tr>
    <tr>
      <td>
        🧋 DrinkSO.cs
      </td>
      <td>
        A scriptable object for holding basic drink data
      </td>
    </tr>
    <tr>
      <td>
        🔧 MachineManager.cs
      </td>
      <td>
        Handles the machines (coffee machine, chocolate dispenser, matcha chawan, fridge)
      </td>
    </tr>
    <tr>
      <td>
        📖 MenuPopup.cs
      </td>
      <td>
        Contains how to play elements that pop up in a menu-like book
      </td>
    </tr>
    <tr>
      <td>
        🖼️ NPCAnimationController.cs
      </td>
      <td>
        Handles NPC sprite direction
      </td>
    </tr>
    <tr>
      <td>
        💁 NPCManager.cs
      </td>
      <td>
        Handles NPC spawning and setup
      </td>
    </tr>
    <tr>
      <td>
        🔵 PathNode.cs
      </td>
      <td>
       Used in path nodes that make up a graph that functions as a path, both for NPCs and cats
      </td>
    </tr>
    <tr>
      <td>
        ✋ PlayerInteraction.cs
      </td>
      <td>
        Handles player interaction, be it with NPCs or Machines
      </td>
    </tr>
    <tr>
      <td>
        ⛹️‍♂️ PlayerMovement.cs
      </td>
      <td>
        Handles player movement
      </td>
    </tr>
    <tr>
      <td>
        🚪 PlayerTriggerCheck.cs
      </td>
      <td>
        Basic script that determines player's target NPC and if they're colliding with anything
      </td>
    </tr>
    <tr>
      <td>
        ❗QTEScript.cs
      </td>
      <td>
        Handles QTE that's triggered when the cats are close and in a certain state
      </td>
    </tr>
    <tr>
      <td>
        📽️ SceneHandler.cs
      </td>
      <td>
        Handles scene transitions
      </td>
    </tr>
    <tr>
      <td>
        💺 SeatHandler.cs
      </td>
      <td>
        Handles empty and occupied seats in the cafe as a whole for assigning where to seat NPCs
      </td>
    </tr>
    <tr>
      <td>
        🪑 SeatScript.cs
      </td>
      <td>
        Marks a seat's occupancy when there are customers and helps with pathing NPCs to said seat
      </td>
    </tr>
    <tr>
      <td>
        ⚙️ Settings.cs
      </td>
      <td>
        Handles resolutions and volume settings for the game
      </td>
    </tr>
    <tr>
      <td>
        ⏰ TimerScript.cs
      </td>
      <td>
        Handles the in game time and how time may influence other scripts 
      </td>
    </tr>
    <tr>
      <td>
        📥 TrashcanScript.cs
      </td>
      <td>
        Discards the item the player is carrying
      </td>
    </tr>
  </tbody>
</table>

---

# 🕹️ Controls
<table width="100%">
  <thead>
    <tr>
      <th width="33%">
        <h4>
          <a>Button</a>
        </h4>
      </th>
      <th width="67%">
        <h4>
          <a>Action</a>
        </h4>
      </th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        WASD
      </td>
      <td>
        Character Movement
      </td>
    </tr>
    <tr>
      <td>
        E
      </td>
      <td>
        Interact with machines, talk with customers at the register, and hand them drinks while facing them
      </td>
    </tr>
    <tr>
      <td>
        Q
      </td>
      <td>
        Get rid of the currently held item
      </td>
    </tr>
    <tr>
      <td>
        M
      </td>
      <td>
        Open the menu/guide book
      </td>
    </tr>
    <tr>
      <td>
        Space Bar
      </td>
      <td>
        Input consistently to complete the quick time event and not get your drink snatched by cats
      </td>
    </tr>
  </tbody>
</table>

---

# Project Members
Angeline Maria Suryadi - Game Artist
Eric - Game Programmer
Nicholas Dwi Putra - Game Designer and Programmer
Rafael Wirasana Wijaya - Game Designer and Artist
Steven Wijaya - Game Programmer
