# Shotgun Girl 武器接入说明

## 资源位置

- 角色预制体：`Assets/Resource/Role/ShotgunGirl_Chartacter.prefab`
- 右手武器：`root/.../hand_r/Weapon_Shotgun`
- 收枪挂点：`root/pelvis/spine_01/spine_02/spine_03/Put_Socket_ShotGun`
- 动画：`Assets/_Res/Animations/Shotgun_Girl/Aiming` 与 `Normal`

## 通用框架接入

- 瞄准使用原来的 AimingModule / PlayerAimIdleState / PlayerAimMoveState，已移除散弹枪专属移动分支与自动搜索敌人的逻辑。
- RangedWeaponSO 保留源框架的装备、卸下、持枪待机、瞄准动画字段，并增加 AnimationProfile 引用。
- HitscanWeaponSO / HitscanWeaponBehaviour 提供可复用的弹匣、备用弹、射线散布、换弹、后坐力与 IK 生命周期。散弹枪类仅保留兼容名称，已有资产和 Prefab GUID 不变。
- 当前 ShotgunItem 已绑定 ShotgunAnimationProfile，使用 8 发弹匣、40 发备用弹、8 颗弹丸和 0.65 秒射击间隔。音效和枪口特效为可选配置，未绑定时不播放。
- IHolsterableItem 让背包按武器提供的时长完成收枪，不依赖具体枪种。
- EquipmentVisualSlot 在角色上绑定武器数据及背挂 Renderer，控制背挂与生成的手持实例互斥；新增武器需配置自己的绑定。
- PlayerConfig_Main.Shotgun 与 ShotgunModule 仅作旧资产兼容保留，运行时武器从自身 AnimationProfile 读取动画配置。

## 输入约定

- 鼠标左键：开火。
- Tab：切换源框架的瞄准/横移模式；这不是自动选择敌人的目标锁定系统。
- 鼠标右键：按住瞄准，松开退出；不选择或锁定敌人。Tab 模式开启时保持瞄准。
- R：换弹。
- Q：在快捷栏内的空手、霰弹枪、AK、加农炮之间循环。

## 场景与预制体要求

1. 武器必须放在右手骨骼下，枪口朝角色前方。
2. 武器预制体要提供 `LeftHandGoal` 与 `Muzzle` 挂点，供左手 IK 和弹道使用。
3. 背包快捷栏至少放入霰弹枪；未装备物品即为空手状态。
4. 角色 Animator 使用 Shotgun Girl 的 Avatar，动画层使用上身 Mask，避免覆盖腿部移动。

## 调参位置

- 装备、收起、瞄准动画：`ShotgunItem`；射击、换弹动画：其 `AnimationProfile`。
- 左手握枪位置：`Weapon_Shotgun/LeftHandGoal`。
- 枪口与射击方向：`Weapon_Shotgun/Muzzle`。
- 基础移动动画：玩家 `LocomotionSO`；持枪上身动画由霰弹枪装备层覆盖。

## Git 提交

```powershell
git switch -c codex/shotgun-gameplay
git status
git add Assets Docs
git commit -m "feat: add shotgun gameplay"
git push -u origin codex/shotgun-gameplay
```

提交前先用 `git diff --check` 和 Unity Console 确认无错误；不要把无关的场景改动一并暂存。
