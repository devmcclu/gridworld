# Gridworld

Projects for MSCH-G 488 Artificial Intelligence for Games

Simulation of "sheep" and "grass".
The sheep can move towards other sheep, create new sheep, move towards and eat grass, and wonder around.
The grass "grows" and "dies" at a set rate, and will randomly create new grass.

Built using Unity 2019.4.19f1.

# How to use
Each scene is used for a specific type of AI
* MainScene - Naive AI
* DijkstraScene - Dijkstra's Algorithm
* AStarScene - A* Pathfinding
* UtilityAI - Utility AI

The GridGenerator Script handles the spawning of the grid and any AI agents. Values such as how many of an AI can be spawned and the size of the grid are changed here.
