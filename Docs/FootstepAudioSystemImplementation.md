# 移动脚步声与多地形系统实现说明

本文记录当前项目中移动脚步声系统的完整实现，并说明如何从零复刻、配置新地形和排查没有声音的问题。

## 一、最终效果

| 状态 | 使用的音效动作 |
| --- | --- |
| 普通 Walk | `Walk` |
| 普通 Jog / Sprint | `Run` |
| 蹲下 Walk | `Walk` |
| 蹲下 CrouchJog | `Run` |
| Tab 锁定移动（AimMove） | 根据当前 LocomotionState 使用 `Walk` 或 `Run` |
| 跳跃落地 | `Land` |
| 未来滑铲 | `Slide`，音频池已预留，等待滑铲状态触发 |

脚步声不是通过每帧播放，而是根据移动动画的归一化播放时间，在脚相位经过阈值时触发。一个完整循环会在循环中点和循环回绕处各触发一次，避免脚步声过密或漏播。

## 二、系统结构

```text
移动动画播放
    ↓
PlayerMoveLoopState / PlayerCrouchMoveState
    ↓ 读取动画归一化时间
FootstepController
    ↓ 向下 Raycast 查找地面
FootstepSurfaceMarker（没有标记时默认 Concrete）
    ↓
AudioDriver.PlayFootstep(surface, action)
    ↓
AudioSO 随机选择对应 AudioClip
    ↓
角色 SfxSource.PlayOneShot()
```

落地音效走另一条路径：

```text
PlayerSfxEvent.Land
    ↓
AudioController
    ↓
FootstepController.ResolveSurface()
    ↓
AudioSO 的地形 Land 音效
```

角色不需要额外挂载脚步 MonoBehaviour。脚步控制器由已有的 `BBBCharacterController` 在运行时创建。

## 三、音频资源目录

当前目录为：

```text
Assets/_Res/Audio/Footsteps/
├─ Concrete/
├─ Dirt/
├─ Grass/
├─ Gravel/
├─ Ice/
├─ Metal/
├─ Mud/
├─ Sand/
├─ Snow/
├─ Water/
└─ Wood/
```

每个地形下面按动作分类：

```text
Concrete/
├─ Walk/
├─ Run/
├─ Land/
├─ Slide/
└─ WalkRun/
```

规则：

- `Walk`：普通走路和蹲下走路。
- `Run`：慢跑、奔跑和蹲下慢跑。
- `Land`：跳跃落地。
- `Slide`：未来滑铲。
- `WalkRun`：同一批音频同时作为 Walk 和 Run 使用。当前 Dirt 使用了这种方式。
- 同一个动作放多个音频，运行时会随机选择，减少重复感。

## 四、代码文件职责

### 1. 地形和动作枚举

文件：

```text
Assets/Scripts/Player/Character/Presentation/FootstepSurfaceType.cs
```

当前地形枚举：

```text
Concrete、Dirt、Grass、Gravel、Ice、Metal、Mud、Sand、Snow、Water、Wood
```

当前动作枚举：

```text
Walk、Run、Land、Slide
```

新增地形时，需要同时在此枚举中增加一个值，并在 `AudioSO` 中配置对应音频。

### 2. 地形标记组件

文件：

```text
Assets/Scripts/Player/Character/Presentation/FootstepSurfaceMarker.cs
```

这是一个非常轻量的 MonoBehaviour，只有一个 `Surface` 字段。系统会在射线命中的 Collider 上调用 `GetComponentInParent<FootstepSurfaceMarker>()`，所以标记可以放在：

- Terrain 所在物体上。
- 地面 Collider 所在物体上。
- Collider 的父物体上。

没有标记时使用 `Concrete`，因此旧场景不配置标记也能播放声音。

### 3. 脚步时机和地形解析

文件：

```text
Assets/Scripts/Player/Character/Core/Driver/FootstepController.cs
```

职责：

1. 接收当前移动动画的归一化时间。
2. 检测动画循环中点或循环回绕。
3. 判断当前是 Walk 还是 Run。
4. 从角色脚下向下 Raycast。
5. 查找 `FootstepSurfaceMarker`。
6. 调用 `AudioDriver.PlayFootstep()`。

### 4. 音频播放

文件：

```text
Assets/Scripts/Player/Character/Core/Driver/AudioDriver.cs
Assets/Scripts/Player/Character/ConfigData/PlayerSOModules/AudioSO.cs
Assets/Scripts/Player/Character/expression/AudioController.cs
```

`AudioSO` 使用地形 + 动作作为索引，例如：

