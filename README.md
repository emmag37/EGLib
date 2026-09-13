# EGLib

Reusable Core, Mechanics, and UI utilities for Unity projects.

## Contents

- **Core** - Assembly independent foundation: object pooling, service location, an event bus,
generic data structures, and file/PlayerPrefs backed storage. No dependencies on any other EGLib
assembly.
- **Mechanics** - Input driven gameplay behaviors (dragging) and board/grid coordinate math.
Depends on the Input System.
- **UI** - Reusable UI widgets and a generic view navigation system, built on TextMeshPro and Core.


### Core

- **Data**
    - `KeyedDatabase<TKey, TValue>` - Base class for a ScriptableObject backed database that maps
keys to values, serialized as an array of key value pairs in the inspector.
- **DataStructures**
    - `CappedRankedList<T>` - A fixed capacity list that keeps items sorted in descending order;
when full, adding a value that outranks the current lowest entry removes the entry to make space.
    - **IncrementList**
        - `IIncrementList<T>` - Represents a collection whose current position can be stepped
forward or backward, returning the item at the new position each time.
        - `RingList<T>` - A list that wraps around at its ends (`Next` past the last returns the
first, `Prev1 before the first returns the last), maintaining a current index that moves with each
call.
- **ObjectPool**
    - `GameObjectPool<TObj, TData>` - Fixed size object pool for pooled components, using an
intrusive doubly-linked free list (via `ILinkable`) to track available instances without additional
allocations.
    - `ILinkable<TSelf, TData>` - Contract for a component usable in an intrusive doubly-linked
list; implementers provide the link fields themselves.
- **Services**
    - `IAudio` - Provides playback control for music and sound effects, identified by audio ID.
    - `IFileStorage` - Generic file based storage contract for saving and loading serializable data
by file name, independent of the underlying storage format.
    - `IVibration` - Provides control over haptic feedback (vibration).
    - `ServiceLocator` - Static registry mapping service interface types to a single implementation
instance each, giving simple global access to shared services without hard dependencies between
classes.
- **Storage**
    - `JsonFileStorage` - JSON backed `IFileStorage` implementation using `JsonUtility`;
distinguishes a missing file (silent fresh start) from a corrupt file (backed up with a timestamped
rename, logged, and replaced with a fresh instance).
    - `PlayerPrefsStorage` - Thin wrapper around `PlayerPrefs` with null checked key/value access,
a proper bool type, and an immediate save after every write.
- **Utilities**
    - `CoroutineRunner` - A static singleton MonoBehaviour providing a persistent GameObject for
running coroutines from non-MonoBehaviour classes or static contexts.
    - `Scaler` - Static utility that computes an applies a scale factor derived from the active
orthographic camera's size relative to a reference resolution, keeping UI and game object scaling
consistent.
    - `Logger` - Thin wrapper around Unity's built-in logger for classes without UnityEngine to use.
- `EventBus` - Static type keyed publish/subscribe event bus; listeners subscribe with a strongly
typed callback for an event type, and publishing an instance invokes all subscribed callbacks for
that type.

### Mechanics

- `BoardGeometry` - Performs the index and world coordinate calculations of a 2D grid on a board.
- `Draggable` - Performs user input dragging for a game object; releases when and where the touch
ends.

### UI

- **Elements**
    - **Button**
        - `UIButton` - Wraps a `Button` to play a sound effect and invoke a callback on click.
        - `UIButtonFactory` - Provides factory methods for building pre-configured `UIButton`
instances for common interaction patterns (navigation, popups, input activation, patch updates,
incrementing/decrementing a list).
    - **ScrollList**
        - `IScrollItem<T>` - Defines the contract for an item view used within a
`UIVerticalScrollList<T>`, letting the list instantiate, activate, and bind data to items without
knowing their concrete type.
        - `UIVerticalScrollList<T>` - A vertical scroll list that activates, binds, and manages a
fixed size pool of pre-instantiated item views implementing `IScrollItem<T>`.
    - **UIUsername**
        - `IProfanityFilter` - Defines a contract for filtering and detecting offensive language or
profanity within text.
        - `UsernameValidator` - Static class to validate a username against length, special characters,
and profanity.
        - 'UIUsernameInputField' - Wraps a `TMP_InputField` with username validation, an inline
error message display, and a shake animation on invalid submission; validated usernames are reported
with a supplied callback.
- **ViewSystem**
    - `ViewController<TView, TType, TData>` - Manages a fixed capacity stack of `TView` instances,
looked up by a `TType` identifier, providing push/pop navigation and in-place data updates; pushing
at capacity replaces (hides) the current top view rather than growing the stack.
    - `UIView<TType>` - Generic base class for a UI view identified by a `TType` enum value, managed
by an `IUIViewHost`; handles show/hide through GameObject activation and delegates data binding to
derived classes through `SetInfo`.
    - `IUIViewHost` - Defines the operations a UI view can access for navigating within its host
and requesting targeted data updates, without exposing the host's full internal implementation.
    - `IUIData` / `IUIPatch` - Marker interfaces holding data types for UIViews, and for patch data
passed from a UIView back to its host for updates, respectively.
- `SafeArea` (`SafeAreaPanel`) - Adjusts a RectTransform's anchors to fit within the device's safe
area (excluding notches, cutouts, rounded corners), applying extra top/bottom insets on notched
devices to keep content clear of the cutout.

## Extending

EGLib ships some interfaces as contracts only - no concrete implementation is included, so the
library won't function until your project provides one. Others are consumer side contracts your own
types implement to plug into a generic EGLib system.

- `IAudio` (Core/Services) - Implement this with your project's audio backend (e.g. wrapping
`AudioSource`/`AudioMixer` or a third party audio middleware) and register it with `ServiceLocator`.
- `IVibration` (Core/Services) - Implement this with your platform's haptics API and register it
with `ServiceLocator`.
- `IProfanityFilter` (UI/Elements/UIUsername) - Implement this with a word list, third party filter,
or moderation API, then pass it to `UsernameValidator.Initialize`.
- `ILinkable<TSelf, TData>` (Core/ObjectPool) - Implement this on any component type you want to
pool with `GameObjectPool<TObj, TData>`, providing the `Next`/`Prev` link fields the pool's free
list relies on.
- `IScrollItem<T>` (UI/Elements/ScrollList) - Implement this on your item view prefab's component so
`UIVerticalScrollList<T>` can instantiate, activate, and bind data to it.
- `IUIViewHost` (UI/ViewSystem) - Implement this on your view controller/manager so views can
navigate and request patch updates through it (`ViewController<TView, TType, TData>` is a ready
made implementation you can use, or wrap with another class).
- `IUIData` / `IUIPatch` (UI/ViewSystem) - Marker interfaces with no members; implement these on
your own data and patch payload types to they can flow through `UIView<TType>` and `IUIViewHost`.

`IFileStorage` is not listed here since `JsonFileStorage` already provides a working implementation
 only implement it yourself if you want a different storage backend.

## Requirements

- Unity 6000.3+
- TextMeshPro 3.0.6
- Input System 1.7.0

## Installation

Add to your project's `Packages/manifest.json`:

\```json
"com.eg.eglib": "file:../path/to/EGLib"
\```

Or, access through the git repository:

\```json
"com.eg.eglib": "https://github.com/[emmag37]/eglib.git"
\```


## License

[license]
