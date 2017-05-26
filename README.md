# CapstoneProject

# Synopsis

These are 3 different verisons of an AI agent created for the purpose of playing Tetris. It makes use of NEAT, a nueral network that is constantly being evolved by a genetic algorithm. The Tetris game was built using Unity gae engine and C#. And the nneural network, genetic algorithm and other components relating to and including the agent were also built using C#.

# ANGELINA

This version of the agent(found in the branch Version-2) evalutates the game play by play rather than only evaluating the state of the game at the end. The process of doing this is that When a new piece is given to te agent to play it will evaluate each possible move it has, find the one that will give the best state of the boards, and then performs an A* search to determine the best route for the piece to reach that position.

# BRAD V1

This version of the agent(found in the branch Version-1.1) evalutates the gameboard after the game has ended, in this version each agent only has one chance to play the game and is evaluted based on that performance.

# BRAD V2

This version of the agent(found in the branch Version-1.2) evalutates the gameboard after the game has ended, in this version each agent is given 3 chances to play the game and is evaluted based on their average performances.

# Collaborators

Keniel Peart, Kimberly Soares, Rajay Bitter, Shanice Bryan

# Installation

For the installation and use of any version of the agent, Unity game engine must be use to open the project folder
