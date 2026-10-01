# Step 11 — integrated WindBot Ryzeal executor

Integrated into this WindBot checkout. ClientCard.ActionIndex is a per-action index array (idle action slots 0-4), while ActionActivateIndex maps activation descriptions to indices. This is not a universal adaptation for all WindBot forks.

RyzealExecutor and its candidate, profile, decision logger, state adapter, and evaluator sources are now under Game/AI/Decks. The observer contract and optional candidate-selection hook are under ExecutorBase/Game/AI. GameAI's existing MainPhaseAction API was retained while integrating candidate dispatch and decision observation. Idle action indices use the matching ActionIndex slot; phase transitions record -1.

GameAI.cs was retained as GameAI.step11-integrated.cs.txt; the integration patch and other .txt files are references only and are not project compile items. WindBot.csproj already includes Game/AI/Decks/*.cs. The WindBot Release target builds with Visual Studio MSBuild when .NET Framework 4.0 reference assemblies are supplied. The full solution also contains legacy libWindbot, which requires Xamarin Android build targets.

Known limitations: enums/material prompt behavior and runtime game state still need duel validation. Node cost checks are heuristic and not a full Lua cost simulator. Replacement yes/no policy remains unfinished. Visibility filtering and chain-event correlation remain unfinished. Legacy policy is still gated behind candidate fallback and may be delayed when candidates exist. No guarantee of optimal play or full effect coverage.

Release build and startup smoke tests passed for Ryzeal and an existing ABC executor. A Step 11 JSONL decision entry requires an actual duel to reach a main-phase decision; no duel replay has been run.
