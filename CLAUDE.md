# CLAUDE.md — SnowblowerSim

SnowblowerSim (working title) is a photorealistic, level-based, third-person snowblowing simulator for PC/Steam, built in Unity 6 LTS with the Universal Render Pipeline (URP) and C#. You start with a single-stage walk-behind blower and clear driveways, sidewalks, and paths against a clock while snow keeps falling; clearing ≥95% of the must-clear area before time runs out unlocks the next level. Money earned buys component upgrades (engine, paddles/auger, drive, chute) and eventually bigger machines (single-stage → two-stage → tracked two-stage → three-stage) across 20 levels in 3 environments (suburban, rural/mountain, small-town main street). The two things that matter most, in order: (1) **snow movement** — how snow is cut, ingested, thrown, lands, piles, and slumps, with every upgrade visibly changing throw distance, arc, bog-down depth, and walking speed into snow; (2) **photorealism** of the blower and the scene. Everything else exists to serve those two.

## Where the work runs (read this first — it shapes everything)

Claude Code runs in a **cloud Linux container** attached to the GitHub repo. That container **does not have Unity**. So there are three places verification happens, and every phase is written around them:

| Tier | Where | What it proves | When |
|---|---|---|---|
| **T1 — Core tests** | Cloud container, `dotnet test` | All physics/economy math (pure C#, no Unity dependency) | After **every** STEP, before commit |
| **T2 — Unity CI** | GitHub Actions (GameCI, Unity 6 Linux) | The Unity project compiles; EditMode + PlayMode tests pass in batchmode | After every push; Claude blocks on the result at CI-GATE steps |
| **T3 — Local gate** | Your Windows machine, Unity Editor | Anything visual, feel, or performance; Editor tools that build prefabs/scenes | At each phase's FINAL ACCEPTANCE, and at marked **LOCAL STEP**s |

Consequences that are baked into the plan:

- **Two code homes.** `src/SnowSim.Core/` is a plain .NET class library (netstandard2.1, zero UnityEngine references) holding every formula: intake sweep, power budget, ballistics, deposition, slump, zone rasterization, payout, spec resolver. It compiles and tests in the container in seconds. The Unity project at `unity/` includes the same source files by reference (a UPM local package pointing at `src/SnowSim.Core`), so Unity and Burst compile the identical code. Nothing in Core may `using UnityEngine`; it uses its own tiny `float2/float3` structs or `System.Numerics` — P0 decides and pins it.
- **No visual authoring tools.** No Shader Graph, no VFX Graph — those are binary-ish JSON assets Claude cannot write reliably without the Editor. Shaders are **hand-written HLSL/ShaderLab** (URP). Particles are **`ParticleSystem` configured entirely from C#** or **`Graphics.RenderMeshIndirect`** instanced quads fed from a GraphicsBuffer. Both are text and testable for compile.
- **Scenes and prefabs are built by code.** One hand-made scene (`Boot.unity`, created by you once in P0) holds a single `Bootstrap` component. Everything else — test scenes, grey-box blower, environment kits, terrain, cameras, UI — is constructed at runtime or by Editor scripts. When a phase needs an Editor script run (import settings, prefab save, screenshot harness), it is marked **LOCAL STEP**: Claude writes the script and opens the PR; you pull, run it in the Editor, commit the result to the same branch.
- **Big binary assets never enter git.** `Assets/ThirdParty/` is gitignored except `*.license.txt` and a `manifest.json`. Fetch scripts run on your machine; CI only needs what compiles.
- **Every phase is one PR.** Claude works on the phase branch, one commit per STEP, pushes after each STEP, opens a draft PR at STEP 1, and marks it ready at the last STEP. You do the LOCAL STEPs and the FINAL ACCEPTANCE on that branch, then merge. `main` is always green.

## Working rules (apply to every phase)

Repo-wide. Claude Code must follow these in every phase:

- **One STEP = one commit = one push.** Never batch commits. Commit message starts with the phase and step, e.g. `P3-S4: intake power budget`.
- **T1 before every commit.** Run `scripts/core-test.sh` (dotnet build + test on `src/SnowSim.Core`). Red means the STEP is not done.
- **T2 at CI-GATE steps.** A STEP marked **[CI-GATE]** touches Unity-side code. After pushing it, run `scripts/ci-wait.sh` (wraps `gh run watch` + `gh run view --log-failed`) and do not start the next STEP until the Unity workflow is green. Fix-forward commits are allowed and are labeled `P3-S4-fix: ...`. Unmarked STEPs push without waiting.
- **The last STEP of every phase is always a CI-GATE.**
- **~150-line file cap.** If a C# file would exceed ~150 lines, split it into partial classes or separate modules.
- **View before editing.** Always read a file's current contents before modifying it.
- **Split large edits.** Several small, surgical edits over one sweeping rewrite.
- **No new packages beyond those named in the phase prompt.** If a phase seems to need one, stop and ask.
- **Snow sim is CPU-side, Burst-compiled, on NativeArrays.** Never move sim logic into shaders or compute. The GPU only renders what the CPU computed.
- **Formulas live in `src/SnowSim.Core`, never in MonoBehaviours.** Each is a static pure method with an xUnit test in the container. Unity-side code (jobs, MonoBehaviours) only calls them. If a Burst job needs the math, the Core method must be Burst-compatible (blittable args, no allocations, no exceptions).
- **Tuning data lives in `src/SnowSim.Core/Data/` as plain C# records, not in ScriptableObjects.** Machine specs, upgrade values, level definitions, weather profiles, economy rates. Reason: the Inspector is not available from the cloud, and plain records are testable in T1 (the difficulty audit is an xUnit test). ScriptableObjects are used only where Unity needs asset references (prefabs, materials, audio clips), and those are generated by a `ScriptableObjectFactory` Editor script — a LOCAL STEP — never typed in by hand.
- **Units are SI.** Meters, kilograms, seconds, watts, radians internally. Convert to inches/hp/mph only in UI.
- **Never hand-edit `.unity`, `.prefab`, `.asset`, `.mat`, or `.meta` YAML.** Write an Editor script and mark a LOCAL STEP.
- **Never touch `Assets/ThirdParty/`.** Imported assets live there untouched with their license files. Wrappers and material overrides live in `Assets/Art/`.
- **Anything you cannot verify from the container is a LOCAL STEP, said out loud.** In the PR description, keep a checklist headed "LOCAL STEPS for Dave" listing each one with the exact menu item or script to run and what to commit afterward.


---

## Invariants

- Units: SI internally. Snow sim is CPU/Burst on NativeArrays. GPU renders only. Formulas live in src/SnowSim.Core.

---

## Repo layout

```
CLAUDE.md                     this file — read first every session
.github/workflows/unity-ci.yml  CI: core (dotnet) → unity-tests (GameCI) → unity-build-smoke (PRs)
scripts/
  core-test.sh                T1: dotnet build + test of src/
  ci-wait.sh                  T2: wait on the latest Actions run for this branch
  sketchfab-search.py         asset recon (stdlib only)
  fetch-thirdparty.sh         downloads unity/Assets/ThirdParty/manifest.json entries (owner's machine only)
src/
  SnowSim.Core/               netstandard2.1 class library, zero UnityEngine refs; also a local UPM
                              package (com.harris.snowsim.core) consumed by unity/
    Math/Float3.cs            blittable float2/float3 (no System.Numerics)
    Data/                     tuning data as plain C# records (added in later phases)
  SnowSim.Core.Tests/         net8.0 xUnit tests for Core (T1)
unity/                        Unity 6 LTS URP project (ProjectSettings/ and Boot.unity are Editor-authored)
  Packages/manifest.json
  Assets/
    _Project/
      Scripts/{Snow,Machine,Throw,Zones,Weather,Economy,Levels,UI}/   one asmdef each: SnowSim.<Folder>
      Scripts/Core/           SnowSim.CoreRuntime (Unity-side; "SnowSim.Core" is the pure package)
      Scripts/Editor/         SnowSim.Editor (Editor-only tools, appliers, guards)
      Tests/EditMode/         SnowSim.Tests.EditMode
      Tests/PlayMode/         SnowSim.Tests.PlayMode
      Shaders/                hand-written HLSL/ShaderLab (URP)
      Scenes/Boot.unity       the only hand-made scene; holds the Bootstrap component
    Art/{Materials,Models}/   wrappers and material overrides for third-party art
    ThirdParty/               gitignored except manifest.json and *.license.txt — never edit
docs/asset-recon/             blower + scenery shopping lists, license/attribution register
```

---

## How to verify

- **T1 — Core tests (every STEP, before commit):** `scripts/core-test.sh`
  Runs `dotnet build` then `dotnet test --logger "console;verbosity=minimal"` over `src/`. Non-zero exit = STEP not done.
- **T2 — Unity CI wait (every [CI-GATE] STEP, after push):** `scripts/ci-wait.sh`
  Finds the latest workflow run for the current branch (`gh run list`), blocks on `gh run watch --exit-status`, and on failure prints `gh run view --log-failed`. If `gh` is not installed in the cloud container, check the same run through the GitHub MCP Actions tools (list runs for the branch → get run → failed job logs). Do not start the next STEP until the run is green.
- **T3 — LOCAL STEP:** anything that needs the Unity Editor or a human eye (running an Editor menu item, saving a scene/prefab/settings asset, visual/feel/perf checks, downloading third-party assets). Claude writes the script, lists it in the PR under **"LOCAL STEPS for Dave"** with the exact menu item or command and what to commit afterward, and continues with what can be done without it. Dave runs it in the Editor on the phase branch and commits the result there.