```text
Concrete + Walk
Concrete + Run
Concrete + Land
Metal + Walk
Metal + Run
Metal + Land
```

每组音频支持多个 `AudioClip`，由 `AudioSO.TryPickFootstep()` 随机取一个。

## 五、从零复刻的具体步骤

### 步骤 1：整理音频

把音频按下面格式放入 `Assets/_Res/Audio/Footsteps`：

```text
地形/动作/音频文件.wav
```

示例：

```text
Assets/_Res/Audio/Footsteps/Concrete/Walk/concrete_walk_01.wav
Assets/_Res/Audio/Footsteps/Concrete/Run/concrete_run_01.wav
Assets/_Res/Audio/Footsteps/Concrete/Land/concrete_land_01.wav
Assets/_Res/Audio/Footsteps/Metal/Slide/metal_slide_01.wav
```

注意：不要把不同地形的音频混在同一个文件夹，否则自动配置脚本无法正确识别。

### 步骤 2：准备 AudioSO

打开：

```text
Assets/_Res/Config/AwCon/Characters/Player/Modules/AudioSO.asset
```

`AudioSO` 脚本中需要有：

```csharp
[SerializeField] private List<FootstepEntry> _footstepEntries;
```

其中每一项包含：

```text
Surface：地形
Action：动作
Clips：音频数组
```

如果手动配置，在 Inspector 的 `Footstep Surface Audio` 区域增加条目，并为每个条目填写地形、动作和音频数组。

### 步骤 3：自动批量绑定音频

推荐使用 Unity MCP 的 `execute_code` 批量配置，而不是手动拖拽 178 个音频。

批量配置逻辑：

1. 使用 `AssetDatabase.FindAssets("t:AudioClip", new[] { audioRoot })` 找到所有音频。
2. 读取音频相对路径的第一级作为地形。
3. 读取第二级作为动作。
4. 找到 `WalkRun` 时，同时写入 Walk 和 Run 两组。
5. 用 `SerializedObject` 写入 `AudioSO._footstepEntries`。
6. 调用 `EditorUtility.SetDirty()` 和 `AssetDatabase.SaveAssets()`。

本次项目已经完成批量配置，当前 `AudioSO` 有 38 组有效组合：

- 11 个地形的 Walk。
- 11 个地形的 Run。
- 11 个地形的 Land。
- 现有资源中存在的 Slide 组合。

### 步骤 4：给 Player 配置绑定 AudioSO

打开：

```text
Assets/_Res/Config/AwCon/Characters/Player/PlayerConfig_Main.asset
```

将：

```text
Audio = Assets/_Res/Config/AwCon/Characters/Player/Modules/AudioSO.asset
```

确认不是空引用。

角色运行时在 `BBBCharacterController.Awake()` 中读取：

```csharp
AudioDriver = new AudioDriver(transform, SfxSource, Config != null ? Config.Audio : null);
```

### 步骤 5：确认角色有 AudioSource

在 `PlayerAnimationTest` 场景中选中：

```text
ShotgunGirl_Chartacter (1)
```

确认 `BBBCharacterController` 的 `SfxSource` 已指向角色的 `AudioSource`。

建议：

- `Play On Awake` 关闭。
- `Loop` 关闭。
- `Volume` 按项目需要设置。
- 如果需要统一音量，可把 AudioSource 输出到 AudioMixer。

当前角色已经存在 `SfxSource`，并且 `PlayerConfig_Main` 已绑定 `AudioSO`。

### 步骤 6：配置地形标记

以金属地面为例：

1. 在 Hierarchy 中选中金属地面物体。
2. 确认该物体或子物体有 Collider / TerrainCollider。
3. 点击 `Add Component`。
4. 添加 `FootstepSurfaceMarker`。
5. 将 `Surface` 设置为 `Metal`。
6. 保存场景。

如果多个地面区域使用不同材质，建议每个区域使用独立 Collider 或独立父物体，并分别添加标记。

不要把 `FootstepSurfaceMarker` 添加到角色身上；它应该添加到被脚踩到的地面层级。

## 六、移动动画接入方式

普通移动在：

```text
Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerMoveLoopState.cs
```

`UpdateFootPhase()` 中读取动画归一化时间并调用：

```csharp
player.FootstepController?.UpdateLoop(cycleTime);
```

蹲下移动在：

```text
Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerCrouchStates.cs
```

`PlayerCrouchMoveState` 使用相同的脚步控制器，所以不需要额外复制一套脚步系统。

Tab 锁定移动使用独立的瞄准混合树，接入位置是：

