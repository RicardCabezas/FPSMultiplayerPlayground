# Unity DOTS FPS Sample – Custom Additions

This project is based on the official **[Competitive Action Multiplayer template
](https://docs.unity3d.com/Packages/com.unity.template.multiplayer-netcode-for-entities@1.0/manual/index.html)**, using **Unity DOTS / Entities** for learning and experimentation purposes.

---

## Goals

- Learn and explore Unity's **Entities** and **NetCode** packages.
- Experiment with custom gameplay features and prediction/interpolation flows.

---

## Implementation Notes

- I tried to keep all custom logic inside the `Implementation/` folder.
- However, some changes required touching other areas due to **tight coupling** in the original codebase.

---

## Features Added

- **Health Bar**:  
  A simple floating health bar is rendered above each player character.

- **Hit Visual Feedback**:  
  When a player is hit, a **predicted visual effect** is spawned locally.  
  This effect is **interpolated on other clients** for visual consistency.

- **Buff System** (WIP):  
  A basic framework for a **buff system** was added.  
  Buffs like `SpeedMultiplier` and `DamageMultiplier` are defined, but the system is still **in progress** and not fully working yet.
