# Movie Mate

A mobile trivia game built in Unity where players guess mystery movies from progressive clues — powered by the [TMDB API](https://www.themoviedb.org/documentation/api).

Browse thousands of films by genre or search by name, dive into detailed cast and crew info, and test your movie knowledge in a Wordle-inspired guessing game.

## Features

- **Mystery Movie Game** — A target film is randomly selected from TMDB's popular listings (validated for rich metadata). Players receive progressive clues: blurred/cropped backdrops, release year and genres, director, top actors, tagline, and finally the poster.
- **Wordle-Style Feedback** — Each wrong guess returns color-coded feedback comparing director, top-3 actors, shared genres, and release year (with directional arrows), giving players meaningful hints.
- **Browse by Genre** — Explore movies filtered by genre with paginated, infinite-scroll lists.
- **Search by Name** — Debounced search queries against TMDB with pagination and instant results.
- **Movie Details** — Full detail view with poster, synopsis, cast, crew, and genre tags.
- **Localization** — English and Russian language support, dynamically switching TMDB query language.
- **Configurable Image Quality** — Low, medium, and high poster resolution options to balance quality vs. bandwidth.

## Architecture

Movie Mate is built on a modular, event-driven architecture using a hybrid **Mediator + MVP** pattern with a dedicated service layer.

### Navigation — Mediator

A centralized `UINavigationMediator` manages global application state. It maintains a registry of all panels, handles screen transitions with an input blocker during async operations, and decouples screens from each other entirely.

### Screens — Panels

Each screen (Main Menu, Game, Movie Details, etc.) is a self-contained feature module governed by a **Panel** (e.g., `GamePanel`, `GenrePanel`). Panels inherit from `ABaseUIMediatorComponent`, which serves as the composition root for the screen — instantiating dependencies, wiring views to controllers, and orchestrating animated entry/exit transitions via DOTween + UniTask.

### Presentation — Views & Controllers

Panels enforce strict separation of concerns by wiring **passive Views** to logic-heavy **Controllers**:

- **Views** are MonoBehaviours that expose UI events (button clicks, scroll, input) and display methods. They hold zero business logic.
- **Controllers** are plain C# classes that subscribe to view events, process user input and game rules, and call back into the view to update the UI. Controllers never touch `UnityEngine.UI` directly.

### Data — Service Layer

All external data operations are delegated to an interface-based **Service Layer**:

| Service | Responsibility |
|---|---|
| `ApiService` | TMDB REST client (popular, discover, search, details, images) with in-memory sprite caching |
| `MoviePickingService` | Random target selection with metadata validation (tagline, director, cast ≥ 3, poster, backdrop) |
| `ClueFactory` | Builds progressive clues from movie data — runtime blur, random crop, text composition |
| `FeedbackService` | Compares guess vs. target across multiple axes and produces color-coded feedback |
| `LocalizationManager` | Language switching with `PlayerPrefs` persistence |

Every service is accessed through an interface (`IApiService`, `IMoviePickService`, etc.), enabling full substitution with mocks in unit tests.

### UI Virtualization & Object Pooling

Movie lists use a custom **virtualized scroll** implementation. Instead of instantiating a GameObject for every item in a dataset (which would destroy performance on large lists), only the items visible on screen (plus a small buffer) are kept alive. As the user scrolls, off-screen items are deactivated and returned to an **object pool**, then recycled and re-bound with new data as new items enter the viewport. This keeps the active object count constant regardless of list size.

## Tech Stack

- **Unity 6** (6000.x) with **Universal Render Pipeline**
- **C#** with async/await via **UniTask**
- **DOTween** for UI animations (fade, slide, keyboard-aware layout shifts)
- **Newtonsoft.Json** for API deserialization
- **TextMeshPro** for text rendering (with Latin + Cyrillic font safety filtering)
- **NUnit + NSubstitute** for unit testing

## Testing

Controller-level unit tests cover the core game loop, search logic, genre loading, movie details, and settings — using **NSubstitute** to mock all service and view interfaces.

```
Assets/Tests/
├── GameControllerTests.cs
├── SearchGameControllerTests.cs
├── MoviesControllerTests.cs
├── GenreControllerTest.cs
├── SearchByNameControllerTests.cs
├── MovieDetailsControllerTests.cs
└── SettingsControllerTests.cs
```

## Project Structure

```
Assets/
├── Scripts/
│   ├── Mediator/              # UINavigationMediator — global screen management
│   ├── ApiService/            # TMDB HTTP client + sprite cache
│   ├── Models/                # DTOs and view models
│   ├── Game/                  # Trivia game (controller, view, services, clues, feedback)
│   ├── Genre/                 # Genre browsing + icon config
│   ├── Movies/                # Genre-filtered movie lists with virtualized scroll
│   ├── SearchSection/         # Search hub + search-by-name flow
│   ├── MovieDetailsPanel/     # Movie detail screen
│   ├── MainMenu/              # Main menu
│   ├── Settings/              # Language & resolution settings
│   ├── OptionsSelector/       # Settings UI widgets
│   ├── Prefab Item Scripts/   # Reusable list items, clue tiles, popups
│   └── Enums/                 # Panel IDs, clue modes, feedback colors
├── Tests/                     # NUnit + NSubstitute unit tests
└── TextFonts/                 # Roboto TMP font assets
```

## Setup

1. Clone the repository.
2. Open the project in **Unity 6** (version `6000.2.6f2` or compatible).
3. Obtain a [TMDB API key](https://www.themoviedb.org/settings/api) and configure it in `ApiService.cs`.
4. Open the main scene and press Play.