```text
Assets/Scripts/Player/Character/States/FullBody/Aiming/PlayerAimMoveState.cs
```

进入 `AimMove` 时重置脚相位；每帧读取瞄准混合树的归一化时间并调用 `FootstepController.UpdateLoop()`。因此锁定移动不会因为使用另一套动画状态而漏掉脚步声。

动作选择规则：

```text
IsCrouchJogging = true       → Run
LocomotionState.Jog          → Run
LocomotionState.Sprint       → Run
其他地面移动                 → Walk
```

## 七、如何接入未来滑铲

目前 `FootstepActionType.Slide` 和音频池已经存在，但当前移动状态还没有滑铲状态。

滑铲状态中，在需要播放滑铲脚步的位置调用：

```csharp
var surface = player.FootstepController.ResolveSurface();
player.AudioDriver.PlayFootstep(surface, FootstepActionType.Slide);
```

如果滑铲是一次性动作，建议使用动画事件或滑铲状态的固定时间点触发，而不是按普通移动循环触发。

## 八、验证步骤

### 编译验证

1. 保存所有 `.cs` 文件。
2. 等待 Unity 编译完成。
3. 检查 Console 没有编译错误。

### 普通移动验证

在 `PlayerAnimationTest` 中：

1. 按 `W`，确认播放 Walk 脚步声。
2. 按住 `Shift + W`，确认播放 Run 脚步声。
3. 松开方向键，确认停止播放新的脚步声。

### 蹲下移动验证

1. 按 `C` 进入下蹲。
2. 按 `W`，确认播放蹲下 Walk 脚步声。
3. 按住 `Shift`，确认切换为蹲下 Run 脚步声。
4. 改变方向，确认八向蹲下动画仍然有脚步声。

### 多地形验证

1. 在两个地面物体上分别添加 `FootstepSurfaceMarker`。
2. 一个设置为 `Concrete`，另一个设置为 `Metal`。
3. 分别走过两个区域。
4. 确认音色随地形改变。

### 落地验证

1. 从地面跳起。
2. 在不同地形上落地。
3. 确认播放对应地形的 `Land` 音效。

## 九、没有声音时的排查顺序

按以下顺序检查：

1. 角色 `BBBCharacterController.SfxSource` 是否为空。
2. `BBBCharacterController.Config` 是否指向 `PlayerConfig_Main`。
3. `PlayerConfig_Main.Audio` 是否指向 `AudioSO.asset`。
4. `AudioSO` 的 `Footstep Surface Audio` 是否有条目。
5. 音频文件是否放在正确的“地形/动作”目录中。
6. `AudioSource` 是否被 Mute、音量是否为 0。
7. 地形 Collider 是否在 `Physics.DefaultRaycastLayers` 中。
8. 地形标记是否挂在命中 Collider 的父层级上。
9. 角色是否真的处于 Walk、Run 或 CrouchMove 状态。
10. Console 是否有脚步音频引用或状态机错误。

没有 `FootstepSurfaceMarker` 时应该仍然播放 Concrete；如果完全没有声音，优先检查 `SfxSource`、`PlayerConfig_Main.Audio` 和 `AudioSO` 引用。

## 十、本次项目已完成的文件

```text
Assets/Scripts/Player/Character/Core/Driver/FootstepController.cs
Assets/Scripts/Player/Character/Core/Driver/AudioDriver.cs
Assets/Scripts/Player/Character/Presentation/FootstepSurfaceMarker.cs
Assets/Scripts/Player/Character/Presentation/FootstepSurfaceType.cs
Assets/Scripts/Player/Character/ConfigData/PlayerSOModules/AudioSO.cs
Assets/Scripts/Player/Character/expression/AudioController.cs
Assets/Scripts/Player/Character/BBBCharacterController.cs
Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerMoveLoopState.cs
Assets/Scripts/Player/Character/States/FullBody/Locomotion/PlayerCrouchStates.cs
Assets/_Res/Config/AwCon/Characters/Player/Modules/AudioSO.asset
Assets/_Res/Config/AwCon/Characters/Player/PlayerConfig_Main.asset
```

## 十一、当前验证记录

- 已整理并绑定 178 个脚步音频。
- 已生成 38 组地形/动作音频配置。
- 11 个地形均有 Walk、Run、Land 音效。
- Slide 音效保留已有资源，等待滑铲状态接入。
- Unity 编译无错误。
- Console 无脚步系统相关错误。
- `PlayerAnimationTest` 中角色已绑定 `AudioSO`。
