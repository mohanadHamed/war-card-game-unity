# War Card Game - Unity Implementation

## 📘 Overview
A simplified version of the classic *War* card game, developed in Unity as part of a technical assignment. Players compete against a bot by drawing cards each round—highest card wins the round and scores a point.

## 🕹️ Game Rules

- The game continues until either the player or the bot scores 8 points.
- Each round consists of:
  1. 👤 The player draws a card.
  2. 🤖 After a short delay, the bot draws a card.
  3. 🆚 The higher card wins the round.
  4. 🏆 First to reach 8 points wins the game.
- 🟰 If both cards are equal, the round ends in a draw (no points awarded).

## ⚠️ Note on 8-Point Rule
The original constraint of a **maximum of 8 rounds** has been removed. This is due to the rarity of achieving 8 points within 8 rounds.  
🔁 The game now continues indefinitely until either side reaches 8 points.

## 🛠️ How to Run

1. 📥 Clone this repository.
2. 🧩 Open the project using Unity version `6000.0.48f1` or newer.
3. 🎬 Load the `MainMenu` scene.
4. ▶️ Press Play!

## 🎥 Demo Video

🔗 [Watch the gameplay demo](https://github.com/user-attachments/assets/029069ca-713a-4ec6-9405-def0402ef04c)

## 🌐 Deck of Cards API

Used to draw and manage playing cards:  
🔗 [https://deckofcardsapi.com/](https://deckofcardsapi.com/)

### 🔌 Integration Highlights:
- API responses are mapped to internal `Card` models.
- 🚫 Handles network failures gracefully.
- 🪪 On connection issues (e.g., during deck shuffle or card draw), a **notification panel** appears allowing the user to:
  - 🔄 Retry the operation.
  - ❌ Quit the game.

## ⚙️ Technical Highlights

- ⏳ Uses `async/await` (via UniTask) for API calls and delays.
- 💉 Dependency Injection in `GameManager` to allow test injection of mock deck responses.
- 🔄 Clean separation of game logic from UI.
- 🧩 `DeckService` handles all external API logic, `GameManager` controls game flow.
- 📦 Card back image is cached in memory for performance.
- 🃏 Card display includes DoTween animations with fallback handling.
- 💾 Game sound settings are persisted using a custom Save System.
- ✅ Unit tests cover:
  - DeckService
  - GameManager

## 🚀 Future Improvements

- ✨ Add advanced visual effects and animations for scoring and end-game feedback.
- 🌐 Support multiplayer via WebSocket simulation.
- 🧪 Expand test coverage across all gameplay logic (requires further refactoring and DI).


## 🧱 Project Structure

- Assets/
  - Prefabs/
    - Game/
      - CardDisplay
      - NetworkNotifyPanel
      - NotifyButton
    - MainMenu/
      - MainMenuButton
      - SettingsToggle
  - Resources/
    - Sounds/
      - bgm
      - card_flip
      - game_over_lose
      - lose
      - victory
      - win
  - Scenes/
    - MainMenu.unity
    - Game.unity
    - Result.unity
  - Scripts/
    - Common/
      - SceneLoader.cs
    - Game/
      - DataTypes/
          - Card
          - GameResult.cs
          - GameSettings.cs
          - RoundResult.cs
      - Interfaces/
          - IGameDeckService.cs
          - ISfxAudioManager.cs
      - Logic/
          - GameDeckService.cs
          - GameManager.cs
          - SfxAudioManager.cs
      - UI/
          - CardDisplay.cs
          - GameUi.cs
          - NetworkNotifyResponse.cs
    - GameDataSave/
      - GameSaveData.cs
      - SaveSystem.cs
    - MainMenu/
      - UI/
          - MainMenuController
          - SettingsController
      - BgmManager.cs
    - Network/
      - DeckService/
          - DataTypes/
              - CardData
              - CardImages
              - DeckShuffleResponse
              - DrawCardResponse
    - Plugins/
      - Demigiant/
          - DOTween/
    - Result/
      - ResultUi.cs
    - Tests/
      - DeckServiceTests.cs
      - GameManagerTests.cs

