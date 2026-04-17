# CapstoneProject — NEAT Tetris Agents (Unity / C#)

This project contains multiple AI agents built to play **Tetris** in **Unity (C#)**.
The agents use **NEAT (NeuroEvolution of Augmenting Topologies)**: a neural network evolved via a genetic algorithm.

## What’s in this repo

There are **3 versions** of the agent (implemented on separate branches):

### ANGELINA (branch: `Version-2`)
- Evaluates gameplay **move-by-move** (not only at the end of the game)
- For each new piece, evaluates candidate placements to choose a target board state
- Uses **A\*** search to route the piece to the chosen placement

### BRAD V1 (branch: `Version-1.1`)
- Evaluates the gameboard **after the game ends**
- Each agent plays **one** full game and is scored by that performance

### BRAD V2 (branch: `Version-1.2`)
- Evaluates the gameboard **after the game ends**
- Each agent plays **three** games and is scored by the **average** performance

> The `master` branch contains the project baseline; the main agent implementations live in the branches above.

## Tech stack

- Unity (Tetris environment)
- C# (NEAT implementation + genetic algorithm + agent logic)

## How to run

1. Install **Unity** (use any version that can open the project in the selected branch).
2. Clone the repo:

```bash
git clone https://github.com/keniel123/CapstoneProject.git
cd CapstoneProject
```

3. Check out the version you want:

```bash
# Angelina
git checkout Version-2

# Brad V1
git checkout Version-1.1

# Brad V2
git checkout Version-1.2
```

4. Open the project folder in **Unity**.
5. Press **Play** to run the environment (exact scene / entrypoint depends on the selected branch).

## Collaborators

- Keniel Peart
- Kimberly Soares
- Rajay Bitter
- Shanice Bryan

## License

See [LICENSE](LICENSE).
