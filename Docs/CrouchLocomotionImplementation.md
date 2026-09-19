# 下蹲八向移动与 CrouchJog 实现说明

本文记录 `PlayerAnimationTest` 场景中 `ShotgunGirl_Chartacter (1)` 的下蹲功能实现，并给出从零复刻的操作步骤。

## 最终行为


| 操作                  | 结果                                                                             |
| --------------------- | -------------------------------------------------------------------------------- |
| 按`C`                 | 播放`IdleToCrouch`，进入蹲下待机 `CrouchIdle`。                                  |
| 蹲下时移动            | 使用 Crouch 文件夹中的八向`CrouchWalk` 动画。                                    |
| 蹲下移动时按住`Shift` | 保持下蹲，切换至对应方向的`CrouchJog` 动画，并以较快的下蹲速度移动。             |
| 松开方向键            | 直接回到`CrouchIdle`，不会经过普通的 Stop/Run 动画。                             |
| 再按`C`               | 播放`CrouchToIdle`，回到普通待机；若仍有移动输入，则该过渡可立即被普通移动打断。 |
| 按跳跃                | 当前输入帧立刻取消下蹲；角色正常起跳、落地后回到普通移动状态。                   |

关键设计是：下蹲不是在普通 `MoveLoop` 状态里临时替换一个动画，而是有一组独立状态：

```text
普通 Idle ──C──> CrouchEnter ──动画结束──> CrouchIdle
                              │
                              └─松开 C──> CrouchExit ──动画结束──> 普通 Idle

CrouchIdle ──有移动输入──> CrouchMove
CrouchMove ──无移动输入──> CrouchIdle
CrouchMove ──Shift──> 同方向 CrouchJog
CrouchMove ──松开 Shift──> 同方向 CrouchWalk
CrouchExit ──仍有移动输入──> 普通 MoveLoop（慢跑/奔跑，不等待起身动画结束）
任意下蹲状态 ──跳跃──> 取消下蹲，再走普通跳跃流程
```

因此，蹲下移动和停止不会先进入普通慢跑或普通停止状态，自然也不会出现短暂闪回正常动画的问题。

## 本次实际修改的内容

### 动画配置与预制体

- 更新 `Assets/_Res/Config/AwCon/Characters/Player/Modules/LocomotionModule.asset`：
  - 配置下蹲进入、下蹲待机、下蹲退出三个动画。
  - 配置 `CrouchWalk` 八方向动画。
  - 配置 `CrouchJog` 八方向动画。
- 下蹲碰撞体逻辑已合并进角色原有的 `BBBCharacterController`，没有额外新增 MonoBehaviour 组件。
  - 它读取权威的 `RuntimeData.IsCrouching`，调整自身 `CharacterController` 的高度和中心点，使碰撞体与蹲下姿势一致。
- `Assets/Resource/Role/ShotgunGirl_Chartacter.prefab` 已移除原先的 `AwConCrouchAdapter`，角色无需额外挂载该组件。

### 代码


