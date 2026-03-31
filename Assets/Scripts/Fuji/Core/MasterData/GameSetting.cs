using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Scriptable Objects/GameSetting")]
public class GameSetting : ScriptableObject
{
    [Header("【川の自動生成】River Generation")]
    
    [Tooltip("第1世代の川のパラメータ（流速、密度など全7項目）に振り分けられるポイントの合計値。高いほど初期から激しい川になります。")]
    public float InitialRiverPoints = 10f; 
    
    [Tooltip("世代交代するごとに加算される合計ポイント。ゲームが進むにつれて川がどれくらい難しくなるか（インフレ具合）を決めます。")]
    public float PointsPerGeneration = 2f; 


    [Header("【チャンク抽選】River Director")]
    
    [Tooltip("川のパラメータ（Curvinessなど）を0.0〜1.0の割合に計算し直すときの基準となる最大値。この値を超えるとMAX(1.0)として扱われます。")]
    public float RiverNormMax = 10f; 
    
    [Tooltip("チャンクの特性と川の要求の合致度（0.0〜1.0）がこの数値を超えた場合、そのチャンクが選ばれやすくなるボーナスが発生します。")]
    public float MatchBonusThreshold = 0.8f; 
    
    [Tooltip("合致度が閾値を超えた場合に、そのチャンクがルーレットで選ばれる確率にかかる倍率（2.0なら2倍出やすくなる）。")]
    public float MatchBonusMultiplier = 2.0f; 


    [Header("【障害物生成】Obstacle Spawning")]
    
    [Tooltip("1つのチャンク内に生成される固定障害物（岩や倒木）の最大数。高すぎると進行不可能になる可能性があります。")]
    public int MaxFixedObstaclesPerChunk = 6;
    
    [Tooltip("岩の当たり判定（物理的な半径）。この数値を元に、他のオブジェクトと重ならないように生成されます。")]
    public float RockRadius = 1.0f;
    
    [Tooltip("倒木の当たり判定（物理的な半径）。岩よりも大きく設定することで、横幅を取る障害物として機能します。")]
    public float FallenTreeRadius = 2.0f;
    
    [Tooltip("川のパラメータから計算された漂流物（小魚、ライバル、流木）の合計密度にかける倍率。最終的な生成数を調整します。")]
    public float DrifterDensityMultiplier = 0.5f;
    
    [Tooltip("1チャンク内に生成される漂流物の最大数。多すぎると処理落ちや回避不能な壁になるため制限をかけます。")]
    public int MaxDriftersPerChunk = 5;


    [Header("【漂流物の速度・AI】Drifter AI")]
    
    [Tooltip("小魚の移動速度（前進する速度）。プラスなら鮭と同じ上流方向に進み、マイナスなら下流へ向かってきます。")]
    public float FishSpeed = 1.0f;
    
    [Tooltip("ライバル鮭の移動速度。基本的にはプレイヤーより少し遅いか同じくらいにして、追い越す/追い越される展開を作ります。")]
    public float RivalSpeed = 2.5f;
    
    [Tooltip("流木の移動速度。マイナスにすることで、上流から下流へ（プレイヤーに向かって）流れてきます。")]
    public float DriftwoodSpeed = -3.0f;
    
    [Tooltip("漂流物が目の前の障害物を避ける時のレーン移動の速さ（横移動スピード）。高いほどシュバッと機敏に避けます。")]
    public float BlockedLaneChangeSpeed = 1.6f;
    
    [Tooltip("ライバル鮭が通常時にレーンをフラフラと変える時の速さ。")]
    public float RivalLaneChangeSpeed = 0.5f;
    
    [Tooltip("小魚など、その他の漂流物が通常時にレーンを変える時の速さ。")]
    public float DefaultLaneChangeSpeed = 0.2f;


    [Header("【プレイヤー移動】Player Movement")]
    
    [Tooltip("プレイヤーの基本となる前進速度。")]
    public float BaseForwardSpeed = 3.0f;
    
    [Tooltip("プレイヤーの基本となる横移動速度（左右への動きやすさ）。")]
    public float BaseHorizontalSpeed = 3.0f;
    
    [Tooltip("プレイヤーのSpeedステータスが、横移動速度にどれくらい影響を与えるか（1ステータスあたりの上昇量）。")]
    public float HorizontalStatMultiplier = 0.1f;
    
    [Tooltip("ダメージを受けて無敵状態になったとき、横移動速度にかかる倍率（ピンチの時に避けやすくするための救済措置）。")]
    public float InvincibleSpeedMultiplier = 1.5f;


    [Header("【スタミナ・ジャンプ】Stamina & Jump")]
    
    [Tooltip("1秒間に減少するスタミナの基本量。")]
    public float BaseDrainRate = 1.0f;
    
    [Tooltip("ジャンプ1回あたりに消費するスタミナの基本量。")]
    public float BaseJumpCost = 15.0f;
    
    [Tooltip("ジャンプコストの最低保証値（ステータスが高くてもこれ以下にはならない）。")]
    public float MinJumpCost = 2.0f;
    
    [Tooltip("Jumpステータスが1上がるごとに、ジャンプコストがどれくらい軽減されるか。")]
    public float JumpStatDiscountMultiplier = 0.5f;
    
    [Tooltip("スタミナ消費の計算に使う川の流速（FlowSpeed）の基準値。流速がこれより高いとスタミナ消費が激しくなります。")]
    public float FlowSpeedNorm = 5.0f; 
    
    [Tooltip("ジャンプ直後の「ジャスト回避 / 攻撃ボーナス」が有効な時間（秒）。")]
    public float JustJumpWindow = 0.5f; 


    [Header("【ダメージ・バフ】Damage & Buff")]
    
