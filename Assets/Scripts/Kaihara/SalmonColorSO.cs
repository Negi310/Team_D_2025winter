using UnityEngine;

[CreateAssetMenu(fileName="SalmonColor",menuName ="Scriptable Objects/SalmonColor")]
public class SalmonColorSO : ScriptableObject
{
    [SerializeField] Color hairOrange;
    [SerializeField] Color hairRed;
    [SerializeField] Color hairBlue;
    [SerializeField] Color hairYellow;
    [SerializeField] Color hairGreen;
    [SerializeField] Color hairPink;
    [SerializeField] Color hairPurple;
    [SerializeField] Color hairSkyBlue;
    [SerializeField] Color hairYellowGreen;

    public Color HairOrange => hairOrange;
    public Color HairRed => hairRed;
    public Color HairBlue => hairBlue;
    public Color HairYellow => hairYellow;
    public Color HairGreen => hairGreen;
    public Color HairPink => hairPink;
    public Color HairPurple => hairPurple;
    public Color HairSkyBlue => hairSkyBlue;
    public Color HairYellowGreen => hairYellowGreen;
    
}