| 文件                                                                                 | 修改目的                                                                                           |
| ------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------- |
| `Assets/Scripts/Player/Character/BBBCharacterController.cs`                         | 在已有角色根控制器中同步下蹲碰撞体，不新增组件。                                                   |
| `Assets/Scripts/Player/Character/ScriptableObjects/LocomotionSO.cs`                  | 新增下蹲动画字段，并把 Inspector 中通用的“设置”标题改为对应功能标题。                            |
| `Assets/Scripts/Player/Inputs/PlayerInputReader.cs`                                  | `C` 控制下蹲；Shift 不再强制取消下蹲；跳跃输入立即取消下蹲。                                       |
| `Assets/Scripts/Player/Character/PlayerRuntimeData.cs`                               | 新增`IsCrouchJogging`。                                                                            |
| `Assets/Scripts/Player/Character/Locomotion/LocomotionIntentProcessor.cs`            | 根据“在地面 + C”计算`IsCrouching`，根据“下蹲 + Shift”计算 `IsCrouchJogging`。                  |
| `Assets/Scripts/Player/Character/Locomotion/MotionDriver.cs`                         | 下蹲走和下蹲慢跑使用独立速度倍率。                                                                 |
| `Assets/Scripts/Player/Character/States/PlayerStateType.cs`                          | 新增四个下蹲状态枚举。                                                                             |
| `Assets/Scripts/Player/Character/States/PlayerStateRegistry.cs`                      | 自动注册四个下蹲状态。                                                                             |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerCrouchStates.cs`   | 新建：实现下蹲进入、待机、八向移动、退出状态。                                                     |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerIdleState.cs`      | 普通待机按 C 时进入`CrouchEnter`。                                                                 |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerMoveStartState.cs` | 普通起步期间检测到下蹲时切到`CrouchMove`。                                                         |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerMoveLoopState.cs`  | 普通移动期间检测到下蹲时切到`CrouchMove`。                                                         |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerStopState.cs`      | C 可立即打断普通 Stop 动画并进入 `CrouchEnter`；若本来已处于下蹲则直接回 `CrouchIdle`。            |
| `Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerLandState.cs`      | 落地后根据当前下蹲标记返回普通或下蹲移动状态。跳跃已在输入层清除下蹲，所以正常情况下会回普通状态。 |

## 动画资源配置

动画根目录：

```text
Assets/_Res/Animations/Female/FemaleRunner/Movements/Crouch/
```

在 `LocomotionModule.asset` 中填写以下字段。


| Locomotion SO 字段                                 | 动画文件                                     |
| -------------------------------------------------- | -------------------------------------------- |
| `Idle To Crouch`                                   | `IdleToCrouch.anim`                          |
| `Crouch Idle`                                      | `CrouchIdle.anim`                            |
| `Crouch To Idle`                                   | `CrouchToIdle.anim`                          |
| `Crouch Walk Fwd` / `Crouch Jog Fwd`               | `CrouchWalk_F.anim` / `CrouchJog_F.anim`     |
| `Crouch Walk Back` / `Crouch Jog Back`             | `CrouchWalk_B.anim` / `CrouchJog_B.anim`     |
| `Crouch Walk Fwd Left` / `Crouch Jog Fwd Left`     | `CrouchWalk_FL.anim` / `CrouchJog_FL.anim`   |
| `Crouch Walk Fwd Right` / `Crouch Jog Fwd Right`   | `CrouchWalk_FR.anim` / `CrouchJog_FR.anim`   |
| `Crouch Walk Back Left` / `Crouch Jog Back Left`   | `CrouchWalk_BL.anim` / `CrouchJog_BL.anim`   |
| `Crouch Walk Back Right` / `Crouch Jog Back Right` | `CrouchWalk_BR.anim` / `CrouchJog_BR.anim`   |
| `Crouch Walk Left` / `Crouch Jog Left`             | `CrouchWalk_L45.anim` / `CrouchJog_L45.anim` |
| `Crouch Walk Right` / `Crouch Jog Right`           | `CrouchWalk_R45.anim` / `CrouchJog_R45.anim` |

其中左右方向使用资源里的 `L45` 与 `R45`：项目当前的方向枚举把它们作为纯左、纯右方向来使用。

## 复刻步骤

### 1. 准备输入

打开 Input Actions，确认 `Player/Crouch` 绑定为：

```text
<Keyboard>/c
```

`Sprint` 保持绑定到 Shift。下蹲慢跑不是另加一个按键，而是“下蹲状态中按住 Shift”。

### 2. 扩展 LocomotionSO

在 `LocomotionSO.cs` 增加三个过渡动画字段和两组八向动画字段。字段应使用与现有动画相同的序列化类型（本项目为 `TransitionAsset`）。逻辑结构如下：

```csharp
[Header("Crouch Locomotion - Transitions")]
public TransitionAsset IdleToCrouch;
public TransitionAsset CrouchIdle;
public TransitionAsset CrouchToIdle;

