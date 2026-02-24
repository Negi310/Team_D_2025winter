[System.Serializable]
public struct CourtshipTraits 
{
    public float Size;
    // 色や形は、数値（float）か、特徴タグ（Enum）にするか仕様次第ですが
    // 比較計算を行うなら数値ベース、相性判定ならEnumやフラグベースが適しています。
    public float ColorValue; 
    public float ShapeValue; 
}