    [Tooltip("岩などの障害物に正面衝突したときに受けるダメージ（スタミナ減少量）。")]
    public float RockDamage = 20f;
    
    [Tooltip("上から流れてくる流木にぶつかったときに受けるダメージ。")]
    public float DriftwoodDamage = 25f;
    
    [Tooltip("ライバル鮭とのバトル（攻撃力勝負）に負けたときに受けるダメージ。")]
    public float RivalLoseDamage = 20f;
    
    [Tooltip("ライバル鮭とのバトルで引き分けたときに受ける削りダメージ。")]
    public float RivalDrawDamage = 5f;
    
    [Tooltip("小魚を取ったときに付与される「スタミナ消費半減バフ」の有効時間（秒）。")]
    public float FishBuffDuration = 5f; 
    
    [Tooltip("ダメージを受けたあとに発生する無敵時間（秒）。")]
    public float InvincibleDuration = 1.5f; 
    

    [Header("【ライバル戦】Rival Battle")]
    
    [Tooltip("ライバル鮭の攻撃力（強さ）のランダム幅の最低値。")]
    public float RivalMinStrength = 5f;
    
    [Tooltip("ライバル鮭の攻撃力（強さ）のランダム幅の最高値。")]
    public float RivalMaxStrength = 15f;
    
    [Tooltip("ジャンプ直後の猶予時間（JustJumpWindow）内にライバルに接触した場合に、プレイヤーの攻撃力に加算されるボーナス値。")]
    public float JustJumpAttackBonus = 10f;


    [Header("【求愛確率】Courtship Probability")]
    
    [Tooltip("求愛成功率のベースとなる基礎確率（例: 0.3 = 30%）。")]
    public float CourtshipBaseRate = 0.3f; 
    
    [Tooltip("お互いの「Size（大きさ）」が一致しているかどうかが、成功率に与える影響の最大値（完全一致で+20%など）。")]
    public float CourtshipSizeWeight = 0.2f; 
    
    [Tooltip("お互いの「Color（色）」がどれくらい近いかが、成功率に与える影響の最大値（完全一致で+30%など）。")]
    public float CourtshipColorWeight = 0.3f; 
    
    [Tooltip("お互いの「Hair（形/特徴）」が完全に一致していた場合に加算される固定ボーナス（+20%）。")]
    public float CourtshipShapeBonus = 0.2f; 
    
    [Tooltip("どんなに相性が悪くても最低限保証される成功率の下限（0.05 = 5%）。")]
    public float CourtshipMinRate = 0.05f; 
    
    [Tooltip("どんなに相性が良くても失敗する可能性を残すための成功率の上限（0.95 = 95%）。")]
    public float CourtshipMaxRate = 0.95f; 
    

    [Header("【環境・地形の影響】Environment Effects")]
    
    [Tooltip("川幅の最小値（これ以上狭くならない基準値）。")]
    public float RiverMinWidth = 2f;
    
    [Tooltip("川幅の最大値（これ以上広くならない基準値）。")]
    public float RiverMaxWidth = 10f;
    
    [Tooltip("川幅が最も狭いときに、横移動スピードがどれくらい落ちるかの度合い（0.0〜1.0）。")]
    public float RiverWidthSpeedPenalty = 0.5f;
    
    [Tooltip("川の中心（最も流れが急な場所）にいるときに、スタミナ消費が最大でどれくらい増えるかの度合い（例：0.5なら最大1.5倍の消費）。")]
    public float CenterDistanceDrainPenalty = 0.5f;
    
    [Tooltip("川の中心から遠い（岸に近い）場所からジャンプするときに、流れに乗れないためジャンプコストがどれくらい増えるかの度合い。")]
    public float CenterDistanceJumpPenalty = 0.5f;


    [Header("【水流・スリップストリーム】Water Flow & Slipstream")]
    
    [Tooltip("センサーが反応する前方の距離（何メートル先からスリップストリーム等の恩恵を受けられるか）。")]
    public float ForwardRayLimit = 5f;
    
    [Tooltip("岩の真後ろ（流れが緩やかになる場所）にいるときのスタミナ消費倍率（例：0.5なら消費半減）。")]
    public float RockSlipstreamDrainMultiplier = 0.5f;
    
    [Tooltip("岩の真後ろからジャンプするときのジャンプコスト倍率（例：0.1ならコスト10分の1で飛べる）。")]
    public float RockSlipstreamJumpMultiplier = 0.1f;
    
    [Tooltip("岩の横など、水が押し出されて激流になっている場所を通る時のスタミナ消費倍率（例：1.2なら消費2割増し）。")]
    public float RockSideTorrentDrainMultiplier = 1.2f;
    
    [Tooltip("ライバル鮭の真後ろ（スリップストリーム）にいるときのスタミナ消費倍率（例：0.8なら消費2割引き）。")]
    public float RivalSlipstreamDrainMultiplier = 0.8f;


    [Header("【ステータス・バフの影響】Stats & Buff Scaling")]
    
    [Tooltip("Staminaステータス1ポイントにつき、ゲーム内のスタミナ総量がいくつ増えるか（例：10ならステータス10でスタミナ100）。")]
    public float StaminaToMaxStaminaRate = 10f;
    
    [Tooltip("Speedステータス1ポイントにつき、スタミナの持続消費量がどれくらいの割合で減少するか（例：0.02ならステータス10で消費20%軽減）。")]
    public float SpeedToDrainReductionRate = 0.02f;
    
    [Tooltip("小魚を取ったときの「スタミナ消費軽減バフ」中の消費倍率（例：0.5なら消費半減）。")]
    public float FishBuffDrainMultiplier = 0.5f;
}