[Header("Crouch Locomotion - Eight Directions")]
public TransitionAsset CrouchWalkFwd;
// ...Back、Left、Right、四个斜向，共八个 CrouchWalk 字段
public TransitionAsset CrouchJogFwd;
// ...同样八个 CrouchJog 字段
```

同时将原本所有泛用的 Inspector 标题“设置”替换成具体分组，例如：

```text
Locomotion - Start Transitions
Locomotion - Loop Transitions
Locomotion - Stop Transitions
Airborne Transitions
Grounded Threshold
Base Locomotion Animations
Crouch Locomotion - Transitions
Crouch Locomotion - Eight Directions
```

这样在 Inspector 中能直接看懂每个动画槽用于什么功能。

### 3. 在 Inspector 填入动画

1. 在 Project 窗口打开 `Assets/_Res/Config/AwCon/Characters/Player/Modules/LocomotionModule.asset`。
2. 展开 **Crouch Locomotion - Transitions**，拖入 `IdleToCrouch`、`CrouchIdle`、`CrouchToIdle`。
3. 展开 **Crouch Locomotion - Eight Directions**，按上一节表格把 16 个动画全部拖入对应字段。
4. 保存项目。

不要直接用文本编辑器修改 `.asset`、`.prefab` 或 `.unity` 文件；这些 Unity 序列化资产应通过 Unity Inspector、Prefab Mode 或编辑器 API 修改。

### 4. 在角色根控制器中同步下蹲碰撞体

不需要新挂组件。`BBBCharacterController` 已经拥有 `CharacterController`、输入管线和运行时数据，因此在该类中：

1. `Awake` 缓存站立时的 `CharacterController.height` 和 `center`。
2. 在输入意图处理完成后读取 `RuntimeData.IsCrouching`。
3. 下蹲时缩短高度并向下偏移中心点；解除下蹲时恢复缓存的站立尺寸。
4. 在对象池回收/重生时恢复站立碰撞体，避免复用时保持下蹲尺寸。

高度比例通过 `BBBCharacterController` Inspector 中的 **下蹲碰撞体 / Crouch Height Multiplier** 调整，当前值为 `0.65`。这不会增加角色上的组件数量。

### 5. 让数据层表达“下蹲”和“下蹲慢跑”

在 `PlayerRuntimeData.cs` 增加：

```csharp
public bool IsCrouchJogging;
```

在 `LocomotionIntentProcessor` 的每帧输入意图处理中加入：

```csharp
_data.IsCrouching = input.CrouchHeld && _data.IsGrounded;
_data.IsCrouchJogging = _data.IsCrouching && input.SprintHeld;
```

这里要求在地面才允许下蹲；处于空中时 `IsCrouching` 会是 `false`。

普通冲刺分支必须排除下蹲，否则 Shift 会进入正常 Sprint 状态：

```csharp
else if (input.SprintHeld && !_data.IsCrouching /* 其余原有条件 */)
{
    // 原有普通冲刺逻辑
}
```

### 6. 设置速度

在 `MotionDriver` 计算移动速度时加入下蹲倍率：

```csharp
float crouchMultiplier = _data.IsCrouching
    ? (_data.IsCrouchJogging ? 0.75f : 0.5f)
    : 1f;
```

然后将该倍率乘到原本的最终移动速度上。`0.5` 和 `0.75` 是当前使用值；可按动画位移感在 Inspector 或代码中继续调整。

### 7. 处理跳跃立即取消下蹲

在 `PlayerInputReader` 得到 `rawData.JumpJustPressed` 后立即清理下蹲：

```csharp
if (rawData.JumpJustPressed)
{
    IsCrouching = false;
    rawData.CrouchHeld = false;
}
```

同时，不要再用“按住 Shift”强制取消下蹲。Shift 在下蹲状态的职责是让 `IsCrouchJogging` 变为 `true`。

这一步很关键：如果只在落地状态里处理，角色起跳时仍会带着下蹲标记，落地时就可能进入蹲下动画；这里在跳跃输入发生的同一帧取消它。

### 8. 创建独立的下蹲状态

在 `PlayerStateType` 新增：

```csharp
CrouchEnter,
CrouchIdle,
CrouchMove,
CrouchExit,
```

新建 `PlayerCrouchStates.cs`，实现四个状态：

- `PlayerCrouchEnterState`：播放 `IdleToCrouch`，动画结束后去 `CrouchIdle`。
- `PlayerCrouchIdleState`：循环 `CrouchIdle`；有移动输入去 `CrouchMove`；取消下蹲去 `CrouchExit`。
- `PlayerCrouchMoveState`：根据八向输入与 `IsCrouchJogging` 选择动画；无移动输入回 `CrouchIdle`；取消下蹲去 `CrouchExit`。
- `PlayerCrouchExitState`：播放 `CrouchToIdle`；结束后回普通 `Idle`。若退出过程中检测到移动输入，则以 `0.15` 秒淡入立刻转到普通 `MoveLoop`，按当前 locomotion 状态选择 Jog 或 Sprint，不等待起身动画结束。

`CrouchMove` 的动画选择逻辑应只查看下蹲专用字段：

```csharp
TransitionAsset clip = data.IsCrouchJogging
    ? SelectCrouchJog(direction)
    : SelectCrouchWalk(direction);
