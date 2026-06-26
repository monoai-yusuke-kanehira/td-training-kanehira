## 1. 神クラス（責務過多）
- GameUI.cs：責務→UI表示＋状態管理(State)＋設置入力＋経済(購入/売却)＋ジオメトリ判定
- LevelManager.cs：責務→ウェーブ進行＋ステージ状態＋勝敗判定＋全体ハブ（55箇所から参照）
- Tower.cs：責務→タワー本体＋6種のSingleton参照＋レベル管理連携
- Targetter.cs：責務：索敵範囲＋ターゲット選定＋物理判定が同居
- Agent.cs：責務→敵の移動＋HP＋経路＋状態を1クラスで保持
- CameraRig.cs：責務→カメラ操作＋入力＋境界計算
- TowerDefenseTouchInput.cs：責務→入力解釈＋7箇所のSingleton操作（UIロジックに侵食）

---

## 2. 密結合ランキング（送信側：.instance でグローバルSingletonを直接叩く回数）
- TowerUI.cs                          16
- EndGameScreen.cs                    14
- TowerDefenseKeyboardMouseInput.cs   11
- GameUI.cs                           10
- PauseMenu.cs                         8
- TowerDefenseTouchInput.cs            7
- Tower.cs                             6
- TouchInput.cs / KeyboardMouseInput   6
- SuperTowerLauncher.cs                5
- TowerPlacementGhost / TowerSpawnButton / TowerUI(HUD)  2前後

---

## 3. 依存のハブ（受信側：他クラスから .instance される回数）
- LevelManager.instance   55回
- GameUI.instance         36回
- InputController.instance 23回
- GameManager.instance    16回
- PoolManager.instance     6回

---

## 4. 密結合マップ（どのファイルがどのSingletonに手を伸ばしているか）

- LevelManager.instance ← 15ファイルが依存
  - CurrencyAffector, Agent, LootDrop, Tower, BallisticLauncher, SuperTowerLauncher,
  EndGameScreen, BuildSidebar, CurrencyUI, GameUI, PlayerBaseHealth,
  TowerSpawnButton, TowerUI, WaveUI, TowerDefenseKeyboardMouseInput

- GameUI.instance ← 8ファイル
  - TowerDefenseKeyboardMouseInput, TowerDefenseTouchInput, EndGameScreen,
  BuildSidebar, TowerPlacementGhost, TowerUI, PauseMenu, InputSchemeSwitcher

- InputController.instance ← 4 / GameManager.instance ← 5 / PoolManager.instance ← 1

---

## 5. static依存の調査

可変なグローバル静的状態は存在せず、見つかった static は全て static readonly（不変）

---

## 6. 毎フレームのポーリングやコールバックのネスト（→ 非同期/Rxで直せそうな箇所）

非同期（UniTask）で直せそうな箇所

- Wave.cs … 敵を一定間隔で出す処理。自作Timerで毎フレーム時間を数えている → 「数秒待つ」を await UniTask.Delay() で書ける
- TimedWave.cs … 次のウェーブまでの待ち時間 → 同上、awaitで待つだけにできる
- TimedLevelIntro.cs … ゲーム開始時のイントロ/カウントダウン待ち → awaitで上から下に読める形に
- CurrencyGainer.cs … 一定間隔でお金が増える処理 → ループ＋awaitで表現できる
- HitscanAttack.cs / SelfDestroyTimer.cs / SuperTowerLauncher.cs / HomeBaseAttacker.cs … 「○秒後に発動／消滅」の遅延 → awaitで遅延を直接書ける
- 基盤の Timer.cs / RepeatingTimer.cs / TimedBehaviour.cs … これらの自作タイマー機構ごとUniTaskで不要にできる

→ 直せること：毎フレーム時間を数える仕組みをなくし、「待つ」処理をawaitで一行にする。中断（ゲーム終了時など）も CancellationToken で止められる。

Rx（R3）で直せそうな箇所

- WaveUI.Update() … 毎フレーム waveProgress を取りに行ってバーを更新 → 値が変わった時だけ通知される形に
- HealthVisualizer / CurrencyUI など UIのUpdate … 毎フレーム値を見て表示更新 → 購読して更新に
- Currency.currencyChanged（お金） … 手書きイベント → ReactiveProperty に
- Damageable の damaged/healed/died/healthChanged（HP） … 手書きイベント → 同上。「HPが0かつ特定状態の時だけ」みたいな合成が楽になる
- Targetter の targetEntersRange/lostTarget（索敵） … enter/exitイベントの合成 → Rxのオペレータで
- InputController の pressed/dragged/tapped など入力イベント … クリック等をストリーム（Observable）として扱える
- 購読解除のやり残し（+= と -= の数が合っていない） … Subscribe(...).AddTo(this) で解除を自動化

→ 直せること：「毎フレーム値を見にいく」のをやめて「変わったら通知してもらう」に変える。値の合成や購読解除も楽で安全になる。

触らない方がいい箇所（直す対象ではない）

- 弾の移動（BallisticProjectile など）、カメラ（CameraRig）、入力の読み取り（InputController など）のUpdate … これは毎フレームやるのが正しいので対象外

---

## 7. テストを書きづらい箇所と、その理由

現状、テストがあるのは HexPoint（純粋な計算クラス）だけ。 これは「Unityを起動せず単体で検証できるクラスしかテストされていない」ことの裏返しで、ゲームの中身はほぼテスト不能です。理由ごとに挙げます。

① Singletonに直接触っている（差し替えられない）
- LevelManager / GameUI / InputController / GameManager / PoolManager
- 理由：コード中で LevelManager.instance.… と本物を直に呼ぶので、テスト用の偽物に差し替えられない。テスト時も本物のシーン・本物のインスタンスが必要になる（シングルトンのデメリットがもろに出ている）

② ロジックがMonoBehaviourに直書き（Unity起動が必須）
- WaveManager（MonoBehaviour）/ Wave（TimedBehaviour）/ Targetter / Tower / Agent / AttackAffector など（全175ファイル中73がMonoBehaviour）
- 理由：new できず、GameObjectに貼って初めて動く。判定ロジックだけを取り出して検証できない（MonoBehaviour依存の弊害？）

③ 神クラスで責務が混ざっている
- GameUI（UI＋状態＋設置入力＋お金の購入判定が同居）
- 理由：「購入できるか」の判定だけテストしたくても、UIや入力ごと動かさないと呼べない

④ 時間・毎フレームに依存している
- Wave / TimedWave / CurrencyGainer / Targetter（Time.deltaTime を毎フレーム数える）
- 理由：実時間とフレーム更新に依存するため、テストで「3秒後」を再現しづらい（時間を注入できない）

⑤ 乱数で結果が変わる
- IListExtensions（共有Random）/ AreaMeshCreator の重み付き抽選
- 理由：呼ぶたび結果が変わり、期待値を固定できない（Randomを外から渡せない）

逆にテスト可能な箇所
- Currency（お金）と Damageable（HP）はすでに純粋なC#クラス（MonoBehaviour非依存）。ここは今すぐ単体テストを書ける
- HexPoint / IntVector2 / Ballistics（計算系）も純粋なのでテスト可能

→ つまり直すべきこと：ゲームロジックをMonoBehaviour/Singletonから引き剥がして純粋クラスにし、依存（時間・乱数・他クラス）を外から注入する。「DIにしたらテストが書けるようになった」を体感できる。