# EchoShift

A 2D dungeon crawler game built with Unity featuring procedural dungeon generation, combat mechanics, and multiple levels.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
- [Controls](#controls)
- [Game Systems](#game-systems)
- [Development](#development)
- [Building for Platforms](#building-for-platforms)
- [API Integration](#api-integration)
- [Contributing](#contributing)

## Overview

This is a 2D action dungeon crawler where players navigate through procedurally generated dungeons, fight enemies, collect loot, and progress through multiple levels. The game features a complete UI system with main menu, pause functionality, and game over screens.

## Features

### Core Gameplay
- **Procedural Dungeon Generation**: Dynamic level creation using corridor-first and room-first algorithms
- **Combat System**: Real-time combat with weapon mechanics and knockback effects
- **Enemy AI**: Intelligent enemies with detection, pathfinding, and combat behaviors
- **Loot System**: Collectible items and destructible props
- **Health System**: Player and enemy health management
- **Portal System**: Level progression mechanics

### Technical Features
- **Mobile Support**: Touch controls and joystick input for mobile devices
- **Audio Management**: Sound effects and background music system
- **Save System**: Player progress tracking and data persistence
- **API Integration**: Backend connectivity for player data and authentication
- **Multiple Build Targets**: Support for PC & Android

### UI Systems
- Main menu with login/register functionality
- In-game HUD with health and status displays
- Pause menu system
- Game over screen with restart options
- Mobile-friendly controls

## Getting Started

### Prerequisites
- Unity 2022.3.49f1 or later
- Visual Studio or VS Code for development
- Android SDK (for Android builds)

### Installation
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd EchoShift
   ```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click "Open" and select the `Game` folder
   - Wait for Unity to import all assets

3. Open the Main Menu scene:
   - Navigate to `Assets/Scenes/Main Menu.unity`
   - Double-click to load the scene

### Running the Game
1. In Unity, press the Play button to start in the editor
2. Use the main menu to navigate through game options
3. Select a level to begin playing

## Controls

### PC Controls
- **Movement**: WASD keys or Arrow keys
- **Attack**: Left mouse button
- **Menu Navigation**: Mouse click

### Mobile Controls
- **Movement**: Virtual joystick (left side of screen)
- **Attack**: Attack button (right side of screen)
- **Menu Navigation**: Touch input

## Game Systems

### Dungeon Generation
The game uses multiple algorithms for creating diverse dungeon layouts:
- **Random Walk**: Creates organic, cave-like structures
- **Corridor First**: Generates corridors then places rooms
- **Rooms First**: Places rooms then connects with corridors

### Enemy AI
Enemies feature sophisticated AI behaviors:
- **Target Detection**: Line of sight and proximity detection
- **Pathfinding**: Navigation around obstacles
- **Combat Behaviors**: Attack patterns and retreat mechanics
- **Steering Behaviors**: Smooth movement and collision avoidance

### Player Systems
- **Movement**: Smooth character controller with physics-based movement
- **Combat**: Weapon handling with damage calculation
- **Health**: Damage system with visual feedback
- **Animation**: Character animations for movement and combat

## Game Flow
![flowchart](https://hackmd.io/_uploads/S17te3v7gl.png)


## Development

### Project Structure
```
Game/
├── Assets/
│   ├── _Scripts/           # All game scripts
│   │   ├── Player/         # Player-related scripts
│   │   ├── Enemy/          # Enemy AI and behaviors
│   │   ├── DungeonGenerator/ # Procedural generation
│   │   ├── UI/             # User interface
│   │   ├── Game/           # Game management
│   │   ├── API/            # Backend integration
│   │   └── Components/     # Reusable components
│   ├── Scenes/             # Game scenes
│   ├── Prefabs/            # Reusable game objects
│   ├── Sprites/            # 2D artwork
│   └── Audio/              # Sound effects and music
├── Library/                # Unity cache files
├── ProjectSettings/        # Unity project settings
└── Packages/               # Package dependencies
```

### Key Scripts
- [`Movement.cs`](Assets/_Scripts/Player/Movement.cs) - Player movement controller
- [`Player.cs`](Assets/_Scripts/Player/Player.cs) - Main player component
- [`Enemy.cs`](Assets/_Scripts/Enemy/Enemy.cs) - Enemy base class
- [`LevelManager.cs`](Assets/_Scripts/Game/LevelManager.cs) - Level progression
- [`AudioManager.cs`](Assets/_Scripts/Game/AudioManager.cs) - Audio system

### Build Locations
- Exports are stored in the [`Exports/`](Exports/) folder
- Separate folders for Desktop and Mobile builds

## API Integration

The game includes backend connectivity for:
- **User Authentication**: Login and registration system
- **Player Data**: Progress tracking and statistics
- **Player Run Records**: Complete history of all player runs with scoring and ranking system

### Backend
- Java Spring Boot backend located in [`Backend/`](Backend/)
- RESTful API endpoints for player data
- PostgreSQL database integration for persistent storage
- Session-based authentication system

### Database Schema
The game uses a PostgreSQL database with the following tables:
- **players**: User accounts with username, password, and experience points
- **player_sessions**: Active user sessions with authentication tokens
- **runs**: Individual game run records with score, time, and level data

### Entity Relationship Diagram
![ERD](https://hackmd.io/_uploads/S1ANVhvXee.png)


### API Endpoints

#### Authentication
- `POST /api/players/register` - Register new player account
- `POST /api/players/login` - Player login with credentials
- `POST /api/players/logout` - End player session
- `GET /api/players/session/validate` - Validate current session token

#### Game Runs Management
- `POST /api/runs` - Start new game run
- `PUT /api/runs/{id}` - Update run progress during gameplay
- `PATCH /api/runs/{id}/end` - End game run and finalize score
- `GET /api/runs/{playerId}` - Get all runs for specific player

### Screenshots
![image](https://hackmd.io/_uploads/H1LG5udmgx.png)
![image](https://hackmd.io/_uploads/H1Pyc_dmlx.png)
![image](https://hackmd.io/_uploads/S1KP9udmll.png)
![image](https://hackmd.io/_uploads/SJij9__mgg.png)
![image](https://hackmd.io/_uploads/BkQZodOXgg.png)


## Acknowledgments

- Unity Technologies for the game engine
- Asset creators for sprites and audio