```

方向变化或 Shift 状态变化时，以短淡入（当前为 `0.2f`）重新播放选出的下蹲动画。

### 9. 注册新状态

在 `PlayerStateRegistry` 的创建逻辑中注册四个状态。当前实现通过 `RegisterIfMissing` 自动补齐，因此不需要再手动编辑已有的 `PlayerBrain` 序列化状态列表：

```csharp
RegisterIfMissing(new PlayerCrouchEnterState(player));
RegisterIfMissing(new PlayerCrouchIdleState(player));
RegisterIfMissing(new PlayerCrouchMoveState(player));
RegisterIfMissing(new PlayerCrouchExitState(player));
```

### 10. 接入现有普通状态

在下列状态的最前面检查 `data.IsCrouching`，并切换到下蹲状态：


| 原状态                 | 下蹲时去向    |
| ---------------------- | ------------- |
| `PlayerIdleState`      | `CrouchEnter` |
| `PlayerMoveStartState` | `CrouchMove`  |
| `PlayerMoveLoopState`  | `CrouchMove`  |
| `PlayerStopState`      | `CrouchIdle`  |

落地状态则根据 `IsCrouching` 决定回普通或下蹲状态。由于第 7 步已经让跳跃立即取消下蹲，正常跳跃落地会回普通 `Idle` 或 `MoveLoop`。

## 验证清单

在 `PlayerAnimationTest` 进入 Play Mode 后按下面顺序测试：

1. 静止按 C：只应播放一次 `IdleToCrouch`，随后停在 `CrouchIdle`。
2. 蹲下后按前、后、左、右及四个斜方向：每个方向均应播放对应 `CrouchWalk`。
3. 保持任一方向后按住 Shift：应切为相同方向的 `CrouchJog`，角色不应站起，也不应播放普通 Run。
4. 先松 Shift：应回相同方向的 `CrouchWalk`。
5. 松开方向键：应直接回 `CrouchIdle`，不应闪过普通 Stop 或普通 Idle。
6. 蹲下待机时再按 C：播放 `CrouchToIdle` 后回普通 Idle。
7. 普通移动后松开方向、Stop 动画尚未结束时按 C：应立刻打断 Stop，开始 `IdleToCrouch`，不应等待 Stop 播放完成。
8. 蹲下移动中按 C、同时不松开移动：应立刻从 `CrouchToIdle` 淡入普通 Jog/Sprint 循环，不应等待起身动画播完。
9. 蹲下时按跳跃：角色立刻解除下蹲后起跳；落地后不应恢复到 `CrouchIdle`。
10. 退出 Play Mode 后检查 prefab：角色根节点没有 `AwConCrouchAdapter`；`BBBCharacterController` 的下蹲碰撞体比例为预期值，`LocomotionModule.asset` 的 19 个下蹲动画槽都保持引用。

## 常见问题

### 为什么以前会短暂播放普通慢跑或停止动画？

原因是下蹲动画曾经被放在普通 `MoveLoop` 的选择逻辑中。普通状态在切换、停止或起步时会自身播放正常 Run/Stop 动画；下蹲动画只能在下一次更新时覆盖它，因此会看到一帧或短暂闪烁。

现在 `CrouchMove` 和 `CrouchIdle` 自己负责移动与停止，不再经过普通移动状态，因此不会触发那些普通动画。

### 为什么按 Shift 不应该取消下蹲？

该需求中的 Shift 是“下蹲慢跑”修饰键，而不是“强制站起/冲刺”键。正确关系是：

```text
IsCrouching     = C 且在地面
IsCrouchJogging = IsCrouching 且 Shift
```

普通 Sprint 逻辑只应在 `!IsCrouching` 时执行。

### 为什么跳跃要在输入层就取消下蹲？

状态机和落地逻辑都读取运行时数据。如果等到落地才清除，下蹲标记会跨越整个空中阶段，落地时仍可能被识别为蹲下。跳跃按下的同一帧清除标记，后续的跳跃、空中和落地流程都会自然回到普通状态。

## 保存与编译

每次修改脚本后，等待 Unity 编译完成，并确认 Console 没有 C# 编译错误。每次修改角色组件后，务必把 Override 应用回：

```text
Assets/Resource/Role/ShotgunGirl_Chartacter.prefab
```

本次最终已完成脚本编译检查；动画行为仍建议按上面的八项清单在你的实际输入、摄像机朝向和动画资源设置下完整走一遍。
